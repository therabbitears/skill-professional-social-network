using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Models;
using Wrly.Models.Listing;

namespace Wrly.Infrastructure.Extended
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequeiredIfSelected : ValidationAttribute, IClientValidatable
    {

        public RequeiredIfSelected(string propertyName, Enums.CareerStage mode = Enums.CareerStage.Employement, bool validateMode = true)
        {
            this.PropertyName = propertyName;
            this.Mode = mode;
        }

        public Enums.CareerStage Mode { get; set; }
        public string PropertyName { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as CareerHistoryWizardViewModel;
            if (Mode == Enums.CareerStage.None || (Mode == Enums.CareerStage.Employement && model == null) || model.CareerStage == (int)Mode)
            {
                if (value == null || (int)value == -1)
                {
                    var otherProperty = validationContext.ObjectInstance.GetType()
                                       .GetProperty(PropertyName);

                    var otherPropertyValue = otherProperty
                                  .GetValue(validationContext.ObjectInstance, null);

                    if (otherPropertyValue != null && (int)otherPropertyValue > 0)
                    {
                        return new ValidationResult(
                          FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requeiredifselected"
            };
            clientValidationRule.ValidationParameters.Add("otherproperty", PropertyName);
            clientValidationRule.ValidationParameters.Add("mode", (int)Mode);
            return new[] { clientValidationRule };
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IfCheckedNeedToSelect : ValidationAttribute, IClientValidatable
    {
        public IfCheckedNeedToSelect(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as CareerHistoryWizardViewModel;
            if (!model.Working)
            {
                if (value == null || (int)value == -1)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "ifcheckedneedtoselect"
            };

            clientValidationRule.ValidationParameters.Add("otherproperty", PropertyName);
            return new[] { clientValidationRule };
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CannotGreaterMonthAndYear : ValidationAttribute, IClientValidatable
    {
        public CannotGreaterMonthAndYear(string propertyName, string otherPropertyMonth, string otherPropertyYear, bool isStart, bool isMonth, bool isYear)
        {
            this.FriendPropertyName = propertyName;
            this.OtherPropertyMonth = otherPropertyMonth;
            this.OtherPropertyYear = otherPropertyYear;
            this.IsStart = isStart;
            this.IsMonth = isMonth;
            this.IsYear = isYear;
        }

        public string FriendPropertyName { get; private set; }
        public string OtherPropertyMonth { get; private set; }
        public string OtherPropertyYear { get; private set; }
        public bool IsStart { get; private set; }
        public bool IsMonth { get; private set; }
        public bool IsYear { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyYear = (int?)validationContext.ObjectInstance.GetType().GetProperty(OtherPropertyYear).GetValue(validationContext.ObjectInstance, null); ;
            var otherPropertyMonth = (int?)validationContext.ObjectInstance.GetType().GetProperty(OtherPropertyMonth).GetValue(validationContext.ObjectInstance, null); ;
            var friendProperty = (int?)validationContext.ObjectInstance.GetType().GetProperty(FriendPropertyName).GetValue(validationContext.ObjectInstance, null); ;
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow;
            if (otherPropertyMonth > 0 && otherPropertyYear > 0 && friendProperty > 0)
            {
                if (IsStart == true)
                {
                    if (IsMonth == true)
                    {
                        startDate = new DateTime((int)friendProperty, (int)value, 1);
                    }
                    else
                    {
                        startDate = new DateTime((int)value, (int)friendProperty, 1);
                    }
                    endDate = new DateTime((int)otherPropertyYear, (int)otherPropertyMonth, 1);
                }
                else
                {
                    if (IsMonth == true && friendProperty > 0)
                    {
                        endDate = new DateTime((int)friendProperty, (int)value, 1);
                    }
                    else
                    {
                        endDate = new DateTime((int)value, (int)friendProperty, 1);
                    }
                    startDate = new DateTime((int)otherPropertyYear, (int)otherPropertyMonth, 1);
                }
                if (startDate > endDate)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "cannotgreatermonthandyear"
            };
            clientValidationRule.ValidationParameters.Add("friendproperty", FriendPropertyName);
            clientValidationRule.ValidationParameters.Add("otherpropertymonth", OtherPropertyMonth);
            clientValidationRule.ValidationParameters.Add("otherpropertyyear", OtherPropertyYear);
            clientValidationRule.ValidationParameters.Add("isstart", IsStart);
            clientValidationRule.ValidationParameters.Add("ismonth", IsMonth);
            clientValidationRule.ValidationParameters.Add("isyear", IsYear);
            return new[] { clientValidationRule };
        }
    }
}