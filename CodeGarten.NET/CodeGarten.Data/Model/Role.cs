using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class RoleType
    {
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        public long StructureId { get; set; }


        public virtual ICollection<Enroll> Enrolls { get; set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; set; }


        public RoleType()
        {
            Enrolls = new LinkedList<Enroll>();
        }
    }

    public class Rule
    {
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        public long StructureId { get; set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; set; }


        public virtual ICollection<ServicePermission> Permissions { get; set; }


        public Rule()
        {
            Permissions = new LinkedList<ServicePermission>();
        }
    }

    public class Role
    {
        public string RoleTypeName { get; set; }

        public long RoleTypeStructureId { get; set; }

        public string ContainerPrototypeName { get; set; }

        public long ContainerPrototypeStructureId { get; set; }

        public string WorkSpaceTypeName { get; set; }

        public long WorkSpaceTypeStructureId { get; set; }

        [Required]
        public virtual Rule Rule { get; set; }


        [ForeignKey("RoleTypeName,RoleTypeStructureId")]
        public virtual RoleType RoleType { get; set; }

        [ForeignKey("ContainerPrototypeName,ContainerPrototypeStructureId")]
        public virtual ContainerPrototype ContainerPrototype { get; set; }

        [ForeignKey("WorkSpaceTypeName,WorkSpaceTypeStructureId")]
        public virtual WorkSpaceType WorkSpaceType { get; set; }
    }
}