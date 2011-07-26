using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeGarten.Data.Access;
using CodeGarten.Data.ModelView;

namespace CodeGarten.Web.Controllers
{
    [Authorize(Users = "SamirHafez FaustinoLeiras")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        //public void Populate()
        //{
        //    DataBaseManager.DropAndCreate();

        //    using (var dataBaseManager = new DataBaseManager())
        //    {
        //        #region CREATE_SERVICES

        //        var serviceManager = new ServiceManager(dataBaseManager);

        //        var serviceSvn = serviceManager.Create("Svn", "System Version Control (Centralized)", new[] { "r", "rw" });

        //        #endregion

        //        #region CREATE_USERS

        //        var userManager = new UserManager(dataBaseManager);

        //        var userFaustino = new UserView()
        //                               {
        //                                   Name = "FaustinoLeiras",
        //                                   Email = "FaustinoLeiras@gmail.com",
        //                                   Password = "FaustinoLeiras12345"
        //                               };

        //        userManager.Create(userFaustino);

        //        var userSamir = new UserView()
        //                            {
        //                                Name = "SamirHafez",
        //                                Email = "SamirHafez@gmail.com",
        //                                Password = "12345678Ab"
        //                            };

        //        userManager.Create(userSamir);

        //        var userRicardo = new UserView()
        //                              {
        //                                  Name = "Ricardo",
        //                                  Email = "Ricardo@gmail.com",
        //                                  Password = "12345678Ab"
        //                              };

        //        userManager.Create(userRicardo);

        //        var userGeada = new UserView()
        //                            {
        //                                Name = "Gueada",
        //                                Email = "Gueada@gmail.com",
        //                                Password = "12345678Ab"
        //                            };

        //        userManager.Create(userGeada);

        //        var userFelix = new UserView()
        //                            {
        //                                Name = "Felix",
        //                                Email = "Felix@gmail.com",
        //                                Password = "12345678Ab"
        //                            };

        //        userManager.Create(userFelix);

        //        var userGuedes = new UserView()
        //                             {
        //                                 Name = "Guedes",
        //                                 Email = "Guedes@gmail.com",
        //                                 Password = "12345678Ab"
        //                             };

        //        userManager.Create(userGuedes);

        //        #endregion

        //        #region CREATE_STRUCTURE

        //        var structureManager = new StructureManager(dataBaseManager);

        //        var structure = new StructureView()
        //                            {
        //                                Name = "AcademicStructure",
        //                                Description = "My Academic Structure :)"
        //                            };

        //        structureManager.Create(structure, userFaustino.Name);

        //        #endregion

        //        #region CREATE_WORKSPACE_TYPE

        //        var workspaceType = new WorkSpaceTypeManager(dataBaseManager);

        //        var workspacePublic = new WorkSpaceTypeView()
        //                                  {
        //                                      Name = "public"
        //                                  };

        //        workspaceType.Create(workspacePublic, structure.Id, new[] {serviceSvn.Name});

        //        var workspacePrivate = new WorkSpaceTypeView()
        //                                   {
        //                                       Name = "private"
        //                                   };

        //        workspaceType.Create(workspacePrivate, structure.Id, new[] {serviceSvn.Name});

        //        #endregion

        //        #region CREATE_CONTAINER_PROTOTYPE

        //        var containerPrototype = new ContainerPrototypeManager(dataBaseManager);

        //        var prototypeGraduation = new ContainerPrototypeView()
        //                                      {
        //                                          Name = "Graduation"
        //                                      };

        //        containerPrototype.Create(prototypeGraduation, structure.Id);

        //        var prototypeCourse = new ContainerPrototypeView()
        //                                  {
        //                                      Name = "Course"
        //                                  };

        //        containerPrototype.Create(prototypeCourse, structure.Id, prototypeGraduation.Name);


        //        var prototypeClass = new ContainerPrototypeView()
        //                                 {
        //                                     Name = "Class"
        //                                 };

        //        containerPrototype.Create(prototypeClass, structure.Id, prototypeCourse.Name);

        //        var prototypeGroup = new ContainerPrototypeView()
        //                                 {
        //                                     Name = "Group"
        //                                 };

        //        containerPrototype.Create(prototypeGroup, structure.Id, prototypeClass.Name);

        //        #endregion

        //        #region ADD_WORKSPACE_TYPES_INTO_CONTAINER_PROTOTYPE

        //        containerPrototype.AddWorkSpaceType(structure.Id, prototypeGraduation.Name, workspacePublic.Name);

        //        containerPrototype.AddWorkSpaceType(structure.Id, prototypeCourse.Name, workspacePublic.Name);

        //        containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePublic.Name);
        //        containerPrototype.AddWorkSpaceType(structure.Id, prototypeClass.Name, workspacePrivate.Name);

        //        containerPrototype.AddWorkSpaceType(structure.Id, prototypeGroup.Name, workspacePublic.Name);

        //        #endregion

        //        #region CREATE_ROLETYPE

        //        var roleType = new RoleTypeManager(dataBaseManager);

        //        var roleTypeTeacher = new RoleTypeView()
        //                                  {
        //                                      Name = "teacher"
        //                                  };

        //        roleType.Create(roleTypeTeacher, structure.Id);

        //        var roleTypeDirector = new RoleTypeView()
        //                                   {
        //                                       Name = "director"
        //                                   };

        //        roleType.Create(roleTypeDirector, structure.Id);

        //        var roleTypeStudant = new RoleTypeView()
        //                                  {
        //                                      Name = "studant"
        //                                  };

        //        roleType.Create(roleTypeStudant, structure.Id);

        //        #endregion

        //        #region CREATE_RULE

        //        var rule = new RuleManager(dataBaseManager);

        //        var ruleReaders = new RuleView()
        //                              {
        //                                  Name = "readers"
        //                              };

        //        rule.Create(ruleReaders, structure.Id, new[]
        //                                                   {
        //                                                       new KeyValuePair<string, string>(serviceSvn.Name, "r")
        //                                                   });

        //        var ruleReadersAndWriters = new RuleView()
        //                                        {
        //                                            Name = "ReadersAndWriters"
        //                                        };

        //        rule.Create(ruleReadersAndWriters, structure.Id, new[]
        //                                                             {
        //                                                                 new KeyValuePair<string, string>(
        //                                                                     serviceSvn.Name,
        //                                                                     "rw")
        //                                                             });

        //        #endregion

        //        #region CREATE_ROLE

        //        var role = new RoleManager(dataBaseManager);

        //        #region ADD_ROLES_COURSE_WORKSPACE_PUBLIC

        //        role.Create(
        //            structure.Id,
        //            prototypeCourse.Name,
        //            workspacePublic.Name,
        //            roleTypeDirector.Name,
        //            ruleReadersAndWriters.Name
        //            );

        //        role.Create(
        //            structure.Id,
        //            prototypeCourse.Name,
        //            workspacePublic.Name,
        //            roleTypeTeacher.Name,
        //            ruleReaders.Name
        //            );

        //        role.Create(
        //            structure.Id,
        //            prototypeCourse.Name,
        //            workspacePublic.Name,
        //            roleTypeStudant.Name,
        //            ruleReaders.Name
        //            );

        //        #endregion

        //        #region ADD_ROLES_CLASS_WORKSPACE_PUBLIC

        //        role.Create(
        //            structure.Id,
        //            prototypeClass.Name,
        //            workspacePublic.Name,
        //            roleTypeDirector.Name,
        //            ruleReaders.Name
        //            );

        //        role.Create(
        //            structure.Id,
        //            prototypeClass.Name,
        //            workspacePublic.Name,
        //            roleTypeTeacher.Name,
        //            ruleReadersAndWriters.Name
        //            );

        //        role.Create(
        //            structure.Id,
        //            prototypeClass.Name,
        //            workspacePublic.Name,
        //            roleTypeStudant.Name,
        //            ruleReaders.Name
        //            );

        //        #endregion

        //        #region ADD_ROLES_CLASS_WORKSPACE_PRIVATE

        //        role.Create(
        //            structure.Id,
        //            prototypeClass.Name,
        //            workspacePrivate.Name,
        //            roleTypeDirector.Name,
        //            ruleReaders.Name
        //            );

        //        role.Create(
        //            structure.Id,
        //            prototypeClass.Name,
        //            workspacePrivate.Name,
        //            roleTypeTeacher.Name,
        //            ruleReadersAndWriters.Name
        //            );

        //        #endregion

        //        #region ADD_ROLES_GROUP_WORKSPACE_PUBLIC

        //        role.Create(
        //            structure.Id,
        //            prototypeGroup.Name,
        //            workspacePublic.Name,
        //            roleTypeDirector.Name,
        //            ruleReaders.Name
        //            );

        //        role.Create(
        //            structure.Id,
        //            prototypeGroup.Name,
        //            workspacePublic.Name,
        //            roleTypeTeacher.Name,
        //            ruleReadersAndWriters.Name
        //            );

        //        role.Create(
        //            structure.Id,
        //            prototypeGroup.Name,
        //            workspacePublic.Name,
        //            roleTypeStudant.Name,
        //            ruleReadersAndWriters.Name
        //            );

        //        #endregion

        //        #endregion
        //    }
        //}
    }
}