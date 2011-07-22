using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class Structure
    {
        public long Id { get; set; }

        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; internal set; }

        [MaxLength(256)]
        public string Description { get; internal set; }

        public bool Public { get; internal set; }

        public bool Developing { get; internal set; }

        public virtual ICollection<User> Administrators { get; internal set; }

        public Structure()
        {
            Administrators = new LinkedList<User>();
        }

        public Structure(string name, string description, bool @public, User administrator):this()
        {
            Name = name;
            Description = description;
            Public = @public;
            Developing = true;

            CreatedOn = DateTime.Now;

            Administrators.Add(administrator);
        }
    }
}