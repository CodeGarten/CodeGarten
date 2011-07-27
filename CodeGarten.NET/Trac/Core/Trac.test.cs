using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using CodeGarten.Service.Utils;
using NUnit.Framework;

namespace Trac
{
    [TestFixture]
    class TracAdminTests
    {        
        private String _envPath = @"C:\CodeGarten\trac\instances\Grupo1";        
        
        [Test]
        public void ShouldAddUserPermission()
        {
            var user = "CodeGarten";
            TracAdmin.Add(user, _envPath, "BROWSER_VIEW");
            var userPrevs = TracAdmin.List(user, _envPath);
            
            Assert.Contains(TracPrivileges.BROWSER_VIEW, userPrevs );
        }

        [Test]
        public void ShouldRemoveUserPermission()
        {
            var user = "CodeGarten";
            TracAdmin.Add(user, _envPath, "BROWSER_VIEW");
            TracAdmin.Add(user, _envPath, "WIKI_VIEW");
            Assert.True(TracAdmin.Remove(user, _envPath, "BROWSER_VIEW"));

            var userPrevs = TracAdmin.List(user, _envPath);
            
            Assert.False(userPrevs.Contains(TracPrivileges.BROWSER_VIEW));   
        }

        [Test]
        public void ShouldListAllUsers()
        {            
            var user1 = "CodeGarten";
            var user2 = "ZeDosTestes";
            TracAdmin.Remove("*", _envPath, "*");
            TracAdmin.Add(user1, _envPath, "BROWSER_VIEW");
            TracAdmin.Add(user2, _envPath, "WIKI_VIEW");
            TracAdmin.Add(user2, _envPath, "WIKI_VIEW");

            var list = TracAdmin.ListAll(_envPath);
            Assert.True(list.ContainsKey(user1));
            Assert.True(list.ContainsKey(user2));
        }

        [Test]
        public void ShouldInitializeEnvironment()
        {
            var env = @"C:\CodeGarten\trac\instances\Grupo3";
            TracAdmin.InitEnv("Grupo3", env);
            Assert.True(TracAdmin.Add("CodeGarten", env, TracPrivileges.WIKI_CREATE.ToString()));
        }
    }

    [TestFixture]
    class PermissionTests
    {
        private TracPermissionManager _permission = new TracPermissionManager (
                                                                                @"C:\CodeGarten\trac\instances\Grupo3"
                                                                              );

        [Test]
        public void ShouldRemoveAllPermissionsAndUsers()
        {
            _permission.Add("CodeGarten", TracPrivileges.BROWSER_VIEW);
            _permission.Add("CodeGarten", TracPrivileges.WIKI_RENAME);

            _permission.RemoveAll();

            Assert.IsEmpty(_permission.List());
        }

        [Test]
        public void ShouldAddVariousPermissionAtTheSameTime()
        {
            _permission.RemoveAll();
            var user = "CodeGarten";
            _permission.Add(user, TracPrivileges.BROWSER_VIEW, TracPrivileges.CHANGESET_VIEW, TracPrivileges.EMAIL_VIEW);

            
            var list = _permission.List("CodeGarten");
            
            Assert.Contains(TracPrivileges.BROWSER_VIEW, list);
            Assert.Contains(TracPrivileges.EMAIL_VIEW, list);
            Assert.Contains(TracPrivileges.CHANGESET_VIEW, list);
        }

        [Test]
        public void ShouldRemoveVariousPermissionAtTheSameTime()
        {
            var user = "CodeGarten";

            _permission.RemoveAll();

            _permission.Add(user, TracPrivileges.BROWSER_VIEW, TracPrivileges.CHANGESET_VIEW, TracPrivileges.EMAIL_VIEW);
            _permission.Remove(user, TracPrivileges.BROWSER_VIEW, TracPrivileges.CHANGESET_VIEW, TracPrivileges.EMAIL_VIEW);

            Assert.IsEmpty(_permission.List());
        }

        [Test]
        public void ShouldAddUserGroupAndListUserGroups()
        {
            _permission.RemoveAll();

            var groupName = "Admins";
            var userName = "CodeGarten";

            _permission.Add(groupName, TracPrivileges.WIKI_VIEW, TracPrivileges.WIKI_RENAME);
            _permission.AddGroupUser(userName, groupName);

            var listGroups = _permission.ListGroups(userName);
            var privilegesesGroup = _permission.List(groupName);

            Assert.Contains(groupName, listGroups);
            Assert.Contains(TracPrivileges.WIKI_VIEW, privilegesesGroup);
            Assert.Contains(TracPrivileges.WIKI_RENAME, privilegesesGroup);
        }
    }

    [TestFixture]
    class EnvironmentTests
    {
        [Test]
        public void ShouldCreateAndInitializeEnvironment()
        {
            var path = @"C:\CodeGarten\trac\instances\Grupo4";
            if(Directory.Exists(path))
                Directory.Delete(path, true);
            
            TracEnvironmentManager environment = TracEnvironmentManager.Create(
                                                                                @"C:\CodeGarten\trac\instances",
                                                                                "Grupo4"
                                                                               );
            Assert.True(Directory.Exists(path));
            Assert.True(environment.Initialize());
        }

        [Test]
        public void ShouldNotCreateEnvironment()
        {
            TracEnvironmentManager environment = TracEnvironmentManager.Create(
                                                                                @"C:\CodeGarten\trac\instances",
                                                                                "Grupo1"
                                                                              );

            Assert.IsNull(environment);
        }

        [Test]
        public void ShouldListEnumPrivileges()
        {
            var list = EnumExtensions.ToEnumerable<TracPrivileges>();
            foreach (var tracPrivilege in list)
            {
                Console.WriteLine(tracPrivilege);
            }
        }
    }
}
