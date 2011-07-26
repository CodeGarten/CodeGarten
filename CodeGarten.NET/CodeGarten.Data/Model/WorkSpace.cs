using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class WorkSpaceType
    {
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        public long StructureId { get; internal set; }


        public virtual ICollection<Service> Services { get; internal set; }


        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }


        public WorkSpaceType()
        {
            Services = new LinkedList<Service>();
        }
    }
}