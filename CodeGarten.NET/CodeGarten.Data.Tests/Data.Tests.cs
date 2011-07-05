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
                dbman.Container.Delete(dbman.Container.Get(2));
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

                var serviceGit = new ServiceView()
                                     {
                                         Name = "Git",
                                         Description = "System Version Control (decentralized)"
                                     };

                serviceManager.Create(serviceGit, new[] {"r", "rw"});

                var serviceSvn = new ServiceView()
                                     {
                                         Name = "Svn",
                                         Description = "System Version Control (Centralized)"
                                     };

                serviceManager.Create(serviceSvn, new[] {"r", "rw"});

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
        public void ShouldTestBasicFeaturesWithoutCache()
        {
            DataBaseManager.DropAndCreate();

            DataBaseManager dataBaseManager;
            dataBaseManager = new DataBaseManager();

            #region CREATE_SERVICES

            var serviceManager = new ServiceManager(dataBaseManager);

            var serviceGit = new ServiceView()
                                 {
                                     Name = "Git",
                                     Description = "System Version Control (decentralized)"
                                 };

            serviceManager.Create(serviceGit, new[] {"r", "rw"});

            var serviceSvn = new ServiceView()
                                 {
                                     Name = "SVN",
                                     Description = "System Version Control (Centralized)"
                                 };

            serviceManager.Create(serviceSvn, new[] {"r", "rw"});

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
            using (dataBaseManager = new DataBaseManager())
            {
                AuthorizationTestes aut = new AuthorizationTestes();
                AuthorizationManager authorizationManager = new AuthorizationManager(dataBaseManager);

                authorizationManager.CreateServiceAuthorizationStruct(aut, serviceSvn.Name);
            }
        }

        //[Test]
        //public void CreateDataBase()
        //{
        //    DataBaseManager.DropAndCreate();

        //    #region CREATE_SERVICES

        //    var serviceGit = new Service()
        //    {
        //        Name = "Git",
        //        Description = "System Version Control (decentralized)"
        //    };

        //    serviceGit.Create(new[] { "r", "rw" });

        //    var serviceSvn = new Service()
        //    {
        //        Name = "SVN",
        //        Description = "System Version Control (Centralized)"
        //    };

        //    serviceSvn.Create(new[] { "r", "rw" });

        //    #endregion

        //    #region CREATE_USERS
        //    var userFaustino = new User()
        //    {
        //        Name = "FaustinoLeiras",
        //        Email = "FaustinoLeiras@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    var userSamir = new User()
        //    {
        //        Name = "SamirHafez",
        //        Email = "SamirHafez@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    var userRicardo = new User()
        //    {
        //        Name = "Ricardo",
        //        Email = "Ricardo@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    var userGeada = new User()
        //    {
        //        Name = "Gueada",
        //        Email = "Gueada@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    var userFelix = new User()
        //    {
        //        Name = "Felix",
        //        Email = "Felix@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    var userGuedes = new User()
        //    {
        //        Name = "Guedes",
        //        Email = "Guedes@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };
        //    #endregion

        //    #region CREATE_STRUCTURE

        //    var structure = new Structure()
        //    {
        //        Name = "AcademicStructure",
        //        Description = "My Academic Structure :)"
        //    };


        //    structure.Create(userFelix.Name);

        //    #endregion

        //    #region CREATE_WORKSPACE_TYPE

        //    var workspacePublic = new WorkSpaceType()
        //    {
        //        Name = "public"
        //    };

        //    workspacePublic.Create(structure.Id, new[] { serviceGit.Name, serviceSvn.Name });

        //    var workspacePrivate = new WorkSpaceType()
        //    {
        //        Name = "private"
        //    };

        //    workspacePrivate.Create(structure.Id, new[] { serviceGit.Name, serviceSvn.Name });

        //    #endregion

        //    #region CREATE_CONTAINER_PROTOTYPE

        //    var prototypeCourse = new ContainerPrototype()
        //    {
        //        Name = "Course"
        //    };

        //    prototypeCourse.Create(structure.Id);

        //    var prototypeClass = new ContainerPrototype()
        //    {
        //        Name = "Class"
        //    };

        //    prototypeClass.Create(structure.Id, prototypeCourse.Name);

        //    var prototypeGroup = new ContainerPrototype()
        //    {
        //        Name = "Group"
        //    };

        //    prototypeGroup.Create(structure.Id, prototypeClass.Name);

        //    #endregion

        //    #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

        //    ContainerPrototypeManager.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

        //    ContainerPrototypeManager.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
        //    ContainerPrototypeManager.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

        //    ContainerPrototypeManager.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

        //    #endregion

        //    #region CREATE_ROLETYPE

        //    var roleTypeTeacher = new RoleType()
        //    {
        //        Name = "teacher"
        //    };

        //    roleTypeTeacher.Create(structure.Id);

        //    var roleTypeDirector = new RoleType()
        //    {
        //        Name = "director"
        //    };

        //    roleTypeDirector.Create(structure.Id);

        //    var roleTypeStudant = new RoleType()
        //    {
        //        Name = "studant"
        //    };

        //    roleTypeStudant.Create(structure.Id);

        //    #endregion

        //    #region CREATE_RULE

        //    var ruleReaders = new Rule()
        //    {
        //        Name = "readers"
        //    };

        //    ruleReaders.Create(structure.Id, new[]
        //                           {
        //                               new KeyValuePair<string, string>(serviceGit.Name, "r"), 
        //                               new KeyValuePair<string, string>(serviceSvn.Name, "r")
        //                           });

        //    var ruleReadersAndWriters = new Rule()
        //    {
        //        Name = "ReadersAndWriters"
        //    };

        //    ruleReadersAndWriters.Create(structure.Id, new[]
        //                           {
        //                               new KeyValuePair<string, string>(serviceGit.Name, "rw"), 
        //                               new KeyValuePair<string, string>(serviceSvn.Name, "rw")
        //                           });

        //    #endregion

        //    #region CREATE_ROLE

        //    #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeCourse.Name,
        //                        workspacePublic.Name,
        //                        roleTypeDirector.Name,
        //                        ruleReadersAndWriters.Name
        //                      );

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeCourse.Name,
        //                        workspacePublic.Name,
        //                        roleTypeTeacher.Name,
        //                        ruleReaders.Name
        //                      );

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeCourse.Name,
        //                        workspacePublic.Name,
        //                        roleTypeStudant.Name,
        //                        ruleReaders.Name
        //                      );
        //    #endregion

        //    #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeClass.Name,
        //                        workspacePublic.Name,
        //                        roleTypeDirector.Name,
        //                        ruleReaders.Name
        //                      );

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeClass.Name,
        //                        workspacePublic.Name,
        //                        roleTypeTeacher.Name,
        //                        ruleReadersAndWriters.Name
        //                      );

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeClass.Name,
        //                        workspacePublic.Name,
        //                        roleTypeStudant.Name,
        //                        ruleReaders.Name
        //                      );
        //    #endregion

        //    #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeClass.Name,
        //                        workspacePrivate.Name,
        //                        roleTypeDirector.Name,
        //                        ruleReaders.Name
        //                      );

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeClass.Name,
        //                        workspacePrivate.Name,
        //                        roleTypeTeacher.Name,
        //                        ruleReadersAndWriters.Name
        //                      );

        //    #endregion

        //    #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeGroup.Name,
        //                        workspacePublic.Name,
        //                        roleTypeDirector.Name,
        //                        ruleReaders.Name
        //                      );

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeGroup.Name,
        //                        workspacePublic.Name,
        //                        roleTypeTeacher.Name,
        //                        ruleReadersAndWriters.Name
        //                      );

        //    RoleManager.Create(
        //                        structure.Id,
        //                        prototypeGroup.Name,
        //                        workspacePublic.Name,
        //                        roleTypeStudant.Name,
        //                        ruleReadersAndWriters.Name
        //                      );
        //    #endregion

        //    #endregion

        //    #region CREATE_CONTAINERS

        //    #region CREATE_COURSE_MPD
        //    var containerMpd = new Container()
        //    {
        //        Name = "MPD",
        //        Description = "Modelo de padoes de desenho"
        //    };

        //    containerMpd.Create(structure.Id, prototypeCourse.Name);

        //    var containerMpdLi31D = new Container()
        //    {
        //        Name = "LI31D",
        //        Description = "Turma 1 de terceiro semestre diurno"
        //    };

        //    containerMpdLi31D.Create(structure.Id, prototypeClass.Name, containerMpd.Id);

        //    var containerMpdG1 = new Container()
        //    {
        //        Name = "Grupo1",
        //        Description = "Grupo de MPD"
        //    };

        //    containerMpdG1.Create(structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

        //    var containerMpdG2 = new Container()
        //    {
        //        Name = "Grupo2",
        //        Description = "Grupo de MPD"
        //    };

        //    containerMpdG2.Create(structure.Id, prototypeGroup.Name, containerMpdLi31D.Id);

        //    #endregion

        //    #region CREATE_COURSE_SD

        //    var containerSd = new Container()
        //    {
        //        Name = "SD",
        //        Description = "Sistemas distribuidos"
        //    };

        //    containerSd.Create(structure.Id, prototypeCourse.Name);

        //    var containerSdLi31D = new Container()
        //    {
        //        Name = "LI31D",
        //        Description = "Turma 1 de terceiro semestre diurno"
        //    };

        //    containerSdLi31D.Create(structure.Id, prototypeClass.Name, containerSd.Id);

        //    var containerSdG1 = new Container()
        //    {
        //        Name = "Grupo1",
        //        Description = "Grupo de SD"
        //    };

        //    containerSdG1.Create(structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

        //    var containerSdG2 = new Container()
        //    {
        //        Name = "Grupo2",
        //        Description = "Grupo de SD"
        //    };

        //    containerSdG2.Create(structure.Id, prototypeGroup.Name, containerSdLi31D.Id);

        //    #endregion

        //    #endregion

        //    #region ENROLL_USER

        //    userFelix.AddRoleType(structure.Id, prototypeCourse.Name, containerMpd.Id, roleTypeDirector.Name);
        //    userGuedes.AddRoleType(structure.Id, prototypeCourse.Name, containerSd.Id, roleTypeDirector.Name);

        //    userFelix.AddRoleType(structure.Id, prototypeClass.Name, containerSdLi31D.Id, roleTypeTeacher.Name);
        //    userGuedes.AddRoleType(structure.Id, prototypeClass.Name, containerMpdLi31D.Id, roleTypeTeacher.Name);

        //    userFaustino.AddRoleType(structure.Id, prototypeGroup.Name, containerMpdG1.Id, roleTypeStudant.Name);
        //    userSamir.AddRoleType(structure.Id, prototypeGroup.Name, containerMpdG1.Id, roleTypeStudant.Name);

        //    userRicardo.AddRoleType(structure.Id, prototypeGroup.Name, containerMpdG2.Id, roleTypeStudant.Name);
        //    userGeada.AddRoleType(structure.Id, prototypeGroup.Name, containerMpdG2.Id, roleTypeStudant.Name);

        //    userFaustino.AddRoleType(structure.Id, prototypeGroup.Name, containerSdG1.Id, roleTypeStudant.Name);
        //    userRicardo.AddRoleType(structure.Id, prototypeGroup.Name, containerSdG1.Id, roleTypeStudant.Name);

        //    userGeada.AddRoleType(structure.Id, prototypeGroup.Name, containerSdG2.Id, roleTypeStudant.Name);
        //    userSamir.AddRoleType(structure.Id, prototypeGroup.Name, containerSdG2.Id, roleTypeStudant.Name);

        //    #endregion
        //}

        //[Test]
        //public void CreateAuthorizationFile()
        //{
        //    AuthorizationTestes aut = new AuthorizationTestes();

        //    AuthorizationManager.CreateServiceAuthorizationStruct(aut, "SVN");
        //}

        //[Test]
        //public void SearchStructures()
        //{
        //    DataBaseManager.DropAndCreate();

        //    #region CREATE_USERS
        //    var userFaustino = new User()
        //    {
        //        Name = "FaustinoLeiras",
        //        Email = "FaustinoLeiras@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    userFaustino.Create();

        //    var userFelix = new User()
        //    {
        //        Name = "Felix",
        //        Email = "Felix@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    userFelix.Create();
        //    #endregion

        //    #region CREATE_STRUCTURE

        //    var structure1 = new Structure()
        //    {
        //        Name = "AcademicStructure",
        //        Description = "My Academic Structure :)",
        //        Developing = true,
        //        Public = true
        //    };

        //    structure1 = structure1.Create(userFelix.Name);

        //    var structure2 = new Structure()
        //    {
        //        Name = "CodeGarten",
        //        Description = "My codeGarten :)",
        //        Developing = false,
        //        Public = true
        //    };

        //    structure2 = structure2.Create(userFelix.Name);

        //    var structure3 = new Structure()
        //    {
        //        Name = "XptoGarten",
        //        Description = "My codeGarten :)",
        //        Developing = false,
        //        Public = true
        //    };

        //    structure3 = structure3.Create(userFelix.Name);

        //    var structure4 = new Structure()
        //    {
        //        Name = "CodeGarten",
        //        Description = "My codeGarten private:)",
        //        Developing = false,
        //        Public = false
        //    };

        //    structure4 = structure4.Create(userFaustino.Name);

        //    #endregion

        //    var get1 = StructureManager.Get(true, userFelix.Name);
        //    var get2 = StructureManager.Get(false, userFelix.Name);
        //    var search1 = StructureManager.Search("Garten");
        //    var search2 = StructureManager.Search("CodeGarten");
        //    var search3 = StructureManager.Search("AcademicStructure");

        //    Assert.IsTrue(get1.Contains(structure1));
        //    Assert.IsTrue(get2.Contains(structure2));
        //    Assert.IsTrue(get2.Contains(structure3));
        //    Assert.IsTrue(search1.Contains(structure2));
        //    Assert.IsTrue(search1.Contains(structure3));
        //    Assert.IsTrue(search2.Contains(structure2));
        //    Assert.IsTrue(search3.Count() == 0);
        //}

        //[Test]
        //public void TestProperty()
        //{
        //    DataBaseManager.DropAndCreate();

        //    #region CREATE_USERS

        //    var userFelix = new User()
        //    {
        //        Name = "Felix",
        //        Email = "Felix@gmail.com",
        //        Password = AuthenticationManager.EncryptPassword("12345678Ab")
        //    };

        //    #endregion

        //    #region CREATE_STRUCTURE

        //    var structure = new Structure()
        //    {
        //        Name = "AcademicStructure",
        //        Description = "My Academic Structure :)"
        //    };


        //    structure.Create(userFelix.Name);

        //    #endregion

        //    var prototypeCourse = new ContainerPrototype()
        //    {
        //        Name = "Course"
        //    };

        //    prototypeCourse.Create(structure.Id);


        //    Console.WriteLine(prototypeCourse.Structure.Id);

        //}

        //public class ThreadTest
        //{
        //    public class ThreadContext
        //    {
        //        public int Id { get; private set; }
        //        public string StringId { get; private set; }

        //        public ThreadContext(int id)
        //        {
        //            Id = id;
        //            StringId = "" + id;
        //        }
        //    }

        //    [ThreadStaticAttribute]
        //    public static ThreadContext Context;

        //    public static void Run(object integer)
        //    {

        //        Context = new ThreadContext((int)integer);

        //        Thread.Sleep(1000);

        //        Console.WriteLine("Thread {0} = context = {1}", Thread.CurrentThread.ManagedThreadId, Context.Id);

        //    }
        //}

        //[Test]
        //public void TesteThreadStaticAttribute()
        //{
        //    ThreadPool.SetMaxThreads(1, 1);
        //    for (int i = 0; i < 5; ++i)
        //    {
        //        ThreadPool.QueueUserWorkItem(ThreadTest.Run, i);
        //    }
        //    Thread.Sleep(100000);

        //}
    }
}