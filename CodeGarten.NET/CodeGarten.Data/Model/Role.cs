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

        public virtual ICollection<Role> Roles { get; set; }

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
        public virtual ICollection<Role> Roles { get; set; }


        public Rule()
        {
            Permissions = new LinkedList<ServicePermission>();
            Roles = new LinkedList<Role>();
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
        public string RoleTypeName { get; set; }

        public long RoleTypeStructureId { get; set; }

        public string ContainerPrototypeName { get; set; }

        public long ContainerPrototypeStructureId { get; set; }

        public string WorkSpaceTypeName { get; set; }

        public long WorkSpaceTypeStructureId { get; set; }

        public ICollection<Rule> Rules { get; set; }

        //public string RuleName { get; set; }

        //public long RuleStructureId { get; set; }

        public int Barrier { get; private set; }
        public RoleBarrier RoleBarrier { get { return (RoleBarrier)Barrier; } set { Barrier = (int) value; } }

        [ForeignKey("RoleTypeName,RoleTypeStructureId")]
        public virtual RoleType RoleType { get; set; }

        [ForeignKey("ContainerPrototypeName,ContainerPrototypeStructureId")]
        public virtual ContainerPrototype ContainerPrototype { get; set; }

        [ForeignKey("WorkSpaceTypeName,WorkSpaceTypeStructureId")]
        public virtual WorkSpaceType WorkSpaceType { get; set; }

        //[ForeignKey("RuleName,RuleStructureId")]
        //public virtual Rule Rule { get; set; }

        public Role()
        {
            Rules = new LinkedList<Rule>();
        }
    }
}