using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class RoleTypeManager
    {
        private readonly Context _dbContext;

        public RoleTypeManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public RoleType Create(long structureId, string name)
        {
            var rt = new RoleType {StructureId = structureId, Name = name};

            _dbContext.RoleTypes.Add(rt);
            _dbContext.SaveChanges();

            return rt;
        }

        public void Delete(long structureId, string name)
        {
            _dbContext.RoleTypes.Remove(Get(structureId, name));
            _dbContext.SaveChanges();
        }

        public IQueryable<RoleType> GetAll(long structureId)
        {
            return _dbContext.RoleTypes.Where(rt => rt.StructureId == structureId);
        }

        public RoleType Get(long structureId, string name)
        {
            return Get(_dbContext, structureId, name);
        }

        internal static RoleType Get(Context context, long structureId, string roleType)
        {
            return context.RoleTypes.Find(roleType, structureId);
        }
    }
}