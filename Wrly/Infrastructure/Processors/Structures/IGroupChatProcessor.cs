using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models;
using Wrly.Models.Chat;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IGroupChatProcessor
    {
        Task<List<ChatGroupMessageModel>> GetChatHistory(long group, int page, int pageSize);
        Task<GroupChatViewModel> StartChat(string userID);
        Task<List<ChatFaceViewModel>> GetChatFaces(int page, int pageSize, long? groupID = null);
        Task<GroupChatViewModel> ChatSession(long eID);
        Task UpdateMessageStatusForGroup(long groupID, int status);
        Task<Result> Send(GroupChatViewModel model, string message);
        Task UpdateLastSeen();
    }
}
