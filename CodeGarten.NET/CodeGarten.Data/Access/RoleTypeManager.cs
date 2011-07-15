using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;
using CodeGarten.Data.ModelView;

namespace CodeGarten.Data.Access
{
    public sealed class RoleTypeManager
    {
        private readonly Context _dbContext;

        public RoleTypeManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public void Create(RoleTypeView roleTypeView, long structure)
        {
            if (roleTypeView == null) throw new ArgumentNullException("roleTypeView");

            var roleType = roleTypeView.Convert();

            var strucureObj = StructureManager.Get(_dbContext, structure);
            if (strucureObj == null) throw new ArgumentException("\"structure\" is a invalid argument");

            roleType.Structure = strucureObj;

            _dbContext.RoleTypes.Add(roleType);
            _dbContext.SaveChanges();
        }

        public void Delete(RoleTypeView roleTypeView, long structure)
        {
            //TODO Do better
            _dbContext.RoleTypes.Remove(_dbContext.RoleTypes.Find(roleTypeView.Name, structure));
            _dbContext.SaveChanges();
        }

        internal static RoleType Get(Context db, long structure, string roleType)
        {
            return db.RoleTypes.Where(
                (rt) =>
                rt.Name == roleType &&
                rt.StructureId == structure
                ).SingleOrDefault();
        }

        public RoleTypeView Get(long structure, string roleType)
        {
            var roleTp = Get(_dbContext, structure, roleType);

            return roleTp == null ? null : roleTp.Convert();
        }
        //TODO
        public IEnumerable<RoleType> GetAll(long structureId)
        {
            return _dbContext.RoleTypes.Where(rt => rt.StructureId== structureId);
        }
    }
}