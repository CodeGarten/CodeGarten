using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGarten.Service;
using CodeGarten.Service.Utils;

namespace Trac
{
    public class Trac : Service
    {
        public Trac() : base(new ServiceModel("Trac", "", ))
        {
        }

        public override void OnServiceInstall()
        {
            throw new NotImplementedException();
        }

        public override bool IsInstaled
        {
            get { throw new NotImplementedException(); }
        }
    }
}
