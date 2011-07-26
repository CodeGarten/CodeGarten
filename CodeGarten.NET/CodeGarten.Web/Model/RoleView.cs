using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Web.Model
{
    public sealed class RoleView
    {
        [Required]
        public string ContainerPrototypeName { get; set; }

        [Required]
        public string RoleTypeName { get; set; }

        [Required]
        public string WorkSpaceTypeName { get; set; }

        public ICollection<RuleView> Rules { get; set; }

        public int RoleBarrier { get; set; }
    }
}