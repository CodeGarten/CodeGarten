using System;
using System.Collections.Generic;
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

        //public RoleView Create(long structure, string containerPrototype, string workspaceType, string roleType,
        //                       string rule)
        //{
        //    var containerPrototypeObj = ContainerPrototypeManager.Get(_dbContext, structure, containerPrototype);
        //    if (containerPrototypeObj == null)
        //        throw new ArgumentException("\"containerPrototype\" or \"structure\" is a invalid argument");

        //    var workspaceTypeObj = WorkSpaceTypeManager.Get(_dbContext, structure, workspaceType);
        //    if (workspaceTypeObj == null)
        //        throw new ArgumentException("\"workspaceType\" or \"structure\" is a invalid argument");

        //    var roleTypeObj = RoleTypeManager.Get(_dbContext, structure, roleType);
        //    if (roleTypeObj == null) throw new ArgumentException("\"roleType\" or \"structure\" is a invalid argument");

        //    var ruleObj = RuleManager.Get(_dbContext, structure, rule);
        //    if (ruleObj == null) throw new ArgumentException("\"rule\" or \"structure\" is a invalid argument");

        //    var role = new Role()
        //                   {
        //                       RoleType = roleTypeObj,
        //                       ContainerPrototype = containerPrototypeObj,
        //                       WorkSpaceType = workspaceTypeObj,
        //                       RoleTypeStructureId = structure,
        //                       Rule = ruleObj
        //                   };

        //    _dbContext.Roles.Add(role);
        //    _dbContext.SaveChanges();

        //    return role.Convert();
        //}

        //public RoleView Get(string containerPrototype, string workspaceType, string roleType)
        //{
        //    return new RoleView
        //               {
        //                   ContainerPrototypeName = containerPrototype,
        //                   WorkSpaceTypeName = workspaceType,
        //                   RoleTypeName = roleType
        //               };
        //}
        ////TODO
        //public IEnumerable<Role> GetAll(long structureId)
        //{
        //    return _dbContext.Roles.Where(rl => rl.RuleStructureId == structureId);
        //}

        //public void Delete(RoleView roleView, long structureId)
        //{
        //    var role = _dbContext.Roles.Include("Rule").Single(r => r.ContainerPrototypeStructureId == structureId &&
        //                                                            r.ContainerPrototypeName ==
        //                                                            roleView.ContainerPrototypeName &&
        //                                                            r.RoleTypeName == roleView.RoleTypeName &&
        //                                                            r.WorkSpaceTypeName == roleView.WorkSpaceTypeName);

        //    _dbContext.Roles.Remove(role);

        //    _dbContext.SaveChanges();
        //}
        public Role Get(long structureId, string containerPrototypeName, string workspaceName, string roleTypeName)
        {
            //TODO - Por a funcionar

            return _dbContext.Roles.Find(containerPrototypeName, structureId, roleTypeName, structureId, workspaceName, structureId);
        }

        public IQueryable<Role> GetAll(long structureId)
        {
            return _dbContext.Roles.Where(r => r.StructureId == structureId);
        }

        public void Delete(long structureId, string containerPrototypeName, string workSpaceTypeName, string roleTypeName)
        {
            _dbContext.Roles.Remove(Get(structureId, containerPrototypeName, workSpaceTypeName, roleTypeName));
            _dbContext.SaveChanges();
        }

        public Role Create(long structureId, string containerPrototypeName, string workSpacetypeName, string roleTypeName, int blockBarrier, IEnumerable<string> rules)
        {
            var role = new Role
                           {
                               StructureId = structureId,
                               ContainerPrototypeName = containerPrototypeName,
                               WorkSpaceTypeName = workSpacetypeName,
                               RoleTypeName = roleTypeName,
                               BlockBarrier = blockBarrier
                           };

            if (rules != null)
                foreach (var rule in _dbContext.Rules.Where(r => r.StructureId == structureId && rules.Contains(r.Name)))
                    role.Rules.Add(rule);

            _dbContext.Roles.Add(role);
            _dbContext.SaveChanges();

            return role;
        }

        public void DeleteAll(long structureId)
        {
            foreach (var role in _dbContext.Roles.Where(r => r.StructureId == structureId))
                _dbContext.Roles.Remove(role);

            _dbContext.SaveChanges();
        }
    }
}