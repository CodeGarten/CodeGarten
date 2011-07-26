using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Access;

namespace CodeGarten.Web.Model
{
    public sealed class WorkSpaceTypeView : IValidatableObject
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"[a-zA-Z0-9_]*", ErrorMessage = "Must only contain letters, numbers and the character '_' (no spaces)")]
        [MinLength(4, ErrorMessage = "Minimum length: 4")]
        [MaxLength(64, ErrorMessage = "Maximum length: 64")]
        public string Name { get; set; }

        public long StructureId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = new DataBaseManager();
            if (context.WorkSpaceType.Get(StructureId, Name) != null)
                yield return new ValidationResult("This workspace already exists", new[] { "Name" });
        }
    }
}