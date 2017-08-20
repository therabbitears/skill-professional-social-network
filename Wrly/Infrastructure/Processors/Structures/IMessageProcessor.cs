using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Models;
using Wrly.Models.Chat;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IMessageProcessor
    {
        Task<List<MessageViewModel>> GetAll(int? status = null);
        Task<Result> Acknowledge(long? groupID = null);
    }
}
