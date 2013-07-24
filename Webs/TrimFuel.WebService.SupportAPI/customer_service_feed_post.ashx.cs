using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using TrimFuel.WebService.SupportAPI.Model.Focus;
using System.Text;
using System.IO;
using System.Xml;
using log4net;
using TrimFuel.Business;

namespace TrimFuel.WebService.SupportAPI
{
    [WebService(Namespace = "http://trianglecrm.org/support/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class customer_service_feed_post : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            StringBuilder response = new StringBuilder();
            Triangle res = null;

            XmlSerializer s = new XmlSerializer(typeof(Triangle), "");
            s.UnknownAttribute += new XmlAttributeEventHandler(s_UnknownAttribute);
            s.UnknownElement += new XmlElementEventHandler(s_UnknownElement);

            string data = context.Request["data"];
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    using (StringReader sr = new StringReader(data))
                    {
                        res = (Triangle)s.Deserialize(sr);
                    }
                    response.AppendLine("Parsing succeed:");
                    response.AppendLine("Number of Calls: " + (res.CSI != null ? res.CSI.CallList != null ? res.CSI.CallList.Count : 0 : 0).ToString());
                }
                catch (Exception ex)
                {
                    response.AppendLine("Parsing failed:");
                    response.AppendLine(ex.ToString());
                }
            }
            else
            {
                response.AppendLine("Parsing failed:");
                response.AppendLine("Empty \"data\" parameter.");
            }

            Process(res);

            context.Response.ContentType = "text/plain";
            context.Response.Write(response);            
        }

        void s_UnknownElement(object sender, XmlElementEventArgs e)
        {
            throw new XmlException("Unknown element '" + e.Element.LocalName + "' occured. Line " + e.LineNumber + ", position " + e.LinePosition + ".");
        }

        void s_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            throw new XmlException("Unknown attribute '" + e.Attr.LocalName + "' occured. Line " + e.LineNumber + ", position " + e.LinePosition + ".");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

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
                    catch(Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                }
            }
            return response.ToString();
        }
    }
}
