using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class ContainerTypeManager
    {
        private readonly DataBaseManager _dbManager;

        public ContainerTypeManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public IQueryable<ContainerType> GetAll(long structureId)
        {
            return _dbManager.DbContext.ContainerPrototypes.Where(cp => cp.StructureId == structureId);
        }

        public ContainerType Get(long structureId, string name)
        {
            return _dbManager.DbContext.ContainerPrototypes.Find(name, structureId);
        }

        public ContainerType Create(long structureId, string name, string parent)
        {
            ContainerType parentCp = null;
            var cp = new ContainerType {StructureId = structureId, Name = name};

            if (!string.IsNullOrEmpty(parent))
                parentCp = Get(structureId, parent);

            if (parentCp != null)
                parentCp.Childs.Add(cp);
            else
                _dbManager.DbContext.ContainerPrototypes.Add(cp);

            _dbManager.DbContext.SaveChanges();

            return cp;
        }

        public void Delete(long structureId, string name)
        {
            _dbManager.DbContext.ContainerPrototypes.Remove(Get(structureId, name));
            _dbManager.DbContext.SaveChanges();
        }

        public ContainerType AddWorkSpaceType(long structureId, string name, string workSpaceName)
        {
            var cp = Get(structureId, name);

            cp.Bindings.Add(new Binding{WorkSpaceTypeName = workSpaceName});

            _dbManager.DbContext.SaveChanges();

            return cp;
        }

        public void ClearAllBindings(long structureId)
        {
            foreach (var cp in _dbManager.DbContext.ContainerPrototypes.Where(cp => cp.StructureId == structureId))
                cp.Bindings.Clear();

            _dbManager.DbContext.SaveChanges();
        }

        public ContainerType Bind(long structureId, string containerPrototypeName, string workSpaceTypeName)
        {
            var cp = Get(structureId, containerPrototypeName);
            var binding = _dbManager.DbContext.Bindings.Find(structureId, containerPrototypeName, workSpaceTypeName) ??
                new Binding { WorkSpaceTypeName = workSpaceTypeName };

            cp.Bindings.Add(binding);

            _dbManager.DbContext.SaveChanges();

            return cp;
        }
    }
}