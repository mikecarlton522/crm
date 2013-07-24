using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business;

namespace KeymailSendPendingOrders
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            KeymailService keymailService = new KeymailService();
            keymailService.SendPendingOrders();
        }
    }
}
