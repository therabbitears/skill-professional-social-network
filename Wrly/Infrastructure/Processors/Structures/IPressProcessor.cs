using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models;
using Wrly.Models.Feeds;
using Wrly.Models.Share;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IPressProcessor
    {
        Task<long> Create(PostViewModel model, byte[] arr);
        Task<FeedDetailViewModel> GetSingle(long id);
        Task<PostViewModel> GetSingle(string url);
        List<ReplyViewModel> GetRepliesForPost(long? nullable);
        Task<long> InsertNewReply(PostViewModel model);
        int Statistics(long id, string action, string en);
        PostOptionsViewModel GetQuestionOptions(int id);
        List<PostViewModel> GetUserAnsweredQuestions(string userName);
        List<PostViewModel> GetUserAskedQuestions(string userName);

        Task<Result> ShareNews(Models.Share.NewsViewModel model);
        Task<Result> ShareNewsGroup(NewsViewModel model, string q);

        Task<Result> AddComment(CommentViewModel model);

        Task<ReplyViewModel> GetSingleReply(long replyID);

        Task<FeedDetailViewModel> GetNewsForReshare(string q);
        Task<HomeFeedViewModel> Feeds(int pageNumber, DateTime? stamp, Types.Enums.FeedType type = Types.Enums.FeedType.Default);
        Task<Result> Reshare(NewsViewModel model);
        Task<Result> React(string q, string reactType, string description, long? entityID2 = default(long?));
        Task<Result> ReactReply(string q, string reactType);
        Task<List<ReplyViewModel>> MoreReplies(string q, int stock, int pageNumber, int pageSize = 10);
        Task<HomeFeedViewModel> TimeLineFeeds(string profileHash, int pageNo, int pageSize, bool group=false);
        Task<List<FeedOption>> MoreOptionsPost(string q);
        Task<FeedDetailViewModel> Report(string q);
        Task<PostViewModel> GetFromDraft(string id);
        Task<FeedDetailViewModel> Detail(long postID);
        Task<List<FeedDetailViewModel>> ReshareMap(long postID);
        Task<List<FeedDetailViewModel>> Feed(long feedID);
        Task<Result> AskOpportunity(AskOpportunityViewModel model);
        Task<Result> ShareOpportunity(ShareOpportunityViewModel model);
        Task<Result> ApplyOpportunity(ApplyViewModel model);
        Task<Result> ReferOpportunity(RerefOpportunityViewModel model);
        Task<Result> ReferForAnOpportunity(RerefOpportunityViewModel model);
        Task<FeedInsightsViewModel> Insights(string q, string reactType);
        Task<List<InsightDetailsViewModel>> InsightDetails(string q, string type);
        Task<Result> Track(string trackType, string[] posts);
        Task<HomeFeedViewModel> GetArticles(int pageNumber, int pageSize);
    }
}
