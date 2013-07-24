using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using TrimFuel.WebService.SupportAPI.Model.Focus;
using TrimFuel.Business;
using System.IO;
using log4net;

namespace TrimFuel.WebService.SupportAPI
{
    /// <summary>
    /// Summary description for customer_service_feed
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.org/support/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class customer_service_feed : System.Web.Services.WebService
    {
        [WebMethod]
        public string Process(Triangle data)
        {
            ILog logger = LogManager.GetLogger(typeof(customer_service_feed));

            StringBuilder response = new StringBuilder();
            response.AppendLine("Parsing succeed:");
            response.AppendLine("Number of Calls: " + (data.CSI != null ? data.CSI.CallList != null ? data.CSI.CallList.Count : 0 : 0).ToString());

            if (data != null && data.CSI != null)
            {
                SupportService srv = new SupportService();

                foreach (Call c in data.CSI.CallList)
                {
                    try
                    {
                        srv.ProcessCall(data.CSI.Partner, data.CSI.Version, c.ID, (c.Time != null ? c.Time.Start : null), (c.Time != null ? c.Time.End : null), (c.Time != null ? c.Time.ANI : null), (c.Time != null ? c.Time.DNIS : null),
                            (c.Agent != null ? c.Agent.ID : null), (c.Agent != null ? c.Agent.Name : null), (c.Agent != null ? c.Agent.Location : null),
                            (c.Disposition != null ? c.Disposition.ID : null), (c.Disposition != null ? c.Disposition.Name : null), (c.Disposition != null ? c.Disposition.AgentNotes : null),
                            (c.Customer != null ? c.Customer.ID : null), (c.Customer != null ? c.Customer.Product : null), c.HoldTime);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }

            return response.ToString();
        }
    }
}
