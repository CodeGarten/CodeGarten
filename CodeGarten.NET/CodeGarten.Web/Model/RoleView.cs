using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Web.Model
{
    public sealed class RoleView
    {
        [Required]
        public string ContainerPrototypeName { get; set; }

        [Required]
        public string WorkSpaceTypeName { get; set; }

        public RoleTypeView RoleType { get; set; }

        public ICollection<RuleView> Rules { get; set; }
    }
}