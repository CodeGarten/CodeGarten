using System;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.ModelView
{
    public sealed class ServiceView : IEquatable<ServiceView>
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        //public IEnumerable<string> Permissions { get; set; }

        //public ServiceView(Service service)
        //{
        //    Name = service.Name;
        //    Description = service.Description;

        //    Permissions = service.Permissions.Select(permission => permission.Name);
        //}

        public bool Equals(ServiceView other)
        {
            return Name.Equals(other.Name) &&
                   Description.Equals(other.Description);
        }
    }

    internal static class MapperService
    {
        public static Service Convert(this ServiceView serviceView)
        {
            return new Service()
                       {
                           Name = serviceView.Name,
                           Description = serviceView.Description
                       };
        }

        public static ServiceView Convert(this Service service)
        {
            return new ServiceView()
                       {
                           Name = service.Name,
                           Description = service.Description,
                       };
        }
    }
}