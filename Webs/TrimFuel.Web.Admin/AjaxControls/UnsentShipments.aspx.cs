using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class UnsentShipments : System.Web.UI.Page
    {
        ReportService reportService = new ReportService();
        Dictionary<int?, List<UnsentUnpayedView>> shipers = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            phMessage1.Visible = false;
            phMessage2.Visible = false;
            phMessage3.Visible = false;
            phMessage4.Visible = false;

            if (!IsPostBack)
            {
                GenerateShipmentsReport();
                sendShippments.Enabled = false;
            }
        }

        private void GenerateShipmentsReport()
        {
            var shippments = reportService.GetUnsentShippmentsFromAggTable().OrderByDescending(u => u.CreateDT).ToList();
            shipers = new Dictionary<int?, List<UnsentUnpayedView>>();
            foreach (var sh in shippments)
            {
                if (shipers.ContainsKey(sh.GroupID))
                    shipers[sh.GroupID].Add(sh);
                else
                {
                    shipers.Add(sh.GroupID, new List<UnsentUnpayedView>());
                    shipers[sh.GroupID].Add(sh);
                }
            }

            rShipers.DataSource = shipers;
            DataBind();
        }

        protected void btnResendShippments_Click(object sender, EventArgs e)
        {
            bool noErrors = true;
            GeneralShipperService ser = new GeneralShipperService();
            var saleIDList = string.IsNullOrEmpty(Request["shippmentToSend"]) ? new string[0] : Request["shippmentToSend"].Split(',');
            foreach (var strSaleID in saleIDList)
            {
                int saleID = int.Parse(strSaleID);
                var unsentShipment = ser.Load<AggUnsentShipments>(saleID);
                if (unsentShipment != null)
                {
                    var res = ser.SendSale(saleID, unsentShipment.ShipperID);
                    if (res == null || res.State == BusinessErrorState.Error)
                        noErrors = false;
                }
            }
            GenerateShipmentsReport();
            if (noErrors)
                phMessage1.Visible = true;
            else
                phMessage2.Visible = true;
        }

        protected string GetShipperNameById(string shipperId)
        {
            string res = string.Empty;
            res = ShipperEnum.Shippers[Utility.TryGetInt(shipperId)];
            res += string.Format(" ({0})", shipers[Utility.TryGetInt(shipperId)].Count);
            return res;
        }

        protected void btnSendShippmentsToDifferentShipper_Click(object sender, EventArgs e)
        {
            var shipperID = Utility.TryGetInt(ShipperDDL.SelectedValue);
            if (shipperID == null)
                return;

            bool noErrors = true;
            GeneralShipperService ser = new GeneralShipperService();
            var saleIDList = string.IsNullOrEmpty(Request["shippmentToSend"]) ? new string[0] : Request["shippmentToSend"].Split(',');
            foreach (var strSaleID in saleIDList)
            {
                int saleID = int.Parse(strSaleID);
                var res = ser.SendSale(saleID, shipperID);
                if (res == null || res.State == BusinessErrorState.Error)
                    noErrors = false;
            }
            GenerateShipmentsReport();
            if (noErrors)
                phMessage1.Visible = true;
            else
                phMessage2.Visible = true;
        }

        protected void btnMarkAsSent_Click(object sender, EventArgs e)
        {
            bool noErrors = true;
            SaleService ser = new SaleService();
            var saleIDList = string.IsNullOrEmpty(Request["shippmentToSend"]) ? new string[0] : Request["shippmentToSend"].Split(',');

            foreach (var strSaleID in saleIDList)
            {
                int saleID = int.Parse(strSaleID);
                var res = ser.BlockShipment(saleID);
                if (res == false)
                    noErrors = false;
            }

            GenerateShipmentsReport();
            if (noErrors)
                phMessage3.Visible = true;
            else
                phMessage4.Visible = true;
        }

        protected void btnMarkAsSentWithNote_Click(object sender, EventArgs e)
        {
            bool noErrors = true;
            GeneralShipperService ser = new GeneralShipperService();
            var saleIDList = string.IsNullOrEmpty(Request["shippmentToSend"]) ? new string[0] : Request["shippmentToSend"].Split(',');
            foreach (var strSaleID in saleIDList)
            {
                int saleID = int.Parse(strSaleID);
                var shipperID = ser.GetShipperByUnsentSaleID(saleID);
                if (shipperID.HasValue)
                {
                    var res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error.");

                    switch(shipperID.Value)
                    {
                        case ShipperEnum.ABF:
                            res = ser.MarkAsSent<ABFRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        case ShipperEnum.AtLastFulfillment:
                            res = ser.MarkAsSent<ABFRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        case ShipperEnum.CustomShipper:
                            res = ser.MarkAsSent<CustomShipperRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        case ShipperEnum.GoFulfillment:
                            res = ser.MarkAsSent<GFRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        case ShipperEnum.Keymail:
                            res = ser.MarkAsSent<KeymailRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        case ShipperEnum.MB:
                            res = ser.MarkAsSent<MBRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        case ShipperEnum.NPF:
                            res = ser.MarkAsSent<NPFRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        case ShipperEnum.TF:
                            res = ser.MarkAsSent<TFRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                        default:
                            res = ser.MarkAsSent<CustomShipperRecord>(Request.Cookies["admName4Net"].Value, saleID);
                            break;
                    }

                    
                    if (res == null || res.State == BusinessErrorState.Error)
                        noErrors = false;
                }
                else
                    noErrors = false;
            }
            GenerateShipmentsReport();
            if (noErrors)
                phMessage5.Visible = true;
            else
                phMessage4.Visible = true;
        }
    }
}