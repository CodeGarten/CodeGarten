using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CodeGarten.Web.Core
{
    public class FormValidationResponse
    {
        public static JsonResult Ok()
        {
            return new JsonResult()
                       {
                           Data = new {Success = true},
                           JsonRequestBehavior = JsonRequestBehavior.AllowGet
                       };
        }

        public static JsonResult Error(ModelStateDictionary modelState)
        {
            return new JsonResult()
            {
                Data = new
                {
                    Success = false,
                    Errors =
                modelState.Where(ms => ms.Value.Errors.Count > 0).Select(
                    ms =>
                    new { Field = ms.Key, Error = ms.Value.Errors.First().ErrorMessage })
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
    public static class ModeStateExtension
    {

        public static void AddGlobalError(this ModelStateDictionary modelState, string msg)
        {
            modelState.AddModelError("form", msg);
        }

        public static bool IsValidField(this ModelStateDictionary modelSate, string key, IValidatableObject validatableObject)
        {
            if (!modelSate.IsValidField(key)) return false;

            var validationResults = validatableObject.Validate(new ValidationContext(validatableObject, null, null));
            foreach (var validationResult in validationResults)
                if (validationResult.MemberNames.Contains(key))
                {
                    modelSate.AddModelError(key, validationResult.ErrorMessage);
                    return false;
                }
            return true;
        }
    }
}