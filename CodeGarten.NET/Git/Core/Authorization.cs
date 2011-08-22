
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Git.Core
{
    public enum Privileges
    {
        r,
        rw
    }

    public sealed class Authorization
    {
        private readonly string _filePath;

        private const string RepositoryType = "repo";
        private const string GroupType = "group";
        private const string UserType = "user";
        private const int Window = 500;

        #region PROPERTIES
        private FileStream this[string repositoryName]
        {
            get
            {
                var repositoryPath = Path(_filePath, repositoryName);
                return new FileStream(repositoryPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
        }
        #endregion

        public Authorization(string filePath)
        {
            _filePath = filePath;
        }

        public void CreateRepository(string name)
        {
            Repository(name).Save(this[name]);
        }

        public void DeleteRepository(string name)
        {
            File.Delete(Path(_filePath, name));
        }

        public void AddUser(string userName, string repositoryName, Privileges permission)
        {
            try
            {
                using (var stream = this[repositoryName])
                {
                    if(stream.Length == 0)
                    {
                        stream.Dispose();
                        File.Delete(Path(_filePath, repositoryName));
                        return;
                    }
                    var repository = XElement.Load(stream);

                    var group =
                        repository.Elements(GroupType).First(
                            g => g.Attribute("perm").Value == permission.ToString());

                    if (group.Elements().Any(u => u.Attribute("name").Value == userName))
                        return;
                    
                    group.Add(User(userName));

                    stream.SetLength(0);
                    repository.Save(stream);
                    stream.Flush();
                }
            }
            catch (IOException)
            {
                Thread.Sleep(Window);
                AddUser(userName, repositoryName, permission);
            }
        }

        public void RemoveUser(string userName, string repositoryName)
        {
            try
            {
                using (var stream = this[repositoryName])
                {
                    if (stream.Length == 0)
                    {
                        stream.Dispose();
                        File.Delete(Path(_filePath, repositoryName));
                        return;
                    }

                    var repository = XElement.Load(stream);

                    var users = repository.Elements(GroupType).Elements(UserType).Where(u => u.Attribute("name").Value == userName);

                    var any = false;

                    foreach (var user in users)
                    {
                        user.Remove();
                        any = true;
                    }

                    if (!any)
                        return;

                    stream.SetLength(0);
                    repository.Save(stream);
                    stream.Flush();
                }
            }
            catch (IOException)
            {
                Thread.Sleep(Window);
                RemoveUser(userName, repositoryName);
            }
        }

        #region HELPERS

        private static XElement Repository(string name)
        {
            var repository = new XElement(RepositoryType);
            repository.SetAttributeValue("location", name);

            repository.Add(Group(Privileges.r));
            repository.Add(Group(Privileges.rw));

            return repository;
        }

        private static XElement Group(Privileges privileges)
        {
            var group = new XElement(GroupType);
            group.SetAttributeValue("perm", privileges.ToString());
            return group;
        }

        private static XElement User(string name)
        {
            var user = new XElement(UserType);
            user.SetAttributeValue("name", name);
            return user;
        }

        private static string Path(string path, string repositoryName)
        {
            return System.IO.Path.Combine(path, string.Format("{0}.xml", repositoryName));
        }

        #endregion
    }
}
