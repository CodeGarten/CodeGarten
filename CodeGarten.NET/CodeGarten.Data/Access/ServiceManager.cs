using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeGarten.Data.Model;
using CodeGarten.Data.ModelView;

namespace CodeGarten.Data.Access
{
    public sealed class ServiceManager
    {
        private readonly Context _dbContext;

        public ServiceManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public void Create(ServiceView serviceView)
        {
            if (serviceView == null) throw new ArgumentNullException("serviceView");

            var service = serviceView.Convert();

            _dbContext.Services.Add(service);

            _dbContext.SaveChanges();
        }

        public void Create(ServiceView serviceView, IEnumerable<string> permissions)
        {
            if (serviceView == null) throw new ArgumentNullException("serviceView");
            if (permissions == null) throw new ArgumentNullException("permissions");

            var service = serviceView.Convert();

            foreach (var permission in permissions)
                service.Permissions.Add(new ServicePermission()
                                            {
                                                Name = permission,
                                                Service = service
                                            });


            _dbContext.Services.Add(service);
            _dbContext.SaveChanges();
        }

        internal static Service Get(Context db, string service)
        {
            return db.Services.Where((s) => s.Name == service).SingleOrDefault();
        }

        public ServiceView Get(string service)
        {
            var serviceObj = Get(_dbContext, service);
            return serviceObj == null ? null : serviceObj.Convert();
        }

        internal static IEnumerable<ServicePermission> GetPermissions(Context db, string service)
        {
            var serviceObj = db.Services.Where((s) => s.Name == service).SingleOrDefault();
            if (serviceObj == null) throw new ArgumentException("\"service\" is a invalid argument");

            foreach (var permission in serviceObj.Permissions)
                yield return permission;
            yield break;
        }

        public IEnumerable<String> GetPermissions(string service)
        {
            foreach (var servicePermission in GetPermissions(_dbContext, service))
                yield return servicePermission.Name;
            yield break;
        }

        internal static ServicePermission GetPermission(Context db, string service, string permission)
        {
            return db.ServicePermissions.Where((sp) =>
                                               sp.Name == permission &&
                                               sp.ServiceName == service
                ).SingleOrDefault();
        }

        public string GetPermission(string service, string permission)
        {
            var servicePermission = GetPermission(_dbContext, service, permission);
            return servicePermission == null ? null : servicePermission.Name;
        }

        public bool AddPermission(string service, string permission)
        {
            var serviceObj = Get(_dbContext, service);
            if (serviceObj == null) throw new ArgumentException("\"service\" is a invalid argument");

            if (serviceObj.Permissions.Where((p) => p.Name == permission).Count() != 0) return false;

            var servicePermissionObj = new ServicePermission
                                           {
                                               Name = permission,
                                           };

            serviceObj.Permissions.Add(servicePermissionObj);

            _dbContext.Entry(serviceObj).State = EntityState.Modified;

            return _dbContext.SaveChanges() != 0;
        }
    }
}