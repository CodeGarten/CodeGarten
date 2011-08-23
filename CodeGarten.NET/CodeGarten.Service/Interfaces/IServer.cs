using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGarten.Service.Interfaces
{
    public interface IServer
    {
        bool CreateUser(string user, string password);
        bool DeleteUser(string user);
        bool ChangePassword(string user, string newPassword);
    }
}
