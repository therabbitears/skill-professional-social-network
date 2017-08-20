using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models.Chat;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Models;
using Wrly.Data.Models;
using Types;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class GroupChatProcessor : BaseProcessor, IGroupChatProcessor
    {

        public async System.Threading.Tasks.Task<List<ChatGroupMessageModel>> GetChatHistory(long group, int page, int pageSize)
        {
            using (var repository = new ChatRepository())
            {
                var groupChat = await repository.GetGroupChatHistory(group, page, pageSize);
                return CreateModelFromDataSet(groupChat);
            }
        }

        private List<ChatGroupMessageModel> CreateModelFromDataSet(System.Data.DataSet groupChat)
        {
            return (from c in groupChat.Tables[0].AsEnumerable()
                    select new ChatGroupMessageModel()
                    {
                        GroupID = Convert.ToInt64(c["groupId"]),
                        ID = Convert.ToInt64(c["Id"]),
                        Message = Convert.ToString(c["Message"]),
                        MessageType = Convert.ToInt32(c["MessageType"]),
                        Name = Convert.ToString(c["FormatedName"] != DBNull.Value ? c["FormatedName"] : c["Name"]),
                        RecordedDate = Convert.ToDateTime(c["CreatedOn"]),
                        UserID = Convert.ToInt64(c["EntityID"]),
                        IsFromCurrentUser = Convert.ToInt64(c["EntityID"]) == UserHashObject.EntityID
                    }).ToList();
        }

        public async System.Threading.Tasks.Task<GroupChatViewModel> StartChat(string userID)
        {
            using (var accountRepositoty = new AccountRepository())
            {
                using (var dsUserInfo = await accountRepositoty.GetChatProfile(userID))
                {
                    var profileViewModel = dsUserInfo.Tables[0].FromDataTable<AuthorViewModel>()[0];
                    using (var groupRepository = new ChatRepository())
                    {
                        var dsGroup = await groupRepository.FindOrCreateGroupByUsers(UserHashObject.EntityID, profileViewModel.EntityID);
                        var model = new GroupChatViewModel()
                        {
                            GroupInfo = dsGroup.Tables[0].FromDataTable<GroupViewModel>()[0],
                            MemberInfo = profileViewModel
                        };
                        model.GroupInfo.Participants = dsGroup.Tables[1].FromDataTable<AuthorViewModel>();
                        return model;
                    }
                }
            }
        }


        public async System.Threading.Tasks.Task<List<ChatFaceViewModel>> GetChatFaces(int page, int pageSize, long? groupID = null)
        {
            using (var repository = new ChatRepository())
            {
                var groupChat = await repository.GetChatFace(UserHashObject.EntityID, page, pageSize, groupID);
                return groupChat.Tables[0].FromDataTable<ChatFaceViewModel>();
            }
        }


        public async System.Threading.Tasks.Task<GroupChatViewModel> ChatSession(long eID)
        {
            using (var accountRepositoty = new AccountRepository())
            {
                using (var dsUserInfo = await accountRepositoty.GetProfile(eID))
                {
                    var profileViewModel = dsUserInfo.Tables[0].FromDataTable<AuthorViewModel>()[0];
                    using (var groupRepository = new ChatRepository())
                    {
                        var dsGroup = await groupRepository.FindOrCreateGroupByUsers(UserHashObject.EntityID, profileViewModel.EntityID);
                        var model = new GroupChatViewModel()
                        {
                            GroupInfo = dsGroup.Tables[0].FromDataTable<GroupViewModel>()[0],
                            MemberInfo = profileViewModel
                        };
                        model.GroupInfo.Participants = dsGroup.Tables[1].FromDataTable<AuthorViewModel>();
                        return model;
                    }
                }
            }
        }


        public async System.Threading.Tasks.Task UpdateMessageStatusForGroup(long groupID, int status)
        {
            using (var groupRepository = new ChatRepository())
            {
                await groupRepository.UpdateReadStatus(groupID, status, UserHashObject.EntityID);
            }
        }


        public async System.Threading.Tasks.Task<Result> Send(GroupChatViewModel model, string message)
        {
            using (var repository = new ChatRepository())
            {
                var groupMessage = new ChatGroupMessage()
                {
                    GroupID = model.GroupInfo.ID,
                    Message = message,
                    MessageType = (int)Enums.MessageType.Text,
                    CreatedOn = DateTime.UtcNow,
                    EntityID = UserHashObject.EntityID,
                    Status = (int)Enums.MessageReadingStatus.Pending
                };
                var result = await repository.InsertMessageToGroup(groupMessage);
                if (result > 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = "Message has been sent." };
                else
                    return new Result() { Type = Enums.ResultType.Error, Description = "There is an error sending message, please give it another try." };
            }
        }


        public async System.Threading.Tasks.Task UpdateLastSeen()
        {
            using (AccountRepository repository = new AccountRepository())
            {
                await repository.SetLastMessageSeenData(Now, UserHashObject.EntityID);
            }
        }
    }
}