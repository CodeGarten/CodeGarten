using System;
using System.Data.Entity;

namespace CodeGarten.Data.Access
{
    public sealed class DataBaseManager : IDisposable
    {
        internal readonly Context DbContext;

        private UserManager _user;

        public UserManager User
        {
            get { return _user ?? (_user = new UserManager(this)); }
            private set { _user = value; }
        }

        public AuthenticationManager Authentication { get; private set; }
        public AuthorizationManager Authorization { get; private set; }
        public ContainerManager Container { get; private set; }
        public ContainerPrototypeManager ContainerPrototype { get; private set; }
        public RoleManager Role { get; private set; }
        public RoleTypeManager RoleType { get; private set; }
        public RuleManager Rule { get; private set; }
        public ServiceManager Service { get; private set; }
        public StructureManager Structure { get; private set; }
        public WorkSpaceTypeManager WorkSpaceType { get; private set; }

        public DataBaseManager()
        {
            DbContext = new Context();

            User = new UserManager(this);
            Authentication = new AuthenticationManager(this);
            Authorization = new AuthorizationManager(this);
            Container = new ContainerManager(this);
            ContainerPrototype = new ContainerPrototypeManager(this);
            Role = new RoleManager(this);
            RoleType = new RoleTypeManager(this);
            Rule = new RuleManager(this);
            Service = new ServiceManager(this);
            Structure = new StructureManager(this);
            WorkSpaceType = new WorkSpaceTypeManager(this);
        }

        public void Dispose()
        {
            if (DbContext != null)
                DbContext.Dispose();
        }
    }
}