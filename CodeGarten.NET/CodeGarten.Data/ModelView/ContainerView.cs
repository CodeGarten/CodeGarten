using System;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.ModelView
{
    public sealed class ContainerView : IEquatable<ContainerView>
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        //public string Parent { get; set; }

        //[Required]
        //public string Prototype { get; set; }

        //[Required]
        //public long StructureId { get; set; }

        //public IEnumerable<string> Childs { get; set; }

        //public ContainerView(Container container)
        //{
        //    Id = container.Id;
        //    Name = container.Name;
        //    Description = container.Description;

        //    //Parent = container.ParentContainer.Name;
        //    //Prototype = container.ContainerPrototype.Name;
        //    //StructureId = container.ContainerPrototype.StructureId;

        //    //Childs = container.Childs.Select(child => child.Name);
        //}

        public bool Equals(ContainerView other)
        {
            return Id.Equals(other.Id);
        }
    }

    public static class MapperContainer
    {
        public static Container Convert(this ContainerView containerView)
        {
            return new Container()
                       {
                           Id = containerView.Id,
                           Name = containerView.Name,
                           Description = containerView.Description
                       };
        }

        public static ContainerView Convert(this Container container)
        {
            return new ContainerView()
                       {
                           Id = container.Id,
                           Name = container.Name,
                           Description = container.Description
                       };
        }
    }

    public sealed class ContainerPrototypeView :  IEquatable<ContainerPrototypeView>
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(2)]
        [MaxLength(64)]
        public string Name { get; set; }
        
        //[Required]
        //public long StructureId { get; set; }

        //public string Parent { get; set; }

        //public IEnumerable<string> Childs { get; set; }

        //public IEnumerable<string> WorkSpaces { get; set; }

        //public ContainerPrototypeView(ContainerPrototype containerPrototype)
        //{
        //    Name = containerPrototype.Name;
        //    //StructureId = containerPrototype.StructureId;

        //    //Parent = containerPrototype.Parent != null ? containerPrototype.Parent.Name : null;

        //    //Childs = containerPrototype.Childs.Select(child => child.Name);
        //    //WorkSpaces = containerPrototype.WorkSpaceTypes.Select(wst => wst.Name);
        //}

        public bool Equals(ContainerPrototypeView other)
        {
            return Name.Equals(other.Name);
        }
    }

    internal static class MapperContainerPrototype
    {
        public static ContainerPrototype Convert(this ContainerPrototypeView containerPrototypeView)
        {
            return new ContainerPrototype()
                       {
                           Name = containerPrototypeView.Name
                       };
        }

        public static ContainerPrototypeView Convert(this ContainerPrototype containerPrototype)
        {
            return new ContainerPrototypeView()
                       {
                           Name = containerPrototype.Name
                       };
        }
    }
}