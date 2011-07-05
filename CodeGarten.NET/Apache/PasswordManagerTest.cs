using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Apache
{
    [TestFixture]
    internal class PasswordManagerTest
    {
        [Test]
        public void UserShouldBeInsertedCorrectly()
        {
            Assert.True(PasswordManager.CreateUser("Samir", "12345", PasswordManager.EncodeType.Sha1));
        }

        [Test]
        public void UserShouldBeDeletedSuccessfully()
        {
            PasswordManager.CreateUser("Samir", "12345", PasswordManager.EncodeType.Sha1);

            Assert.True(PasswordManager.DeleteUser("Samir"));
        }
    }
}