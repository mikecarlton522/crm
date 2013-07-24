using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class IPRestriction : System.Web.UI.Page
    {
        DashboardService ds = new DashboardService();

        public int? RestrictLevelID
        {
            get
            {
                return Utility.TryGetInt(Request["restrictLevelID"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            var restrictLevel = ds.GetRestrictLevelByID(RestrictLevelID);
            if (restrictLevel == null)
                return;

            chbAllowAll.Checked = restrictLevel.AllowAllIP ?? false;
            rIPList.DataSource = ds.GetIPRestrictListByID(RestrictLevelID);

            //var ipList = ds.GetIPRestrictListByID(RestrictLevelID);

            //StringBuilder sb = new StringBuilder();
            //foreach (var ipItem in ipList)
            //    sb.AppendLine(ipItem.IP);

            //txtIP.Text = sb.ToString();
        }

        protected void rIPList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                int? ipID = Convert.ToInt32(e.CommandArgument);
                ds.DeleteIPRestriction(ipID);
                SaveAllowAllIPs();
                Note.Text = "IP was successfuly removed";
                DataBind();
            }
        }

        protected void SaveAllowChanges_Click(object sender, EventArgs e)
        {
            SaveAllowAllIPs();
            Note.Text = "IP Restriction was successfuly updated";
            DataBind();
        }

        protected void AddNew_Click(object sender, EventArgs e)
        {
            var newIPRestrinction = new TrimFuel.Model.IPRestriction()
            {
                IP = txtNewIP.Text,
                RestrictLevelID = RestrictLevelID
            };
            ds.Save<TrimFuel.Model.IPRestriction>(newIPRestrinction);
            SaveAllowAllIPs();
            Note.Text = "IP Restriction was successfuly updated";
            DataBind();
        }

        protected void btnEditIP_Click(object source, EventArgs e)
        {
            var existing = ds.Load<TrimFuel.Model.IPRestriction>(Convert.ToInt32(Request["lblIPRestrictionID"]));
            if (existing != null)
            {
                existing.IP = Request["editTbIP"];
                ds.Save<TrimFuel.Model.IPRestriction>(existing);
            }
            SaveAllowAllIPs();
            Note.Text = "IP Restriction was successfuly updated";
            DataBind();
        }

        private void SaveAllowAllIPs()
        {
            var restrictLevel = ds.GetRestrictLevelByID(RestrictLevelID);
            if (restrictLevel != null)
            {
                restrictLevel.AllowAllIP = chbAllowAll.Checked;
                ds.Save<RestrictLevel>(restrictLevel);
            }
        }
    }
}