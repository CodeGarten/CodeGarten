using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeGarten.Data.Model;
using CodeGarten.Data.ModelView;

namespace CodeGarten.Data.Access
{
    public sealed class WorkSpaceTypeManager
    {
        private readonly Context _dbContext;

        public WorkSpaceTypeManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public void Create(WorkSpaceTypeView workSpaceTypeView, long structure)
        {
            if (workSpaceTypeView == null) throw new ArgumentNullException("workSpaceTypeView");

            var structureObj = StructureManager.Get(_dbContext, structure);
            if (structureObj == null) throw new ArgumentException("\"structure\" is a invalid argument");

            var workSpaceType = workSpaceTypeView.Convert();

            workSpaceType.Structure = structureObj;

            _dbContext.WorkSpaceTypes.Add(workSpaceType);

            _dbContext.SaveChanges();
        }


        public void Create(WorkSpaceTypeView workSpaceTypeView, long structure, IEnumerable<string> services)
        {
            if (workSpaceTypeView == null) throw new ArgumentNullException("workSpaceTypeView");
            if (services == null) throw new ArgumentNullException("services");

            var structureObj = StructureManager.Get(_dbContext, structure);
            if (structureObj == null) throw new ArgumentException("\"structure\" is a invalid argument");

            var workSpaceType = workSpaceTypeView.Convert();

            workSpaceType.Structure = structureObj;

            foreach (var service in services)
            {
                var serviceObj = ServiceManager.Get(_dbContext, service);
                if (serviceObj == null)
                    throw new ArgumentException(String.Format("The service {0} is a invalid argument", service));
                //TO DO CONTAINS
                workSpaceType.Services.Add(serviceObj);
            }

            _dbContext.WorkSpaceTypes.Add(workSpaceType);

            _dbContext.SaveChanges();
        }

        public void Delete(WorkSpaceTypeView workSpaceTypeView, long structureId)
        {
            //_dbContext.Entry(_dbContext.WorkSpaceTypes.Find(workSpaceTypeView.Name,structureId)).State = EntityState.Deleted;
            _dbContext.WorkSpaceTypes.Remove(_dbContext.WorkSpaceTypes.Find(workSpaceTypeView.Name, structureId));

            _dbContext.SaveChanges();
        }

        internal static WorkSpaceType Get(Context db, long structure, string workspace)
        {
            return db.WorkSpaceTypes.Where(
                (wk) =>
                wk.Name == workspace &&
                wk.StructureId == structure
                ).SingleOrDefault();
        }

        public WorkSpaceTypeView Get(long structure, string workspace)
        {
            var workSpaceTypeObj = Get(_dbContext, structure, workspace);
            return workSpaceTypeObj == null ? null : workSpaceTypeObj.Convert();
        }

        public void AddService(long structure, string workSpaceType, string service)
        {
            if (service == null) throw new ArgumentNullException("service");

            var workSpaceTypeObj = Get(_dbContext, structure, workSpaceType);
            if (workSpaceTypeObj == null)
                throw new ArgumentException("\"structure\" or \"workSpaceType\" is a invalid argument");


            var serviceObj = ServiceManager.Get(_dbContext, service);
            if (serviceObj == null) throw new ArgumentException("\"service\" is a invalid argument");
            //TO DO CONTAINS
            workSpaceTypeObj.Services.Add(serviceObj);

            _dbContext.Entry(workSpaceTypeObj).State = EntityState.Modified;

            _dbContext.SaveChanges();
        }

        public void RemoveService(long structure, string workSpaceType, string service)
        {
            if (service == null) throw new ArgumentNullException("service");

            var workSpaceTypeObj = Get(_dbContext, structure, workSpaceType);
            if (workSpaceTypeObj == null)
                throw new ArgumentException("\"structure\" or \"workSpaceType\" is a invalid argument");


            var serviceObj = ServiceManager.Get(_dbContext, service);
            if (serviceObj == null) throw new ArgumentException("\"service\" is a invalid argument");
            //TO DO CONTAINS
            workSpaceTypeObj.Services.Remove(serviceObj);

            _dbContext.Entry(workSpaceTypeObj).State = EntityState.Modified;

            _dbContext.SaveChanges();
        }
    }
}