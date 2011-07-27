using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class ContainerPrototypeManager
    {
        private readonly Context _dbContext;

        public ContainerPrototypeManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        internal static ContainerPrototype Get(Context db, long structureId, string name)
        {
            return db.ContainerPrototypes.Find(name, structureId);
        }

        public IQueryable<ContainerPrototype> GetAll(long structureId)
        {
            return _dbContext.ContainerPrototypes.Where(cp => cp.StructureId == structureId);
        }

        public ContainerPrototype Get(long structureId, string name)
        {
            return Get(_dbContext, structureId, name);
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
                _dbContext.ContainerPrototypes.Add(cp);

            _dbContext.SaveChanges();

            return cp;
        }

        public void Delete(long structureId, string name)
        {
            _dbContext.ContainerPrototypes.Remove(Get(structureId, name));
            _dbContext.SaveChanges();
        }

        public ContainerPrototype AddWorkSpace(long structureId, string name, string workSpaceName)
        {
            var cp = Get(structureId, name);

            cp.Bindings.Add(new Binding{WorkSpaceTypeName = workSpaceName});

            _dbContext.SaveChanges();

            return cp;
        }

        public void ClearAllBindings(long structureId)
        {
            foreach(var cp in _dbContext.ContainerPrototypes.Where(cp => cp.StructureId == structureId))
                cp.Bindings.Clear();

            _dbContext.SaveChanges();
        }

        public ContainerPrototype Bind(long structureId, string containerPrototypeName, string workSpaceTypeName)
        {
            var cp = Get(structureId, containerPrototypeName);
            var binding = _dbContext.Bindings.Find(structureId, containerPrototypeName, workSpaceTypeName) ??
                new Binding { WorkSpaceTypeName = workSpaceTypeName };

            cp.Bindings.Add(binding);

            _dbContext.SaveChanges();

            return cp;
        }
    }
}