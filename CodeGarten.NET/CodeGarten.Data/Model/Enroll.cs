using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class Enroll
    {
        public string UserName { get; internal set; }

        public long ContainerId { get; internal set; }

        public string RoleTypeName { get; internal set; }

        public long StructureId { get; internal set; }

        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }

        [Required]
        [ForeignKey("UserName")]
        public virtual User User { get; internal set; }

        [Required]
        [ForeignKey("ContainerId")]
        public virtual Container Container { get; internal set; }

        [Required]
        [ForeignKey("RoleTypeName,StructureId")]
        public virtual RoleType RoleType { get; internal set; }
    }
}