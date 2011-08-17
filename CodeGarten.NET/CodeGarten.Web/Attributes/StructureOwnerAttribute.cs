using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Attributes
{
    public sealed class StructureOwnerAttribute : AuthorizeAttribute
    {
        private String StructureIdField { get; set; }

        public StructureOwnerAttribute(string structureIdField)
        {
            StructureIdField = structureIdField;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var dataBaseManager = httpContext.Items["DataBaseManager"] as DataBaseManager;

            var user = dataBaseManager.User.Get(httpContext.User.Identity.Name);
            var structureId =
                Int32.Parse((httpContext.Request.RequestContext.RouteData.Values[StructureIdField] ??
                             httpContext.Request[StructureIdField]) as string);

            if(dataBaseManager.Structure.Get(structureId) == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());

            return user.Structures.Select(s => s.Id).Contains(structureId);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            throw new HttpException((int)HttpStatusCode.Forbidden, HttpStatusCode.Forbidden.ToString());
        }
    }
}