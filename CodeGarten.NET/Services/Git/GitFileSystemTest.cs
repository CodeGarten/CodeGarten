using System.IO;
using NUnit.Framework;

namespace Git
{
    [TestFixture]
    class GitFileSystemTest
    {
        [Test]
        public void ShouldCreateRepositoryCorrectly()
        {
            GitFileSystem.DeleteRepository("Grupo2");

            Assert.True(GitFileSystem.CreateRepository("Grupo2"));

            Assert.True(Directory.Exists(@"D:\ISEL\Semestre6\PS\Testings\Repositories\Grupo2.git"));
        }

        [Test]
        public void ShouldDeleteRepositoryCorrectly()
        {
            GitFileSystem.CreateRepository("Grupo2");

            Assert.True(Directory.Exists(@"D:\ISEL\Semestre6\PS\Testings\Repositories\Grupo2.git"));

            Assert.True(GitFileSystem.DeleteRepository("Grupo2"));

            Assert.False(Directory.Exists(@"D:\ISEL\Semestre6\PS\Testings\Repositories\Grupo2.git"));
        }
    }
}
