using System.Threading;
using Git.Core;
using NUnit.Framework;

namespace Git.Tests
{
    [TestFixture]
    public sealed class ThreadSafeFileSystem
    {
        private const long RepositoryCount = 100;

        [Test]
        public void FileSystemCreate()
        {
            var fileSystem = new FileSystem(@"c:\temp\");
            int[] threadCount = { 0 };

            for (var i = 0; i < RepositoryCount; ++i)
            {
                int i1 = i;
                new TestDelegate(() => Assert.True(fileSystem.CreateRepository(string.Format("repo_{0}", i1)))).BeginInvoke(
                    delegate
                    {
                        Interlocked.Increment(ref threadCount[0]);
                        Assert.NotNull(fileSystem[string.Format("repo_{0}", i1)]);
                    }, null);
            }

            while (threadCount[0] != RepositoryCount)
            {
                Thread.Sleep(3000);
            }
        }

        [Test]
        public void FileSystemDelete()
        {
            var fileSystem = new FileSystem(@"c:\temp\");
            int[] threadCount = { 0 };

            for (var i = 0; i < RepositoryCount; ++i)
            {
                int i1 = i;
                new TestDelegate(() => Assert.True(fileSystem.DeleteRepository(string.Format("repo_{0}", i1)))).BeginInvoke(
                    delegate {
                            Interlocked.Increment(ref threadCount[0]);
                            Assert.IsNull(fileSystem[string.Format("repo_{0}", i1)]);
                        }, null);
            }

            while (threadCount[0] != RepositoryCount)
            {
                Thread.Sleep(3000);
            }
        }
    }

    [TestFixture]
    public sealed class ThreadSafeAuthorization
    {
        private const long RepositoryCount = 1000;

        [Test]
        public void AuthorizeCreateRepositoryThreadSafe()
        {
            var authorization = new Authorization(@"c:\temp\");
            int[] threadCount = { 0 };

            for (var i = 0; i < RepositoryCount; ++i)
            {
                int i1 = i;
                new TestDelegate(() => authorization.CreateRepository(string.Format("repo_{0}", i1))).BeginInvoke(
                    async => Interlocked.Increment(ref threadCount[0]), null);
            }

            while (threadCount[0] != RepositoryCount)
            {
                Thread.Sleep(3000);
            }
        }

        [Test]
        public void AuthorizeDeleteRepositoryThreadSafe()
        {
            var authorization = new Authorization(@"c:\temp\");
            int[] threadCount = { 0 };

            for (var i = 0; i < RepositoryCount; ++i)
            {
                int i1 = i;
                new TestDelegate(() => authorization.DeleteRepository(string.Format("repo_{0}", i1))).BeginInvoke(
                    async => Interlocked.Increment(ref threadCount[0]), null);
            }

            while (threadCount[0] != RepositoryCount)
            {
                Thread.Sleep(3000);
            }
        }

        [Test]
        public void AuthorizeAddUserThreadSafe()
        {
            var authorization = new Authorization(@"c:\temp\");
            int[] threadCount = { 0 };

            for (var i = 0; i < RepositoryCount; ++i)
            {
                int i1 = i;
                new TestDelegate(() => authorization.AddUser("Samir_" + i1,"repo_0", Privileges.r)).BeginInvoke(
                    async => Interlocked.Increment(ref threadCount[0]), null);
            }

            while (threadCount[0] != RepositoryCount)
            {
                Thread.Sleep(500);
            }
        }

        [Test]
        public void AuthorizeRemoveUserThreadSafe()
        {
            var authorization = new Authorization(@"c:\temp\");
            int[] threadCount = { 0 };

            for (var i = 0; i < RepositoryCount; ++i)
            {
                int i1 = i;
                new TestDelegate(() => authorization.RemoveUser("Samir_" + i1, string.Format("repo_{0}", 0))).BeginInvoke(
                    async => Interlocked.Increment(ref threadCount[0]), null);
            }

            while (threadCount[0] != RepositoryCount)
            {
                Thread.Sleep(500);
            }
        }
    }
}
