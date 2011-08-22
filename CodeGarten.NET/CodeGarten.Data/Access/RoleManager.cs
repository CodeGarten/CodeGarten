using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{

    public sealed class RoleManager
    {
        private readonly DataBaseManager _dbManager;

        public RoleManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public Role Create(long structure, string containerPrototype, string workspaceType, string roleType,
                               IEnumerable<string> rules = null, RoleBarrier roleBarrier = RoleBarrier.None)
        {
            var role = new Role()
                           {
                               RoleTypeName = roleType,
                               StructureId = structure,
                               ContainerPrototypeName = containerPrototype,
                               WorkSpaceTypeName = workspaceType,
                               RoleBarrier = roleBarrier
                           };

            if (rules != null)
                foreach(var r in rules.Select(e => _dbManager.Rule.Get(structure, e)))
                    role.Rules.Add(r);

            _dbManager.DbContext.Roles.Add(role);
            _dbManager.DbContext.SaveChanges();

            return role;
        }

        public Role Get(long structure, string containerPrototype, string workspaceType, string roleType)
        {
            return _dbManager.DbContext.Roles.Find(
                                            containerPrototype,
                                            structure,
                                            roleType,
                                            structure,
                                            workspaceType,
                                            structure
                                         );
        }

        public void AddRule(long structure, string containerPrototype, string workspaceType, string roleType, string rule)
        {
            Get(structure, containerPrototype, workspaceType, roleType).Rules.Add(_dbManager.Rule.Get(structure, rule));
        }

        public IEnumerable<Role> GetAll(long structureId)
        {
            return _dbManager.DbContext.Roles.Where(rl => rl.StructureId == structureId);
        }

        public void Delete(long structureId, string containerPrototype, string workspaceType, string roleType)
        {
            var role = Get(structureId, containerPrototype, workspaceType, roleType);

            _dbManager.DbContext.Roles.Remove(role);

            _dbManager.DbContext.SaveChanges();
        }
    }
}