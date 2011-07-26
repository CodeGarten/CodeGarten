using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class ContainerPrototype
    {
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        public long StructureId { get; internal set; }

        public string ParentName { get; internal set; }

        [ForeignKey("ParentName, StructureId")]
        public virtual ContainerPrototype Parent { get; internal set; }

        public virtual ICollection<ContainerPrototype> Childs { get; internal set; }

        public virtual ICollection<Role> Roles { get; internal set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }


        public ContainerPrototype()
        {
            Roles = new LinkedList<Role>();
            Childs = new LinkedList<ContainerPrototype>();
        }
    }

    public class Container
    {
        [Key]
        public long Id { get; internal set; }

        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        [MaxLength(256)]
        public string Description { get; internal set; }


        public virtual Container Parent { get; internal set; }

        public virtual ICollection<Container> Childs { get; internal set; }

        [Required]
        public virtual ContainerPrototype Prototype { get; internal set; }


        public Container()
        {
            Childs = new LinkedList<Container>();
        }
    }
}