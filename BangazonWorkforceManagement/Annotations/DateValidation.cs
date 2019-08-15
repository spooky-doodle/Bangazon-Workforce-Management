//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Mvc;

//namespace BangazonWorkforceManagement.Annotations
//{
//    public class DateValidation : ValidationAttribute
//    {

//        private DateTime _minDate;

//        public DateValidation()
//        {
//            _minDate = DateTime.Today;
//        }
//        public DateValidation(string minDate)
//        {
//            _minDate = DateTime.Parse(minDate, CultureInfo.InvariantCulture);
//        }
//        protected override ValidationResult IsValid(
//            object value, 
//            ValidationContext validationContext
//            )
//        {
//            DateTime targetDate = DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture);
//            int compare = targetDate.CompareTo(_minDate);
//            if (compare >= 0)
//            {
//                // Target date is after min date
//                return ValidationResult.Success;
//            }
//            return new ValidationResult(ErrorMessage);
//        }

//        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
//        {
//            var rule = new ModelClientValidationRule
//            {
//                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
//                ValidationType = "restrictbackdates",
//            };
//            rule.ValidationParameters.Add("mindate", _minDate);
//            yield return rule;
//        }
//    }
//}
