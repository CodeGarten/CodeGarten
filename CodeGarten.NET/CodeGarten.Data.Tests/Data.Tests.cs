using System;
using System.Collections.Generic;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;
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
                dbman.Structure.Delete(dbman.Structure.Get(1));
        }

        [Test]
        public void DeleteRoleType()
        {
            using (var dbman = new DataBaseManager())
                dbman.RoleType.Delete(dbman.RoleType.Get(1, "studant"), 1);
        }

        [Test]
        public void DeleteWorkSpaceType()
        {
            using (var dbman = new DataBaseManager())
                dbman.WorkSpaceType.Delete(dbman.WorkSpaceType.Get(1, "public"), 1);
        }

        [Test]
        public void DeleteRule()
        {
            using (var dbman = new DataBaseManager())
                dbman.Rule.Delete(dbman.Rule.Get(1, "studant"), 1);
        }

        [Test]
        public void DeleteContainerPrototype()
        {
            using (var dbman = new DataBaseManager())
                dbman.ContainerPrototype.Delete(dbman.ContainerPrototype.Get(1, "Course"), 1);
        }

        [Test]
        public void DeleteContainer()
        {
            using (var dbman = new DataBaseManager())
                dbman.Container.Delete(dbman.Container.Get(2).Convert());
        }

        [Test]
        public void Drop()
        {
            using (var dataBaseManager = new DataBaseManager())
                dataBaseManager.DbContext.Database.Delete();
        }

        [Test]
        public void Create()
        {
            DataBaseManager.Initializer();
        }

        [Test]
        public void ShouldTestBasicFeatures()
        {
            DataBaseManager.Initializer();
            using (var dataBaseManager = new DataBaseManager())
            {
                #region CREATE_SERVICES

                var serviceManager = new ServiceManager(dataBaseManager);

                var serviceGit = serviceManager.Create("Git","System Version Control (decentralized)", new[] {"r", "rw"});
                
                var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] {"r", "rw"});

                var serviceTrac = serviceManager.Create("Trac", "WIKI system", new []{"TRAC_ADMIN"});

                #endregion

                #region CREATE_USERS

                var userManager = new UserManager(dataBaseManager);

                var userFaustino = new UserView()
                                       {
                                           Name = "FaustinoLeiras",
                                           Email = "FaustinoLeiras@gmail.com",
                                           Password = "FaustinoLeiras12345"
                                       };

                userManager.Create(userFaustino);

                var userSamir = new UserView()
                                    {
                                        Name = "SamirHafez",
                                        Email = "SamirHafez@gmail.com",
                                        Password = "12345678Ab"
                                    };

                userManager.Create(userSamir);

                var userRicardo = new UserView()
                                      {
                                          Name = "Ricardo",
                                          Email = "Ricardo@gmail.com",
                                          Password = "12345678Ab"
                                      };

                userManager.Create(userRicardo);

                var userGeada = new UserView()
                                    {
                                        Name = "Gueada",
                                        Email = "Gueada@gmail.com",
                                        Password = "12345678Ab"
                                    };

                userManager.Create(userGeada);

                var userFelix = new UserView()
                                    {
                                        Name = "Felix",
                                        Email = "Felix@gmail.com",
                                        Password = "12345678Ab"
                                    };

                userManager.Create(userFelix);

                var userGuedes = new UserView()
                                     {
                                         Name = "Guedes",
                                         Email = "Guedes@gmail.com",
                                         Password = "12345678Ab"
                                     };

                userManager.Create(userGuedes);

                #endregion

                #region CREATE_STRUCTURE

                var structureManager = new StructureManager(dataBaseManager);

                var structure = new StructureView()
                                    {
                                        Name = "AcademicStructure",
                                        Description = "My Academic Structure :)"
                                    };

                structureManager.Create(structure, userFaustino.Name);

                #endregion

                #region CREATE_WORKSPACE_TYPE

                var workspaceType = new WorkSpaceTypeManager(dataBaseManager);

                var workspacePublic = new WorkSpaceTypeView()
                                          {
                                              Name = "public"
                                          };

                workspaceType.Create(workspacePublic, structure.Id, new[] {serviceGit.Name, serviceSvn.Name, serviceTrac.Name});

                var workspacePrivate = new WorkSpaceTypeView()
                                           {
                                               Name = "private"
                                           };

                workspaceType.Create(workspacePrivate, structure.Id, new[] {serviceGit.Name, serviceSvn.Name, serviceTrac.Name});

                #endregion

                #region CREATE_CONTAINER_PROTOTYPE

                var containerPrototype = new ContainerPrototypeManager(dataBaseManager);

                var prototypeGraduation = new ContainerPrototypeView()
                                              {
                                                  Name = "Graduation"
                                              };

                containerPrototype.Create(prototypeGraduation, structure.Id);

                var prototypeCourse = new ContainerPrototypeView()
                                          {
                                              Name = "Course"
                                          };

                containerPrototype.Create(prototypeCourse, structure.Id, prototypeGraduation.Name);


                var prototypeClass = new ContainerPrototypeView()
                                         {
                                             Name = "Class"
                                         };

                containerPrototype.Create(prototypeClass, structure.Id, prototypeCourse.Name);

                var prototypeGroup = new ContainerPrototypeView()
                                         {
                                             Name = "Group"
                                         };

                containerPrototype.Create(prototypeGroup, structure.Id, prototypeClass.Name);

                #endregion

                #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGraduation.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

                #endregion

                #region CREATE_ROLETYPE

                var roleType = new RoleTypeManager(dataBaseManager);

                var roleTypeTeacher = new RoleTypeView()
                                          {
                                              Name = "teacher"
                                          };

                roleType.Create(roleTypeTeacher, structure.Id);

                var roleTypeDirector = new RoleTypeView()
                                           {
                                               Name = "director"
                                           };

                roleType.Create(roleTypeDirector, structure.Id);

                var roleTypeStudant = new RoleTypeView()
                                          {
                                              Name = "studant"
                                          };

                roleType.Create(roleTypeStudant, structure.Id);

                #endregion

                #region CREATE_RULE

                var rule = new RuleManager(dataBaseManager);

                var ruleReaders = new RuleView()
                                      {
                                          Name = "readers"
                                      };

                rule.Create(ruleReaders, structure.Id, new[]
                                                           {
                                                               new KeyValuePair<string, string>(serviceGit.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceSvn.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceTrac.Name, "TRAC_ADMIN")
                                                           });

                var ruleReadersAndWriters = new RuleView()
                                                {
                                                    Name = "ReadersAndWriters"
                                                };

                rule.Create(ruleReadersAndWriters, structure.Id, new[]
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

                var role = new RoleManager(dataBaseManager);

                #region ADD_ROLES_GRADUATION_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGraduation.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReadersAndWriters.Name
                    );

                #endregion

                #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReadersAndWriters.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    ruleReaders.Name
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    ruleReadersAndWriters.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    ruleReaders.Name
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeDirector.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeTeacher.Name,
                    ruleReadersAndWriters.Name
                    );

                #endregion

                #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    ruleReadersAndWriters.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    ruleReadersAndWriters.Name
                    );

                #endregion

                #endregion

                #region CREATE_CONTAINERS

                var container = new ContainerManager(dataBaseManager);

                var containerLeic = new ContainerView()
                                        {
                                            Name = "LEIC",
                                            Description = "Licencitura Egenharia informatica e de computadores"
                                        };

                container.Create(containerLeic, structure.Id, prototypeGraduation.Name);

                #region CREATE_COURSE_MPD

                var containerMpd = new ContainerView()
                                       {
                                           Name = "MPD",
                                           Description = "Modelo de padoes de desenho"
                                       };

                container.Create(containerMpd, structure.Id, prototypeCourse.Name, containerLeic.Id);

                var containerMpdLi31D = new ContainerView()
                                            {
                                                Name = "LI31D",
                                                Description = "Turma 1 de terceiro semestre diurno"
                                            };

                container.Create(containerMpdLi31D, structure.Id, prototypeClass.Name, containerMpd.Id);

                var containerMpdG1 = new ContainerView()
                                         {
                                             Name = "Grupo1",
                                             Description = "Grupo de MPD"
                                         };

                container.Create(containerMpdG1, structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

                var containerMpdG2 = new ContainerView()
                                         {
                                             Name = "Grupo2",
                                             Description = "Grupo de MPD"
                                         };

                container.Create(containerMpdG2, structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

                #endregion

                #region CREATE_COURSE_SD

                var containerSd = new ContainerView()
                                      {
                                          Name = "SD",
                                          Description = "Sistemas distribuidos"
                                      };

                container.Create(containerSd, structure.Id, prototypeCourse.Name, containerLeic.Id);

                var containerSdLi31D = new ContainerView()
                                           {
                                               Name = "LI31D",
                                               Description = "Turma 1 de terceiro semestre diurno"
                                           };

                container.Create(containerSdLi31D, structure.Id, prototypeClass.Name, containerSd.Id);

                var containerSdG1 = new ContainerView()
                                        {
                                            Name = "Grupo1",
                                            Description = "Grupo de SD"
                                        };

                container.Create(containerSdG1, structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

                var containerSdG2 = new ContainerView()
                                        {
                                            Name = "Grupo2",
                                            Description = "Grupo de SD"
                                        };

                container.Create(containerSdG2, structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

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
            DataBaseManager.Initializer();
            using (var dataBaseManager = new DataBaseManager())
            {

                #region CREATE_SERVICES

                var serviceManager = new ServiceManager(dataBaseManager);

                var serviceGit = serviceManager.Create("Git", "System Version Control (decentralized)", new[] { "r", "rw" });

                var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] { "r", "rw" });

                #endregion

                #region CREATE_USERS

                var userManager = new UserManager(dataBaseManager);

                var userFaustino = new UserView()
                                       {
                                           Name = "FaustinoLeiras",
                                           Email = "FaustinoLeiras@gmail.com",
                                           Password = "FaustinoLeiras12345"
                                       };

                userManager.Create(userFaustino);

                var userSamir = new UserView()
                                    {
                                        Name = "SamirHafez",
                                        Email = "SamirHafez@gmail.com",
                                        Password = "12345678Ab"
                                    };

                userManager.Create(userSamir);

                var userRicardo = new UserView()
                                      {
                                          Name = "Ricardo",
                                          Email = "Ricardo@gmail.com",
                                          Password = "12345678Ab"
                                      };

                userManager.Create(userRicardo);

                var userGeada = new UserView()
                                    {
                                        Name = "Gueada",
                                        Email = "Gueada@gmail.com",
                                        Password = "12345678Ab"
                                    };

                userManager.Create(userGeada);

                var userFelix = new UserView()
                                    {
                                        Name = "Felix",
                                        Email = "Felix@gmail.com",
                                        Password = "12345678Ab"
                                    };

                userManager.Create(userFelix);

                var userGuedes = new UserView()
                                     {
                                         Name = "Guedes",
                                         Email = "Guedes@gmail.com",
                                         Password = "12345678Ab"
                                     };

                userManager.Create(userGuedes);

                #endregion

                #region CREATE_STRUCTURE

                var structureManager = new StructureManager(dataBaseManager);

                var structure = new StructureView()
                                    {
                                        Name = "AcademicStructure",
                                        Description = "My Academic Structure :)"
                                    };

                structureManager.Create(structure, userFaustino.Name);

                #endregion

                #region CREATE_WORKSPACE_TYPE

                var workspaceType = new WorkSpaceTypeManager(dataBaseManager);

                var workspacePublic = new WorkSpaceTypeView()
                                          {
                                              Name = "public"
                                          };

                workspaceType.Create(workspacePublic, structure.Id, new[] {serviceGit.Name, serviceSvn.Name});

                var workspacePrivate = new WorkSpaceTypeView()
                                           {
                                               Name = "private"
                                           };

                workspaceType.Create(workspacePrivate, structure.Id, new[] {serviceGit.Name, serviceSvn.Name});

                #endregion

                #region CREATE_CONTAINER_PROTOTYPE

                var containerPrototype = new ContainerPrototypeManager(dataBaseManager);

                var prototypeGraduation = new ContainerPrototypeView()
                                              {
                                                  Name = "Graduation"
                                              };

                containerPrototype.Create(prototypeGraduation, structure.Id);

                var prototypeCourse = new ContainerPrototypeView()
                                          {
                                              Name = "Course"
                                          };

                containerPrototype.Create(prototypeCourse, structure.Id, prototypeGraduation.Name);


                var prototypeClass = new ContainerPrototypeView()
                                         {
                                             Name = "Class"
                                         };

                containerPrototype.Create(prototypeClass, structure.Id, prototypeCourse.Name);

                var prototypeGroup = new ContainerPrototypeView()
                                         {
                                             Name = "Group"
                                         };

                containerPrototype.Create(prototypeGroup, structure.Id, prototypeClass.Name);

                #endregion

                #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGraduation.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
                containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

                containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

                #endregion

                #region CREATE_ROLETYPE

                var roleType = new RoleTypeManager(dataBaseManager);

                var roleTypeTeacher = new RoleTypeView()
                                          {
                                              Name = "teacher"
                                          };

                roleType.Create(roleTypeTeacher, structure.Id);

                var roleTypeDirector = new RoleTypeView()
                                           {
                                               Name = "director"
                                           };

                roleType.Create(roleTypeDirector, structure.Id);

                var roleTypeStudant = new RoleTypeView()
                                          {
                                              Name = "studant"
                                          };

                roleType.Create(roleTypeStudant, structure.Id);

                #endregion

                #region CREATE_RULE

                var rule = new RuleManager(dataBaseManager);

                var ruleReaders = new RuleView()
                                      {
                                          Name = "readers"
                                      };

                rule.Create(ruleReaders, structure.Id, new[]
                                                           {
                                                               new KeyValuePair<string, string>(serviceGit.Name, "r"),
                                                               new KeyValuePair<string, string>(serviceSvn.Name, "r")
                                                           });

                var ruleReadersAndWriters = new RuleView()
                                                {
                                                    Name = "ReadersAndWriters"
                                                };

                rule.Create(ruleReadersAndWriters, structure.Id, new[]
                                                                     {
                                                                         new KeyValuePair<string, string>(
                                                                             serviceGit.Name,
                                                                             "rw"),
                                                                         new KeyValuePair<string, string>(
                                                                             serviceSvn.Name,
                                                                             "rw")
                                                                     });

                #endregion

                #region CREATE_ROLE

                var role = new RoleManager(dataBaseManager);

                #region ADD_ROLES_GRADUATION_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGraduation.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReadersAndWriters.Name
                    );

                #endregion

                #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReadersAndWriters.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeCourse.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    ruleReaders.Name
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    ruleReadersAndWriters.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    ruleReaders.Name
                    );

                #endregion

                #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeDirector.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeClass.Name,
                    workspacePrivate.Name,
                    roleTypeTeacher.Name,
                    ruleReadersAndWriters.Name
                    );

                #endregion

                #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeDirector.Name,
                    ruleReaders.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeTeacher.Name,
                    ruleReadersAndWriters.Name
                    );

                role.Create(
                    structure.Id,
                    prototypeGroup.Name,
                    workspacePublic.Name,
                    roleTypeStudant.Name,
                    ruleReadersAndWriters.Name
                    );

                #endregion

                #endregion

                #region CREATE_CONTAINERS

                var container = new ContainerManager(dataBaseManager);

                var containerLeic = new ContainerView()
                                        {
                                            Name = "LEIC",
                                            Description = "Licencitura Egenharia informatica e de computadores"
                                        };

                container.Create(containerLeic, structure.Id, prototypeGraduation.Name);

                #region CREATE_COURSE_MPD

                var containerMpd = new ContainerView()
                                       {
                                           Name = "MPD",
                                           Description = "Modelo de padoes de desenho"
                                       };

                container.Create(containerMpd, structure.Id, prototypeCourse.Name, containerLeic.Id);

                var containerMpdLi31D = new ContainerView()
                                            {
                                                Name = "LI31D",
                                                Description = "Turma 1 de terceiro semestre diurno"
                                            };

                container.Create(containerMpdLi31D, structure.Id, prototypeClass.Name, containerMpd.Id);

                var containerMpdG1 = new ContainerView()
                                         {
                                             Name = "Grupo1",
                                             Description = "Grupo de MPD"
                                         };

                container.Create(containerMpdG1, structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

                var containerMpdG2 = new ContainerView()
                                         {
                                             Name = "Grupo2",
                                             Description = "Grupo de MPD"
                                         };

                container.Create(containerMpdG2, structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

                #endregion

                #region CREATE_COURSE_SD

                var containerSd = new ContainerView()
                                      {
                                          Name = "SD",
                                          Description = "Sistemas distribuidos"
                                      };

                container.Create(containerSd, structure.Id, prototypeCourse.Name, containerLeic.Id);

                var containerSdLi31D = new ContainerView()
                                           {
                                               Name = "LI31D",
                                               Description = "Turma 1 de terceiro semestre diurno"
                                           };

                container.Create(containerSdLi31D, structure.Id, prototypeClass.Name, containerSd.Id);

                var containerSdG1 = new ContainerView()
                                        {
                                            Name = "Grupo1",
                                            Description = "Grupo de SD"
                                        };

                container.Create(containerSdG1, structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

                var containerSdG2 = new ContainerView()
                                        {
                                            Name = "Grupo2",
                                            Description = "Grupo de SD"
                                        };

                container.Create(containerSdG2, structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

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
        public void ShouldTestBasicFeaturesWithoutCache()
        {
            DataBaseManager.DropAndCreate();

            DataBaseManager dataBaseManager;
            dataBaseManager = new DataBaseManager();

            #region CREATE_SERVICES

            var serviceManager = new ServiceManager(dataBaseManager);

            var serviceGit = serviceManager.Create("Git", "System Version Control (decentralized)", new[] { "r", "rw" });

            var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] { "r", "rw" });

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_USERS

            var userManager = new UserManager(dataBaseManager);

            var userFaustino = new UserView()
                                   {
                                       Name = "FaustinoLeiras",
                                       Email = "FaustinoLeiras@gmail.com",
                                       Password = AuthenticationManager.EncryptPassword("12345678Ab")
                                   };

            userManager.Create(userFaustino);

            var userSamir = new UserView()
                                {
                                    Name = "SamirHafez",
                                    Email = "SamirHafez@gmail.com",
                                    Password = AuthenticationManager.EncryptPassword("12345678Ab")
                                };

            userManager.Create(userSamir);

            var userRicardo = new UserView()
                                  {
                                      Name = "Ricardo",
                                      Email = "Ricardo@gmail.com",
                                      Password = AuthenticationManager.EncryptPassword("12345678Ab")
                                  };

            userManager.Create(userRicardo);

            var userGeada = new UserView()
                                {
                                    Name = "Gueada",
                                    Email = "Gueada@gmail.com",
                                    Password = AuthenticationManager.EncryptPassword("12345678Ab")
                                };

            userManager.Create(userGeada);

            var userFelix = new UserView()
                                {
                                    Name = "Felix",
                                    Email = "Felix@gmail.com",
                                    Password = AuthenticationManager.EncryptPassword("12345678Ab")
                                };

            userManager.Create(userFelix);

            var userGuedes = new UserView()
                                 {
                                     Name = "Guedes",
                                     Email = "Guedes@gmail.com",
                                     Password = AuthenticationManager.EncryptPassword("12345678Ab")
                                 };

            userManager.Create(userGuedes);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_STRUCTURE

            var structureManager = new StructureManager(dataBaseManager);

            var structure = new StructureView()
                                {
                                    Name = "AcademicStructure",
                                    Description = "My Academic Structure :)"
                                };

            structureManager.Create(structure, userFelix.Name);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_WORKSPACE_TYPE

            var workspaceType = new WorkSpaceTypeManager(dataBaseManager);

            var workspacePublic = new WorkSpaceTypeView()
                                      {
                                          Name = "public"
                                      };

            workspaceType.Create(workspacePublic, structure.Id, new[] {serviceGit.Name, serviceSvn.Name});

            var workspacePrivate = new WorkSpaceTypeView()
                                       {
                                           Name = "private"
                                       };

            workspaceType.Create(workspacePrivate, structure.Id, new[] {serviceGit.Name, serviceSvn.Name});

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_CONTAINER_PROTOTYPE

            var containerPrototype = new ContainerPrototypeManager(dataBaseManager);

            var prototypeCourse = new ContainerPrototypeView()
                                      {
                                          Name = "Course"
                                      };

            containerPrototype.Create(prototypeCourse, structure.Id);

            var prototypeClass = new ContainerPrototypeView()
                                     {
                                         Name = "Class"
                                     };

            containerPrototype.Create(prototypeClass, structure.Id, prototypeCourse.Name);

            var prototypeGroup = new ContainerPrototypeView()
                                     {
                                         Name = "Group"
                                     };

            containerPrototype.Create(prototypeGroup, structure.Id, prototypeClass.Name);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

            containerPrototype = new ContainerPrototypeManager(dataBaseManager);

            containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

            containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
            containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

            containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_ROLETYPE

            var roleType = new RoleTypeManager(dataBaseManager);

            var roleTypeTeacher = new RoleTypeView()
                                      {
                                          Name = "teacher"
                                      };

            roleType.Create(roleTypeTeacher, structure.Id);

            var roleTypeDirector = new RoleTypeView()
                                       {
                                           Name = "director"
                                       };

            roleType.Create(roleTypeDirector, structure.Id);

            var roleTypeStudant = new RoleTypeView()
                                      {
                                          Name = "studant"
                                      };

            roleType.Create(roleTypeStudant, structure.Id);

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_RULE

            var rule = new RuleManager(dataBaseManager);

            var ruleReaders = new RuleView()
                                  {
                                      Name = "readers"
                                  };

            rule.Create(ruleReaders, structure.Id, new[]
                                                       {
                                                           new KeyValuePair<string, string>(serviceGit.Name, "r"),
                                                           new KeyValuePair<string, string>(serviceSvn.Name, "r")
                                                       });

            var ruleReadersAndWriters = new RuleView()
                                            {
                                                Name = "ReadersAndWriters"
                                            };

            rule.Create(ruleReadersAndWriters, structure.Id, new[]
                                                                 {
                                                                     new KeyValuePair<string, string>(
                                                                         serviceGit.Name,
                                                                         "rw"),
                                                                     new KeyValuePair<string, string>(
                                                                         serviceSvn.Name,
                                                                         "rw")
                                                                 });

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_ROLE

            var role = new RoleManager(dataBaseManager);

            #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

            role.Create(
                structure.Id,
                prototypeCourse.Name,
                workspacePublic.Name,
                roleTypeDirector.Name,
                ruleReadersAndWriters.Name
                );

            role.Create(
                structure.Id,
                prototypeCourse.Name,
                workspacePublic.Name,
                roleTypeTeacher.Name,
                ruleReaders.Name
                );

            role.Create(
                structure.Id,
                prototypeCourse.Name,
                workspacePublic.Name,
                roleTypeStudant.Name,
                ruleReaders.Name
                );

            #endregion

            #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePublic.Name,
                roleTypeDirector.Name,
                ruleReaders.Name
                );

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePublic.Name,
                roleTypeTeacher.Name,
                ruleReadersAndWriters.Name
                );

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePublic.Name,
                roleTypeStudant.Name,
                ruleReaders.Name
                );

            #endregion

            #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePrivate.Name,
                roleTypeDirector.Name,
                ruleReaders.Name
                );

            role.Create(
                structure.Id,
                prototypeClass.Name,
                workspacePrivate.Name,
                roleTypeTeacher.Name,
                ruleReadersAndWriters.Name
                );

            #endregion

            #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

            role.Create(
                structure.Id,
                prototypeGroup.Name,
                workspacePublic.Name,
                roleTypeDirector.Name,
                ruleReaders.Name
                );

            role.Create(
                structure.Id,
                prototypeGroup.Name,
                workspacePublic.Name,
                roleTypeTeacher.Name,
                ruleReadersAndWriters.Name
                );

            role.Create(
                structure.Id,
                prototypeGroup.Name,
                workspacePublic.Name,
                roleTypeStudant.Name,
                ruleReadersAndWriters.Name
                );

            #endregion

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region CREATE_CONTAINERS

            var container = new ContainerManager(dataBaseManager);

            #region CREATE_COURSE_MPD

            var containerMpd = new ContainerView()
                                   {
                                       Name = "MPD",
                                       Description = "Modelo de padoes de desenho"
                                   };

            container.Create(containerMpd, structure.Id, prototypeCourse.Name);

            var containerMpdLi31D = new ContainerView()
                                        {
                                            Name = "LI31D",
                                            Description = "Turma 1 de terceiro semestre diurno"
                                        };

            container.Create(containerMpdLi31D, structure.Id, prototypeClass.Name, containerMpd.Id);

            var containerMpdG1 = new ContainerView()
                                     {
                                         Name = "Grupo1",
                                         Description = "Grupo de MPD"
                                     };

            container.Create(containerMpdG1, structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

            var containerMpdG2 = new ContainerView()
                                     {
                                         Name = "Grupo2",
                                         Description = "Grupo de MPD"
                                     };

            container.Create(containerMpdG2, structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

            #endregion

            #region CREATE_COURSE_SD

            var containerSd = new ContainerView()
                                  {
                                      Name = "SD",
                                      Description = "Sistemas distribuidos"
                                  };

            container.Create(containerSd, structure.Id, prototypeCourse.Name);

            var containerSdLi31D = new ContainerView()
                                       {
                                           Name = "LI31D",
                                           Description = "Turma 1 de terceiro semestre diurno"
                                       };

            container.Create(containerSdLi31D, structure.Id, prototypeClass.Name, containerSd.Id);

            var containerSdG1 = new ContainerView()
                                    {
                                        Name = "Grupo1",
                                        Description = "Grupo de SD"
                                    };

            container.Create(containerSdG1, structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

            var containerSdG2 = new ContainerView()
                                    {
                                        Name = "Grupo2",
                                        Description = "Grupo de SD"
                                    };

            container.Create(containerSdG2, structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

            #endregion

            #endregion

            dataBaseManager.Dispose();
            dataBaseManager = new DataBaseManager();

            #region ENROLL_USER

            userManager = new UserManager(dataBaseManager);

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