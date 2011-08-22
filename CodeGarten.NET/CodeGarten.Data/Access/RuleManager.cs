using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class RuleManager
    {
        private readonly DataBaseManager _dbManager;

        public RuleManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public Rule Create(long structureId, string name, IEnumerable<string> permissions)
        {
            var rule = new Rule { Name = name, StructureId = structureId };

            foreach (var permission in permissions.Select(p => p.Split(' ')))
            {
                var serviceName = permission[0];
                var permissionName = permission[1];
                rule.Permissions.Add(_dbManager.DbContext.ServicePermissions.Find(permissionName, serviceName));
            }

            _dbManager.DbContext.Rules.Add(rule);
            _dbManager.DbContext.SaveChanges();

            return rule;
        }

        public Rule Create(long structureId, string name, IEnumerable<KeyValuePair<string, string>> permissions)
        {
            var rule = new Rule { Name = name, StructureId = structureId };

            foreach (var permission in permissions)
                rule.Permissions.Add(_dbManager.DbContext.ServicePermissions.Find(permission.Value, permission.Key));

            _dbManager.DbContext.Rules.Add(rule);
            _dbManager.DbContext.SaveChanges();

            return rule;
        }

        public Rule Get(long structureId, string name)
        {
            return _dbManager.DbContext.Rules.Find(name, structureId);
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
                    rule.Permissions.Add(_dbManager.DbContext.ServicePermissions.Find(permissionName, serviceName));
                }

            _dbManager.DbContext.SaveChanges();
        }

        public void Delete(long structureId, string name)
        {
            _dbManager.DbContext.Rules.Remove(Get(structureId, name));
            _dbManager.DbContext.SaveChanges();
        }

        public IQueryable<Rule> GetAll(long structureId)
        {
            return _dbManager.DbContext.Rules.Where(r => r.StructureId == structureId);
        }
    }
}