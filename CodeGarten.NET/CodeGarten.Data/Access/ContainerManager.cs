using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;
using CodeGarten.Data.ModelView;

namespace CodeGarten.Data.Access
{

    public class ContainerEventArgs : EventArgs
    {
        public ContainerEventArgs(Container container)
        {
            Container = container;
        }

        public Container Container { get; private set; }
    }

    public sealed class ContainerManager
    {
        private readonly Context _dbContext;

        public ContainerManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        #region Events

        private static event EventHandler<ContainerEventArgs> _onCreateContainer;

        public static event EventHandler<ContainerEventArgs> OnCreateContainer
        {
            add { _onCreateContainer += value; }
            remove { _onCreateContainer -= value; }
        }

        private static event EventHandler<ContainerEventArgs> _onDeleteContainer;

        public static event EventHandler<ContainerEventArgs> OnDeleteContainer
        {
            add { _onDeleteContainer += value; }
            remove { _onDeleteContainer -= value; }
        }

        #endregion

        public Container Create(ContainerView containerView, long structure, long? parent)
        {
            if (containerView == null) throw new ArgumentNullException("containerView");

            var container = containerView.Convert();

            if (parent != null)
            {
                var parentobj = _dbContext.Containers.Find(parent);

                container.ContainerPrototype = parentobj.ContainerPrototype.Childs.First();
                container.ParentContainer = parentobj;
            }
            else
            {
                container.ContainerPrototype =
                    _dbContext.ContainerPrototypes.Where(cp => cp.StructureId == structure && cp.Parent == null).
                        SingleOrDefault();
            }

            _dbContext.Containers.Add(container);

            _dbContext.SaveChanges();

            containerView.Id = container.Id;

            try
            {
                InvokeOnCreateContainer(container);
            }
            catch
            {
                //TODO DataLogger
            }

            return container;
        }

        public Container Create(ContainerView containerView, long structure, string containerPrototype)
        {
            if (containerView == null) throw new ArgumentNullException("containerView");

            var container = containerView.Convert();

            var containerPrototypeObj = ContainerPrototypeManager.Get(_dbContext, structure, containerPrototype);
            if (containerPrototypeObj == null) throw new ArgumentException("Invalid argument");

            container.ContainerPrototype = containerPrototypeObj;

            _dbContext.Containers.Add(container);

            _dbContext.SaveChanges();

            containerView.Id = container.Id;
            
            try
            {
                InvokeOnCreateContainer(container);
            }
            catch
            {
                //TODO DataLogger
            }

            return container;
        }

        public Container Create(ContainerView containerView, long structure, string containerPrototype, long parentContainer)
        {
            if (containerView == null) throw new ArgumentNullException("containerView");

            var container = containerView.Convert();

            var containerPrototypeObj = ContainerPrototypeManager.Get(_dbContext, structure, containerPrototype);
            if (containerPrototypeObj == null) throw new ArgumentException("Invalid argument");

            var parentContainerObj = Get(_dbContext, parentContainer);
            if (containerPrototypeObj.Parent.Name != parentContainerObj.ContainerPrototype.Name) throw new Exception();

            container.ParentContainer = parentContainerObj;

            container.ContainerPrototype = containerPrototypeObj;

            _dbContext.Containers.Add(container);

            _dbContext.SaveChanges();

            containerView.Id = container.Id;

            
            try
            {
                InvokeOnCreateContainer(container);
            }
            catch
            {
                //TODO DataLogger
            }

            return container;
        }

        public void Delete(ContainerView containerView)
        {
            _dbContext.Containers.Remove(_dbContext.Containers.Find(containerView.Id));

            _dbContext.SaveChanges();
        }

        public void AddPassword(long structure, long container, string roletype, string password)
        {
            var enrollPassword = new EnrollPassword()
                                     {
                                         ContainerId = container,
                                         RoleTypeName = roletype,
                                         RoleTypeStructureId = structure,
                                         Password = AuthenticationManager.EncryptPassword(password)
                                     };
            
            _dbContext.EnrollPassWords.Add(enrollPassword);

            _dbContext.SaveChanges();
        }
        
        public bool HasPassword(long structure, long container, string roletype)
        {
            return _dbContext.EnrollPassWords.Find(container, roletype, structure) != null;
        }

        internal static Container Get(Context db, long container)
        {
            return db.Containers.Find(container);
        }

        public Container Get(long container)
        {
            return _dbContext.Containers.Find(container);
        }

        #region InvokeEvents

        private void InvokeOnCreateContainer(Container container)
        {
            var eventArgs = new ContainerEventArgs(container);

            var handler = _onCreateContainer;
            if (handler != null) handler(this, eventArgs);
        }

        //TODO
        private void InvokeOnDeleteContainer(Container container)
        {
            var eventArgs = new ContainerEventArgs(container);

            var handler = _onCreateContainer;
            if (handler != null) handler(this, eventArgs);
        }

        #endregion
    }
}