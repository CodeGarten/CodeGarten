using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Web.Model
{
    public sealed class StructureView
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"[a-zA-Z0-9_]*", ErrorMessage = "Must only contain letters, numbers and the character '_' (no spaces)")]
        [MinLength(4, ErrorMessage = "Minimum length: 4")]
        [MaxLength(64, ErrorMessage = "Maximum length: 64")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(256, ErrorMessage = "Maximum length: 256")]
        public string Description { get; set; }

        public bool Public { get; set; }
    }
}