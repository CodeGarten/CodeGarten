using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class ServiceType
    {
        [Key]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        [MaxLength(256)]
        public string Description { get; internal set; }


        public virtual ICollection<ServiceTypePermission> Permissions { get; internal set; }


        public ServiceType()
        {
            Permissions = new LinkedList<ServiceTypePermission>();
        }
    }

    public class ServiceTypePermission
    {
        public string Name { get; internal set; }

        public string ServiceName { get; internal set; }


        [ForeignKey("ServiceName")]
        public virtual ServiceType ServiceType { get; internal set; }
    }
}