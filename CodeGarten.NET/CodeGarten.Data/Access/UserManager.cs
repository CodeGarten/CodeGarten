using System;
using System.Linq;
using CodeGarten.Data.Model;

namespace CodeGarten.Data.Access
{
    public sealed class UserManager
    {
        private readonly Context _dbContext;

        public UserManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

    //    #region Events

    //    private static event EventHandler _onCreateUser;

    //    public static event EventHandler OnCreateUser
    //    {
    //        add { _onCreateUser += value; }
    //        remove { _onCreateUser -= value; }
    //    }

    //    private static event EventHandler _onEnrollUser;

    //    public static event EventHandler OnEnrollUser
    //    {
    //        add { _onEnrollUser += value; }
    //        remove { _onEnrollUser -= value; }
    //    }

    //    #endregion

    //    public void Create(UserView userView)
    //    {
    //        if (userView == null) throw new ArgumentNullException("userView");

    //        var user = userView.Convert();

    //        _dbContext.Users.Add(user);
    //        _dbContext.SaveChanges();

    //        //TODO view Event Args
    //        InvokeOnCreateUser(null);
    //    }

    //    public bool Enroll(string user, long structure, long container, string roleType)
    //    {
    //        var userObj = Get(_dbContext, user);
    //        if (userObj == null) throw new ArgumentException("\"user\" is a invalid argument");

    //        var containerObj = ContainerManager.Get(_dbContext, container);
    //        if (containerObj == null)
    //            throw new ArgumentException(
    //                "\"structure\" or \"containerPrototype\" or \"container\" is a invalid argument");

    //        var roleTypeObj = RoleTypeManager.Get(_dbContext, structure, roleType);
    //        if (roleTypeObj == null) throw new ArgumentException("\"structure\" or \"roleType\" is a invalid argument");

    //        var enroll = new Enroll()
    //                         {
    //                             User = userObj,
    //                             Container = containerObj,
    //                             RoleType = roleTypeObj
    //                         };

    //        _dbContext.Enrolls.Add(enroll);

    //        var changes = _dbContext.SaveChanges() != 0;

    //        if (changes)
    //            InvokeOnEnrollUser(null);

    //        return changes;
    //    }

    //    internal static User Get(Context db, string user)
    //    {
    //        return db.Users.Where((u) => u.Name == user).SingleOrDefault();
    //    }

    //    public UserView Get(string user)
    //    {
    //        var userObj = Get(_dbContext, user);
    //        return userObj == null ? null : userObj.Convert();
    //    }

    //    #region InvokeEvents

    //    private void InvokeOnCreateUser(EventArgs e)
    //    {
    //        var handler = _onCreateUser;
    //        if (handler != null) handler(this, e);
    //    }

    //    private void InvokeOnEnrollUser(EventArgs e)
    //    {
    //        var handler = _onEnrollUser;
    //        if (handler != null) handler(this, e);
    //    }

    //    #endregion
        public User Get(string name)
        {
            return _dbContext.Users.Find(name);
        }

        public IQueryable<Enroll> Enrolls(string user, long structureId)
        {
            return _dbContext.Enrolls.Where(e => e.UserName == user && e.RoleTypeStructureId == structureId);
        }

        public void Leave(string name, long containerId)
        {
            throw new NotImplementedException();
        }

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

        public IQueryable<User> GetAll()
        {
            return _dbContext.Users;
        }
    }
}