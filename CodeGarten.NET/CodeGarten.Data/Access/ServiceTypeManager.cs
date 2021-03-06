﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class ServiceTypeManager
    {
        private readonly DataBaseManager _dbManager;

        public ServiceTypeManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public ServiceType Create(string name, string description)
        {
            var service = new ServiceType
                              {
                Name = name,
                Description = description
            };

            _dbManager.DbContext.Services.Add(service);

            _dbManager.DbContext.SaveChanges();

            return service;
        }

        public ServiceType Create(string name, string description, IEnumerable<string> permissions)
        {
            var service = new ServiceType
                              {
                                  Name = name,
                                  Description = description
                              };

            foreach (var permission in permissions)
                service.Permissions.Add(new ServiceTypePermission
                                            {
                                                Name = permission,
                                                ServiceType = service
                                            });


            _dbManager.DbContext.Services.Add(service);
            _dbManager.DbContext.SaveChanges();

            return service;
        }

        public ServiceType Get(string service)
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

            var servicePermissionObj = new ServiceTypePermission
                                           {
                                               Name = permission,
                                           };

            serviceObj.Permissions.Add(servicePermissionObj);

            _dbManager.DbContext.Entry(serviceObj).State = EntityState.Modified;

            return _dbManager.DbContext.SaveChanges() != 0;
        }

        public IQueryable<ServiceType> GetAll()
        {
            return _dbManager.DbContext.Services;
        }
    }
}