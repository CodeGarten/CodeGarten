﻿using System;
using System.Configuration;
using System.IO;
using CodeGarten.Utils;

namespace CodeGarten.Data.Access
{
    public sealed class DataBaseManager : IDisposable
    {
        internal readonly Context DbContext;
        internal static Logger Logger;

        static DataBaseManager()
        {
            var path = ConfigurationManager.AppSettings["DataLoggerPath"];
            var loggerPath = path ?? @"C:\DataLogger.log";
            var fileLogger = File.Exists(loggerPath) ? File.AppendText(loggerPath) : File.CreateText(loggerPath);
            Logger = new Logger(fileLogger);
            Logger.Start();
        }

        private UserManager _user;

        public UserManager User
        {
            get { return _user ?? (_user = new UserManager(this)); }
            private set { _user = value; }
        }

        public AuthenticationManager Authentication { get; private set; }
        public ContainerManager Container { get; private set; }
        public ContainerTypeManager ContainerType { get; private set; }
        public RoleManager Role { get; private set; }
        public RoleTypeManager RoleType { get; private set; }
        public RuleManager Rule { get; private set; }
        public ServiceTypeManager ServiceType { get; private set; }
        public StructureManager Structure { get; private set; }
        public WorkSpaceTypeManager WorkSpaceType { get; private set; }

        public DataBaseManager()
        {
            DbContext = new Context();

            User = new UserManager(this);
            Authentication = new AuthenticationManager(this);
            Container = new ContainerManager(this);
            ContainerType = new ContainerTypeManager(this);
            Role = new RoleManager(this);
            RoleType = new RoleTypeManager(this);
            Rule = new RuleManager(this);
            ServiceType = new ServiceTypeManager(this);
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