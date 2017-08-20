using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Types;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models.Chat;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class MessageProcessor : BaseProcessor, IMessageProcessor
    {
        public async Task<List<MessageViewModel>> GetAll(int? status = null)
        {
            if (true)
            {
                using (AccountRepository repository = new AccountRepository())
                {
                    await repository.SetLastMessageSeenData(Now, UserHashObject.EntityID);
                }
            }
            using (var repository = new ChatRepository())
            {
                var groupChat = await repository.GetChatFace(UserHashObject.EntityID, 0, 20, null);
                return groupChat.Tables[0].FromDataTable<MessageViewModel>();
            }
        }


        public async Task<Models.Result> Acknowledge(long? groupID = null)
        {
            using (var repository = new ChatRepository())
            {
                var result = await repository.Acknowledge(groupID, UserHashObject.EntityID);
                if (result > 0)
                    return new Models.Result() { Type = Enums.ResultType.Success, Description = "All the conversations has been marked as Acknowledged.", ReferenceID = groupID };
                else
                    return new Models.Result() { Type = Enums.ResultType.Error, Description = "Error while processing the requuest, please make another try.", ReferenceID = groupID };
            }
        }
    }
}