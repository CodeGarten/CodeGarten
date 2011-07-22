using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rule = CodeGarten.Data.Model.Rule;

namespace CodeGarten.Data.Access
{
    public sealed class RuleManager
    {
        private readonly Context _dbContext;

        public RuleManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        //public void Create(RuleView ruleView, long structure)
        //{
        //    if (ruleView == null) throw new ArgumentNullException("ruleView");

        //    var rule = ruleView.Convert();

        //    var structureObj = StructureManager.Get(_dbContext, structure);
        //    if (structureObj == null) throw new ArgumentException("\"structure\" is a invalid argument");

        //    rule.Structure = structureObj;

        //    _dbContext.Rules.Add(rule);

        //    _dbContext.SaveChanges();
        //}

        //public void Delete(RuleView ruleView, long structure)
        //{
        //    //_dbContext.Entry(_dbContext.Rules.Find(ruleView.Name, structure)).State = EntityState.Deleted;

        //    _dbContext.Rules.Remove(_dbContext.Rules.Find(ruleView.Name, structure));

        //    _dbContext.SaveChanges();
        //}

        //public void Create(RuleView ruleView, long structure,
        //                   IEnumerable<KeyValuePair<string, string>> pairServicePermissions)
        //{
        //    if (ruleView == null) throw new ArgumentNullException("ruleView");
        //    if (pairServicePermissions == null) throw new ArgumentNullException("pairServicePermissions");

        //    var rule = ruleView.Convert();

        //    var structureObj = StructureManager.Get(_dbContext, structure);
        //    if (structureObj == null) throw new ArgumentException("\"structure\" is a invalid argument");

        //    rule.Structure = structureObj;

        //    foreach (var pairServicePermission in pairServicePermissions)
        //    {
        //        var servicePermissionObj = ServiceManager.GetPermission(_dbContext, pairServicePermission.Key,
        //                                                                pairServicePermission.Value);
        //        if (servicePermissionObj == null)
        //            throw new ArgumentException(
        //                String.Format("The Service Permission {0} {1} is a invalid argument",
        //                              pairServicePermission.Key,
        //                              pairServicePermission.Value)
        //                );

        //        //TO DO verify is rule. permissions contains servicePermission
        //        rule.Permissions.Add(servicePermissionObj);
        //    }

        //    _dbContext.Rules.Add(rule);

        //    _dbContext.SaveChanges();
        //}

        //internal static Rule Get(Context db, long structure, string rule)
        //{
        //    return db.Rules.Where(
        //        (rl) =>
        //        rl.Name == rule &&
        //        rl.StructureId == structure
        //        ).SingleOrDefault();
        //}

        //public RuleView Get(long structure, string rule)
        //{
        //    var ruleObj = Get(_dbContext, structure, rule);

        //    return ruleObj == null ? null : ruleObj.Convert();
        //}

        //public IEnumerable<Rule> GetAll(long structureId)
        //{
        //    return _dbContext.Rules.Where(rl => rl.StructureId == structureId);
        //}

        //public bool AddPermission(long structure, string rule, string service, string permission)
        //{
        //    var ruleObj = Get(_dbContext, structure, rule);
        //    if (ruleObj == null) throw new ArgumentException("\"rule\" is a invalid argument");

        //    var servicePermissionObj = ServiceManager.GetPermission(_dbContext, service, permission);
        //    if (servicePermissionObj == null)
        //        throw new ArgumentException("\"service\" or \"permission\" is a invalid argument");

        //    if (ruleObj.Permissions.Where((sp) =>
        //                                  sp.Name == permission &&
        //                                  sp.ServiceName == service
        //            ).Count() != 0) return false;

        //    ruleObj.Permissions.Add(servicePermissionObj);

        //    _dbContext.Entry(ruleObj).State = EntityState.Modified;

        //    return _dbContext.SaveChanges() != 0;
        //}

        //public bool RemovePermission(long structure, string rule, string service, string permission)
        //{
        //    var ruleObj = Get(_dbContext, structure, rule);
        //    if (ruleObj == null) throw new ArgumentException("\"rule\" is a invalid argument");

        //    var servicePermissionObj = ServiceManager.GetPermission(_dbContext, service, permission);
        //    if (servicePermissionObj == null)
        //        throw new ArgumentException("\"service\" or \"permission\" is a invalid argument");

        //    ruleObj.Permissions.Remove(servicePermissionObj);

        //    _dbContext.Entry(ruleObj).State = EntityState.Modified;

        //    return _dbContext.SaveChanges() != 0;
        //}
        public Rule Create(long structureId, string name, IEnumerable<string> permissions)
        {
            var rule = new Rule { Name = name, StructureId = structureId };

            if (permissions != null)
                foreach (var permission in permissions.Select(p => p.Split(' ')))
                {
                    var serviceName = permission[0];
                    var permissionName = permission[1];
                    rule.Permissions.Add(_dbContext.ServicePermissions.Find(permissionName, serviceName));
                }

            _dbContext.Rules.Add(rule);
            _dbContext.SaveChanges();

            return rule;
        }

        public Rule Get(long structureId, string name)
        {
            return _dbContext.Rules.Find(name, structureId);
        }

        public void Edit(long structureId, string name, IEnumerable<string> permissions)
        {
            var rule = Get(structureId, name);

            rule.Permissions.Clear();

            if (permissions != null)
                foreach (var permission in permissions.Select(p => p.Split(' ')))
                {
                    var serviceName = permission[0];
                    var permissionName = permission[1];
                    rule.Permissions.Add(_dbContext.ServicePermissions.Find(permissionName, serviceName));
                }

            _dbContext.SaveChanges();
        }

        public void Delete(long structureId, string name)
        {
            _dbContext.Rules.Remove(Get(structureId, name));
            _dbContext.SaveChanges();
        }

        public IQueryable<Rule> GetAll(long structureId)
        {
            return _dbContext.Rules.Where(r => r.StructureId == structureId);
        }
    }
}