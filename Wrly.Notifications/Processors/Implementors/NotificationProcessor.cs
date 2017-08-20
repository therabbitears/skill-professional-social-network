using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Data.Repositories.Implementors;

namespace Wrly.Notifications.Processors.Implementors
{
    public class InstantNotificationProcessor
    {
        Hashtable Values;
        string CreatedBy { get { return Values["CreatedBy"].ToString(); } }

        DateTime CreatedOn { get { return (DateTime)Values["CreatedOn"]; } }

        string EditedBy { get { return Values["EditedBy"].ToString(); } }

        DateTime EditedOn { get { return (DateTime)Values["EditedOn"]; } }

        string Url { get { return Values["Url"].ToString(); } }

        public InstantNotificationProcessor(Hashtable values)
        {
            Values = values;
        }
        public async Task<bool> AddNotification(Enums.NotificationType type, long? newsID, long? refrenceID, long entityID, Hashtable parameters = null, long? skillID = null, long? awardID = null, long? activityID = null)
        {
            switch (type)
            {

                case Enums.NotificationType.PostLike:
                    return await ProcessPostLikeNotification(newsID.ToString(), entityID);
                case Enums.NotificationType.PostComment:
                    return await ProcessPostCommentNotification(newsID.ToString(), entityID);
                case Enums.NotificationType.ProfileVisited:
                    return await AddOpportunityNotification(null, entityID, type);
                case Enums.NotificationType.GroupInvitation:
                    return await AddOpportunityNotification(null, entityID, type, null, refrenceID);
                case Enums.NotificationType.OpportunityResponse:
                    return await AddOpportunityNotification(newsID.ToString(), entityID, type);
                case Enums.NotificationType.OpportunityReference:
                    return await AddOpportunityNotification(newsID.ToString(), entityID, type);
                case Enums.NotificationType.OpportunityApplied:
                    return await AddOpportunityNotification(newsID.ToString(), entityID, type);
                case Enums.NotificationType.ReferenceForAnOpportunity:
                    return await AddOpportunityNotification(newsID.ToString(), entityID, type);
                case Enums.NotificationType.ReferedForAnOpportunity:
                    return await AddOpportunityNotification(newsID.ToString(), entityID, type);
                case Enums.NotificationType.ReferenceOnAnOpportunity:
                    return await AddOpportunityNotification(newsID.ToString(), entityID, type);
                case Enums.NotificationType.Reshare:
                    return await ProcessPostReshareNotification(newsID.ToString(), entityID);
                //case Enums.NotificationType.PostTaggedInLike:
                //return ProcessPostTaggedInLikeNotification(type, sourceID, entityID);
                //case Enums.NotificationType.PostTaggedInComment:
                //return ProcessPostTaggedInCommentNotification(type, sourceID, entityID);
                case Enums.NotificationType.UserAsksForReccomend:
                    return await ReferenceNotification(refrenceID, entityID, type, skillID);
                case Enums.NotificationType.UserRecommendsMe:
                    return await ReferenceNotification(refrenceID, entityID, type, skillID);
                case Enums.NotificationType.UserAppriciatesMe:
                    return await ReferenceNotification(refrenceID, entityID, type, skillID);
                case Enums.NotificationType.Endoreced:
                    return await EndorsedNotification(skillID, entityID);
                case Enums.NotificationType.AwardCongratulated:
                    return await AwardCongratulatedNotification(awardID, entityID);

                case Enums.NotificationType.WelcomedOnJoiningCompany:
                    return await ActionOnJoining(activityID, entityID, type);
                case Enums.NotificationType.LikedAnActivity:
                    return await ActionOnJoining(activityID, entityID, type);
                case Enums.NotificationType.WellWishOnJoiningCompany:
                    return await ActionOnJoining(activityID, entityID, type);
                case Enums.NotificationType.CongratulateOnAnniversary:
                    return await ActionOnJoining(activityID, entityID, type);
                default:
                    return true;
            }
        }

        private async Task<bool> AddOpportunityNotification(string newsID, long entityID, Enums.NotificationType type, long? referenceID = null, long? referenceEntityID = null)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)type,
                Updatable = false,
                Url = Url,
                NewsID = newsID != null ? Convert.ToInt64(newsID) : default(long?),
                EntityID = entityID,
                ReferenceID = referenceID,
                ReferenceEntityID = referenceEntityID
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add(new Data.Models.NotificationSubscriber() { EntityID = entityID, Status = (byte)Enums.NotificationStatus.Pending, Subscribed = true });
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }

        private async Task<bool> ActionOnJoining(long? activityID, long entityID, Enums.NotificationType type)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)type,
                Updatable = false,
                Url = Url,
                NewsID = null,
                EntitySkillID = null,
                AwardID = null,
                ActivityID = activityID
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add(new Data.Models.NotificationSubscriber() { EntityID = entityID, Status = (byte)Enums.NotificationStatus.Pending, Subscribed = true });
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }

        private async Task<bool> AwardCongratulatedNotification(long? awardID, long entityID)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)Enums.NotificationType.AwardCongratulated,
                Updatable = false,
                Url = Url,
                NewsID = null,
                EntitySkillID = null,
                AwardID = awardID
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add(new Data.Models.NotificationSubscriber() { EntityID = entityID, Status = (byte)Enums.NotificationStatus.Pending, Subscribed = true });
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }

        private async Task<bool> EndorsedNotification(long? skillID, long entityID)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)Enums.NotificationType.Endoreced,
                Updatable = true,
                Url = Url,
                NewsID = null,
                EntitySkillID = skillID
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add(new Data.Models.NotificationSubscriber() { EntityID = entityID, Status = (byte)Enums.NotificationStatus.Pending, Subscribed = true });
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }

        private async Task<bool> ProcessPostReshareNotification(string sourceID, long entityID)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)Enums.NotificationType.Reshare,
                Updatable = false,
                Url = Url,
                NewsID = Convert.ToInt64(sourceID)
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add(new Data.Models.NotificationSubscriber() { EntityID = entityID, Status = (byte)Enums.NotificationStatus.Pending, Subscribed = true });
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }

        private bool ProcessPostAlsoCommentedByConnectionNotification(Enums.NotificationType type, string sourceID, long entityID)
        {
            throw new NotImplementedException();
        }

        private bool ProcessPostAlsoLikedByConnectionNotification(Enums.NotificationType type, string sourceID, long entityID)
        {
            throw new NotImplementedException();
        }

        private bool ProcessPostIAmAppriciatedNotification(Enums.NotificationType type, string sourceID, long entityID)
        {
            throw new NotImplementedException();
        }

        private bool ProcessPostIAmRecommendedNotification(Enums.NotificationType type, string sourceID, long entityID)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> ReferenceNotification(long? refrenceID, long entityID, Enums.NotificationType type, long? entitySkillID)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)type,
                Updatable = false,
                Url = Url,
                ReferenceID = refrenceID,
                EntitySkillID = entitySkillID
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add
            (
                        new Data.Models.NotificationSubscriber()
                        {
                            EntityID = entityID,
                            Status = (byte)Enums.NotificationStatus.Pending,
                            Subscribed = true
                        }
            );
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }

        private bool ProcessPostTaggedInCommentNotification(Enums.NotificationType type, string sourceID, long entityID)
        {
            throw new NotImplementedException();
        }

        private bool ProcessPostTaggedInLikeNotification(Enums.NotificationType type, string sourceID, long entityName)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> ProcessPostCommentNotification(string sourceID, long entityID)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)Enums.NotificationType.PostComment,
                Updatable = false,
                Url = Url,
                NewsID = Convert.ToInt64(sourceID)
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add(new Data.Models.NotificationSubscriber() { EntityID = entityID, Status = (byte)Enums.NotificationStatus.Pending, Subscribed = true });
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }

        private async Task<bool> ProcessPostLikeNotification(string sourceID, long entityID)
        {
            var notification = new Wrly.Data.Models.Notificaction()
            {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                EditedBy = EditedBy,
                EditedOn = EditedOn,
                Type = (int)Enums.NotificationType.PostLike,
                Updatable = false,
                Url = Url,
                NewsID = Convert.ToInt64(sourceID)
            };
            notification.NotificationSubscribers = new HashSet<Wrly.Data.Models.NotificationSubscriber>();
            notification.NotificationSubscribers.Add(new Data.Models.NotificationSubscriber() { EntityID = entityID, Status = (byte)Enums.NotificationStatus.Pending, Subscribed = true });
            using (var repository = new NotificationRepository())
            {
                return await repository.Save(notification);
            }
        }


    }
}