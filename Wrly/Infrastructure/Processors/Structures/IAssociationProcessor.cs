using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Models;
using Wrly.Models.Listing;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IAssociationProcessor
    {
        Task<List<AssociateProfileViewModel>> GetSuggestions(int pageNumber, int pageSize);

        Task<long> SendConnectRequest(Models.SendAssociationViewModel model);

        Task<List<ActionAssociateProfileViewModel>> GetRequests(int pageNumber, int pageSize, Enums.AssociationRequestDirection direction = Enums.AssociationRequestDirection.Received);

        Task<long> Action(ActionAssociateProfileViewModel model,string actn);

        Task<List<AssociateProfileViewModel>> GetConnections(int pageNumber, int pageSize);
        Task<List<InviteAssociateProfileViewModel>> GetConnections(string q,string keyword, int pageNumber, int pageSize);

        Task<List<AssociateProfileViewModel>> GetFollowings(int pageNumber, int pageSize);

        Task<List<AssociateProfileViewModel>> GetFollowers(int pageNumber, int pageSize);

        Task<AssociateProfileActionViewModel> GetConnectionActions(string q);

        Task<List<HappeningsViewModel>> GetNetworkHappenings(int pageNumber, int pageSize);

        Task<Result> ActivityAction(BaseViewModel model, string action);

        Task<Result> FeedIntialNetworkData();

        Task<Result> PrepareStock();

        Task<List<AssociateProfileViewModel>> GetFollowers(long entityID, int pageNumber, int pageSize);

        Task<List<AssociateProfileViewModel>> GetConnections(long entityID, int pageNumber, int pageSize);

        Task<List<MyHappeningsViewModel>> GetMyHappenings(int pageNumber, int pageSize);

        Task<Result> InviteToGroup(string hash);

        Task<Result> ApproveAll(string hash);
    }
}
