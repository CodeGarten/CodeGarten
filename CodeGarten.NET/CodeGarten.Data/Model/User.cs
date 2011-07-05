using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//using Database.CustomValidation;

namespace CodeGarten.Data.Model
{
    public class User
    {
        [Key]
        [RegularExpression(@"[a-zA-Z0-9]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$")]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        //[CustomValidation(typeof(UniqueValidation),"UniqueEmail")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@\""]+"
                           + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                           + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                           + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                           + @"[a-zA-Z]{2,}))$")]
        public string Email { get; set; }


        public virtual ICollection<Structure> Structures { get; set; }

        public virtual ICollection<Enroll> Enrolls { get; set; }


        public User()
        {
            Structures = new LinkedList<Structure>();

            Enrolls = new LinkedList<Enroll>();
        }
    }
}