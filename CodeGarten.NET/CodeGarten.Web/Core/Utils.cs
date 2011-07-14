using System;
using System.Collections.Generic;
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
    }
}