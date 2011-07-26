using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class RoleType
    {
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        public long StructureId { get; internal set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }
    }

    public class Rule
    {
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        public long StructureId { get; internal set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }


        public virtual ICollection<ServicePermission> Permissions { get; internal set; }


        public Rule()
        {
            Permissions = new LinkedList<ServicePermission>();
        }
    }

    public class Role
    {
        public long StructureId { get; internal set; }

        public string RoleTypeName { get; internal set; }

        public string ContainerPrototypeName { get; internal set; }

        public string WorkSpaceTypeName { get; internal set; }

        public virtual ICollection<Rule> Rules { get; internal set; }

        public int BlockBarrier { get; internal set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }

        [ForeignKey("RoleTypeName,StructureId")]
        public virtual RoleType RoleType { get; internal set; }

        [ForeignKey("ContainerPrototypeName,StructureId")]
        public virtual ContainerPrototype ContainerPrototype { get; internal set; }

        [ForeignKey("WorkSpaceTypeName,StructureId")]
        public virtual WorkSpaceType WorkSpaceType { get; internal set; }

        public Role()
        {
            Rules = new LinkedList<Rule>();
        }
    }
}