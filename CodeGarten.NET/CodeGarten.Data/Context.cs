using System.Data.Entity;
using CodeGarten.Data.Model;

namespace CodeGarten.Data
{
    internal sealed class Context : DbContext
    {
        public DbSet<Structure> Structures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContainerType> ContainerPrototypes { get; set; }
        public DbSet<WorkSpaceType> WorkSpaceTypes { get; set; }
        public DbSet<Binding> Bindings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Enroll> Enrolls { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<RoleType> RoleTypes { get; set; }
        public DbSet<ServiceTypePermission> ServicePermissions { get; set; }
        public DbSet<ServiceType> Services { get; set; }
        public DbSet<EnrollKey> EnrollPassWords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            Database.SetInitializer(new CodeGartenInitializer());

            modelBuilder.Entity<Container>().HasRequired(c => c.Type).WithMany().
                WillCascadeOnDelete(false);

            modelBuilder.Entity<ContainerType>().HasKey(cp => new {cp.Name, cp.StructureId});
            modelBuilder.Entity<ContainerType>().HasRequired(cp => cp.Structure).WithMany().WillCascadeOnDelete(
                false);

            modelBuilder.Entity<WorkSpaceType>().HasKey(wt => new {wt.Name, wt.StructureId});
            modelBuilder.Entity<WorkSpaceType>().HasRequired(wt => wt.Structure).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<WorkSpaceType>().HasMany(wst => wst.Services).WithMany();

            modelBuilder.Entity<ServiceTypePermission>().HasKey(sp => new {sp.Name, sp.ServiceName});

            modelBuilder.Entity<RoleType>().HasKey(rt => new {rt.Name, rt.StructureId});
            modelBuilder.Entity<RoleType>().HasRequired(rt => rt.Structure).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Rule>().HasKey(r => new {r.Name, r.StructureId});
            modelBuilder.Entity<Rule>().HasRequired(rt => rt.Structure).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Rule>().HasMany(rule => rule.Permissions).WithMany();

            modelBuilder.Entity<Role>().HasKey(
                r =>
                new
                    {
                        r.StructureId,
                        r.ContainerPrototypeName,
                        r.RoleTypeName,
                        r.WorkSpaceTypeName
                    });
            modelBuilder.Entity<Role>().HasMany(r => r.Rules).WithMany();
            modelBuilder.Entity<Role>().HasRequired(r => r.RoleType).WithMany().WillCascadeOnDelete(false);


            modelBuilder.Entity<EnrollKey>().HasKey(e => new { e.ContainerId, e.RoleTypeName, e.StructureId });
            
            modelBuilder.Entity<Enroll>().HasKey(
                e =>
                new
                    {
                        e.UserName,
                        e.ContainerId,
                        e.RoleTypeName,
                        e.StructureId
                    });
            modelBuilder.Entity<Enroll>().HasRequired(e => e.RoleType).WithMany().WillCascadeOnDelete(
                false);






            modelBuilder.Entity<Role>().HasRequired(r => r.Binding).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Binding>().HasMany(cpwst => cpwst.Roles).WithRequired(
                r => r.Binding).WillCascadeOnDelete(false);

            modelBuilder.Entity<Binding>().HasKey(
                cpwst => new {cpwst.StructureId, cpwst.ContainerPrototypeName, cpwst.WorkSpaceTypeName});

            modelBuilder.Entity<Binding>().HasRequired(b => b.ContainerType).WithMany(cp => cp.Bindings).WillCascadeOnDelete(false);
            modelBuilder.Entity<Binding>().HasRequired(b => b.WorkSpaceType).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Binding>().HasRequired(b => b.Structure).WithMany().WillCascadeOnDelete(false);

            //modelBuilder.Entity<ContainerPrototype>().HasMany(cp => cp.WorkSpaceTypes).WithRequired(cpwst => cpwst.ContainerPrototype);

            //        modelBuilder.Entity<WorkSpaceType>().HasMany(wst => wst.ContainerPrototypes).WithRequired(
            //cpwst => cpwst.WorkSpaceType);
        }
    }
}