namespace CodeGarten.Data.Access
{

    public sealed class AuthorizationManager
    {
        private readonly DataBaseManager _dbManager;

        public AuthorizationManager(DataBaseManager db)
        {
            _dbManager = db;
        }
    }
}