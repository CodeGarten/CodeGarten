using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGarten.Data.Access;

namespace CodeGarten.Service
{
    public class ServiceBuilder
    {
        public ServiceBuilder()
        {
        }

        public event EventHandler<ContainerEventArgs> OnCreateContainer
        {
            add { ContainerManager.OnCreateContainer += value; }
            remove { ContainerManager.OnCreateContainer -= value; }
        }

        public event EventHandler OnEnrollUser
        {
            add { UserManager.OnEnrollUser += value; }
            remove { UserManager.OnEnrollUser += value; }
        }

        public event EventHandler OnCreateUser
        {
            add { UserManager.OnCreateUser += value; }
            remove { UserManager.OnCreateUser -= value; }
        }
    }
}