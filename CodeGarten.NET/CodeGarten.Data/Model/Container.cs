using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class ContainerPrototype
    {
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        public long StructureId { get; set; }

        public virtual ContainerPrototype Parent { get; set; }

        public virtual ICollection<ContainerPrototype> Childs { get; set; }

        public virtual ICollection<WorkSpaceType> WorkSpaceTypes { get; set; }

        public virtual ICollection<Container> Containers { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; set; }


        public ContainerPrototype()
        {
            Childs = new LinkedList<ContainerPrototype>();
            WorkSpaceTypes = new LinkedList<WorkSpaceType>();
            Containers = new LinkedList<Container>();
        }
    }

    public class Container
    {
        [Key]
        public long Id { get; set; }

        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        //public string ContainerPrototypeName { get; set; }

        //public long ContainerPrototypeStructureId { get; set; }

        public virtual Container ParentContainer { get; set; }

        public virtual ICollection<Container> Childs { get; set; }

        public virtual ICollection<EnrollPassword> Passwords { get; set; }

        [Required]
        public virtual ContainerPrototype ContainerPrototype { get; set; }


        public Container()
        {
            Childs = new LinkedList<Container>();
        }
    }
}