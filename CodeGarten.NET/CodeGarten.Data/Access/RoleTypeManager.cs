using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class RoleTypeManager
    {
        private readonly DataBaseManager _dbManager;

        public RoleTypeManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public RoleType Create(long structureId, string name)
        {
            var rt = new RoleType {StructureId = structureId, Name = name};

            _dbManager.DbContext.RoleTypes.Add(rt);
            _dbManager.DbContext.SaveChanges();

            return rt;
        }

        public void Delete(long structureId, string name)
        {
            _dbManager.DbContext.RoleTypes.Remove(Get(structureId, name));
            _dbManager.DbContext.SaveChanges();
        }

        public IQueryable<RoleType> GetAll(long structureId)
        {
            return _dbManager.DbContext.RoleTypes.Where(rt => rt.StructureId == structureId);
        }

        public RoleType Get(long structureId, string name)
        {
            return _dbManager.DbContext.RoleTypes.Find(name, structureId);
        }
    }
}