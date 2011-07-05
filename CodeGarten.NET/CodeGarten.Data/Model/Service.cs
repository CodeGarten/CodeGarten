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
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }


        public virtual ICollection<ServicePermission> Permissions { get; set; }

        public virtual ICollection<WorkSpaceType> WorkSpaceTypes { get; set; }


        public Service()
        {
            Permissions = new LinkedList<ServicePermission>();
            WorkSpaceTypes = new LinkedList<WorkSpaceType>();
        }
    }

    public class ServicePermission
    {
        public string Name { get; set; }

        public string ServiceName { get; set; }


        public virtual ICollection<Rule> Rules { get; set; }


        //[Required]
        [ForeignKey("ServiceName")]
        public virtual Service Service { get; set; }


        public ServicePermission()
        {
            Rules = new LinkedList<Rule>();
        }
    }
}