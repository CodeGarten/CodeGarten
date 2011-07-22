using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class WorkSpaceType
    {
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        public long StructureId { get; set; }

        public virtual ICollection<Service> Services { get; set; }

        public virtual ICollection<ContainerPrototype> ContainerPrototypes { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; set; }


        public WorkSpaceType()
        {
            Services = new LinkedList<Service>();
            ContainerPrototypes = new LinkedList<ContainerPrototype>();
        }
    }
}