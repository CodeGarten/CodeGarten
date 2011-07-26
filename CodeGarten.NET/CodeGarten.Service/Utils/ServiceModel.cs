using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Model;

namespace CodeGarten.Service.Utils
{
    public sealed class ServiceModel : IEquatable<ServiceModel>
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public IEnumerable<string> Permissions { get; set; }

        //public IEnumerable<string> Permissions { get; set; }

        //public ServiceView(Service service)
        //{
        //    Name = service.Name;
        //    Description = service.Description;

        //    Permissions = service.Permissions.Select(permission => permission.Name);
        //}

        public ServiceModel (string name, string description = null, IEnumerable<string> permissions = null)
        {
            Name = name;
            Description = description;
            Permissions = permissions ?? new LinkedList<string>();
        }

        public bool Equals(ServiceModel other)
        {
            return Name.Equals(other.Name);
        }
    }
}