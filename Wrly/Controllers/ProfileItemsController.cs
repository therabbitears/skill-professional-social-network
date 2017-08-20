using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    public class ProfileItemsController : BaseController
    {
        IAccountProcessor _AccountProcessor;
        IAccountProcessor AccountProcessor
        {
            get
            {
                if (_AccountProcessor == null)
                {
                    _AccountProcessor = new AccountProcessor();
                }
                return _AccountProcessor;
            }
        }

        IPushNotificationProcessor _PushNotificationProcessor;
        IPushNotificationProcessor PushNotificationProcessor
        {
            get
            {
                if (_PushNotificationProcessor == null)
                {
                    _PushNotificationProcessor = new PushNotificationProcessor();
                }
                return _PushNotificationProcessor;
            }
        }

        IMessageProcessor _MessageProcessor;
        IMessageProcessor MessageProcessor
        {
            get
            {
                if (_MessageProcessor == null)
                {
                    _MessageProcessor = new MessageProcessor();
                }
                return _MessageProcessor;
            }
        }

        ISkillHistoryProcessor _skillProcessor;
        public ISkillHistoryProcessor SkillProcessor
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

        IAwardProcessor _Processor;
        IAwardProcessor AwardProcessor
        {

            get
            {
                if (_Processor == null)
                {
                    _Processor = new AwardProcessor();
                }
                return _Processor;
            }
        }

        ICareerHistoryProcessor _CareerProcessor;
        ICareerHistoryProcessor CareerProcessor
        {

            get
            {
                if (_CareerProcessor == null)
                {
                    _CareerProcessor = new CareerHistoryProcessor();
                }
                return _CareerProcessor;
            }
        }

        IReferenceProcessor _ReferenceProcessor;
        IReferenceProcessor ReferenceProcessor
        {
            get
            {
                if (_ReferenceProcessor == null)
                {
                    _ReferenceProcessor = new ReferenceProcessor();
                }
                return _ReferenceProcessor;
            }
        }


        public async Task<ActionResult> Awards(string q)
        {
            var history = await AwardProcessor.GetAwardsForProfile(q);
            return WJson(history);
        }

        public async Task<ActionResult> ListProjects(string q)
        {
            var history = await AwardProcessor.GetAssignmentsForProfile(q);
            return WJson(history);
        }

        public async Task<ActionResult> Findings(string q)
        {
            var history = await AwardProcessor.GetFindingsForProfile(q);
            return WJson(history);
        }

        public async Task<ActionResult> Researches(string q)
        {
            var history = await AwardProcessor.GetResearchesForProfile(q);
            return WJson(history);
        }

        public async Task<ActionResult> Publications(string q)
        {
            var history = await AwardProcessor.GetPublicationForProfile(q);
            return WJson(history);
        }

        public async Task<ActionResult> Compositions(string q)
        {
            var history = await AwardProcessor.GetCompositionsForProfile(q);
            return WJson(history);
        }

        public async Task<ActionResult> career(string q)
        {
            var history = await CareerProcessor.GetCareerHisotryForProfile((int)Enums.CareerHistoryMode.Profession, q);
            return WJson(history);
        }

        [CompressFilter]
        public async Task<ActionResult> education(string q)
        {
            var history = await CareerProcessor.GetCareerHisotryForProfile((int)Enums.CareerHistoryMode.Education, q, Enums.EducationType.Course.ToString());
            return WJson(history);
        }

        [CompressFilter]
        public async Task<ActionResult> Certification(string q)
        {
            var history = await CareerProcessor.GetCareerHisotryForProfile((int)Enums.CareerHistoryMode.Education, q, Enums.EducationType.Certification.ToString());
            return WJson(history);
        }

        [CompressFilter]
        public async Task<ActionResult> Appriciations(string q)
        {
            var history = await ReferenceProcessor.GetAppriciationsForProfile(q);
            return WJson(history);
        }

        [CompressFilter]
        public async Task<ActionResult> Recommendation(string q)
        {
            var history = await ReferenceProcessor.GetRecommendationsForProfile(q);
            return WJson(history);
        }

        [CompressFilter]
        public async Task<ActionResult> Skill(string q)
        {
            var history = await SkillProcessor.GetSkillHisotryForProfile(q);
            return WJson(history);
        }

        // For business

        [CompressFilter]
        public async Task<ActionResult> Services(string q)
        {
            var history = await AwardProcessor.GetServicesForProfile(q);
            return WJson(history);
        }


        [CompressFilter]
        public async Task<ActionResult> Products(string q)
        {
            var history = await AwardProcessor.GetProductsForProfile(q);
            return WJson(history);
        }

        [CompressFilter]
        public async Task<ActionResult> Affiliations(string q)
        {
            var history = await CareerProcessor.GetCareerHisotryForProfile((int)Enums.CareerHistoryMode.Affiliation, q);
            return WJson(history);
        }


        [CompressFilter]
        public async Task<ActionResult> BusinessAwards(string q)
        {
            var history = await AwardProcessor.GetAwardsForProfile(q);
            return WJson(history);
        }

        [CompressFilter]
        public async Task<ActionResult> BusinessRecommendations(string q)
        {
            var history = await ReferenceProcessor.GetRecommendationsForProfile(q);
            return WJson(history);
        }


        [CompressFilter]
        public async Task<ActionResult> BusinessTestimonials(string q)
        {
            var history = await ReferenceProcessor.GetAppriciationsForProfile(q);
            return WJson(history);
        }

        ////////////////////////////// For Businenss

        [CompressFilter]
        public async Task<ActionResult> careerline(string q, string id, bool includeS = true, bool includeA = true)
        {
            var data = await AccountProcessor.GetCareerLineForProfile(q, id, includeS, includeA);
            return new JsonResult() { ContentType = "application/json", Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [CompressFilter]
        public async Task<ActionResult> CommonSkills(string q)
        {
            var data = await AccountProcessor.GetCommonSkills(q);
            return new JsonResult() { ContentType = "application/json", Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [CompressFilter]
        public async Task<ActionResult> CommonCompanies(string q)
        {
            var data = await AccountProcessor.GetCommonCompanies(q);
            return new JsonResult() { ContentType = "application/json", Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> ExecuteAction(string action, string entity)
        {
            if (entity.Equals("notifications", StringComparison.InvariantCultureIgnoreCase))
            {
                if (action.Equals("acknowledged", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = await PushNotificationProcessor.Acknowledge();
                    return WJson(data);
                }
            }
            if (entity.Equals("conversations", StringComparison.InvariantCultureIgnoreCase))
            {
                if (action.Equals("acknowledged", StringComparison.InvariantCultureIgnoreCase))
                {
                    var data = await MessageProcessor.Acknowledge();
                    return WJson(data);
                }
                if (action.Equals("new", StringComparison.InvariantCultureIgnoreCase))
                {
                    return PartialView("_NewConversationFace");
                }
            }
            return null;
        }

        [CompressFilter]
        public async Task<ActionResult> hovercard(string id)
        {
            var entityDetails = await AccountProcessor.HoverCard(id);
            return PartialView("_Hovercard", entityDetails);
        }

    }
}