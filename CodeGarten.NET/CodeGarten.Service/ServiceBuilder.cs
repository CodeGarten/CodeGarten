using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGarten.Data.Access;

namespace CodeGarten.Service
{
    public class ServiceBuilder : IDisposable
    {
        public ServiceBuilder()
        {
            //TODO change load services and remove registerevents form here
            RegisterEvents();
        }

        public void RegisterEvents()
        {
            ContainerManager.OnCreateContainer += CreateServiceInstances;
            ContainerManager.OnDeleteContainer += DeleteServiceInstance;
            UserManager.OnEnrollUser += EnrollUser;
            UserManager.OnDisenrollUser += DisenrollUser;
        }

        public void UnregisterEvents()
        {
            ContainerManager.OnCreateContainer -= CreateServiceInstances;
            ContainerManager.OnDeleteContainer -= DeleteServiceInstance;
            UserManager.OnEnrollUser -= EnrollUser;
            UserManager.OnDisenrollUser -= DisenrollUser;
        }

        private static void ParallelExecute<T>(object sender, T eventArgs, EventHandler<T> eventHandler) where T : EventArgs
        {
            var tasks =
                eventHandler.GetInvocationList().Select(
                    d => Task.Factory.StartNew(() =>((EventHandler<T>) d)(sender, eventArgs))).ToArray();

            try
            {
                Task.WaitAll(tasks);
            }catch(AggregateException e)
            {
                ServiceFactory.ServiceLogger.Log(
                    String.Format("Call service method fail Exception from: {0} message:{1}",
                                                        e.InnerException.Source, e.InnerException.Message));
            }
        }

        public void CreateServiceInstances(object sender, ContainerEventArgs eventArgs)
        {
            ParallelExecute(sender, eventArgs, _onCreateContainer);
        }

        public void DeleteServiceInstance(object sender, ContainerEventArgs eventArgs)
        {
            ParallelExecute(sender, eventArgs, _onDeleteContiner);
        }

        public void EnrollUser(object sender, EnrollEventArgs eventArgs)
        {
            ParallelExecute(sender, eventArgs, _onEnrollUser);
        }

        public void DisenrollUser(object sender, EnrollEventArgs eventArgs)
        {
            ParallelExecute(sender, eventArgs, _onDisenrollUser);
        }


        private event EventHandler<ContainerEventArgs> _onCreateContainer;
        public event EventHandler<ContainerEventArgs> OnCreateContainer
        {
            add { _onCreateContainer += value; }
            remove { _onCreateContainer -= value; }
        }

        private event EventHandler<ContainerEventArgs> _onDeleteContiner;
        public event EventHandler<ContainerEventArgs> OnDeleteContainer
        {
            add { _onDeleteContiner += value; }
            remove { _onDeleteContiner -= value; }
        }

        private event EventHandler<EnrollEventArgs> _onEnrollUser;
        public event EventHandler<EnrollEventArgs> OnEnrollUser
        {
            add { _onEnrollUser += value; }
            remove { _onEnrollUser += value; }
        }

        private event EventHandler<EnrollEventArgs> _onDisenrollUser;
        public event EventHandler<EnrollEventArgs> OnDisenrollUser
        {
            add { _onDisenrollUser += value; }
            remove { _onDisenrollUser -= value; }
        }

        public void Dispose()
        {
            UnregisterEvents();
        }

    }
}