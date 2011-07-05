using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class Structure
    {
        public long Id { get; set; }

        [RegularExpression(@"[a-zA-Z0-9_]*")]
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public bool Public { get; set; }

        public bool Developing { get; set; }

        public virtual ICollection<User> Administrators { get; set; }

        //public virtual ICollection<ContainerPrototype> ContainerPrototypes { get; set; }
        //public virtual ICollection<WorkSpaceType> WorkSpaceTypes { get; set; }
        //public virtual ICollection<RoleType> RoleTypes { get; set; }
        //public virtual ICollection<Rule> Rules { get; set; }

        public Structure()
        {
            Administrators = new LinkedList<User>();

            //ContainerPrototypes = new LinkedList<ContainerPrototype>();
            //WorkSpaceTypes = new LinkedList<WorkSpaceType>();
            //RoleTypes = new LinkedList<RoleType>();
            //Rules = new LinkedList<Rule>();
        }
    }
}