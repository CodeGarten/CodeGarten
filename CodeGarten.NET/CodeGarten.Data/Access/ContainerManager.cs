using System;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{

    public class ContainerEventArgs : EventArgs
    {
        public ContainerEventArgs(Container container, ContainerPrototype containerPrototype)
        {
            Container = container;
            Prototype = containerPrototype;
        }

        public Container Container { get; private set; }
        public ContainerPrototype Prototype { get; set; }
    }

    public sealed class ContainerManager
    {
        private readonly DataBaseManager _dbManager;

        public ContainerManager(DataBaseManager db)
        {
            _dbManager = db;
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

        public Container Create(long structure, string containerName, string description, long? parent)
        {
            var container = new Container()
                                {
                                    Name = containerName,
                                    Description = description,
                                };

            if (parent != null)
            {
                var parentobj = _dbManager.Container.Get(parent.Value);

                container.Prototype = parentobj.Prototype.Childs.First();
                container.Parent = parentobj;
            }
            else
            {
                container.Prototype =
                    _dbManager.ContainerPrototype.GetAll(structure).Where(
                        cp => cp.StructureId == structure && cp.Parent == null).SingleOrDefault();
            }

            _dbManager.DbContext.Containers.Add(container);

            _dbManager.DbContext.SaveChanges();

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

        public Container Create(long structure, string containerName, string description, long? parent, string prototypeName)
        {
            var container = new Container
                                {
                                    Prototype = _dbManager.ContainerPrototype.Get(structure, prototypeName),
                                    Description = description,
                                    Name = containerName,
                                    Parent = parent == null ? null : Get(parent.Value)
                                };
            _dbManager.DbContext.Containers.Add(container);

            _dbManager.DbContext.SaveChanges();

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

        public Container Delete(long containerId)
        {
            var container = Get(containerId);
            var prototype = container.Prototype;

            var parent = container.Parent;

            foreach(var child in container.Childs.ToList())
            {
                Delete(child.Id);
            }

            _dbManager.DbContext.Containers.Remove(container);

            _dbManager.DbContext.SaveChanges();

            try
            {
                InvokeOnDeleteContainer(container, prototype);
            }
            catch
            {
                //TODO DataLogger
            }
            return parent;
        }

        public void AddPassword(long structure, long container, string roletype, string password)
        {
            var enrollPassword = new EnrollPassword()
                                     {
                                         ContainerId = container,
                                         RoleTypeName = roletype,
                                         StructureId = structure,
                                         Password = AuthenticationManager.EncryptPassword(password)
                                     };

            _dbManager.DbContext.EnrollPassWords.Add(enrollPassword);

            _dbManager.DbContext.SaveChanges();
        }

        public bool HasPassword(long structure, long container, string roletype)
        {
            return _dbManager.DbContext.EnrollPassWords.Find(container, roletype, structure) != null;
        }

        public Container Get(long container)
        {
            return _dbManager.DbContext.Containers.Find(container);
        }

        #region InvokeEvents

        private void InvokeOnCreateContainer(Container container)
        {
            var eventArgs = new ContainerEventArgs(container, container.Prototype);

            var handler = _onCreateContainer;
            if (handler != null) handler(this, eventArgs);
        }

        //TODO
        private void InvokeOnDeleteContainer(Container container, ContainerPrototype containerPrototype)
        {
            var eventArgs = new ContainerEventArgs(container, containerPrototype);

            var handler = _onDeleteContainer;
            if (handler != null) handler(this, eventArgs);
        }

        #endregion

        public IQueryable<Container> Search(string query)
        {
            return _dbManager.DbContext.Containers.Where(c => c.Name.StartsWith(query.Trim()));
        }

        public IQueryable<Container> GetInstances(long structureId)
        {
            return _dbManager.DbContext.Containers.Where(c => c.Prototype.StructureId == structureId && c.Parent == null);
        }
    }
}