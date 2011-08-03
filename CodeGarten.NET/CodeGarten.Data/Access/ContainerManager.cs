using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;

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

        public Container Create(long structure, string containerName, string description, long? parent)
        {
            var container = new Container()
                                {
                                    Name = containerName,
                                    Description = description,
                                };

            if (parent != null)
            {
                var parentobj = _dbContext.Containers.Find(parent);

                container.Prototype = parentobj.Prototype.Childs.First();
                container.Parent = parentobj;
            }
            else
            {
                container.Prototype =
                    _dbContext.ContainerPrototypes.Where(cp => cp.StructureId == structure && cp.Parent == null).
                        SingleOrDefault();
            }

            _dbContext.Containers.Add(container);

            _dbContext.SaveChanges();

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

        public void Delete(long containerId)
        {
            _dbContext.Containers.Remove(_dbContext.Containers.Find(containerId));

            _dbContext.SaveChanges();
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

        public IQueryable<Container> Search(string query)
        {
            return _dbContext.Containers.Where(c => c.Name.StartsWith(query.Trim()));
        }

        public IQueryable<Container> GetInstances(long structureId)
        {
            return _dbContext.Containers.Where(c => c.Prototype.StructureId == structureId && c.Parent == null);
        }
    }
}