using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class ContainerPrototypeManager
    {
        private readonly DataBaseManager _dbManager;

        public ContainerPrototypeManager(DataBaseManager db)
        {
            _dbManager = db;
        }

        public IQueryable<ContainerPrototype> GetAll(long structureId)
        {
            return _dbManager.DbContext.ContainerPrototypes.Where(cp => cp.StructureId == structureId);
        }

        public ContainerPrototype Get(long structureId, string name)
        {
            return _dbManager.DbContext.ContainerPrototypes.Find(name, structureId);
        }

        public ContainerPrototype Create(long structureId, string name, string parent)
        {
            ContainerPrototype parentCp = null;
            var cp = new ContainerPrototype {StructureId = structureId, Name = name};

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

        public ContainerPrototype AddWorkSpaceType(long structureId, string name, string workSpaceName)
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

        public ContainerPrototype Bind(long structureId, string containerPrototypeName, string workSpaceTypeName)
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