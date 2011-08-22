using System;
using System.Security.Cryptography;
using System.Text;
using CodeGarten.Data.Interfaces;

namespace CodeGarten.Data.Access
{
    public sealed class AuthenticationManager
    {
        private readonly DataBaseManager _dbManager;

        public AuthenticationManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public void CreateAuthenticationDataBase(IAuthentication authenticaton)
        {
            foreach (var user in _dbManager.User.GetAll())
                authenticaton.CreateUser(user.Name, user.Password);
        }

        public static string EncryptPassword(string plainText)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();

            return Encoding.ASCII.GetString(sha1.ComputeHash(Encoding.ASCII.GetBytes(plainText)));
        }

        public bool Authenticate(string user, string passwordPlainText)
        {
            var userObj = _dbManager.User.Get(user);
            if (user == null) throw new Exception();

            return userObj.Password == EncryptPassword(passwordPlainText);
        }
    }
}