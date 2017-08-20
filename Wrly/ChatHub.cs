using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity;
using Wrly.Models.Chat;
using Types;
using Wrly.Data.Models;
using Wrly.Infrastuctures.Utils;
using Wrly.Data.Repositories.Implementors;
using System.Web.Mvc;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        public enum MessageType
        {
            Mesaage = 1,
            Notification = 2,
            CurrentTypingStatus = 3,
            ChatClosed = 4,
            FileSending = 5
        }

        #region Data Members

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        #endregion

        #region Methods

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
            else { OnReconnected(); }
        }

        [ValidateInput(true)]
        public Task SendToUser(string userName, string message, int messageType, string currentUserID, long entityId, string currentUserName, long groupID, int messageID)
        {
            return Task.Run(async () =>
            {
                // Broad cast message
                await Clients.User(userName).messageReceived(currentUserName, message, messageType, currentUserID, groupID);
                await Clients.Caller.____callack((int)Enums.MessageCallbackFor.ReceivedToServer, messageID);
                await InsertMessage(entityId, message, groupID, (Enums.MessageType)messageType);
            });
        }

        public Task sendReadReceipt(long groupID)
        {
            return Task.Run(async () =>
            {
                await UpdateReadStatusForReceiver(groupID);
            });
        }

        private async Task UpdateReadStatusForReceiver(long groupID)
        {
            using (var repository = new ChatRepository())
            {
                await repository.UpdateReadStatus(groupID, (int)Enums.MessageReadingStatus.ReadByReceiver);
            }
        }

        public Task SendTypingStatus(string userName, bool isTyping)
        {
            return Task.Run(async () =>
            {
                // Broad cast message
                await Clients.User(userName).setTypingStatus(isTyping);
            });
        }

        private async Task InsertMessage(long entityId, string message, long groupID, Enums.MessageType messageType)
        {
            using (var repository = new ChatRepository())
            {
                ChatGroupMessage groupMessage = new ChatGroupMessage()
                {
                    GroupID = groupID,
                    Message = message,
                    MessageType = (int)messageType,
                    CreatedOn = DateTime.UtcNow,
                    EntityID = entityId,
                    Status = (int)Enums.MessageReadingStatus.Pending
                };
                await repository.InsertMessageToGroup(groupMessage);
            }
        }

        #endregion

    }

    public class MyConnectionFactory : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.User.Identity.Name;
        }
    }
}