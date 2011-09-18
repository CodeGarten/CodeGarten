using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class Enroll
    {
        
        public string UserName { get; internal set; }

        public long ContainerId { get; internal set; }

        public bool Inherited { get; internal set; }

        public int InheritedCount {  get; internal set; }

        public string RoleTypeName { get; internal set; }

        public long StructureId { get; internal set; }

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

    public class EnrollKey
    {
        public long ContainerId { get; internal set; }

        public string RoleTypeName { get; internal set; }

        public long StructureId { get; internal set; }

        [Required]
        [MinLength(8)]
        public string Credential { get; internal set; }

        [ForeignKey("ContainerId")]
        public Container Container { get; internal set; }

        [ForeignKey("RoleTypeName,StructureId")]
        public RoleType RoleType { get; internal set; }
    }
    
}