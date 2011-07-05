using System;
using System.ComponentModel.DataAnnotations;
using CodeGarten.Data.Access;
using CodeGarten.Data.CustomAttributes;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.ModelView
{
    public sealed class UserView : IEquatable<UserView>
    {
        [Required]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$")]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [UniqueEmail]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+"
                           + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                           + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                           + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                           + @"[a-zA-Z]{2,}))$")]
        public string Email { get; set; }

        //public IEnumerable<long> Structures { get; set; }

        //public UserView(User user)
        //{
        //    Name = user.Name;
        //    Password = user.Password;
        //    Email = user.Email;

        //    //Structures = user.Structures.Select(structure => structure.Id);
        //}

        public bool Equals(UserView other)
        {
            return Name.Equals(other.Name);
        }
    }

    internal static class MapperUser
    {
        public static User Convert(this UserView userView)
        {
            return new User()
                       {
                           Name = userView.Name,
                           Password = AuthenticationManager.EncryptPassword(userView.Password),
                           Email = userView.Email
                       };
        }

        public static UserView Convert(this User user)
        {
            return new UserView()
                       {
                           Name = user.Name,
                           Password = user.Password,
                           Email = user.Email
                       };
        }
    }
}