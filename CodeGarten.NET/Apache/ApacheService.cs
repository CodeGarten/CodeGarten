using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using CodeGarten.Service;
using CodeGarten.Service.Utils;
using CodeGarten.Data.Model;
using CodeGarten.Data.Access;
using Service = CodeGarten.Service.Service;


namespace Apache
{
    [Export(typeof(Service))]
    public class ApacheService : Service
    {

        public ApacheService()
            : base(new ServiceModel("Apache"))
        {}

        public override string GetInstancePath(Container container, WorkSpaceType workSpaceType)
        {
            throw new NotImplementedException();
        }

        public override bool  IsInstaled
        {
	        get 
	        { 
		         return true;
	        }
        }

        public override void OnServiceCreating(ServiceBuilder serviceBuilder)
        {
            serviceBuilder.OnCreateUser += CreateUser;
            serviceBuilder.OnRemoveUser += DeleteUser;
            serviceBuilder.OnUserChangePassword += ChangePasswordUser;
        }

        private void CreateUser(object sender, UserEventArgs eventArgs)
        {
            if (!AuthenticationManager.CreateUser(eventArgs.User.Name, eventArgs.PasswordPlainText))
                Logger.Log(String.Format("Service {0} -> Creating user \"{1}\" fail", Name, eventArgs.User.Name));
        }

        private void DeleteUser(object sender, UserEventArgs eventArgs)
        {
            if (!AuthenticationManager.DeleteUser(eventArgs.User.Name))
                Logger.Log(String.Format("Service {0} -> Deleting user \"{1}\" fail", Name, eventArgs.User.Name));
        }

        private void ChangePasswordUser(object sender, UserEventArgs eventArgs)
        {
            if (!AuthenticationManager.CreateUser(eventArgs.User.Name, eventArgs.PasswordPlainText))
                Logger.Log(String.Format("Service {0} -> Change password user \"{1}\" fail", Name, eventArgs.User.Name));
        }
    }
}