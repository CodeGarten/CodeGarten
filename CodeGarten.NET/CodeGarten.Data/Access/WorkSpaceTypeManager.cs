using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class WorkSpaceTypeManager
    {
        private readonly Context _dbContext;

        public WorkSpaceTypeManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public WorkSpaceType Create(long structureId, string name, IEnumerable<string> services)
        {
            var workspace = new WorkSpaceType { StructureId = structureId, Name = name };

            foreach (var service in services)
                workspace.Services.Add(_dbContext.Services.Find(service));

            _dbContext.WorkSpaceTypes.Add(workspace);
            _dbContext.SaveChanges();

            return workspace;
        }

        public WorkSpaceType Get(long structureId, string name)
        {
            return _dbContext.WorkSpaceTypes.Find(name, structureId);
        }

        public WorkSpaceType Edit(long structureId, string name, IEnumerable<string> services)
        {
            var workspace = Get(structureId, name);

            workspace.Services.Clear();

            foreach (var service in services)
                workspace.Services.Add(_dbContext.Services.Find(service));

            _dbContext.SaveChanges();

            return workspace;
        }

        public void Delete(long structureId, string name)
        {
            _dbContext.WorkSpaceTypes.Remove(Get(structureId, name));
            _dbContext.SaveChanges();
        }

        public IQueryable<WorkSpaceType> GetAll(long structureId)
        {
            return _dbContext.WorkSpaceTypes.Where(wst => wst.StructureId == structureId);
        }
    }
}