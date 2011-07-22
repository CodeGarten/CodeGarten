using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;
using CodeGarten.Data.ModelView;

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
                               string rule = null, RoleBarrier roleBarrier = RoleBarrier.None)
        {
            var role = new Role()
                           {
                               RoleTypeName = roleType,
                               RoleTypeStructureId = structure,
                               ContainerPrototypeName = containerPrototype,
                               ContainerPrototypeStructureId = structure,
                               WorkSpaceTypeName = workspaceType,
                               WorkSpaceTypeStructureId = structure,
                               RoleBarrier = roleBarrier
                           };

            if (rule != null)
                role.Rules.Add(RuleManager.Get(_dbContext, structure, rule));

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
            return _dbContext.Roles.Where(rl => rl.ContainerPrototypeStructureId == structureId);
        }

        public void Delete(RoleView roleView, long structureId)
        {
            var role = _dbContext.Roles.Include("Rule").Single(r => r.ContainerPrototypeStructureId == structureId &&
                                                                    r.ContainerPrototypeName ==
                                                                    roleView.ContainerPrototypeName &&
                                                                    r.RoleTypeName == roleView.RoleTypeName &&
                                                                    r.WorkSpaceTypeName == roleView.WorkSpaceTypeName);

            _dbContext.Roles.Remove(role);

            _dbContext.SaveChanges();
        }
    }
}