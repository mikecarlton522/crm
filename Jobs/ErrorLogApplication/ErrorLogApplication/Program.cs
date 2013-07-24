using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErrorLogApplication.Core;
using ErrorLogApplication.Model;

namespace ErrorLogApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DateTime from = DateTime.Now.AddHours(-1);

                foreach (ErrorLogApplication.Model.Application app in Config.Applications)
                {
                    var logs = new LogParser().ParseLog(app.LogPath, app.ApplicationName, from,  app.Max);
                    ErrorLogService logService = new ErrorLogService();
                    foreach (var log in logs)
                    {
                        logService.WriteError(log);
                    }
                }

            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
