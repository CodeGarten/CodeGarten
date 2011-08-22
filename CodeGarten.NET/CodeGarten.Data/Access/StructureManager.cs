using System;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class StructureManager
    {
        private readonly DataBaseManager _dbManager;

        public StructureManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public Structure Create(string name, string description, bool @public, string administrator)
        {
            var structure = new Structure
                                {
                                    Name = name,
                                    Description = description,
                                    Public = @public,
                                    Developing = true,
                                    CreatedOn = DateTime.Now
                                };

            structure.Administrators.Add(_dbManager.User.Get(administrator));
            _dbManager.DbContext.Structures.Add(structure);
            _dbManager.DbContext.SaveChanges();

            return structure;
        }

        public IQueryable<Structure> GetAll(string username = null)
        {
            return username == null
                       ? _dbManager.DbContext.Structures
                       : _dbManager.DbContext.Structures.Where(s => s.Administrators.Select(a => a.Name).Contains(username));
        }

        public Structure Get(long id)
        {
            return _dbManager.DbContext.Structures.Find(id);
        }

        public void Delete(long id)
        {
            foreach (var instance in _dbManager.Container.GetInstances(id).ToList())
                _dbManager.Container.Delete(instance.Id);

            _dbManager.DbContext.Structures.Remove(Get(id));
            _dbManager.DbContext.SaveChanges();
        }

        public void Publish(long id)
        {
            var structure = Get(id);
            var cps = _dbManager.ContainerPrototype.GetAll(id);


            if (cps.Count() == 0)
                throw new InvalidOperationException("A structure must contain at least one container prototype.");

            foreach (var cp in cps)
            {
                if (cp.Bindings.Count == 0)
                    throw new InvalidOperationException(
                        String.Format(
                            "A container prototype must contain at least one workspace. Error occured at container prototype '{0}'.", cp.Name));
                foreach (var binding in cp.Bindings)
                {
                    if (binding.Roles.Count == 0)
                        throw new InvalidOperationException(
                            String.Format(
                                "A workspace must contain at least one role type. Error occured at container prototype '{0}', workspace '{1}'.",
                                binding.ContainerPrototypeName, binding.WorkSpaceTypeName));
                    foreach (var role in binding.Roles)
                        if (role.Rules.Count == 0)
                            throw new InvalidOperationException(
                            String.Format(
                                "A role type must contain at least one rule. Error occured at container prototype '{0}', workspace '{1}', role type '{2}'.",
                                binding.ContainerPrototypeName, binding.WorkSpaceTypeName, role.RoleTypeName));
                }
            }

            structure.Developing = false;
            _dbManager.DbContext.SaveChanges();
        }

        public IQueryable<Structure> Search(string query)
        {
            return _dbManager.DbContext.Structures.Where(s => s.Name.StartsWith(query.Trim()));
        }

        public bool AddAdministrator(long id, string userName)
        {
            var structure = Get(id);
            if (structure.Administrators.Select(a => a.Name).Contains(userName))
                return false;

            structure.Administrators.Add(_dbManager.User.Get(userName));
            _dbManager.DbContext.SaveChanges();
            return true;
        }

        public bool RemoveAdministrator(long id, string userName)
        {
            var structure = Get(id);
            if (!structure.Administrators.Select(a => a.Name).Contains(userName))
                return false;

            structure.Administrators.Remove(_dbManager.User.Get(userName));
            _dbManager.DbContext.SaveChanges();
            return true;
        }
    }
}