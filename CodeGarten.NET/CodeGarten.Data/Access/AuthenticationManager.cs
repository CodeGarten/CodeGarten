using System;
using System.Security.Cryptography;
using System.Text;

namespace CodeGarten.Data.Access
{
    public sealed class AuthenticationManager
    {
        private readonly DataBaseManager _dbManager;

        public AuthenticationManager(DataBaseManager db)
        {
            _dbManager = db;
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