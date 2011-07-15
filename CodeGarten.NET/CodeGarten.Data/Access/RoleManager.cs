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

        public RoleView Create(long structure, string containerPrototype, string workspaceType, string roleType,
                               string rule)
        {
            var containerPrototypeObj = ContainerPrototypeManager.Get(_dbContext, structure, containerPrototype);
            if (containerPrototypeObj == null)
                throw new ArgumentException("\"containerPrototype\" or \"structure\" is a invalid argument");

            var workspaceTypeObj = WorkSpaceTypeManager.Get(_dbContext, structure, workspaceType);
            if (workspaceTypeObj == null)
                throw new ArgumentException("\"workspaceType\" or \"structure\" is a invalid argument");

            var roleTypeObj = RoleTypeManager.Get(_dbContext, structure, roleType);
            if (roleTypeObj == null) throw new ArgumentException("\"roleType\" or \"structure\" is a invalid argument");

            var ruleObj = RuleManager.Get(_dbContext, structure, rule);
            if (ruleObj == null) throw new ArgumentException("\"rule\" or \"structure\" is a invalid argument");

            var role = new Role()
                           {
                               RoleType = roleTypeObj,
                               ContainerPrototype = containerPrototypeObj,
                               WorkSpaceType = workspaceTypeObj,
                               RoleTypeStructureId = structure,
                               Rule = ruleObj
                           };

            _dbContext.Roles.Add(role);
            _dbContext.SaveChanges();

            return role.Convert();
        }

        public RoleView Get(string containerPrototype, string workspaceType, string roleType)
        {
            return new RoleView
                       {
                           ContainerPrototypeName = containerPrototype,
                           WorkSpaceTypeName = workspaceType,
                           RoleTypeName = roleType
                       };
        }
        //TODO
        public IEnumerable<Role> GetAll(long structureId)
        {
            return _dbContext.Roles.Where(rl => rl.RuleStructureId == structureId);
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