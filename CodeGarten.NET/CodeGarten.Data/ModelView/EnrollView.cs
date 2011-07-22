using System;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.ModelView
{
    public sealed class EnrollView : IEquatable<EnrollView>
    {
        [Required]
        public string User { get; set; }

        [Required]
        public string RoleType { get; set; }

        [Required]
        public long Container { get; set; }

        public bool Inherited { get; set; }

        //public EnrollView(Enroll enroll)
        //{
        //    User = enroll.UserName;
        //    RoleType = enroll.RoleTypeName;
        //    Container = enroll.ContainerId;
        //}

        public bool Equals(EnrollView other)
        {
            return User.Equals(other.User) &&
                   RoleType.Equals(other.RoleType) &&
                   Container.Equals(other.Container);
        }
    }

    internal static class MapperEnroll
    {
        public static Enroll Convert(this EnrollView enrollView)
        {
            return new Enroll()
                       {
                           UserName = enrollView.User,
                           RoleTypeName = enrollView.RoleType,
                           ContainerId = enrollView.Container,
                           Inherited = enrollView.Inherited
                       };
        }

        public static EnrollView Convert(this Enroll enroll)
        {
            return new EnrollView()
                       {
                           User = enroll.UserName,
                           RoleType = enroll.RoleTypeName,
                           Container = enroll.ContainerId,
                           Inherited = enroll.Inherited
                       };
        }
    }
}