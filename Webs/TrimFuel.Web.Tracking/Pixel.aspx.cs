using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;

namespace TrimFuel.Web.Tracking
{
    public partial class Pixel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var conversionService = new ConversionService();
            try
            {
                InitFileds();
                if(!CheckQueryString())
                {
                    throw new Exception("Incorrect input parameters");
                }

                Conversion conversion = conversionService.GetConversionByFilter(
                Convert.ToInt32(Request.QueryString["CID"]), Convert.ToInt32(Request.QueryString["PID"]), Request.QueryString["Aff"], Request.QueryString["Sub"]);
                var time = DateTime.Now;
                if (conversion == null)
                {
                    conversion = new Conversion();
                    conversion.CampaignID = Convert.ToInt32(Request.QueryString["CID"]);
                    conversion.PageTypeID = Convert.ToInt32(Request.QueryString["PID"]);
                    conversion.Affiliate = Request.QueryString["Aff"];
                    conversion.SubAffiliate = Request.QueryString["Sub"];
                    conversion.Hits = 1;
                    conversion.CreateDT = time;
                    conversion.Hour = Convert.ToByte(time.Hour);
                }
                else
                    conversion.Hits++;

                conversionService.SetConversion(conversion);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            
            //conversionService.SetConversions(conversions, Convert.ToInt32(Request.QueryString["CID"]),
            //    Convert.ToInt32(Request.QueryString["PID"]),Request.QueryString["Aff"], Request.QueryString["Sub"]);
        }

        protected void InitFileds()
        {
            CampaignID.Text = Request.QueryString["CID"];
            PageTypeID.Text = Request.QueryString["PID"];
            Affiliate.Text = Request.QueryString["Aff"];
            SubAffiliate.Text = Request.QueryString["Sub"];
        }

        protected bool CheckQueryString()
        {
            try
            {
                var cid = Convert.ToInt32(Request.QueryString["CID"]);
                var pid = Convert.ToInt32(Request.QueryString["PID"]);
               
                if(string.IsNullOrEmpty(Request.QueryString["Aff"]) || 
                    string.IsNullOrEmpty(Request.QueryString["Sub"]) || 
                    string.IsNullOrEmpty(Request.QueryString["PID"]) || 
                    string.IsNullOrEmpty(Request.QueryString["CID"]) )
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }
    }
}