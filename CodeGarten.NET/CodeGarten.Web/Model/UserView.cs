using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Model
{
    public sealed class UserView : IValidatableObject
    {
        [Required(ErrorMessage = "Required.")]
        [RegularExpression(@"[a-zA-Z0-9_]*", ErrorMessage = "Must only contain letters and numbers (no spaces).")]
        [MinLength(4, ErrorMessage = "Minimum length: 4")]
        [MaxLength(64, ErrorMessage = "Maximum length: 64")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$", ErrorMessage = "Must use upper and lowercase letters and numbers.")]
        [MinLength(8, ErrorMessage = "Minimum length: 8")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+"
                           + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                           + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                           + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                           + @"[a-zA-Z]{2,}))$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbMan = new DataBaseManager();

            if (dbMan.User.Get(Name) != null)
                yield return new ValidationResult("This name has already been taken.", new[] { "Name" });

            if (dbMan.User.GetAll().Any(u => u.Email == Email))
                yield return new ValidationResult("This email is already registered.", new[] { "Email" });
        }
    }
}