using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class ServiceManager
    {
        private readonly Context _dbContext;

        public ServiceManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public Service Create(string name, string description)
        {
            var service = new Service
                              {
                Name = name,
                Description = description
            };

            _dbContext.Services.Add(service);

            _dbContext.SaveChanges();

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


            _dbContext.Services.Add(service);
            _dbContext.SaveChanges();

            return service;
        }

        internal static Service Get(Context db, string service)
        {
            return db.Services.Find(service);
        }

        public Service Get(string service)
        {
            return Get(_dbContext, service);
        }

        internal static IEnumerable<ServicePermission> GetPermissions(Context db, string service)
        {
            return Get(db, service).Permissions;
        }

        public IEnumerable<String> GetPermissions(string service)
        {
            return GetPermissions(_dbContext, service).Select(servicePermission => servicePermission.Name);
        }

        internal static ServicePermission GetPermission(Context db, string service, string permission)
        {
            return db.ServicePermissions.Find(permission, service);
        }

        public string GetPermission(string service, string permission)
        {
            var servicePermission = GetPermission(_dbContext, service, permission);
            return servicePermission == null ? null : servicePermission.Name;
        }

        public bool AddPermission(string service, string permission)
        {
            var serviceObj = Get(_dbContext, service);

            if (serviceObj.Permissions.Where(p => p.Name == permission).Count() != 0) return false;

            var servicePermissionObj = new ServicePermission
                                           {
                                               Name = permission,
                                           };

            serviceObj.Permissions.Add(servicePermissionObj);

            _dbContext.Entry(serviceObj).State = EntityState.Modified;

            return _dbContext.SaveChanges() != 0;
        }

        public IQueryable<Service> GetAll()
        {
            return _dbContext.Services;
        }
    }
}