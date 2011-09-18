using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class WorkSpaceTypeManager
    {
        private readonly DataBaseManager _dbManager;

        public WorkSpaceTypeManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public WorkSpaceType Create(long structureId, string name, IEnumerable<string> services)
        {
            var workspace = new WorkSpaceType { StructureId = structureId, Name = name };

            foreach (var service in services)
                workspace.Services.Add(_dbManager.ServiceType.Get(service));

            _dbManager.DbContext.WorkSpaceTypes.Add(workspace);
            _dbManager.DbContext.SaveChanges();

            return workspace;
        }

        public WorkSpaceType Get(long structureId, string name)
        {
            return _dbManager.DbContext.WorkSpaceTypes.Find(name, structureId);
        }

        public WorkSpaceType Edit(long structureId, string name, IEnumerable<string> services)
        {
            var workspace = Get(structureId, name);

            workspace.Services.Clear();

            foreach (var service in services)
                workspace.Services.Add(_dbManager.ServiceType.Get(service));

            _dbManager.DbContext.SaveChanges();

            return workspace;
        }

        public void Delete(long structureId, string name)
        {
            _dbManager.DbContext.WorkSpaceTypes.Remove(Get(structureId, name));
            _dbManager.DbContext.SaveChanges();
        }

        public IQueryable<WorkSpaceType> GetAll(long structureId)
        {
            return _dbManager.DbContext.WorkSpaceTypes.Where(wst => wst.StructureId == structureId);
        }
    }
}