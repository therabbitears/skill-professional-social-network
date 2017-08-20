using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Types;
using Wrly.Data.Models;
using Wrly.Models;
using Wrly.Models.Listing;
using Wrly.Models.Share;
using Wrly.Models.Feeds;
using Wrly.Models.Import;
using Wrly.Models.Business;
using Wrly.Models.Knowledge;
using Wrly.Data.Models.Extended;

namespace Wrly.App_Start
{
    public static class AutoMapperConfig
    {
        internal static void RegisterMappings()
        {

            Mapper.CreateMap<OrganizationSignupViewModel, Organization>()
          .ForMember(src => src.CategoryID, dest => dest.MapFrom(c => c.IndustryID))
          .ForMember(src => src.Name, dest => dest.MapFrom(c => c.Name));

            Mapper.CreateMap<OrganizationSignupViewModel, OrganizationProfile>()
                .ForMember(src => src.Website, dest => dest.MapFrom(c => c.Website));

            Mapper.CreateMap<ProfileHashViewModel, EntityState>();

            Mapper.CreateMap<OrganizationSignupViewModel, Email>()
                .ForMember(src => src.EmailAddress, dest => dest.MapFrom(c => c.EmailAddress))
                .ForMember(src => src.EmailType, dest => dest.UseValue(Enums.EmailType.Primary))
                .ForMember(src => src.Active, dest => dest.UseValue(true));

            Mapper.CreateMap<NewOrganizationViewModel, Organization>()
                .ForMember(src => src.CategoryID, dest => dest.MapFrom(c => c.IndustryID))
                .ForMember(src => src.EmployeeStrength, dest => dest.MapFrom(c => c.EmployeeStrength))
                .ForMember(src => src.EstablishedYear, dest => dest.MapFrom(c => c.EstablishedYear))
                .ForMember(src => src.Name, dest => dest.MapFrom(c => c.Name));

            Mapper.CreateMap<NewOrganizationViewModel, OrganizationProfile>()
                .ForMember(src => src.Website, dest => dest.MapFrom(c => c.Website))
                .ForMember(src => src.TagLine, dest => dest.MapFrom(c => c.Slogan));

            Mapper.CreateMap<GroupViewModel, Organization>()
              .ForMember(src => src.CategoryID, dest => dest.MapFrom(c => c.IndustryID))
              .ForMember(src => src.Name, dest => dest.MapFrom(c => c.Name));

            Mapper.CreateMap<GroupViewModel, OrganizationProfile>()
                .ForMember(src => src.TagLine, dest => dest.MapFrom(c => c.Slogan));


            Mapper.CreateMap<AddressViewModel, Address>()
                .ForMember(src => src.City, dest => dest.MapFrom(c => c.City))
                .ForMember(src => src.State, dest => dest.MapFrom(c => c.State))
                .ForMember(src => src.ZipCode, dest => dest.MapFrom(c => c.ZipCode))
                .ForMember(src => src.Country, dest => dest.MapFrom(c => c.Country));

            Mapper.CreateMap<NewOrganizationViewModel, Address>()
                .ForMember(src => src.City, dest => dest.MapFrom(c => c.CityName))
                .ForMember(src => src.State, dest => dest.MapFrom(c => c.StateName))
                .ForMember(src => src.ZipCode, dest => dest.MapFrom(c => c.ZipCode))
                .ForMember(src => src.Country, dest => dest.MapFrom(c => c.Country))
                .ForMember(src => src.County, dest => dest.MapFrom(c => c.County));

            Mapper.CreateMap<NewOrganizationViewModel, Email>()
                .ForMember(src => src.EmailAddress, dest => dest.MapFrom(c => c.EmailAddress))
                .ForMember(src => src.EmailType, dest => dest.UseValue(Enums.EmailType.Primary))
                .ForMember(src => src.Active, dest => dest.UseValue(true));

            Mapper.CreateMap<NewOrganizationViewModel, Phone>()
                .ForMember(src => src.Phone1, dest => dest.MapFrom(c => c.Phone1))
                .ForMember(src => src.PhoneType, dest => dest.UseValue(Enums.PhoneType.Primary))
                .ForMember(src => src.Active, dest => dest.UseValue(true));


            Mapper.CreateMap<ExtendedInfoViewModel, Phone>()
                .ForMember(src => src.Phone1, dest => dest.MapFrom(c => c.Phone1))
                .ForMember(src => src.PhoneType, dest => dest.UseValue(Enums.PhoneType.Primary))
                .ForMember(src => src.Active, dest => dest.UseValue(true));


            Mapper.CreateMap<ConnectAccountViewModel, CareerHistory>();

            Mapper.CreateMap<ConnectAccountViewModel, JobTitle>();

            Mapper.CreateMap<CareerHistoryViewModel, CareerHistory>()
                .ForMember(src => src.OrganizationID, dest => dest.MapFrom(c => c.OrganizationID > 0 ? c.OrganizationID : default(long?)))
                .ForMember(src => src.OrganizationName, dest => dest.MapFrom(c => c.OrganizationName))
                .ForMember(src => src.JobTitleID, dest => dest.MapFrom(c => c.JobTitleID))
                .ForMember(src => src.CareerHistoryID, dest => dest.MapFrom(c => c.CareerHistoryID));

            Mapper.CreateMap<CareerHistoryViewModel, JobTitle>()
                .ForMember(src => src.Name, dest => dest.MapFrom(c => c.JobTitleName))
                .ForMember(src => src.JobTitleID, dest => dest.MapFrom(c => c.JobTitleID));

            Mapper.CreateMap<SkillViewModel, EntitySkill>();
            Mapper.CreateMap<SkillViewModel, Skill>();

            Mapper.CreateMap<AwardViewModel, EntityAwardAndCompletion>();

            Mapper.CreateMap<AppreciationAndRecommendationViewModel, AppreciationAndRecommendation>();

            Mapper.CreateMap<AskRecommendationViewModel, AppreciationAndRecommendation>();

            Mapper.CreateMap<SendAssociationViewModel, Association>();

            Mapper.CreateMap<AssociateProfileViewModel, Association>();

            Mapper.CreateMap<BaseViewModel, EntityWidget>();

            Mapper.CreateMap<AccomplishmentReportViewModel, AccomplishmentState>();

            Mapper.CreateMap<SkillViewModel, EntitySkillState>();

            Mapper.CreateMap<AwardViewModel, AccomplishmentState>();

            Mapper.CreateMap<NewsViewModel, Post>()
                .ForMember(src => src.ShortDescription, dest => dest.MapFrom(c => c.Text))
                .ForMember(src => src.PostType, dest => dest.MapFrom(c => c.Feedtype));

            Mapper.CreateMap<AskOpportunityViewModel, Post>()
                .ForMember(src => src.ShortDescription, dest => dest.MapFrom(c => c.Text))
                .ForMember(src => src.PostType, dest => dest.UseValue((int)Enums.PostTypes.AskOpportunity))
                .ForMember(src => src.Skills, dest => dest.Ignore())
                .ForMember(src => src.JobTitles, dest => dest.Ignore());

            Mapper.CreateMap<ShareOpportunityViewModel, Post>()
                .ForMember(src => src.ShortDescription, dest => dest.MapFrom(c => c.Text))
                .ForMember(src => src.PostType, dest => dest.UseValue((int)Enums.PostTypes.ShareOpportunity))
                .ForMember(src => src.Skills, dest => dest.Ignore())
                .ForMember(src => src.JobTitles, dest => dest.Ignore());

            Mapper.CreateMap<PostViewModel, Post>();

            Mapper.CreateMap<CommentViewModel, PostReply>()
                .ForMember(src => src.Reply, dest => dest.MapFrom(c => c.Comment))
                .ForMember(src => src.ReplyType, dest => dest.UseValue((byte)Enums.ReplyType.CommentOnPost));


            Mapper.CreateMap<FeedDetailViewModel, PostInteraction>();

            Mapper.CreateMap<ReplyViewModel, PostReplyInteraction>();


            Mapper.CreateMap<CareerHistoryWizardViewModel, CareerHistory>()
                .ForMember(src => src.OrganizationID, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? (c.OrganizationID > 0 ? c.OrganizationID : default(long?)) : (c.UniversityID > 0 ? c.UniversityID : default(long?))))
                .ForMember(src => src.OrganizationName, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.OrganizationName : c.UniversityName))
                .ForMember(src => src.JobTitleID, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.JobTitleID : c.CourseID))
                .ForMember(src => src.StartFromMonth, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.StartFromMonth : c.EducationStartFromMonth))
                .ForMember(src => src.StartFromYear, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.StartFromYear : c.EducationStartFromYear))
                .ForMember(src => src.EndFromMonth, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.EndFromMonth : c.EducationEndFromMonth))
                .ForMember(src => src.EndFromYear, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.EndFromYear : c.EducationEndFromYear))
                .ForMember(src => src.CareerHistoryID, dest => dest.MapFrom(c => c.CareerHistoryID));

            Mapper.CreateMap<CareerHistoryWizardViewModel, JobTitle>()
                .ForMember(src => src.JobTitleID, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.JobTitleID : c.CourseID))
                .ForMember(src => src.Name, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? c.JobTitleName : c.CourseName))
                .ForMember(src => src.Type, dest => dest.MapFrom(c => c.CareerStage == (int)Enums.CareerStage.Employement ? (int)Enums.JobtTitleType.Professional : (int)Enums.JobtTitleType.Educational));

            Mapper.CreateMap<CareerHistoryWizardViewModel, OrganizationProfile>();
            Mapper.CreateMap<CareerHistoryViewModel, OrganizationProfile>();
            Mapper.CreateMap<ListSkillViewModel, Skill>();
            Mapper.CreateMap<ListSkillViewModel, EntitySkill>();

            Mapper.CreateMap<ActionAssociateProfileViewModel, Association>()
                .ForMember(src => src.AssociationID, dest => dest.MapFrom(c => c.AssociationID));

            Mapper.CreateMap<EntitySearchViewModel, EntitySearch>();

            Mapper.CreateMap<HappeningsViewModel, ActivityAction>();


            Mapper.CreateMap<ContactViewModel, EntityImportContact>()
                .ForMember(src => src.Name, dest => dest.MapFrom(c => c.Name))
                .ForMember(src => src.EmailAddresses, dest => dest.MapFrom(c => string.Join(",", c.EmailList)));

            Mapper.CreateMap<ContactViewModel, EntityImportInvite>()
            .ForMember(src => src.InviteType, dest => dest.UseValue((int)Enums.InviteType.Email))
            .ForMember(src => src.CreatedOn, dest => dest.UseValue(DateTime.UtcNow))
            .ForMember(src => src.EntityImportContactID, dest => dest.MapFrom(c => c.ID));

            Mapper.CreateMap<PhoneViewModel, Phone>();
            Mapper.CreateMap<TopicViewModel, PageTopic>();
        }
    }
}