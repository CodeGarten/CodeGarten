using System;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.ModelView
{
    public sealed class WorkSpaceTypeView : IEquatable<WorkSpaceTypeView>
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        //[Required]
        //public long StructureId { get; set; }

        //internal WorkSpaceView(WorkSpaceType workSpaceType)
        //{
        //    Name = workSpaceType.Name;
        //    //StructureId = workSpaceType.StructureId;
        //}

        public bool Equals(WorkSpaceTypeView other)
        {
            return Name.Equals(other.Name);
        }
    }

    internal static class MapperWorkSpaceType
    {
        public static WorkSpaceType Convert(this WorkSpaceTypeView workSpaceTypeView)
        {
            return new WorkSpaceType()
                       {
                           Name = workSpaceTypeView.Name
                       };
        }

        public static WorkSpaceTypeView Convert(this WorkSpaceType workSpaceType)
        {
            return new WorkSpaceTypeView()
                       {
                           Name = workSpaceType.Name
                       };
        }
    }
}