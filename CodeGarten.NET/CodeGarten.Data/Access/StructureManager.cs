﻿using System;
using System.Collections.Generic;
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

        //public void Create(StructureView structureView, string user)
        //{
        //    if (structureView == null) throw new ArgumentNullException("structureView");

        //    var structure = structureView.Convert();

        //    var userObj = UserManager.Get(_dbContext, user);
        //    if (user == null) throw new ArgumentException("\"user\" is a invalid argument");

        //    structure.Administrators.Add(userObj);

        //    structure.CreatedOn = DateTime.Now;

        //    _dbContext.Structures.Add(structure);
        //    _dbContext.SaveChanges();

        //    //TODO return structure view
        //    structureView.Id = structure.Id;
        //}

        //public void Delete(StructureView structureView)
        //{
        //    _dbContext.Structures.Remove(_dbContext.Structures.Find(structureView.Id));
        //    //_dbContext.Entry(_dbContext.Structures.Find(structureView.Id)).State = EntityState.Deleted;

        //    _dbContext.SaveChanges();
        //}


        //internal static Structure Get(Context db, long structure)
        //{
        //    return db.Structures.Where((st) => st.Id == structure).SingleOrDefault();
        //}

        //public StructureView Get(long structure)
        //{
        //    var structureObj = Get(_dbContext, structure);
        //    return structureObj == null ? null : structureObj.Convert();
        //}

        //public IEnumerable<StructureView> Get(bool developing, string user)
        //{
        //    var userObj = UserManager.Get(_dbContext, user);
        //    if (userObj == null) throw new ArgumentException("\"user\" is a invalid argument");

        //    return userObj.Structures.Where((st) => st.Developing == developing).Select((st) => st.Convert());
        //}

        //public IEnumerable<StructureView> Search(string name)
        //{
        //    return _dbContext.Structures.Where(
        //        (st) =>
        //        st.Public &&
        //        st.Developing == false &&
        //        st.Name.StartsWith(name)
        //        ).Distinct().Select(st => st.Convert());
        //}
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

        public IQueryable<Structure> GetAll()
        {
            return _dbContext.Structures;
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
                    foreach(var role in binding.Roles)
                        if(role.Rules.Count == 0)
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