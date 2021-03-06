﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class User
    {
        [Key]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        [Required]
        [MinLength(8)]
        public string Credential { get; internal set; }

        [Required]
        public string Email { get; internal set; }


        public virtual ICollection<Structure> Structures { get; internal set; }

        public virtual ICollection<Enroll> Enrolls { get; internal set; }


        public User()
        {
            Structures = new LinkedList<Structure>();

            Enrolls = new LinkedList<Enroll>();
        }
    }
}