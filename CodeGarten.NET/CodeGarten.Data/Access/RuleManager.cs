using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class RuleManager
    {
        private readonly Context _dbContext;

        public RuleManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public Rule Create(long structureId, string name, IEnumerable<string> permissions)
        {
            var rule = new Rule { Name = name, StructureId = structureId };

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

        internal static Rule Get(Context db, long structureId, string name)
        {
            return db.Rules.Find(name, structureId);
        }

        public Rule Get(long structureId, string name)
        {
            return Get(_dbContext, structureId, name);
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