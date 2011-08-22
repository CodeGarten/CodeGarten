using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class ServiceManager
    {
        private readonly DataBaseManager _dbManager;

        public ServiceManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public Service Create(string name, string description)
        {
            var service = new Service
                              {
                Name = name,
                Description = description
            };

            _dbManager.DbContext.Services.Add(service);

            _dbManager.DbContext.SaveChanges();

            return service;
        }

        public Service Create(string name, string description, IEnumerable<string> permissions)
        {
            var service = new Service
                              {
                                  Name = name,
                                  Description = description
                              };

            foreach (var permission in permissions)
                service.Permissions.Add(new ServicePermission
                                            {
                                                Name = permission,
                                                Service = service
                                            });


            _dbManager.DbContext.Services.Add(service);
            _dbManager.DbContext.SaveChanges();

            return service;
        }

        public Service Get(string service)
        {
            return _dbManager.DbContext.Services.Find(service);
        }

        public IEnumerable<String> GetPermissions(string service)
        {
            return Get(service).Permissions.Select(servicePermission => servicePermission.Name);
        }

        public string GetPermission(string service, string permission)
        {
            var servicePermission = _dbManager.DbContext.ServicePermissions.Find(permission, service);
            return servicePermission == null ? null : servicePermission.Name;
        }

        public bool AddPermission(string service, string permission)
        {
            var serviceObj = Get(service);

            if (serviceObj.Permissions.Where(p => p.Name == permission).Count() != 0) return false;

            var servicePermissionObj = new ServicePermission
                                           {
                                               Name = permission,
                                           };

            serviceObj.Permissions.Add(servicePermissionObj);

            _dbManager.DbContext.Entry(serviceObj).State = EntityState.Modified;

            return _dbManager.DbContext.SaveChanges() != 0;
        }

        public IQueryable<Service> GetAll()
        {
            return _dbManager.DbContext.Services;
        }
    }
}