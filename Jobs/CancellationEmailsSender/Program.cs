using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business;
using TrimFuel.Model.Enums;

namespace CancellationEmailsSender
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            (new EmailService()).SendCancellationEmailsOnCurrentDateAndHour(ProductEnum.ECigarettes);
            (new EmailService()).SendReactivationEmailsOnCurrentDateAndHour(ProductEnum.ECigarettes);
        }
    }
}
