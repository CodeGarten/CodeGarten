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


        public virtual ICollection<ServiceTypePermission> Permissions { get; internal set; }


        public Rule()
        {
            Permissions = new LinkedList<ServiceTypePermission>();
        }
    }

    public enum RoleBarrier
    {
        None = 0,
        Top,
        Bottom,
        All
    }

    public class Role
    {
        public long StructureId { get; internal set; }

        public string RoleTypeName { get; internal set; }

        public string ContainerTypeName { get; internal set; }

        public string WorkSpaceTypeName { get; internal set; }

        public virtual ICollection<Rule> Rules { get; internal set; }

        public int Barrier { get; private set; }
        public RoleBarrier RoleBarrier { get { return (RoleBarrier)Barrier; } internal set { Barrier = (int)value; } }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }

        [ForeignKey("RoleTypeName,StructureId")]
        public virtual RoleType RoleType { get; internal set; }

        [ForeignKey("StructureId, ContainerTypeName, WorkSpaceTypeName")]
        public virtual Binding Binding { get; internal set; }

        public Role()
        {
            Rules = new LinkedList<Rule>();
        }
    }
}