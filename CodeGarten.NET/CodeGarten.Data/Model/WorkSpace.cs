﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGarten.Data.Model
{
    public class WorkSpaceType
    {
        [MinLength(4)]
        [MaxLength(64)]
        public string Name { get; internal set; }

        public long StructureId { get; internal set; }


        public virtual ICollection<ServiceType> Services { get; internal set; }


        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }


        public WorkSpaceType()
        {
            Services = new LinkedList<ServiceType>();
        }
    }

    public class Binding
    {
        public long StructureId { get; internal set; }

        public string ContainerTypeName { get; internal set; }

        public string WorkSpaceTypeName { get; internal set; }

        public virtual ICollection<Role> Roles { get; internal set; }


        [ForeignKey("StructureId")]
        public virtual Structure Structure { get; internal set; }

        [ForeignKey("ContainerTypeName, StructureId")]
        public virtual ContainerType ContainerType { get; internal set; }

        [ForeignKey("WorkSpaceTypeName, StructureId")]
        public virtual WorkSpaceType WorkSpaceType { get; internal set; }

        public Binding()
        {
            Roles = new LinkedList<Role>();
        }
    }
}