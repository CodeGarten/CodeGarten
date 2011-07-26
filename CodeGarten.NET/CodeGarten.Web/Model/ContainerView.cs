using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Web.Model
{
    public sealed class ContainerView
    {
        [Required(ErrorMessage = "Required")]
        [MinLength(4, ErrorMessage = "Minimum length: 4")]
        [MaxLength(64, ErrorMessage = "Maximum length: 64")]
        public string Name { get; set; }

        [MaxLength(256, ErrorMessage = "Maximum length: 256")]
        public string Description { get; set; }
    }
}