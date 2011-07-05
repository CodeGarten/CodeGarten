using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class Enroll
    {
        public string UserName { get; set; }

        public long ContainerId { get; set; }

        //public string ContainerPrototypeName { get; set; }

        //public long ContainerPrototypeStructureId { get; set; }

        public string RoleTypeName { get; set; }

        public long RoleTypeStructureId { get; set; }

        [Required]
        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        [Required]
        [ForeignKey("ContainerId")]
        public virtual Container Container { get; set; }

        [Required]
        [ForeignKey("RoleTypeName,RoleTypeStructureId")]
        public virtual RoleType RoleType { get; set; }
    }
}