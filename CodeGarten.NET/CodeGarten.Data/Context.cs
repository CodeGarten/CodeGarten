using System.Data.Entity;
using CodeGarten.Data.Model;

namespace CodeGarten.Data
{
    public sealed class Context : DbContext
    {
        public DbSet<Structure> Structures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContainerPrototype> ContainerPrototypes { get; set; }
        public DbSet<WorkSpaceType> WorkSpaceTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Enroll> Enrolls { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<RoleType> RoleTypes { get; set; }
        public DbSet<ServicePermission> ServicePermissions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<EnrollPassword> EnrollPassWords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            Database.SetInitializer(new CodeGartenInitializer());

            modelBuilder.Entity<Container>().HasRequired(c => c.ContainerPrototype).WithMany(cp => cp.Containers).
                WillCascadeOnDelete(false);

            modelBuilder.Entity<ContainerPrototype>().HasKey(cp => new {cp.Name, cp.StructureId});
            modelBuilder.Entity<ContainerPrototype>().HasRequired(cp => cp.Structure).WithMany().WillCascadeOnDelete(
                false);

            modelBuilder.Entity<WorkSpaceType>().HasKey(wt => new {wt.Name, wt.StructureId});
            modelBuilder.Entity<WorkSpaceType>().HasRequired(wt => wt.Structure).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<ServicePermission>().HasKey(sp => new {sp.Name, sp.ServiceName});

            modelBuilder.Entity<RoleType>().HasKey(rt => new {rt.Name, rt.StructureId});
            modelBuilder.Entity<RoleType>().HasRequired(rt => rt.Structure).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Rule>().HasKey(r => new {r.Name, r.StructureId});
            modelBuilder.Entity<Rule>().HasRequired(rt => rt.Structure).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>().HasKey(
                r =>
                new
                    {
                        r.ContainerPrototypeName,
                        r.ContainerPrototypeStructureId,
                        r.RoleTypeName,
                        r.RoleTypeStructureId,
                        r.WorkSpaceTypeName,
                        r.WorkSpaceTypeStructureId
                    });

            modelBuilder.Entity<EnrollPassword>().HasKey(e => new {e.ContainerId, e.RoleTypeName, e.RoleTypeStructureId});
            //modelBuilder.Entity<Role>().HasRequired(r => r.ContainerPrototype).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Role>().HasRequired(r => r.WorkSpaceType).WithMany().WillCascadeOnDelete(false);
            //modelBuilder.Entity<Role>().HasRequired(r => r.Rule).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Role>().HasRequired(r => r.RoleType).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Enroll>().HasKey(
                e =>
                new
                    {
                        e.UserName,
                        e.ContainerId,
                        e.RoleTypeName,
                        e.RoleTypeStructureId
                    });
            modelBuilder.Entity<Enroll>().HasRequired(e => e.RoleType).WithMany(rt => rt.Enrolls).WillCascadeOnDelete(
                false);
        }
    }
}