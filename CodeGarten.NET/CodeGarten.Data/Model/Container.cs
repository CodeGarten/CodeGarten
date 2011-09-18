using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class ContainerType
    {
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        public long StructureId { get; internal set; }

        public string ParentName { get; internal set; }

        [ForeignKey("ParentName, StructureId")]
        public virtual ContainerType Parent { get; internal set; }

        public virtual ICollection<ContainerType> Childs { get; internal set; }

        public virtual ICollection<Binding> Bindings { get; internal set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }


        public ContainerType()
        {
            Childs = new LinkedList<ContainerType>();

            Bindings = new LinkedList<Binding>();
        }
    }

    public class Container
    {
        [Key]
        public long Id { get; internal set; }

        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        [MaxLength(256)]
        public string Description { get; internal set; }


        public virtual Container Parent { get; internal set; }

        public virtual ICollection<Container> Childs { get; internal set; }

        [Required]
        public virtual ContainerType Type { get; internal set; }


        public Container()
        {
            Childs = new LinkedList<Container>();
        }
    }
}