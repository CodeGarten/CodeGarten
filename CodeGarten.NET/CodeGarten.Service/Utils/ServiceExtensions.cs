using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGarten.Data.Access;
using CodeGarten.Data.Model;

namespace CodeGarten.Service.Utils
{
    public static class ServiceExtensions
    {
        public static string UniqueGroupName(this Container container, string roleTypeName)
        {
            return String.Format("G_{0}_{1}", container.Id, roleTypeName);
        }

        public static string UniqueInstanceName(this Container container, WorkSpaceType workSpaceType)
        {
            return String.Format("{0}_{1}", container.Id, workSpaceType.Name);
        }
    }

    public static class EnumExtensions
    {
        public static IEnumerable<string> ToEnumerable<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(t => t.ToString());
        }   
    }

    public static class DataAccessExtensions
    {
        public static IEnumerable<WorkSpaceType> WorkSpaceTypeWithService(this Container container, string serviceName)
        {
            return container.ContainerPrototype.WorkSpaceTypes.Where(
                                                                    workSpaceType =>
                                                                    workSpaceType.Services.Where(
                                                                                            s =>
                                                                                            s.Name == serviceName
                                                                                            ).Any());
        }
    }
}
