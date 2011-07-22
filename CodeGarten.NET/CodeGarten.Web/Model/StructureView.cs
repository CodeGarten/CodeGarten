using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Web.Model
{
    public sealed class StructureView
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public bool Public { get; set; }
    }
}