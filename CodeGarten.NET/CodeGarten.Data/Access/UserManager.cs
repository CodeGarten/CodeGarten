using System;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{

    public class EnrollEventArgs : EventArgs
    {
        public EnrollEventArgs(Enroll enroll, Container container)
        {
            Enroll = enroll;
            Container = container;
        }

        public Enroll Enroll { get; private set; }
        public Container Container { get; private set; }
    }

    public class UserEventArgs : EventArgs
    {
        public UserEventArgs(User user)
        {
            User = user;
        }

        public User User { get; private set; }
    }

    public sealed class UserManager
    {
        private readonly Context _dbContext;

        public UserManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        #region Events

        private static event EventHandler<UserEventArgs> _onCreateUser;

        public static event EventHandler<UserEventArgs> OnCreateUser
        {
            add { _onCreateUser += value; }
            remove { _onCreateUser -= value; }
        }

        private static event EventHandler<UserEventArgs> _onRemoveUser;

        public static event EventHandler<UserEventArgs> OnRemoveUser
        {
            add { _onRemoveUser += value; }
            remove { _onRemoveUser -= value; }
        }

        private static event EventHandler<EnrollEventArgs> _onEnrollUser;

        public static event EventHandler<EnrollEventArgs> OnEnrollUser
        {
            add { _onEnrollUser += value; }
            remove { _onEnrollUser -= value; }
        }

        public static event EventHandler<EnrollEventArgs> _onDisenrollUser;

        public static event EventHandler<EnrollEventArgs> OnDisenrollUser
        {
            add { _onDisenrollUser += value; }
            remove { _onDisenrollUser -= value; }
        }

        #endregion

        public User Create(string name, string password, string email)
        {
            var user = new User
            {
                Name = name,
                Password = AuthenticationManager.EncryptPassword(password),
                Email = email
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return user;
        }

        public void Delete(string user)
        {
            _dbContext.Users.Remove(Get(user));


        }

        private bool PermissionToEnroll(Container container, RoleType roleType, RoleBarrier roleBarrier)
        {
            var barrier = (int) roleBarrier;

            return _dbContext.Roles.Where(r =>
                                          r.ContainerPrototypeName == container.Prototype.Name &&
                                          r.RoleTypeName == roleType.Name &&
                                          r.Barrier != barrier
                ).Any();
        }

        private bool ExisteRole(Container container, RoleType roleType)
        {
            return _dbContext.Roles.Where(r =>
                                          r.ContainerPrototypeName == container.Prototype.Name &&
                                          r.RoleTypeName == roleType.Name
                ).Any();
        }

        #region Enroll_methods

        private bool EnrollInherited(User user, Container container, RoleType roleType)
        {
            var enroll = _dbContext.Enrolls.Find(user.Name, container.Id, roleType.Name, container.Prototype.StructureId);

            if (enroll != null)
            {
                enroll.InheritedCount += 1;

                return true;
            }

            enroll = new Enroll()
            {
                User = user,
                Container = container,
                RoleType = roleType,
                Inherited = true,
                InheritedCount = 1
            };

            _dbContext.Enrolls.Add(enroll);

            InvokeOnEnrollUser(enroll);

            return true;
        }

        private void InheritedEnrollParents(User user, Container container, RoleType roleType)
        {
            var current = container;

            while (current != null){
                if (!PermissionToEnroll(current, roleType, RoleBarrier.Bottom))
                    return;    

                EnrollInherited(user, current, roleType);

                current = current.Parent;
            }
        }

        private void InheritedEnrollChilds(User user, Container container, RoleType roleType)
        {

            foreach (var child in container.Childs)
            {
                if (!PermissionToEnroll(child, roleType, RoleBarrier.Top))
                    continue;

                EnrollInherited(user, child, roleType);

                InheritedEnrollChilds(user, child, roleType);
            }
        }

        #endregion

        #region Disenroll_methods

        private bool InheritedDisenroll(User user, Container container, RoleType roleType)
        {
            var enroll = _dbContext.Enrolls.Find(user.Name, container.Id, roleType.Name, container.Prototype.StructureId);

            if (enroll == null)
                return false;

            enroll.InheritedCount -= 1;

            if (enroll.InheritedCount == 0)
                if (enroll.Inherited)
                {
                    _dbContext.Enrolls.Remove(enroll);

                    InvokeOnDisenrollUser(enroll, container);
                }

            return true;
        }

        private void InheritedDisenrollParents(User user, Container container, RoleType roleType)
        {
            var current = container;
            
            while (current != null){
                if (!PermissionToEnroll(current, roleType, RoleBarrier.Bottom))
                    return;

                InheritedDisenroll(user, current, roleType);

                current = current.Parent;
            } 
        }

        private void InheritedDisenrollChilds(User user, Container container, RoleType roleType)
        {
            foreach (var child in container.Childs)
            {
                if (!PermissionToEnroll(child, roleType, RoleBarrier.Top))
                    continue;

                InheritedDisenrollChilds(user, child, roleType);

                InheritedDisenroll(user, child, roleType);
            }
        }

        #endregion

        public bool Disenroll(string user, long structure, long container, string roleType)
        {
            var userObj = Get(_dbContext, user);

            var containerObj = ContainerManager.Get(_dbContext, container);

            var roleTypeObj = RoleTypeManager.Get(_dbContext, structure, roleType);

            var enroll = _dbContext.Enrolls.Find(user, container, roleType, structure);

            if (enroll == null)
                return false; //TODO throw exception

            if (enroll.Inherited)
                return false; //TODO throw exception

            if (enroll.InheritedCount == 0)
            {
                _dbContext.Enrolls.Remove(enroll);
                
                InvokeOnDisenrollUser(enroll, containerObj);
                
            }else
                enroll.Inherited = true;

            InheritedDisenrollChilds(userObj, containerObj, roleTypeObj);
            InheritedDisenrollParents(userObj, containerObj.Parent, roleTypeObj);

            return _dbContext.SaveChanges() != 0;
        }

        public bool Enroll(string user, long structure, long container, string roleType, string password = null)
        {
            var userObj = Get(_dbContext, user);

            var containerObj = ContainerManager.Get(_dbContext, container);

            var roleTypeObj = RoleTypeManager.Get(_dbContext, structure, roleType);

            if (!ExisteRole(containerObj, roleTypeObj))
                return false; // throw exception

            var enroll = _dbContext.Enrolls.Find(user, container, roleType, structure);

            if (enroll != null)
            {
                if (!enroll.Inherited)
                    return false; // throw exception 

                var pass = _dbContext.EnrollPassWords.Find(container, roleType, structure);
                if (pass != null)
                    if (password==null || pass.Password != AuthenticationManager.EncryptPassword(password))
                        return false;
                
                enroll.Inherited = false;
            }else
            {
                enroll = new Enroll()
                             {
                                 User = userObj,
                                 Container = containerObj,
                                 RoleType = roleTypeObj,
                                 Inherited = false,
                                 InheritedCount = 0
                             };

                _dbContext.Enrolls.Add(enroll);

                InvokeOnEnrollUser(enroll);
            }

            InheritedEnrollChilds(userObj, containerObj, roleTypeObj);    
            
            InheritedEnrollParents(userObj, containerObj.Parent, roleTypeObj);

            return _dbContext.SaveChanges() != 0;
        }

        internal static User Get(Context db, string user)
        {
            return db.Users.Where((u) => u.Name == user).SingleOrDefault();
        }

        public User Get(string user)
        {
            return Get(_dbContext, user);
        }

        #region InvokeEvents
        
        private void InvokeOnCreateUser(User user)
        {
            var e = new UserEventArgs(user);
            if (_onCreateUser != null) _onCreateUser(this, e);
        }

        private void InvokeOnRemoveUser(User user)
        {
            var e = new UserEventArgs(user);
            if (_onRemoveUser != null) _onRemoveUser(this, e);
        }

        private void InvokeOnEnrollUser(Enroll enroll)
        {
            var e = new EnrollEventArgs(enroll, enroll.Container);
            if (_onEnrollUser != null) _onEnrollUser(this, e);
        }

        private void InvokeOnDisenrollUser(Enroll enroll, Container container)
        {
            var e = new EnrollEventArgs(enroll, container);
            if (_onDisenrollUser != null) _onDisenrollUser(this, e);
        }

        #endregion

        public IQueryable<User> GetAll()
        {
            return _dbContext.Users;
        }
    }
}