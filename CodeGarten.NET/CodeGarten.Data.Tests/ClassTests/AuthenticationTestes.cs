using System;
using CodeGarten.Data.Interfaces;

namespace CodeGarten.Data.Tests.ClassTests
{
    internal class AuthenticationTestes : IAuthentication
    {
        public void CreateUser(string user, string password)
        {
            Console.WriteLine("UserName = {0} -> Password = {1}", user, password);
        }
    }
}