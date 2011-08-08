using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CodeGarten.Service
{
    public static class ServiceConfig
    {
        public static String ServicesDllLocation { get; private set; }
        public static String ServicesResourceLibLocation { get; private set; }

        static ServiceConfig()
        {
            ServicesDllLocation = ConfigurationManager.AppSettings["ServicesDllLocation"];
            ServicesResourceLibLocation = ConfigurationManager.AppSettings["ServicesResourceLocation"];
        }
    }
}