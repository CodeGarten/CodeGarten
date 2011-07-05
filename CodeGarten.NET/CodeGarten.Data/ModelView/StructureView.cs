using System;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.ModelView
{
    public sealed class StructureView : IEquatable<StructureView>
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

        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        //public IEnumerable<string> Administrators { get; set; }

        //public StructureView(Structure structure)
        //{
        //    Id = structure.Id;
        //    Name = structure.Name;
        //    Description = structure.Description;

        //    //Administrators = structure.Administrators.Select(administrator => administrator.Name);
        //}

        public bool Equals(StructureView other)
        {
            return Id.Equals(other.Id);
        }
    }

    internal static class MapperStructure
    {
        public static Structure Convert(this StructureView structureView)
        {
            return new Structure()
                       {
                           Id = structureView.Id,
                           Name = structureView.Name,
                           Description = structureView.Description,
                           CreatedOn = structureView.CreatedOn
                       };
        }

        public static StructureView Convert(this Structure structure)
        {
            return new StructureView()
                       {
                           Id = structure.Id,
                           Name = structure.Name,
                           Description = structure.Description,
                           CreatedOn = structure.CreatedOn
                       };
        }
    }
}