using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    //TODO change location
    //public class ContainerEventArgs : EventArgs
    //{
    //    public long Strucuture { get; set; }
    //    public ContainerView Container { get; set; }
    //    public IDictionary<string, IEnumerable<WorkSpaceTypeView>> Services;
    //}

    public sealed class ContainerManager
    {
        private readonly Context _dbContext;

        public ContainerManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

    //    #region Events

    //    private static event EventHandler<ContainerEventArgs> _onCreateContainer;

    //    public static event EventHandler<ContainerEventArgs> OnCreateContainer
    //    {
    //        add { _onCreateContainer += value; }
    //        remove { _onCreateContainer -= value; }
    //    }

    //    #endregion

    //    public void Create(ContainerView containerView, long structure, long? parent)
    //    {
    //        if (containerView == null) throw new ArgumentNullException("containerView");

    //        var container = containerView.Convert();

    //        if (parent != null)
    //        {
    //            var parentobj = _dbContext.Containers.Find(parent);

    //            container.ContainerPrototype = parentobj.ContainerPrototype.Childs.First();
    //            container.ParentContainer = parentobj;
    //        }
    //        else
    //        {
    //            container.ContainerPrototype =
    //                _dbContext.ContainerPrototypes.Where(cp => cp.StructureId == structure && cp.Parent == null).
    //                    SingleOrDefault();
    //        }

    //        _dbContext.Containers.Add(container);

    //        _dbContext.SaveChanges();

    //        //TODO return container view
    //        containerView.Id = container.Id;

    //        //TODO 
    //        try
    //        {
    //            InvokeOnCreateContainer(container);
    //        }
    //        catch
    //        {
    //            //TODO ROLLBACK
    //            throw;
    //        }
    //    }

    //    public void Create(ContainerView containerView, long structure, string containerPrototype)
    //    {
    //        if (containerView == null) throw new ArgumentNullException("containerView");

    //        var container = containerView.Convert();

    //        var containerPrototypeObj = ContainerPrototypeManager.Get(_dbContext, structure, containerPrototype);
    //        if (containerPrototypeObj == null) throw new ArgumentException("Invalid argument");

    //        container.ContainerPrototype = containerPrototypeObj;

    //        _dbContext.Containers.Add(container);

    //        _dbContext.SaveChanges();

    //        //TODO return container view
    //        containerView.Id = container.Id;

    //        //TODO 
    //        try
    //        {
    //            InvokeOnCreateContainer(container);
    //        }
    //        catch
    //        {
    //            //TODO ROLLBACK
    //            throw;
    //        }
    //    }

    //    public void Create(ContainerView containerView, long structure, string containerPrototype, long parentContainer)
    //    {
    //        if (containerView == null) throw new ArgumentNullException("containerView");

    //        var container = containerView.Convert();

    //        var containerPrototypeObj = ContainerPrototypeManager.Get(_dbContext, structure, containerPrototype);
    //        if (containerPrototypeObj == null) throw new ArgumentException("Invalid argument");

    //        var parentContainerObj = Get(_dbContext, parentContainer);
    //        if (containerPrototypeObj.Parent.Name != parentContainerObj.ContainerPrototype.Name) throw new Exception();

    //        container.ParentContainer = parentContainerObj;

    //        container.ContainerPrototype = containerPrototypeObj;

    //        _dbContext.Containers.Add(container);

    //        _dbContext.SaveChanges();

    //        //TODO return container view
    //        containerView.Id = container.Id;

    //        //TODO 
    //        try
    //        {
    //            InvokeOnCreateContainer(container);
    //        }
    //        catch
    //        {
    //            //TODO ROLLBACK
    //            throw;
    //        }
    //    }

    //    public void Delete(ContainerView containerView)
    //    {
    //        //_dbContext.Entry(_dbContext.Containers.Find(containerView.Id)).State = EntityState.Deleted;

    //        _dbContext.Containers.Remove(_dbContext.Containers.Find(containerView.Id));

    //        _dbContext.SaveChanges();
    //    }

    //    internal static Container Get(Context db, long container)
    //    {
    //        return db.Containers.Where(c =>
    //                                   c.Id == container
    //            ).SingleOrDefault();
    //    }

    //    public ContainerView Get(long container)
    //    {
    //        var cont = Get(_dbContext, container);

    //        return cont == null ? null : cont.Convert();
    //    }

    //    #region InvokeEvents

    //    private void InvokeOnCreateContainer(Container container)
    //    {
    //        var dicionay = new Dictionary<string, IEnumerable<WorkSpaceTypeView>>();
    //        foreach (var service in _dbContext.Services)
    //        {
    //            var serviceContext = service;
    //            var wk = container.ContainerPrototype.WorkSpaceTypes.Where(
    //                w => w.Services.Contains(serviceContext)
    //                ).Select(
    //                    w => w.Convert()
    //                );
    //            if (wk.Any())
    //                dicionay.Add(service.Name, wk.ToList());
    //        }

    //        var eventArgs = new ContainerEventArgs()
    //                            {
    //                                Strucuture = container.ContainerPrototype.StructureId,
    //                                Container = container.Convert(),
    //                                Services = dicionay
    //                            };
    //        var handler = _onCreateContainer;
    //        if (handler != null) handler(this, eventArgs);
    //    }

    //    #endregion
        public Container Get(long id)
        {
            return _dbContext.Containers.Find(id);
        }

        public Container Create(long structureId, string name, long? parent)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            _dbContext.Containers.Remove(_dbContext.Containers.Find(id));
            _dbContext.SaveChanges();
        }
    }
}