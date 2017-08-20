using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using AutoMapper;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.infrastuctures.Utils;
using Wrly.Models.Listing;
using Wrly.Infrastructure.Utils;
using Wrly.Models;
using Wrly.Notifications.Processors.Implementors;
using Wrly.Models.Business;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class AssociationProcessor : BaseProcessor, IAssociationProcessor
    {
        private const string REQUEST_ACTION = "ActionOnRequest";
        private const string HAPPENING_ACTION = "ActionOnHappening";

        IAwardProcessor _AwardProcessor;
        IAwardProcessor AwardProcessor
        {

            get
            {
                if (_AwardProcessor == null)
                {
                    _AwardProcessor = new AwardProcessor();
                }
                return _AwardProcessor;
            }
        }


        ISkillHistoryProcessor _skillProcessor;
        public ISkillHistoryProcessor SkillHistoryProcessor
        {
            get
            {
                if (_skillProcessor == null)
                {
                    _skillProcessor = new SkillHistoryProcessor();
                }
                return _skillProcessor;
            }
        }

        public async System.Threading.Tasks.Task<long> SendConnectRequest(Models.SendAssociationViewModel model)
        {
            var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
            //NOTE: Entity2 is always the current entity.
            if (UserHashObject.EntityID == entityInfo.EntityID2)
            {
                using (var repository = new AssociationRepository())
                {
                    var association = Mapper.Map<Association>(entityInfo);
                    association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Pending;
                    association.AssociationType = (byte)Enums.AssociationType.NetworkConnection;
                    association.IpAddress = IpAddress;
                    var result = await repository.SaveAssociation(association);
                    //if (result > 0)
                    //{
                    // Removing entities from Stock.
                    result = await repository.RemoveStock(association.EntityID2, association.EntityID);
                    if (result > 0)
                    {
                        result = await repository.RemoveStock(association.EntityID, association.EntityID2);
                        if (result > 0)
                        {
                            // Preparing stock for entity.
                            await repository.ShuffleStock(association.EntityID);
                            // Preparing stock for another entity.
                            await repository.ShuffleStock(association.EntityID2);
                        }
                    }
                    //}
                }
            }
            return -1;
        }


        public async System.Threading.Tasks.Task<List<ActionAssociateProfileViewModel>> GetRequests(int pageNumber, int pageSize, Enums.AssociationRequestDirection direction = Enums.AssociationRequestDirection.Received)
        {
            if (pageNumber == 1)
            {
                using (AccountRepository repository = new AccountRepository())
                {
                    await repository.SetLastRequestSeenData(Now, UserHashObject.EntityID);
                }
            }
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetRequests(UserHashObject.EntityID, pageNumber, pageSize, (byte)Enums.AssociationRequestStatus.Pending, (byte)direction))
                {
                    var list = dataSet.Tables[0].FromDataTable<ActionAssociateProfileViewModel>();
                    if (list != null && list.Count > 0)
                    {
                        string cookieToken, formToken;
                        foreach (var item in list)
                        {
                            var table = new Hashtable();
                            table.Add("Action", REQUEST_ACTION);
                            table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                            table.Add("EntityID", item.EntityID2);
                            table.Add("AssociationID", item.AssociationID);
                            table.Add("EntityID2", item.EntityID);
                            item.Hash = QueryStringHelper.Encrypt(table);
                            AntiForgery.GetTokens(null, out cookieToken, out formToken);
                            item.Token = string.Format("{0}:{1}", cookieToken, formToken);
                        }
                    }
                    return list;
                }
            }
        }

        public async System.Threading.Tasks.Task<long> Action(ActionAssociateProfileViewModel model, string actn)
        {
            var hashedObect = model.Hash.ToObject<ActionAssociateProfileViewModel>(null);
            if (!string.IsNullOrEmpty(actn) &&
                (
                    actn.Equals("Approve", StringComparison.InvariantCultureIgnoreCase) ||
                    actn.Equals("Decline", StringComparison.InvariantCultureIgnoreCase))
                )
            {
                if (hashedObect.EntityID2 == UserHashObject.EntityID && hashedObect.Action == REQUEST_ACTION)
                {
                    using (var repository = new AssociationRepository())
                    {
                        var requestObject = new Association();
                        if (hashedObect.AssociationID > 0)
                            requestObject = await GetAssociation(hashedObect.AssociationID, repository);
                        else
                            requestObject = await GetAssociation(hashedObect.EntityID, hashedObect.EntityID2, repository);

                        var association = Mapper.Map<Association>(hashedObect);
                        if (actn == "Approve")
                            association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Approve;
                        if (actn == "Decline")
                            association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Rejected;

                        association.AssociationType = (byte)Enums.AssociationType.NetworkConnection;
                        association.IpAddress = IpAddress;

                        if (actn == "Approve")
                        {
                            association.OppositeRowID = requestObject.AssociationID;
                            requestObject.EditedOn = association.EditedOn = Now;
                            requestObject.EditedBy = association.EditedBy = User;
                            requestObject.ObjectStatus = (byte)Enums.AssociationRequestStatus.Approve;
                            return await repository.ApproveRequest(requestObject, association);
                        }
                        else if (actn == "Decline")
                        {
                            association.OppositeRowID = requestObject.AssociationID;
                            requestObject.EditedOn = association.EditedOn = Now;
                            requestObject.EditedBy = association.EditedBy = User;
                            requestObject.ObjectStatus = (byte)Enums.AssociationRequestStatus.Rejected;
                            return await repository.RejectRequest(requestObject, association);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("Cancel", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
                //NOTE: Entity is always the current entity, as request is received.
                if (UserHashObject.EntityID == entityInfo.EntityID || (UserHashObject.EntityID == entityInfo.EntityID2 && model.Mode.Equals("out")))
                {
                    using (var repository = new AssociationRepository())
                    {
                        var requestObject = await GetAssociation(entityInfo.EntityID, entityInfo.EntityID2, repository);
                        requestObject.ObjectStatus = (byte)Enums.AssociationRequestStatus.Canceled;
                        return await repository.UpdateAssociation(requestObject);
                    }
                }
                return -1;
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("Connect", StringComparison.InvariantCultureIgnoreCase) || actn.Equals("Skip", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
                //NOTE: Entity2 is always the current entity.
                if (UserHashObject.EntityID == entityInfo.EntityID2 && entityInfo.EntityID2 != entityInfo.EntityID)
                {
                    using (var repository = new AssociationRepository())
                    {
                        var association = Mapper.Map<Association>(entityInfo);
                        if (actn.Equals("Skip", StringComparison.InvariantCultureIgnoreCase))
                            association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Skiped;
                        else
                            association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Pending;

                        association.AssociationType = (byte)Enums.AssociationType.NetworkConnection;
                        association.IpAddress = IpAddress;
                        var result = await repository.SaveAssociation(association);
                        if (result > 0)
                        {
                            result = await repository.RemoveStock(association.EntityID, association.EntityID2);
                            if (result > 0)
                            {
                                // Preparing stock for entity.
                                await repository.ShuffleStock(association.EntityID);
                                // Preparing stock for another entity.
                                await repository.ShuffleStock(association.EntityID2);
                            }
                        }
                    }
                }
                return -1;
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("Follow", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
                //NOTE: Entity2 is always the current entity.
                if (UserHashObject.EntityID == entityInfo.EntityID2 && entityInfo.EntityID2 != entityInfo.EntityID)
                {
                    using (var repository = new AssociationRepository())
                    {
                        var association = Mapper.Map<Association>(entityInfo);
                        association.ObjectStatus = !entityInfo.RequirePermission ? (byte)Enums.AssociationRequestStatus.Approve : (byte)Enums.AssociationRequestStatus.Pending;
                        association.AssociationType = (byte)Enums.AssociationType.Follow;
                        association.IpAddress = IpAddress;
                        await repository.SaveAssociation(association);
                        //ADD Notification here.
                    }
                }
                return -1;
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("ApproveFollow", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
                using (var repository = new AssociationRepository())
                {
                    var association = Mapper.Map<Association>(hashedObect);
                    if (actn == "Approve")
                        association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Approve;
                    if (actn == "Decline")
                        association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Rejected;

                    var requestObject = await GetAssociation(hashedObect.AssociationID, repository);
                    requestObject.EditedOn = association.EditedOn = Now;
                    requestObject.EditedBy = association.EditedBy = User;
                    requestObject.ObjectStatus = (byte)Enums.AssociationRequestStatus.Approve;
                    return await repository.UpdateAssociation(requestObject);
                }
                return -1;
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("Block", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
                //NOTE: Entity2 is always the current entity.
                if (UserHashObject.EntityID == entityInfo.EntityID2 && entityInfo.EntityID2 != entityInfo.EntityID)
                {
                    using (var repository = new AssociationRepository())
                    {
                        var association = Mapper.Map<Association>(entityInfo);
                        association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Approve;
                        association.AssociationType = (byte)Enums.AssociationType.Block;
                        association.IpAddress = IpAddress;
                        // Insert block for entity.
                        return await repository.Block(association);
                    }
                }
                return -1;
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("UnBlock", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
                //NOTE: Entity2 is always the current entity.
                if (UserHashObject.EntityID == entityInfo.EntityID2)
                {
                    using (var repository = new AssociationRepository())
                    {
                        // UnBlock.
                        return await repository.UnBlock(UserHashObject.EntityID, entityInfo.EntityID);
                    }
                }
                return -1;
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("UnFollow", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);
                //NOTE: Entity2 is always the current entity.
                if (UserHashObject.EntityID == entityInfo.EntityID2)
                {
                    using (var repository = new AssociationRepository())
                    {
                        // UnFollow.
                        return await repository.UnFollow(UserHashObject.EntityID, entityInfo.EntityID);
                    }
                }
                return -1;
            }
            else if (!string.IsNullOrEmpty(actn) && (actn.Equals("Remove", StringComparison.InvariantCultureIgnoreCase)))
            {
                var entityInfo = model.Hash.ToObject<AssociateProfileViewModel>(null);

                using (var repository = new AssociationRepository())
                {
                    var associationObject = await GetAssociation(hashedObect.AssociationID, repository);
                    var oppositeAssociationObject = await GetOppositeAssociation(hashedObect.AssociationID, repository);
                    if (
                            (associationObject.EntityID == UserHashObject.EntityID && UserHashObject.EntityID == oppositeAssociationObject.EntityID2)
                            &&
                            (associationObject.EntityID == oppositeAssociationObject.EntityID2 && associationObject.EntityID2 == oppositeAssociationObject.EntityID))
                    {
                        var association = Mapper.Map<Association>(hashedObect);
                        association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Removed;
                        oppositeAssociationObject.ObjectStatus = (byte)Enums.AssociationRequestStatus.Removed;
                        oppositeAssociationObject.EditedOn = association.EditedOn;
                        oppositeAssociationObject.EditedBy = association.EditedBy;
                        return await repository.RemoveAssociations(association, oppositeAssociationObject);
                    }


                }
                return -1;
            }
            // for unfollow, diconnect, removing, cancelling requests etc
            else
            {
                using (var repository = new AssociationRepository())
                {
                    var requestObject = new Association();
                    if (hashedObect.AssociationID > 0)
                        requestObject = await GetAssociation(hashedObect.AssociationID, repository);
                    else
                        requestObject = await GetAssociation(hashedObect.EntityID, hashedObect.EntityID2, repository);

                    var association = Mapper.Map<Association>(hashedObect);
                    if (actn.Equals("Unfollow", StringComparison.InvariantCultureIgnoreCase))
                        association.ObjectStatus = (byte)Enums.AssociationRequestStatus.Removed;
                    return await repository.UpdateAssociation(association);
                }
            }
            return -1;
        }

        private async Task<Association> GetAssociation(long entityID, long entityID2, AssociationRepository repository)
        {
            using (var dataSet = await repository.GetSingleAsObject(entityID, entityID2, (int)Enums.AssociationRequestStatus.Pending))
            {
                return dataSet.Tables[0].FromDataTable<Association>().FirstOrDefault();
            }
        }

        private async Task<Association> GetOppositeAssociation(long id, AssociationRepository repository)
        {
            using (var dataSet = await repository.GetSingleOppositeAsObject(id))
            {
                return dataSet.Tables[0].FromDataTable<Association>().FirstOrDefault();
            }
        }

        private async Task<Association> GetAssociation(long id, AssociationRepository repository)
        {
            using (var dataSet = await repository.GetSingleAsObject(id))
            {
                return dataSet.Tables[0].FromDataTable<Association>().FirstOrDefault();
            }
        }

        public async System.Threading.Tasks.Task<List<AssociateProfileViewModel>> GetSuggestions(int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetSuggestions(UserHashObject.EntityID, pageNumber, pageSize, UserCacheManager.Settings.Network.NetworkCoverageLevel))
                {
                    var list = dataSet.Tables[0].FromDataTable<AssociateProfileViewModel>();
                    if (list != null && list.Count > 0)
                    {
                        string cookieToken, formToken;
                        foreach (var item in list)
                        {
                            var table = new Hashtable();
                            table.Add("Action", "Associate");
                            table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                            table.Add("EntityID", item.EntityID);
                            table.Add("EntityID2", UserHashObject.EntityID);
                            item.Hash = QueryStringHelper.Encrypt(table);
                            AntiForgery.GetTokens(null, out cookieToken, out formToken);
                            item.Token = string.Format("{0}:{1}", cookieToken, formToken);
                        }
                    }
                    return list.Randomize();
                }
            }
        }

        public async Task<List<AssociateProfileViewModel>> GetConnections(int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetExisitingAssociations(UserHashObject.EntityID, pageNumber, pageSize))
                {
                    return ToAssciationProfile(dataSet);
                }
            }
        }

        public async Task<List<AssociateProfileViewModel>> GetFollowings(int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetFollowers(UserHashObject.EntityID, pageNumber, pageSize, (byte)Enums.AssociationRequestDirection.Sent))
                {
                    return ToAssciationProfile(dataSet, true);
                }
            }
        }

        public async Task<List<AssociateProfileViewModel>> GetFollowers(int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetFollowers(UserHashObject.EntityID, pageNumber, pageSize, (byte)Enums.AssociationRequestDirection.Received))
                {
                    return ToAssciationProfile(dataSet);
                }
            }
        }

        private List<AssociateProfileViewModel> ToAssciationProfile(System.Data.DataSet dataSet, bool reverse = false)
        {
            var list = dataSet.Tables[0].FromDataTable<AssociateProfileViewModel>();
            if (list != null && list.Count > 0)
            {
                string cookieToken, formToken;
                foreach (var item in list)
                {
                    var table = new Hashtable();
                    table.Add("Action", REQUEST_ACTION);
                    table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                    if (reverse)
                    {
                        table.Add("EntityID", item.EntityID);
                        table.Add("EntityID2", item.EntityID2);
                    }
                    else
                    {
                        table.Add("EntityID", item.EntityID2);
                        table.Add("EntityID2", item.EntityID);
                    }

                    table.Add("AssociationID", item.AssociationID);
                    item.Hash = QueryStringHelper.Encrypt(table);
                    AntiForgery.GetTokens(null, out cookieToken, out formToken);
                    item.Token = string.Format("{0}:{1}", cookieToken, formToken);
                }
            }
            return list;
        }


        public async Task<AssociateProfileActionViewModel> GetConnectionActions(string q)
        {
            var baseModel = q.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetConnectionActions(UserHashObject.EntityID, baseModel.EntityID))
                {
                    var model = dataSet.Tables[0].FromDataTable<AssociateProfileActionViewModel>().FirstOrDefault();
                    var table = new Hashtable();
                    table.Add("Action", REQUEST_ACTION);
                    table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                    table.Add("EntityID", baseModel.EntityID);
                    table.Add("EntityID2", UserHashObject.EntityID);
                    table.Add("Location", "Location.ProfileFace");
                    model.NetworkHash = QueryStringHelper.Encrypt(table);
                    return model;
                }
            }
        }


        public async Task<List<HappeningsViewModel>> GetNetworkHappenings(int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetNetworkHappenings(UserHashObject.EntityID, pageNumber, pageSize))
                {
                    var model = dataSet.Tables[0].FromDataTable<HappeningsViewModel>();
                    string cookieToken, formToken;
                    foreach (var item in model)
                    {
                        var table = new Hashtable();
                        table.Add("Action", HAPPENING_ACTION);
                        table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                        table.Add("EntityID", item.EntityID);
                        table.Add("ID", item.ID);
                        table.Add("EntityID2", UserHashObject.EntityID);
                        table.Add("AwardID", item.AwardID ?? 0);
                        table.Add("CareerHistoryID", item.CareerHistoryID ?? 0);
                        table.Add("EntitySkillID", item.SkillID ?? 0);
                        table.Add("ForEntityID", item.ForEntityID ?? 0);
                        table.Add("Location", "Location.ProfileFace");
                        AntiForgery.GetTokens(null, out cookieToken, out formToken);
                        item.Token = string.Format("{0}:{1}", cookieToken, formToken);
                        item.Hash = QueryStringHelper.Encrypt(table);
                    }
                    return model;
                }
            }
        }


        public async Task<Result> ActivityAction(BaseViewModel model, string action)
        {
            var happeningsViewModel = model.Hash.ToObject<HappeningsViewModel>(null);
            var actionModel = Mapper.Map<ActivityAction>(happeningsViewModel);
            int result = 0;
            switch (action.ToLower())
            {
                case "welcome":
                    actionModel.Action = (int)Enums.NetworkActivityAction.Welcomed;
                    break;
                case "like":
                    actionModel.Action = (int)Enums.NetworkActivityAction.Liked;
                    break;
                case "goodluck":
                    actionModel.Action = (int)Enums.NetworkActivityAction.SentGoodluck;
                    break;
                case "next":
                    actionModel.Action = (int)Enums.NetworkActivityAction.Skipped;
                    break;
                case "congratulate":
                    actionModel.Action = (int)Enums.NetworkActivityAction.Congratulated;
                    break;
                case "endorse":
                    actionModel.Action = (int)Enums.NetworkActivityAction.Endorsed;
                    break;
                default:
                    break;
            }

            using (var repository = new AssociationRepository())
            {
                actionModel.EntityID = UserHashObject.EntityID;
                actionModel.IpAddress = IpAddress;
                result = await repository.ActivityAction(actionModel);
            }

            if (result >= 0)
            {
                switch (action.ToLower())
                {
                    case "welcome":
                        {
                            string url = string.Format("/network/activities/#activity{0}", actionModel.ID);
                            if (EntityHash.ContainsKey("Url"))
                                EntityHash.Remove("Url");
                            EntityHash.Add("Url", url);
                            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                            await notificationProcessor.AddNotification(Enums.NotificationType.WelcomedOnJoiningCompany, null, null, happeningsViewModel.EntityID, null, null, null, happeningsViewModel.ID);
                        }
                        break;
                    case "like":
                        {
                            string url = string.Format("/network/activities/#activity{0}", actionModel.ID);
                            if (EntityHash.ContainsKey("Url"))
                                EntityHash.Remove("Url");
                            EntityHash.Add("Url", url);
                            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                            await notificationProcessor.AddNotification(Enums.NotificationType.LikedAnActivity, null, null, happeningsViewModel.EntityID, null, null, null, happeningsViewModel.ID);
                        }
                        break;
                    case "goodluck":
                        {
                            string url = string.Format("/network/activities/#activity{0}", actionModel.ID);
                            if (EntityHash.ContainsKey("Url"))
                                EntityHash.Remove("Url");
                            EntityHash.Add("Url", url);
                            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                            await notificationProcessor.AddNotification(Enums.NotificationType.WellWishOnJoiningCompany, null, null, happeningsViewModel.EntityID, null, null, null, happeningsViewModel.ID);
                        }
                        break;
                    case "congratulateonanniversary":
                        {
                            string url = string.Format("/network/activities/#activity{0}", actionModel.ID);
                            if (EntityHash.ContainsKey("Url"))
                                EntityHash.Remove("Url");
                            EntityHash.Add("Url", url);
                            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                            await notificationProcessor.AddNotification(Enums.NotificationType.CongratulateOnAnniversary, null, null, happeningsViewModel.EntityID, null, null, null, happeningsViewModel.ID);
                        }
                        break;
                    case "congratulateonaward":
                        await AwardProcessor.Congratulate(model.Hash);
                        break;
                    case "endorse":
                        await SkillHistoryProcessor.Endorse(model.Hash);
                        break;
                    default:
                        break;
                }
            }
            if (result >= 0)
                return new Result() { Type = Enums.ResultType.Success };
            return new Result() { Type = Enums.ResultType.Error };
        }


        public async Task<Result> FeedIntialNetworkData()
        {
            using (var repository = new AssociationRepository())
            {
                var result = await GeneralExtentions.Do<Task<int>>(() => repository.InsertSeedRequests(UserHashObject.EntityID), TimeSpan.FromSeconds(1), 5);
                if (result > 0)
                    return new Result() { Type = Enums.ResultType.Success };
                return new Result() { Type = Enums.ResultType.Error };
            }
        }


        public async Task<Result> PrepareStock()
        {
            using (var repository = new AssociationRepository())
            {
                var result = await GeneralExtentions.Do<Task<long>>(() => repository.ShuffleStock(UserHashObject.EntityID), TimeSpan.FromSeconds(1), 5);
                if (result > 0)
                    return new Result() { Type = Enums.ResultType.Success };
                return new Result() { Type = Enums.ResultType.Error };
            }
        }


        public async Task<List<AssociateProfileViewModel>> GetFollowers(long entityID, int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetFollowers(entityID, pageNumber, pageSize, (byte)Enums.AssociationRequestDirection.Received, UserHashObject.EntityID))
                {
                    return ToAssciationProfile(dataSet);
                }
            }
        }

        public async Task<List<AssociateProfileViewModel>> GetConnections(long entityID, int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetExisitingAssociations(entityID, pageNumber, pageSize, UserHashObject.EntityID))
                {
                    return ToAssciationProfile(dataSet);
                }
            }
        }


        public async Task<List<MyHappeningsViewModel>> GetMyHappenings(int pageNumber, int pageSize)
        {
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetMyHappenings(UserHashObject.EntityID, pageNumber, pageSize))
                {
                    var model = dataSet.Tables[0].FromDataTable<MyHappeningsViewModel>();
                    string cookieToken, formToken;
                    foreach (var item in model)
                    {
                        var table = new Hashtable();
                        table.Add("Action", HAPPENING_ACTION);
                        table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                        table.Add("EntityID", item.EntityID);
                        table.Add("ID", item.ID);
                        table.Add("EntityID2", UserHashObject.EntityID);
                        table.Add("AwardID", item.AwardID ?? 0);
                        table.Add("CareerHistoryID", item.CareerHistoryID ?? 0);
                        table.Add("EntitySkillID", item.SkillID ?? 0);
                        table.Add("ForEntityID", item.ForEntityID ?? 0);
                        table.Add("Location", "Location.ProfileFace");
                        AntiForgery.GetTokens(null, out cookieToken, out formToken);
                        item.Token = string.Format("{0}:{1}", cookieToken, formToken);
                        item.Hash = QueryStringHelper.Encrypt(table);
                    }
                    return model;
                }
            }
        }


        public async Task<List<InviteAssociateProfileViewModel>> GetConnections(string q, string keyword, int pageNumber, int pageSize)
        {
            var profileModel = q.ToObject<ProfileViewModel>(null);
            using (var repository = new AssociationRepository())
            {
                using (var dataSet = await repository.GetExisitingAssociationsToInvite(keyword, profileModel.EntityID, UserHashObject.EntityID, pageNumber, pageSize))
                {
                    var model = dataSet.Tables[0].FromDataTable<InviteAssociateProfileViewModel>();
                    foreach (var item in model)
                    {
                        var table = new Hashtable();
                        table.Add("Action", HAPPENING_ACTION);
                        table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                        table.Add("EntityID", item.EntityID2);
                        table.Add("ID", item.AssociationID);
                        table.Add("EntityID2", UserHashObject.EntityID);
                        table.Add("ReferenceEntityID", profileModel.EntityID);
                        table.Add("Location", "Location.ProfileFace");
                        item.Hash = QueryStringHelper.Encrypt(table);
                    }
                    return model;
                }
            }
        }


        public async Task<Result> InviteToGroup(string hash)
        {
            var state = hash.ToObject<EntityState>(null);
            state.EntityID2 = UserHashObject.EntityID;
            state.IpAddress = IpAddress;
            state.CreatedOn = Now;
            state.Type = (int)Enums.EntityStateType.InviteForGroup;
            state.Status = (byte)Enums.Status.Active;
            using (var Repository = new AccountRepository())
            {
                GroupViewModel groupCard = null;
                var result = await Repository.EntityTrack(state);
                if (result > 0)
                {
                    using (var repository = new AccountRepository())
                    {
                        EntityProfileViewModel currentCard = null;
                        using (var candidate = await repository.HoverCard(UserHashObject.EntityID.ToString(), UserHashObject.EntityID))
                        {
                            currentCard = candidate.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                        }

                        using (var businessRepository = new BusinessRepository())
                        {
                            using (var group = await businessRepository.Groups(null, UserHashObject.EntityID, null, state.ReferenceEntityID))
                            {
                                groupCard = group.Tables[0].FromDataTable<GroupViewModel>().FirstOrDefault();
                                string url = string.Format("/groups/{0}", groupCard.ProfileName);
                                if (EntityHash.ContainsKey("Url"))
                                    EntityHash.Remove("Url");
                                EntityHash.Add("Url", url);
                                var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                                await notificationProcessor.AddNotification(Enums.NotificationType.GroupInvitation, null, state.ReferenceEntityID, state.EntityID, null, null, null);
                            }
                        }

                        using (var employer = await repository.HoverCard(state.EntityID.ToString(), UserHashObject.EntityID))
                        {
                            var employerCard = employer.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                            // Sending email
                            string nameTitle = employerCard.EntityType == (int)Enums.EntityTypes.Person ? employerCard.FormatedName : employerCard.Name;
                            string currentName = currentCard.EntityType == (int)Enums.EntityTypes.Person ? currentCard.FormatedName : currentCard.Name;
                            string currentProfileHeading = currentCard.EntityType == (int)Enums.EntityTypes.Person ? currentCard.ProfileHeading : currentCard.Category;
                            var hashTable = new Hashtable();
                            hashTable.Add("**CurrentName**", currentName);
                            hashTable.Add("**CurrentHeading**", currentProfileHeading);
                            hashTable.Add("**GroupName**", groupCard.Name);
                            hashTable.Add("**GroupDesc**", groupCard.Description);
                            hashTable.Add("**PhotoUrl**", !string.IsNullOrEmpty(groupCard.ProfileImagePath) ? groupCard.ProfileImagePath.ImagePath(groupCard.ProfileImagePath, 100) : "https://www.sklative.com/Content/images/group.png");
                            hashTable.Add("**Name**", nameTitle);
                            hashTable.Add("**Type**", ((Enums.GroupType)groupCard.SubType).GetDescription());
                            hashTable.Add("**Url**", groupCard.ProfileName);
                            await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.GroupInvitation, employerCard.EmailAddress, true);
                        }
                    }
                    return new Result() { Type = Enums.ResultType.Success, Description = string.Format("Your invitation has been sent to join the group \"{0}\".", groupCard.Name) };
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "There is an error while sending invitation." };
            }
        }


        public async Task<Result> ApproveAll(string hash)
        {
            GroupViewModel model = hash.ToObject<GroupViewModel>(null);
            using (var respository = new AssociationRepository())
            {
                var result = await respository.ApproveAll(model.EntityID, UserHashObject.EntityID);
                if (result > 0)
                {
                    return new Result() { Type = Enums.ResultType.Success, Description = "All the joining requests has been approved." };
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "There is an error while approving members." };
            }
        }
    }
}