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

        //TODO tratar de possiveis erros nos eventos dos serviços
        public event EventHandler<ContainerEventArgs> OnCreateContainer
        {
            add { ContainerManager.OnCreateContainer += value; }
            remove { ContainerManager.OnCreateContainer -= value; }
        }

        public event EventHandler<EnrollEventArgs> OnEnrollUser
        {
            add { UserManager.OnEnrollUser += value; }
            remove { UserManager.OnEnrollUser += value; }
        }

        public event EventHandler<UserEventArgs> OnCreateUser
        {
            add { UserManager.OnCreateUser += value; }
            remove { UserManager.OnCreateUser -= value; }
        }

        public event EventHandler<EnrollEventArgs> OnDisenrollUser
        {
            add { UserManager.OnDisenrollUser += value; }
            remove { UserManager.OnDisenrollUser -= value; }
        }

        public event EventHandler<ContainerEventArgs> OnDeleteContainer
        {
            //TODO
            add{}
            remove {}
        }
    }
}