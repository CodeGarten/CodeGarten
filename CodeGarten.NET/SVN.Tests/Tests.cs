﻿using System;
using System.IO;
using NUnit.Framework;
using SVN.Core;

namespace SVN.Tests
{
    [TestFixture]
    public class SVNAuthorizationTests
    {
        [Test]
        public void ShouldCreateSNVAuthorizationFile()
        {
            //using (var svnAuthorization = new SVNAuthorization(@"C:\CodeGarten\svn\etc\svn_acl"))
            //{
            //    svnAuthorization.CreateGroup("GrupoDosFortes",
            //                                 new String[] {"CodeGarten", "FaustinoLeiras", "SamirHafez"});
            //    svnAuthorization.CreateGroup("GrupoDosFracos", new String[] {"ZeTestes"});
            //    svnAuthorization.AddAllPermissionsToRepository("repository2", SVNPrivileges.r);
            //    svnAuthorization.AddUserPermissionsToRepository("repository1", "Albertina", SVNPrivileges.r);
            //    svnAuthorization.AddGroupPermissionsToRepository("repository1", "GrupoDosFortes", SVNPrivileges.rw);
            //    svnAuthorization.AddGroupPermissionsToRepository("repository2", "GrupoDosFracos", SVNPrivileges.rw);
            //}
        }

        [Test]
        public void ShouldCreateAndInitializeRepositorySVN()
        {
            var path = @"C:\CodeGarten\svn\repositories\repo4";
            if (Directory.Exists(path))
                Directory.Delete(path, true);


            SVNRepositoryManager repository = SVNRepositoryManager.Create(
                @"C:\CodeGarten\svn\repositories",
                "repo4"
                );

            Assert.IsNotNull(repository);
            Assert.True(Directory.Exists(path));
            Assert.True(repository.Initialize());
        }

        [Test]
        public void ShouldGetConfigurtions()
        {
            
        }
    }
}