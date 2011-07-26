using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class Service
    {
        [Key]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        [MaxLength(256)]
        public string Description { get; internal set; }


        public virtual ICollection<ServicePermission> Permissions { get; internal set; }


        public Service()
        {
            Permissions = new LinkedList<ServicePermission>();
        }
    }

    public class ServicePermission
    {
        public string Name { get; internal set; }

        public string ServiceName { get; internal set; }


        [ForeignKey("ServiceName")]
        public virtual Service Service { get; internal set; }
    }
}