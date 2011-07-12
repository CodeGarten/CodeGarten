using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CodeGarten.Web.Model
{
    public static class ModeStateExtension
    {
        public static JsonResult ToJson(this ModelStateDictionary modelState)
        {
            return new JsonResult()
                                 {
                                     Data = new
                                                 {
                                                     Errors =
                                                 modelState.Where(ms => ms.Value.Errors.Count > 0).Select(
                                                     ms =>
                                                     new {Field = ms.Key, Error = ms.Value.Errors.First().ErrorMessage})
                                                 },
                                     JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                 };
        }
    }
}