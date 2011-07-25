using System;
using System.Collections.Generic;
using System.Text;
using CodeGarten.Data.Model;

namespace CodeGarten.Service.Utils
{
    public static class ServiceExtensions
    {
        public static string UniqueGroupName(this Container container, Role role)
        {
            return String.Format("G_{0}_{1}", container.Id, role.RoleTypeName);
        }

        public static string UniqueInstanceName(this Container container, WorkSpaceType workSpaceType)
        {
            return String.Format("{0}_{1}", container.Id, workSpaceType.Name);
        }
    }
}
