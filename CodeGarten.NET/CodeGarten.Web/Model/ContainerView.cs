using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Web.Model
{
    public sealed class ContainerView
    {
        [Required(ErrorMessage = "Required")]
        [MinLength(2, ErrorMessage = "Minimum length: 2")]
        [MaxLength(64, ErrorMessage = "Maximum length: 64")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(256, ErrorMessage = "Maximum length: 256")]
        public string Description { get; set; }

        public ICollection<PasswordView> Passwords { get; set; }
    }

    public class PasswordView
    {
        public string RoleType { get; set; }
        public string Password { get; set; }
    }
}