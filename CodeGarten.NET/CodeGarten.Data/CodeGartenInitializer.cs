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
                                               "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.Name = dbo.Roles.ContainerPrototypeName AND deleted.StructureId = dbo.Roles.StructureId " +
                                               "DELETE dbo.Bindings FROM deleted, dbo.Bindings WHERE deleted.Name = dbo.Bindings.ContainerPrototypeName AND deleted.StructureId = dbo.Bindings.StructureId; " +
                                               "WITH q AS " +
                                               "(SELECT  Name, StructureId " +
                                               "FROM    deleted UNION ALL " +
                                               "SELECT  c.Name, c.StructureId " +
                                               "FROM    q " +
                                               "JOIN    dbo.ContainerPrototypes c " +
                                               "ON      c.ParentName = q.Name AND c.StructureId = q.StructureId) " +
                                               "DELETE dbo.ContainerPrototypes " +
                                               "WHERE EXISTS ( SELECT  Name, StructureId INTERSECT SELECT  Name, StructureId FROM    q )"
                );

            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_rt ON dbo.RoleTypes " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON " +
                                               "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.Name = dbo.Roles.RoleTypeName AND deleted.StructureId = dbo.Roles.StructureId " +
                                               "DELETE dbo.Enrolls FROM deleted, dbo.Enrolls WHERE deleted.Name = dbo.Enrolls.RoleTypeName AND deleted.StructureId = dbo.Enrolls.StructureId " +
                                               "DELETE dbo.RoleTypes FROM deleted WHERE deleted.Name = dbo.RoleTypes.Name AND deleted.StructureId = dbo.RoleTypes.StructureId"
                );

            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_wst ON dbo.WorkSpaceTypes " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON " +
                                               "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.Name = dbo.Roles.WorkSpaceTypeName AND deleted.StructureId = dbo.Roles.StructureId " +
                                               "DELETE dbo.Bindings FROM deleted, dbo.Bindings WHERE deleted.Name = dbo.Bindings.WorkSpaceTypeName AND deleted.StructureId = dbo.Bindings.StructureId " +
                                               "DELETE dbo.WorkSpaceTypes FROM deleted WHERE deleted.Name = dbo.WorkSpaceTypes.Name AND deleted.StructureId = dbo.WorkSpaceTypes.StructureId"
                );

            context.Database.ExecuteSqlCommand("CREATE TRIGGER delete_binding ON dbo.Bindings " +
                                               "INSTEAD OF DELETE AS SET NOCOUNT ON " +
                                               "DELETE dbo.Roles FROM deleted, dbo.Roles WHERE deleted.WorkSpaceTypeName = dbo.Roles.WorkSpaceTypeName AND deleted.ContainerPrototypeName = dbo.Roles.ContainerPrototypeName AND deleted.StructureId = dbo.Roles.StructureId " +
                                               "DELETE dbo.Bindings FROM deleted WHERE deleted.WorkSpaceTypeName = dbo.Bindings.WorkSpaceTypeName AND deleted.ContainerPrototypeName = dbo.Bindings.ContainerPrototypeName AND deleted.StructureId = dbo.Bindings.StructureId"
                );

        }
    }
}