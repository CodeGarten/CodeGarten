namespace CodeGarten.Data.Interfaces
{
    public interface IAuthentication
    {
        void CreateUser(string user, string password);
    }
}