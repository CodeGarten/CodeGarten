using System;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{

    public class ContainerEventArgs : EventArgs
    {
        public ContainerEventArgs(Container container, ContainerType containerType)
        {
            Container = container;
            Type = containerType;
        }

        public Container Container { get; private set; }
        public ContainerType Type { get; set; }
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

                container.Type = parentobj.Type.Childs.First();
                container.Parent = parentobj;
            }
            else
            {
                container.Type =
                    _dbManager.ContainerType.GetAll(structure).Where(
                        cp => cp.StructureId == structure && cp.Parent == null).SingleOrDefault();
            }

            _dbManager.DbContext.Containers.Add(container);

            _dbManager.DbContext.SaveChanges();

            InvokeOnCreateContainer(container);

            _dbManager.User.SyncronizeEnrolls(container);

            _dbManager.DbContext.SaveChanges();

            return container;
        }

        public Container Create(long structure, string containerName, string description, long? parent, string prototypeName)
        {
            var container = new Container
                                {
                                    Type = _dbManager.ContainerType.Get(structure, prototypeName),
                                    Description = description,
                                    Name = containerName,
                                    Parent = parent == null ? null : Get(parent.Value)
                                };
            _dbManager.DbContext.Containers.Add(container);

            _dbManager.DbContext.SaveChanges();

            InvokeOnCreateContainer(container);

            _dbManager.User.SyncronizeEnrolls(container);

            _dbManager.DbContext.SaveChanges();

            return container;
        }

        public Container Delete(long containerId)
        {
            var container = Get(containerId);
            var prototype = container.Type;

            var parent = container.Parent;

            foreach(var child in container.Childs.ToList())
            {
                Delete(child.Id);
            }

            _dbManager.DbContext.Containers.Remove(container);

            _dbManager.DbContext.SaveChanges();

            InvokeOnDeleteContainer(container, prototype);
            
            return parent;
        }

        public void AddPassword(long structure, long container, string roletype, string password)
        {
            var enrollPassword = new EnrollKey()
                                     {
                                         ContainerId = container,
                                         RoleTypeName = roletype,
                                         StructureId = structure,
                                         Credential = AuthenticationManager.EncryptPassword(password)
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
            var eventArgs = new ContainerEventArgs(container, container.Type);

            if (_onCreateContainer != null) 
                try
                {
                    _onCreateContainer(this, eventArgs);    
                }catch(Exception e)
                {
                    DataBaseManager.Logger.Log(String.Format("InvokeOnCreateContainer fail - {0}", e.Message));
                }
        }

        
        private void InvokeOnDeleteContainer(Container container, ContainerType containerType)
        {
            var eventArgs = new ContainerEventArgs(container, containerType);

            if (_onDeleteContainer != null) 
                try
                {
                    _onDeleteContainer(this, eventArgs);    
                }catch(Exception e)
                {
                    DataBaseManager.Logger.Log(String.Format("InvokeOnDeleteContainer fail - {0}", e.Message));
                }
                
        }

        #endregion

        public IQueryable<Container> Search(string query)
        {
            return _dbManager.DbContext.Containers.Where(c => c.Name.StartsWith(query.Trim()));
        }

        public IQueryable<Container> GetInstances(long structureId)
        {
            return _dbManager.DbContext.Containers.Where(c => c.Type.StructureId == structureId && c.Parent == null);
        }
    }
}