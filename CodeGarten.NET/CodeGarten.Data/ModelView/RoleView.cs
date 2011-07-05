using System;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.ModelView
{
    public sealed class RoleView : IEquatable<RoleView>
    {
        [Required]
        public string RoleTypeName { get; set; }

        [Required]
        public string ContainerPrototypeName { get; set; }

        [Required]
        public string WorkSpaceTypeName { get; set; }

        //public RoleView(Role role)
        //{
        //    RoleTypeName = role.RoleTypeName;
        //    ContainerPrototypeName = role.ContainerPrototypeName;
        //    WorkSpaceTypeName = role.WorkSpaceTypeName;
        //}

        public bool Equals(RoleView other)
        {
            return RoleTypeName.Equals(other.RoleTypeName)
                   && ContainerPrototypeName.Equals(other.ContainerPrototypeName)
                   && WorkSpaceTypeName.Equals(other.WorkSpaceTypeName);
        }
    }

    internal static class MapperRole
    {
        public static Role Convert(this RoleView roleView)
        {
            return new Role()
                       {
                           RoleTypeName = roleView.RoleTypeName,
                           ContainerPrototypeName = roleView.ContainerPrototypeName,
                           WorkSpaceTypeName = roleView.WorkSpaceTypeName
                       };
        }

        public static RoleView Convert(this Role role)
        {
            return new RoleView()
                       {
                           RoleTypeName = role.RoleTypeName,
                           ContainerPrototypeName = role.ContainerPrototypeName,
                           WorkSpaceTypeName = role.WorkSpaceTypeName
                       };
        }
    }

    public sealed class RoleTypeView : IEquatable<RoleTypeView>
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        //public RoleTypeView(RoleType roleType)
        //{
        //    Name = roleType.Name;
        //}

        public bool Equals(RoleTypeView other)
        {
            return Name.Equals(other.Name);
        }
    }

    internal static class MapperRoleType
    {
        public static RoleType Convert(this RoleTypeView roleTypeView)
        {
            return new RoleType()
                       {
                           Name = roleTypeView.Name
                       };
        }

        public static RoleTypeView Convert(this RoleType roleType)
        {
            return new RoleTypeView()
                       {
                           Name = roleType.Name
                       };
        }
    }

    public sealed class RuleView : IEquatable<RuleView>
    {
        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        //public RuleView(Rule rule)
        //{
        //    Name = rule.Name;
        //}

        public bool Equals(RuleView other)
        {
            return Name.Equals(other.Name);
        }
    }

    internal static class MapperRule
    {
        public static Rule Convert(this RuleView ruleView)
        {
            return new Rule()
                       {
                           Name = ruleView.Name
                       };
        }

        public static RuleView Convert(this Rule rule)
        {
            return new RuleView()
                       {
                           Name = rule.Name
                       };
        }
    }
}