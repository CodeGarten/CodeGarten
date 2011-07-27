using System;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class StructureManager
    {
        private readonly Context _dbContext;

        public StructureManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
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

            structure.Administrators.Add(_dbContext.Users.Find(administrator));
            _dbContext.Structures.Add(structure);
            _dbContext.SaveChanges();

            return structure;
        }

        public IQueryable<Structure> GetAll(string username)
        {
            return username == null
                       ? _dbContext.Structures
                       : _dbContext.Structures.Where(s => s.Administrators.Select(a => a.Name).Contains(username));
        }

        public Structure Get(long id)
        {
            return _dbContext.Structures.Find(id);
        }

        public void Delete(long id)
        {
            _dbContext.Structures.Remove(Get(id));
            _dbContext.SaveChanges();
        }

        public void Publish(long id)
        {
            var structure = Get(id);
            var cps = _dbContext.ContainerPrototypes.Where(cp => cp.StructureId == id);


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
            _dbContext.SaveChanges();
        }
    }
}