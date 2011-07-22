using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace CodeGarten.Data
{
    internal class CodeGartenInitializer : CreateDatabaseIfNotExists<Context>
    {
        protected override void Seed(Context context)
        {
            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_structure ON dbo.Structures " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON " +
                                               "DELETE dbo.ContainerPrototypes FROM deleted, dbo.ContainerPrototypes WHERE deleted.Id = dbo.ContainerPrototypes.StructureId " +
                                               "DELETE dbo.RoleTypes FROM deleted, dbo.RoleTypes WHERE deleted.Id = dbo.RoleTypes.StructureId " +
                                               "DELETE dbo.Rules FROM deleted, dbo.Rules WHERE deleted.Id = dbo.Rules.StructureId " +
                                               "DELETE dbo.WorkSpaceTypes FROM deleted, dbo.WorkSpaceTypes WHERE deleted.Id = dbo.WorkSpaceTypes.StructureId " +
                                               "DELETE dbo.Structures FROM deleted WHERE deleted.Id = dbo.Structures.Id"
                );

            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_cp ON dbo.ContainerPrototypes " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON " +
                                               "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.Name = dbo.Roles.ContainerPrototypeName AND deleted.StructureId = dbo.Roles.ContainerPrototypeStructureId " +
                                               "DELETE dbo.Containers FROM deleted, dbo.Containers WHERE deleted.Name = dbo.Containers.ContainerPrototype_Name AND deleted.StructureId = dbo.Containers.ContainerPrototype_StructureId; " +
                                               "WITH q AS " +
                                               "(SELECT  Name, StructureId " +
                                               "FROM    deleted UNION ALL " +
                                               "SELECT  c.Name, c.StructureId " +
                                               "FROM    q " +
                                               "JOIN    dbo.ContainerPrototypes c " +
                                               "ON      c.Parent_Name = q.Name AND c.Parent_StructureId = q.StructureId) " +
                                               "DELETE dbo.ContainerPrototypes " +
                                               "WHERE EXISTS ( SELECT  Name, StructureId INTERSECT SELECT  Name, StructureId FROM    q )"
                );

            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_rt ON dbo.RoleTypes " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON " +
                                               "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.Name = dbo.Roles.RoleTypeName AND deleted.StructureId = dbo.Roles.ContainerPrototypeStructureId " +
                                               "DELETE dbo.Enrolls FROM deleted, dbo.Enrolls WHERE deleted.Name = dbo.Enrolls.RoleTypeName AND deleted.StructureId = dbo.Enrolls.RoleTypeStructureId " +
                                               "DELETE dbo.RoleTypes FROM deleted WHERE deleted.Name = dbo.RoleTypes.Name AND deleted.StructureId = dbo.RoleTypes.StructureId"
                );

            //context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_rule ON dbo.Rules " +
            //                                   "INSTEAD OF DELETE AS SET NOCOUNT ON " +
            //                                   "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.Name = dbo.Roles.RuleName AND deleted.StructureId = dbo.Roles.ContainerPrototypeStructureId " +
            //                                   "DELETE dbo.Rules FROM deleted WHERE deleted.Name = dbo.Rules.Name AND deleted.StructureId = dbo.Rules.StructureId"
            //    );

            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_wst ON dbo.WorkSpaceTypes " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON " +
                                               "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.Name = dbo.Roles.WorkSpaceTypeName AND deleted.StructureId = dbo.Roles.ContainerPrototypeStructureId " +
                                               "DELETE dbo.WorkSpaceTypes FROM deleted WHERE deleted.Name = dbo.WorkSpaceTypes.Name AND deleted.StructureId = dbo.WorkSpaceTypes.StructureId"
                );

            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_container on dbo.Containers " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON; " +
                                               "WITH q AS " +
                                               "(SELECT  Id " +
                                               "FROM    deleted " +
                                               "UNION ALL " +
                                               "SELECT  c.Id " +
                                               "FROM    q " +
                                               "JOIN    dbo.Containers c " +
                                               "ON      c.ParentContainer_Id = q.Id) " +
                                               "DELETE FROM    dbo.Containers " +
                                               "WHERE   EXISTS ( SELECT  Id INTERSECT SELECT  Id FROM    q )"
                );
        }
    }
}