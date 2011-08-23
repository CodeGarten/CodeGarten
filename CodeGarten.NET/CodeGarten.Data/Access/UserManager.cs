using System;
using System.Collections.Generic;
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

        public UserEventArgs(User user, string passwordPlainText) : this(user)
        {
            PasswordPlainText = passwordPlainText;
        }

        public User User { get; private set; }
        public string PasswordPlainText { get; private set; }
    }

    public sealed class UserManager
    {
        private readonly DataBaseManager _db;

        public UserManager(DataBaseManager db)
        {
            _db = db;            
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

        private static event EventHandler<EnrollEventArgs> _onDisenrollUser;
        public static event EventHandler<EnrollEventArgs> OnDisenrollUser
        {
            add { _onDisenrollUser += value; }
            remove { _onDisenrollUser -= value; }
        }

        private static event EventHandler<UserEventArgs> _onUserChangePassword;
        public static event EventHandler<UserEventArgs> OnUserChangePassword
        {
            add { _onUserChangePassword += value; }
            remove { _onUserChangePassword -= value; }
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
            _db.DbContext.Users.Add(user);
            _db.DbContext.SaveChanges();

            InvokeOnCreateUser(user, password);

            return user;
        }

        public void ChangeEmail(string name, string newEmail)
        {
            Get(name).Email = newEmail;

            _db.DbContext.SaveChanges();
        }

        public void ChangePassword(string name, string newPassword)
        {
            var user = Get(name);
            user.Password = AuthenticationManager.EncryptPassword(newPassword);

            _db.DbContext.SaveChanges();

            InvokeOnUserChangePassword(user, newPassword);
        }

        public void Delete(string userName)
        {
            var user = Get(userName);

            var enrolls = _db.DbContext.Enrolls.Where(e => e.UserName == userName && !e.Inherited).ToList();
            foreach (var enroll in enrolls)
                Disenroll(userName, enroll.RoleType.StructureId, enroll.ContainerId, enroll.RoleTypeName);

            var structures = _db.DbContext.Structures.Where(s => s.Administrators.Count == 1 && s.Administrators.Select(st=> st.Name).Contains(user.Name)).ToList();
            foreach (var structure in structures)
                _db.Structure.Delete(structure.Id);

            _db.DbContext.Users.Remove(user);
            _db.DbContext.SaveChanges();

            InvokeOnRemoveUser(user);
        }

        private bool PermissionToEnroll(Container container, RoleType roleType, RoleBarrier roleBarrier)
        {
            var barrier = (int) roleBarrier;

            return _db.DbContext.Roles.Where(r =>
                                          r.ContainerPrototypeName == container.Prototype.Name &&
                                          r.RoleTypeName == roleType.Name &&
                                          r.Barrier != barrier
                ).Any();
        }

        private bool ExisteRole(Container container, RoleType roleType)
        {
            return _db.DbContext.Roles.Where(r =>
                                          r.ContainerPrototypeName == container.Prototype.Name &&
                                          r.RoleTypeName == roleType.Name
                ).Any();
        }

        #region Enroll_methods

        private bool EnrollInherited(User user, Container container, RoleType roleType)
        {
            var enroll = _db.DbContext.Enrolls.Find(user.Name, container.Id, roleType.Name, container.Prototype.StructureId);

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

            _db.DbContext.Enrolls.Add(enroll);

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
            var enroll = _db.DbContext.Enrolls.Find(user.Name, container.Id, roleType.Name, container.Prototype.StructureId);

            if (enroll == null)
                return false;

            enroll.InheritedCount -= 1;

            if (enroll.InheritedCount == 0)
                if (enroll.Inherited)
                {
                    _db.DbContext.Enrolls.Remove(enroll);

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
            var userObj = Get(user);

            var containerObj = _db.Container.Get(container);

            var roleTypeObj = _db.RoleType.Get( structure, roleType);

            var enroll = _db.DbContext.Enrolls.Find(user, container, roleType, structure);

            if (enroll == null)
                return false; //TODO throw exception

            if (enroll.Inherited)
                return false; //TODO throw exception

            if (enroll.InheritedCount == 0)
            {
                _db.DbContext.Enrolls.Remove(enroll);

                InvokeOnDisenrollUser(enroll, containerObj);
                
            }else
                enroll.Inherited = true;

            InheritedDisenrollChilds(userObj, containerObj, roleTypeObj);
            InheritedDisenrollParents(userObj, containerObj.Parent, roleTypeObj);

            return _db.DbContext.SaveChanges() != 0;
        }

        public bool Enroll(string user, long structure, long container, string roleType, string password = null)
        {
            var userObj = Get(user);

            var containerObj = _db.Container.Get(container);

            var roleTypeObj = _db.RoleType.Get(structure, roleType);

            if (!ExisteRole(containerObj, roleTypeObj))
                return false; //TODO throw exception

            var pass = _db.DbContext.EnrollPassWords.Find(container, roleType, structure);
            if (pass != null)
                if (password == null || pass.Password != AuthenticationManager.EncryptPassword(password))
                    return false;

            var enroll = _db.DbContext.Enrolls.Find(user, container, roleType, structure);
            if (enroll != null)
            {
                if (!enroll.Inherited)
                    return false; //TODO throw exception 

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

                _db.DbContext.Enrolls.Add(enroll);

                InvokeOnEnrollUser(enroll);
            }

            InheritedEnrollChilds(userObj, containerObj, roleTypeObj);    
            
            InheritedEnrollParents(userObj, containerObj.Parent, roleTypeObj);

            return _db.DbContext.SaveChanges() != 0;
        }
        
        internal void SyncronizeEnrolls(Container container)
        {

            var roles = _db.DbContext.Roles.Where(r => r.ContainerPrototypeName == container.Prototype.Name && r.Barrier != (int)RoleBarrier.Top);
            if (roles.Count() == 0)
                return;

            var currentContainer = container.Parent;
            while(currentContainer!=null)
            {
                var currentContext = currentContainer;
                var currentRoles = _db.DbContext.Roles.Where(
                    r => r.ContainerPrototypeName == currentContext.Name && r.Barrier == (int)RoleBarrier.Top);

                var rolesContext = roles = roles.Where(r => !currentRoles.Where(cr => cr.RoleTypeName == r.RoleTypeName).Any());
                if (roles.Count() == 0)
                    return;

                var enrolls = _db.DbContext.Enrolls.Where(
                                e =>    e.ContainerId == currentContext.Id && 
                                        e.Inherited == false &&
                                        rolesContext.Where(r => r.RoleTypeName == e.RoleTypeName).Any()
                                                        );
                foreach (var enroll in enrolls)
                    EnrollInherited(enroll.User, container, enroll.RoleType);

                currentContainer = currentContainer.Parent;
            }
        }

        public User Get(string user)
        {
            return _db.DbContext.Users.Find(user);
        }

        public IQueryable<Enroll> GetEnrolls(string userName)
        {
            return _db.DbContext.Enrolls.Where(e => e.UserName == userName);
        }

        #region InvokeEvents

        private void InvokeOnUserChangePassword(User user, string passwordPlainText)
        {
            var eventArgs = new UserEventArgs(user);

            if (_onUserChangePassword != null)
                try
                {
                    _onUserChangePassword(this, eventArgs);
                }
                catch (Exception e)
                {
                    DataBaseManager.Logger.Log(String.Format("InvokeOnUserChangePassword fail - {0}", e.Message));
                }
        }

        private void InvokeOnCreateUser(User user, string passwordPlainText)
        {
            var eventArgs = new UserEventArgs(user, passwordPlainText);
            
            if (_onCreateUser != null) 
                try
                {
                    _onCreateUser(this, eventArgs);
                }catch(Exception e)
                {
                    DataBaseManager.Logger.Log(String.Format("InvokeOnCreateUser fail - {0}", e.Message));
                }
        }

        private void InvokeOnRemoveUser(User user)
        {
            var eventArgs = new UserEventArgs(user);
            
            if (_onRemoveUser != null) 
                try
                {
                    _onRemoveUser(this, eventArgs);
                }catch(Exception e)
                {
                    DataBaseManager.Logger.Log(String.Format("InvokeOnRemoveUser fail - {0}", e.Message));
                }
        }

        private void InvokeOnEnrollUser(Enroll enroll)
        {
            var eventArgs = new EnrollEventArgs(enroll, enroll.Container);

            if (_onEnrollUser != null) 
                try
                {
                    _onEnrollUser(this, eventArgs);
                }catch(Exception e)
                {
                   DataBaseManager.Logger.Log(String.Format("InvokeOnEnrollUser fail - {0}", e.Message));
                }
                
        }

        private void InvokeOnDisenrollUser(Enroll enroll, Container container)
        {
            var eventArgs = new EnrollEventArgs(enroll, container);

            if (_onDisenrollUser != null) 
                try
                {
                    _onDisenrollUser(this, eventArgs);    
                }catch(Exception e)
                {
                    DataBaseManager.Logger.Log(String.Format("InvokeOnDisenrollUser fail - {0}", e.Message));
                }
                
        }

        #endregion

        public IQueryable<User> GetAll()
        {
            return _db.DbContext.Users;
        }
 
        public IQueryable<User> Search(string query)
        {
            return _db.DbContext.Users.Where(u => u.Name.StartsWith(query.Trim()));
        }
    }
}