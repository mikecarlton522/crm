using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business;

namespace RefererPromotion1BatchEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            JobService jobService = new JobService();
            jobService.SendRefererPromotion1BatchEmail();
        }
    }
}
