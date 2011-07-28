using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{

    public sealed class RoleManager
    {
        private readonly Context _dbContext;

        public RoleManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
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
                foreach(var r in rules.Select(e => RuleManager.Get(_dbContext, structure, e)))
                    role.Rules.Add(r);

            _dbContext.Roles.Add(role);
            _dbContext.SaveChanges();

            return role;
        }

        public Role Get(long structure, string containerPrototype, string workspaceType, string roleType)
        {
            return _dbContext.Roles.Find(
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
            Get(structure, containerPrototype, workspaceType, roleType).Rules.Add(RuleManager.Get(_dbContext, structure, rule));
        }

        public IEnumerable<Role> GetAll(long structureId)
        {
            return _dbContext.Roles.Where(rl => rl.StructureId == structureId);
        }

        public void Delete(long structureId, string containerPrototype, string workspaceType, string roleType)
        {
            var role = Get(structureId, containerPrototype, workspaceType, roleType);

            _dbContext.Roles.Remove(role);

            _dbContext.SaveChanges();
        }
    }
}