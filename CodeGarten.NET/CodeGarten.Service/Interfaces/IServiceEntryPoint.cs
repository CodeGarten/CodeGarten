using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CodeGarten.Service.Interfaces
{
    public interface IServiceEntryPoint
    {
        ActionResult Index(long structureId, long containerId, string workspaceTypeName);
    }
}
