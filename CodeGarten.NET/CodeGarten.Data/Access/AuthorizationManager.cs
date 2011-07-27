using System;
using System.Collections.Generic;
using System.Linq;
using CodeGarten.Data.Interfaces;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{

    public sealed class AuthorizationManager
    {
        private readonly Context _dbContext;

        public AuthorizationManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }
    }
}