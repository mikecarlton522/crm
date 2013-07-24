using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business;

namespace ABFStatusCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            ABFService abfService = new ABFService();
            abfService.CheckShipmentsState();
        }
    }
}
