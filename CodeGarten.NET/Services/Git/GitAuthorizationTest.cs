using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;

namespace Git
{
    [TestFixture]
    class GitAuthorizationTest
    {
        [Test]
        public void ShouldBeAbleToCreateAnEntireStructureFromScratch()
        {
            var rootElement = GitAuthorization.CreateMainStructure();
            var groups = new[]
                            {
                                GitAuthorization.CreateGroup("Grupo1", "Samir", "Faustino"), 
                                GitAuthorization.CreateGroup("Grupo2", "CodeGarten", "Ze"), 
                                GitAuthorization.CreateGroup("Grupo3", "To", "Quim")
                            };

            var repositories = new[]
                                   {
                                       GitAuthorization.CreateRepository("Repo1",null),
                                       GitAuthorization.CreateRepository("Repo2",null),
                                       GitAuthorization.CreateRepository("Repo3",null)
                                   };

            GitAuthorization.AddRepositoryGroup(repositories[0], "Grupo1", "r");
            GitAuthorization.AddRepositoryGroup(repositories[0], "Grupo2", "rw");

            GitAuthorization.AddRepositoryGroup(repositories[1], "Grupo2", "r");
            GitAuthorization.AddRepositoryGroup(repositories[1], "Grupo3", "rw");

            GitAuthorization.AddRepositoryGroup(repositories[2], "Grupo3", "r");
            GitAuthorization.AddRepositoryGroup(repositories[2], "Grupo1", "rw");

            rootElement.Add(groups);
            rootElement.Add(repositories);

            const string correctXml = @" <autho_file>
                                             <group ID='Grupo1'>
                                                  <user>Samir</user> 
                                                  <user>Faustino</user> 
                                             </group>
                                             <group ID='Grupo2'>
                                                  <user>CodeGarten</user> 
                                                  <user>Ze</user> 
                                             </group>
                                             <group ID='Grupo3'>
                                                  <user>To</user> 
                                                  <user>Quim</user> 
                                             </group>
                                             <repo location='Repo1'>
                                                  <group perm='r'>Grupo1</group> 
                                                  <group perm='rw'>Grupo2</group> 
                                             </repo>
                                             <repo location='Repo2'>
                                                  <group perm='r'>Grupo2</group> 
                                                  <group perm='rw'>Grupo3</group> 
                                             </repo>
                                             <repo location='Repo3'>
                                                  <group perm='r'>Grupo3</group> 
                                                  <group perm='rw'>Grupo1</group> 
                                             </repo>
                                         </autho_file>";

            var correctParsedXElement = XElement.Parse(correctXml);

            Assert.True(XNode.DeepEquals(rootElement, correctParsedXElement));
        }
    }
}
