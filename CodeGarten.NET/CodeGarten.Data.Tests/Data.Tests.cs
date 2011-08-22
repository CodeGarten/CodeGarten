using System;
using System.Collections.Generic;
using CodeGarten.Data.Access;
using CodeGarten.Data.Tests.ClassTests;
using NUnit.Framework;

namespace CodeGarten.Data.Tests
{
    [TestFixture]
    internal class Data
    {
        [Test]
        public void DeleteStructure()
        {
            using (var dbman = new DataBaseManager())
                dbman.Structure.Delete(1);
        }

        [Test]
        public void DeleteRoleType()
        {
            using (var dbman = new DataBaseManager())
                dbman.RoleType.Delete(1, "studant");
            
        }

        [Test]
        public void DeleteWorkSpaceType()
        {
            using (var dbman = new DataBaseManager())
                dbman.WorkSpaceType.Delete(1, "public");
        }

        [Test]
        public void DeleteRule()
        {
            using (var dbman = new DataBaseManager())
                dbman.Rule.Delete(1, "studant");
        }

        [Test]
        public void DeleteContainerPrototype()
        {
            using (var dbman = new DataBaseManager())
                dbman.ContainerPrototype.Delete(1, "Course");
        }

        [Test]
        public void DeleteContainer()
        {
            using (var dbman = new DataBaseManager())
                dbman.Container.Delete(2);
        }

        //[Test]
        //public void Drop()
        //{
        //    using (var dataBaseManager = new DataBaseManager())
        //        dataBaseManager.DbContext.Database.Delete();
        //}

        //[Test]
        //public void Create()
        //{
        //    DataBaseManager.Initializer();
        //}

        [Test]
        public void ShouldTestBasicFeatures()
        {
            using (var dataBaseManager = new DataBaseManager())
            {
                #region CREATE_SERVICES

                var serviceManager = dataBaseManager.Service;

                var serviceGit = serviceManager.Create("Git", "System Version Control (decentralized)", new[] { "r", "rw" });

                var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] { "r", "rw" });

                var serviceTrac = serviceManager.Create("Trac", "WIKI system", new[] { "TRAC_ADMIN" });

                #endregion

                #region CREATE_USERS

                var userManager = dataBaseManager.User;

                var userFaustino = userManager.Create("FaustinoLeiras", "FaustinoLeiras12345", "FaustinoLeiras@gmail.com");

                var userSamir = userManager.Create("SamirHafez", "SamirHafez12345", "SamirHafez@gmail.com");

                var userRicardo = userManager.Create("Ricardo", "Ricardo12345", "Ricardo@gmail.com");

                var userGeada = userManager.Create("Gueada", "Gueada12345", "Gueada@gmail.com");

                var userFelix = userManager.Create("Felix", "Felix12345", "Felix@gmail.com");

                var userGuedes = userManager.Create("Guedes", "Guedes12345", "Guedes@gmail.com");

                #endregion

                #region CREATE_STRUCTURE

                var structureManager = dataBaseManager.Structure;

                var structure = structureManager.Create("AcademicStructure", "My Academic Structure :)", true, userFaustino.Name);
                
                #endregion

                #region CREATE_WORKSPACE_TYPE

                var workspaceType = dataBaseManager.WorkSpaceType;

                var workspacePublic = workspaceType.Create(structure.Id, "public", new[] { serviceGit.Name, serviceSvn.Name, serviceTrac.Name });

                var workspacePrivate = workspaceType.Create(structure.Id, "private", new[] { serviceGit.Name, serviceSvn.Name, serviceTrac.Name });

                #endregion

                #region CREATE_CONTAINER_PROTOTYPE

                var containerPrototype = dataBaseManager.ContainerPrototype;

                var prototypeGraduation = containerPrototype.Create(structure.Id, "Graduation", null);

                var prototypeCourse = containerPrototype.Create(structure.Id, "Course", prototypeGraduation.Name);


                var prototypeClass = containerPrototype.Create(structure.Id, "Class", prototypeCourse.Name);

                var prototypeGroup = containerPrototype.Create(structure.Id, "Group", prototypeClass.Name);

                #endregion

                #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGraduation.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

                #endregion

                #region CREATE_ROLETYPE

                var roleType = dataBaseManager.RoleType;

                var roleTypeTeacher = roleType.Create(structure.Id, "teacher");

                var roleTypeDirector = roleType.Create(structure.Id, "director");

                var roleTypeStudant = roleType.Create(structure.Id, "studant");

                #endregion

                #region CREATE_RULE

                var rule = dataBaseManager.Rule;

                var ruleReaders = rule.Create(structure.Id, "readers", new[]
                                                           {
                                                               new KeyValuePair<string, string>(serviceGit.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceSvn.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceTrac.Name, "TRAC_ADMIN")
                                                           });

                var ruleReadersAndWriters = rule.Create(structure.Id, "ReadersAndWriters", new[]
                                                                     {
                                                                         new KeyValuePair<string, string>(
                                                                             serviceGit.Name,
                                                                             "rw"),
                                                                         new KeyValuePair<string, string>(
                                                                             serviceSvn.Name,
                                                                             "rw"),
                                                                         new KeyValuePair<string, string>(
                                                                             serviceTrac.Name,
                                                                             "TRAC_ADMIN")
                                                                     });

                #endregion

                #region CREATE_ROLE

                var role = dataBaseManager.Role;

                #region ADD_ROLES_GRADUATION_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGraduation.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new [] {ruleReadersAndWriters.Name}
                    );

                #endregion

                #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new [] {ruleReadersAndWriters.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new [] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new [] {ruleReaders.Name}
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new [] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new [] {ruleReadersAndWriters.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new [] {ruleReaders.Name}
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeDirector.Name,
                    new [] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeTeacher.Name,
                    new [] {ruleReadersAndWriters.Name}
                    );

                #endregion

                #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new [] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new [] {ruleReadersAndWriters.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new [] {ruleReadersAndWriters.Name}
                    );

                #endregion

                #endregion

                #region CREATE_CONTAINERS

                var container = dataBaseManager.Container;

                var containerLeic = container.Create(structure.Id, "LEIC", "Licencitura Egenharia informatica e de computadores", null);
                
                #region CREATE_COURSE_MPD

                var containerMpd = container.Create(structure.Id, "MPD", "Modelo de padoes de desenho", containerLeic.Id);

                var containerMpdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerMpd.Id);

                var containerMpdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de MPD", containerMpdLi31D.Id);

                var containerMpdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de MPD", containerMpdLi31D.Id);

                #endregion

                #region CREATE_COURSE_SD

                var containerSd = container.Create(structure.Id, "SD", "Sistemas distribuidos", containerLeic.Id);

                var containerSdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerSd.Id);

                var containerSdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de SD", containerSdLi31D.Id);

                var containerSdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de SD", containerSdLi31D.Id);

                #endregion

                #endregion

                #region ENROLL_USER

                userManager.Enroll(userFelix.Name, structure.Id, containerLeic.Id, roleTypeDirector.Name);

                userManager.Enroll(userFelix.Name, structure.Id, containerMpd.Id, roleTypeDirector.Name);
                userManager.Enroll(userGuedes.Name, structure.Id, containerSd.Id, roleTypeDirector.Name);

                userManager.Enroll(userFelix.Name, structure.Id, containerSdLi31D.Id, roleTypeTeacher.Name);
                userManager.Enroll(userGuedes.Name, structure.Id, containerMpdLi31D.Id, roleTypeTeacher.Name);

                userManager.Enroll(userFaustino.Name, structure.Id, containerMpdG1.Id, roleTypeStudant.Name);
                userManager.Enroll(userSamir.Name, structure.Id, containerMpdG1.Id, roleTypeStudant.Name);

                userManager.Enroll(userRicardo.Name, structure.Id, containerMpdG2.Id, roleTypeStudant.Name);
                userManager.Enroll(userGeada.Name, structure.Id, containerMpdG2.Id, roleTypeStudant.Name);

                userManager.Enroll(userFaustino.Name, structure.Id, containerSdG1.Id, roleTypeStudant.Name);
                userManager.Enroll(userRicardo.Name, structure.Id, containerSdG1.Id, roleTypeStudant.Name);

                userManager.Enroll(userGeada.Name, structure.Id, containerSdG2.Id, roleTypeStudant.Name);
                userManager.Enroll(userSamir.Name, structure.Id, containerSdG2.Id, roleTypeStudant.Name);

                #endregion

                //AuthorizationTestes aut = new AuthorizationTestes();
                //AuthorizationManager authorizationManager = new AuthorizationManager(dataBaseManager);

                //authorizationManager.CreateServiceAuthorizationStruct(aut, serviceSvn.Name);
            }
        }

        [Test]
        public void ShouldTestDisentoll()
        {

            using (var dataBaseManager = new DataBaseManager())
            {

                #region CREATE_SERVICES

                var serviceManager = dataBaseManager.Service;

                var serviceGit = serviceManager.Create("Git", "System Version Control (decentralized)", new[] { "r", "rw" });

                var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] { "r", "rw" });

                var serviceTrac = serviceManager.Create("Trac", "WIKI system", new[] { "TRAC_ADMIN" });

                #endregion

                #region CREATE_USERS

                var userManager = dataBaseManager.User;

                var userFaustino = userManager.Create("FaustinoLeiras", "FaustinoLeiras12345", "FaustinoLeiras@gmail.com");

                var userSamir = userManager.Create("SamirHafez", "SamirHafez12345", "SamirHafez@gmail.com");

                var userRicardo = userManager.Create("Ricardo", "Ricardo12345", "Ricardo@gmail.com");

                var userGeada = userManager.Create("Gueada", "Gueada12345", "Gueada@gmail.com");

                var userFelix = userManager.Create("Felix", "Felix12345", "Felix@gmail.com");

                var userGuedes = userManager.Create("Guedes", "Guedes12345", "Guedes@gmail.com");

                #endregion

                #region CREATE_STRUCTURE

                var structureManager = dataBaseManager.Structure;

                var structure = structureManager.Create("AcademicStructure", "My Academic Structure :)", true, userFaustino.Name);

                #endregion

                #region CREATE_WORKSPACE_TYPE

                var workspaceType = dataBaseManager.WorkSpaceType;

                var workspacePublic = workspaceType.Create(structure.Id, "public", new[] { serviceGit.Name, serviceSvn.Name, serviceTrac.Name });

                var workspacePrivate = workspaceType.Create(structure.Id, "private", new[] { serviceGit.Name, serviceSvn.Name, serviceTrac.Name });

                #endregion

                #region CREATE_CONTAINER_PROTOTYPE

                var containerPrototype = dataBaseManager.ContainerPrototype;

                var prototypeGraduation = containerPrototype.Create(structure.Id, "Graduation", null);

                var prototypeCourse = containerPrototype.Create(structure.Id, "Course", prototypeGraduation.Name);


                var prototypeClass = containerPrototype.Create(structure.Id, "Class", prototypeCourse.Name);

                var prototypeGroup = containerPrototype.Create(structure.Id, "Group", prototypeClass.Name);

                #endregion

                #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGraduation.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

                #endregion

                #region CREATE_ROLETYPE

                var roleType = dataBaseManager.RoleType;

                var roleTypeTeacher = roleType.Create(structure.Id, "teacher");

                var roleTypeDirector = roleType.Create(structure.Id, "director");

                var roleTypeStudant = roleType.Create(structure.Id, "studant");

                #endregion

                #region CREATE_RULE

                var rule = dataBaseManager.Rule;

                var ruleReaders = rule.Create(structure.Id, "readers", new[]
                                                           {
                                                               new KeyValuePair<string, string>(serviceGit.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceSvn.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceTrac.Name, "TRAC_ADMIN")
                                                           });

                var ruleReadersAndWriters = rule.Create(structure.Id, "ReadersAndWriters", new[]
                                                                     {
                                                                         new KeyValuePair<string, string>(
                                                                             serviceGit.Name,
                                                                             "rw"),
                                                                         new KeyValuePair<string, string>(
                                                                             serviceSvn.Name,
                                                                             "rw"),
                                                                         new KeyValuePair<string, string>(
                                                                             serviceTrac.Name,
                                                                             "TRAC_ADMIN")
                                                                     });

                #endregion

                #region CREATE_ROLE

                var role = dataBaseManager.Role;

                #region ADD_ROLES_GRADUATION_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGraduation.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] { ruleReadersAndWriters.Name }
                    );

                #endregion

                #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] { ruleReadersAndWriters.Name }
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new[] { ruleReaders.Name }
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new[] { ruleReaders.Name }
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] { ruleReaders.Name }
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new[] { ruleReadersAndWriters.Name }
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new[] { ruleReaders.Name }
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeDirector.Name,
                    new[] { ruleReaders.Name }
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeTeacher.Name,
                    new[] { ruleReadersAndWriters.Name }
                    );

                #endregion

                #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] { ruleReaders.Name }
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new[] { ruleReadersAndWriters.Name }
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new[] { ruleReadersAndWriters.Name }
                    );

                #endregion

                #endregion

                #region CREATE_CONTAINERS

                var container = dataBaseManager.Container;

                var containerLeic = container.Create(structure.Id, "LEIC", "Licencitura Egenharia informatica e de computadores", null);

                #region CREATE_COURSE_MPD

                var containerMpd = container.Create(structure.Id, "MPD", "Modelo de padoes de desenho", containerLeic.Id);

                var containerMpdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerMpd.Id);

                var containerMpdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de MPD", containerMpdLi31D.Id);

                var containerMpdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de MPD", containerMpdLi31D.Id);

                #endregion

                #region CREATE_COURSE_SD

                var containerSd = container.Create(structure.Id, "SD", "Sistemas distribuidos", containerLeic.Id);

                var containerSdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerSd.Id);

                var containerSdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de SD", containerSdLi31D.Id);

                var containerSdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de SD", containerSdLi31D.Id);

                #endregion

                #endregion

                #region ENROLL_USER

                userManager.Enroll(userFelix.Name, structure.Id, containerLeic.Id, roleTypeDirector.Name);

                userManager.Enroll(userFelix.Name, structure.Id, containerMpd.Id, roleTypeDirector.Name);

                #endregion

                #region DISENROLL_USER

                userManager.Disenroll(userFelix.Name, structure.Id, containerLeic.Id, roleTypeDirector.Name);

                userManager.Disenroll(userFelix.Name, structure.Id, containerMpd.Id, roleTypeDirector.Name);

                #endregion

            }
        }

        [Test]
        public void ShouldTestSyncronizeEnrollContainers()
        {
            using (var dataBaseManager = new DataBaseManager())
            {
                #region CREATE_SERVICES

                var serviceManager = dataBaseManager.Service;

                var serviceGit = serviceManager.Create("Git", "System Version Control (decentralized)",
                                                       new[] {"r", "rw"});

                var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] {"r", "rw"});

                var serviceTrac = serviceManager.Create("Trac", "WIKI system", new[] {"TRAC_ADMIN"});

                #endregion

                #region CREATE_USERS

                var userManager = dataBaseManager.User;

                var userFaustino = userManager.Create("FaustinoLeiras", "FaustinoLeiras12345",
                                                      "FaustinoLeiras@gmail.com");

                var userSamir = userManager.Create("SamirHafez", "SamirHafez12345", "SamirHafez@gmail.com");

                var userRicardo = userManager.Create("Ricardo", "Ricardo12345", "Ricardo@gmail.com");

                var userGeada = userManager.Create("Gueada", "Gueada12345", "Gueada@gmail.com");

                var userFelix = userManager.Create("Felix", "Felix12345", "Felix@gmail.com");

                var userGuedes = userManager.Create("Guedes", "Guedes12345", "Guedes@gmail.com");

                #endregion

                #region CREATE_STRUCTURE

                var structureManager = dataBaseManager.Structure;

                var structure = structureManager.Create("AcademicStructure", "My Academic Structure :)", true,
                                                        userFaustino.Name);

                #endregion

                #region CREATE_WORKSPACE_TYPE

                var workspaceType = dataBaseManager.WorkSpaceType;

                var workspacePublic = workspaceType.Create(structure.Id, "public",
                                                           new[] {serviceGit.Name, serviceSvn.Name, serviceTrac.Name});

                var workspacePrivate = workspaceType.Create(structure.Id, "private",
                                                            new[] {serviceGit.Name, serviceSvn.Name, serviceTrac.Name});

                #endregion

                #region CREATE_CONTAINER_PROTOTYPE

                var containerPrototype = dataBaseManager.ContainerPrototype;

                var prototypeGraduation = containerPrototype.Create(structure.Id, "Graduation", null);

                var prototypeCourse = containerPrototype.Create(structure.Id, "Course", prototypeGraduation.Name);


                var prototypeClass = containerPrototype.Create(structure.Id, "Class", prototypeCourse.Name);

                var prototypeGroup = containerPrototype.Create(structure.Id, "Group", prototypeClass.Name);

                #endregion

                #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGraduation.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

                #endregion

                #region CREATE_ROLETYPE

                var roleType = dataBaseManager.RoleType;

                var roleTypeTeacher = roleType.Create(structure.Id, "teacher");

                var roleTypeDirector = roleType.Create(structure.Id, "director");

                var roleTypeStudant = roleType.Create(structure.Id, "studant");

                #endregion

                #region CREATE_RULE

                var rule = dataBaseManager.Rule;

                var ruleReaders = rule.Create(structure.Id, "readers", new[]
                                                                           {
                                                                               new KeyValuePair<string, string>(
                                                                                   serviceGit.Name, "r"),
                                                                               new KeyValuePair<string, string>(
                                                                                   serviceSvn.Name, "r"),
                                                                               new KeyValuePair<string, string>(
                                                                                   serviceTrac.Name, "TRAC_ADMIN")
                                                                           });

                var ruleReadersAndWriters = rule.Create(structure.Id, "ReadersAndWriters", new[]
                                                                                               {
                                                                                                   new KeyValuePair
                                                                                                       <string, string>(
                                                                                                       serviceGit.Name,
                                                                                                       "rw"),
                                                                                                   new KeyValuePair
                                                                                                       <string, string>(
                                                                                                       serviceSvn.Name,
                                                                                                       "rw"),
                                                                                                   new KeyValuePair
                                                                                                       <string, string>(
                                                                                                       serviceTrac.Name,
                                                                                                       "TRAC_ADMIN")
                                                                                               });

                #endregion

                #region CREATE_ROLE

                var role = dataBaseManager.Role;

                #region ADD_ROLES_GRADUATION_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGraduation.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] {ruleReadersAndWriters.Name}
                    );

                #endregion

                #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] {ruleReadersAndWriters.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new[] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new[] {ruleReaders.Name}
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new[] {ruleReadersAndWriters.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new[] {ruleReaders.Name}
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeDirector.Name,
                    new[] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeTeacher.Name,
                    new[] {ruleReadersAndWriters.Name}
                    );

                #endregion

                #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    new[] {ruleReaders.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    new[] {ruleReadersAndWriters.Name}
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    new[] {ruleReadersAndWriters.Name}
                    );

                #endregion

                #endregion

                var container = dataBaseManager.Container;

                var containerLeic = container.Create(structure.Id, "LEIC", "Licencitura Egenharia informatica e de computadores", null);

                #region CREATE_COURSE_MPD

                var containerMpd = container.Create(structure.Id, "MPD", "Modelo de padoes de desenho", containerLeic.Id);

                var containerMpdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerMpd.Id);

                var containerMpdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de MPD", containerMpdLi31D.Id);

                var containerMpdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de MPD", containerMpdLi31D.Id);

                #endregion

                #region ENROLL_USER

                userManager.Enroll(userFelix.Name, structure.Id, containerLeic.Id, roleTypeDirector.Name);

                userManager.Enroll(userFelix.Name, structure.Id, containerMpd.Id, roleTypeDirector.Name);

                #endregion

                #region CREATE_COURSE_SD

                var containerSd = container.Create(structure.Id, "SD", "Sistemas distribuidos", containerLeic.Id);

                var containerSdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerSd.Id);

                var containerSdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de SD", containerSdLi31D.Id);

                var containerSdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de SD", containerSdLi31D.Id);

                #endregion
            }
        }

        [Test]
        public void ShouldTestBasicFeaturesWithoutCache()
        {
            
            var dataBaseManager = new DataBaseManager();

            #region CREATE_SERVICES

            var serviceManager = dataBaseManager.Service;

            var serviceGit = serviceManager.Create("Git", "System Version Control (decentralized)", new[] { "r", "rw" });

            var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] { "r", "rw" });

            var serviceTrac = serviceManager.Create("Trac", "WIKI system", new[] { "TRAC_ADMIN" });

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_USERS

            var userManager = dataBaseManager.User;

            var userFaustino = userManager.Create("FaustinoLeiras", "FaustinoLeiras12345", "FaustinoLeiras@gmail.com");

            var userSamir = userManager.Create("SamirHafez", "SamirHafez12345", "SamirHafez@gmail.com");

            var userRicardo = userManager.Create("Ricardo", "Ricardo12345", "Ricardo@gmail.com");

            var userGeada = userManager.Create("Gueada", "Gueada12345", "Gueada@gmail.com");

            var userFelix = userManager.Create("Felix", "Felix12345", "Felix@gmail.com");

            var userGuedes = userManager.Create("Guedes", "Guedes12345", "Guedes@gmail.com");

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_STRUCTURE

            var structureManager = dataBaseManager.Structure;

            var structure = structureManager.Create("AcademicStructure", "My Academic Structure :)", true, userFaustino.Name);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_WORKSPACE_TYPE

            var workspaceType = dataBaseManager.WorkSpaceType;

            var workspacePublic = workspaceType.Create(structure.Id, "public", new[] { serviceGit.Name, serviceSvn.Name, serviceTrac.Name });

            var workspacePrivate = workspaceType.Create(structure.Id, "private", new[] { serviceGit.Name, serviceSvn.Name, serviceTrac.Name });

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_CONTAINER_PROTOTYPE

            var containerPrototype = dataBaseManager.ContainerPrototype;

            var prototypeGraduation = containerPrototype.Create(structure.Id, "Graduation", null);

            var prototypeCourse = containerPrototype.Create(structure.Id, "Course", prototypeGraduation.Name);


            var prototypeClass = containerPrototype.Create(structure.Id, "Class", prototypeCourse.Name);

            var prototypeGroup = containerPrototype.Create(structure.Id, "Group", prototypeClass.Name);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

            containerPrototype.AddWorkSpaceType(structure.Id, prototypeGraduation.Name, workspacePublic.Name);

            containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

            containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
            containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

            containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_ROLETYPE

            var roleType = dataBaseManager.RoleType;

            var roleTypeTeacher = roleType.Create(structure.Id, "teacher");

            var roleTypeDirector = roleType.Create(structure.Id, "director");

            var roleTypeStudant = roleType.Create(structure.Id, "studant");

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_RULE

            var rule = dataBaseManager.Rule;

            var ruleReaders = rule.Create(structure.Id, "readers", new[]
                                                           {
                                                               new KeyValuePair<string, string>(serviceGit.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceSvn.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceTrac.Name, "TRAC_ADMIN")
                                                           });

            var ruleReadersAndWriters = rule.Create(structure.Id, "ReadersAndWriters", new[]
                                                                     {
                                                                         new KeyValuePair<string, string>(
                                                                             serviceGit.Name,
                                                                             "rw"),
                                                                         new KeyValuePair<string, string>(
                                                                             serviceSvn.Name,
                                                                             "rw"),
                                                                         new KeyValuePair<string, string>(
                                                                             serviceTrac.Name,
                                                                             "TRAC_ADMIN")
                                                                     });

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_ROLE

            var role = dataBaseManager.Role;

            #region ADD_ROLES_GRADUATION_WORKSPACE_PUBLIC

            role.Create(
                structure.Id,
                prototypeGraduation.Name,
                workspacePublic.Name,
                roleTypeDirector.Name,
                new[] { ruleReadersAndWriters.Name }
                );

            #endregion

            #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

            role.Create(
                structure.Id,
                prototypeCourse.Name,
                workspacePublic.Name,
                roleTypeDirector.Name,
                new[] { ruleReadersAndWriters.Name }
                );

            role.Create(
                structure.Id,
                prototypeCourse.Name,
                workspacePublic.Name,
                roleTypeTeacher.Name,
                new[] { ruleReaders.Name }
                );

            role.Create(
                structure.Id,
                prototypeCourse.Name,
                workspacePublic.Name,
                roleTypeStudant.Name,
                new[] { ruleReaders.Name }
                );

            #endregion

            #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePublic.Name,
                roleTypeDirector.Name,
                new[] { ruleReaders.Name }
                );

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePublic.Name,
                roleTypeTeacher.Name,
                new[] { ruleReadersAndWriters.Name }
                );

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePublic.Name,
                roleTypeStudant.Name,
                new[] { ruleReaders.Name }
                );

            #endregion

            #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePrivate.Name,
                roleTypeDirector.Name,
                new[] { ruleReaders.Name }
                );

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePrivate.Name,
                roleTypeTeacher.Name,
                new[] { ruleReadersAndWriters.Name }
                );

            #endregion

            #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

            role.Create(
                structure.Id,
                prototypeGroup.Name,
                workspacePublic.Name,
                roleTypeDirector.Name,
                new[] { ruleReaders.Name }
                );

            role.Create(
                structure.Id,
                prototypeGroup.Name,
                workspacePublic.Name,
                roleTypeTeacher.Name,
                new[] { ruleReadersAndWriters.Name }
                );

            role.Create(
                structure.Id,
                prototypeGroup.Name,
                workspacePublic.Name,
                roleTypeStudant.Name,
                new[] { ruleReadersAndWriters.Name }
                );

            #endregion

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_CONTAINERS

            var container = dataBaseManager.Container;

            var containerLeic = container.Create(structure.Id, "LEIC", "Licencitura Egenharia informatica e de computadores", null);

            #region CREATE_COURSE_MPD

            var containerMpd = container.Create(structure.Id, "MPD", "Modelo de padoes de desenho", containerLeic.Id);

            var containerMpdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerMpd.Id);

            var containerMpdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de MPD", containerMpdLi31D.Id);

            var containerMpdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de MPD", containerMpdLi31D.Id);

            #endregion

            #region CREATE_COURSE_SD

            var containerSd = container.Create(structure.Id, "SD", "Sistemas distribuidos", containerLeic.Id);

            var containerSdLi31D = container.Create(structure.Id, "LI31D", "Turma 1 de terceiro semestre diurno", containerSd.Id);

            var containerSdG1 = container.Create(structure.Id, "Grupo1", "Grupo 1 de SD", containerSdLi31D.Id);

            var containerSdG2 = container.Create(structure.Id, "Grupo2", "Grupo 2 de SD", containerSdLi31D.Id);

            #endregion

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region ENROLL_USER

            userManager.Enroll(userFelix.Name, structure.Id, containerLeic.Id, roleTypeDirector.Name);

            userManager.Enroll(userFelix.Name, structure.Id, containerMpd.Id, roleTypeDirector.Name);
            userManager.Enroll(userGuedes.Name, structure.Id, containerSd.Id, roleTypeDirector.Name);

            userManager.Enroll(userFelix.Name, structure.Id, containerSdLi31D.Id, roleTypeTeacher.Name);
            userManager.Enroll(userGuedes.Name, structure.Id, containerMpdLi31D.Id, roleTypeTeacher.Name);

            userManager.Enroll(userFaustino.Name, structure.Id, containerMpdG1.Id, roleTypeStudant.Name);
            userManager.Enroll(userSamir.Name, structure.Id, containerMpdG1.Id, roleTypeStudant.Name);

            userManager.Enroll(userRicardo.Name, structure.Id, containerMpdG2.Id, roleTypeStudant.Name);
            userManager.Enroll(userGeada.Name, structure.Id, containerMpdG2.Id, roleTypeStudant.Name);

            userManager.Enroll(userFaustino.Name, structure.Id, containerSdG1.Id, roleTypeStudant.Name);
            userManager.Enroll(userRicardo.Name, structure.Id, containerSdG1.Id, roleTypeStudant.Name);

            userManager.Enroll(userGeada.Name, structure.Id, containerSdG2.Id, roleTypeStudant.Name);
            userManager.Enroll(userSamir.Name, structure.Id, containerSdG2.Id, roleTypeStudant.Name);

            #endregion

            dataBaseManager.Dispose();
            //using (dataBaseManager = new DataBaseManager())
            //{
            //    AuthorizationTestes aut = new AuthorizationTestes();
            //    AuthorizationManager authorizationManager = new AuthorizationManager(dataBaseManager);

            //    authorizationManager.CreateServiceAuthorizationStruct(aut, serviceSvn.Name);
            //}
        }

    }
}