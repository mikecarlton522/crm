using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class DropDownUpsell : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                UpsellTypeID = Utility.TryGetInt(ddlUpsellTypes.SelectedValue);
                foreach (var item in ddlUpsellTypes.Items.Cast<ListItem>())
                {
                    if (Utility.TryGetInt(item.Value) != null && Utility.TryGetInt(item.Value) < 0 &&
                        item.Attributes.Count == 0)
                    {
                        item.Attributes.Add("disabled", "disabled");
                    }
                }
            }
        }

        public int? UpsellTypeID { get; set; }
        public int? ProductID { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            foreach (var c in Controls)
            {
                if (c is DropDownList)
                {
                    DropDownList ddl = (DropDownList)c;
                    if (ddl.ID != ddlUpsellTypes.ID && ddl.Visible)
                    {
                        //Copy elements
                        ddlUpsellTypes.Items.Clear();
                        foreach (var item in ddl.Items.Cast<ListItem>())
                        {
                            var newItem = new ListItem()
                            {
                                Enabled = item.Enabled,
                                Text = item.Text,
                                Value = item.Value
                            };
                            if (Utility.TryGetInt(item.Value) != null && Utility.TryGetInt(item.Value)  < 0)
                            {
                                newItem.Attributes.Add("disabled", "disabled");
                            }
                            ddlUpsellTypes.Items.Add(newItem);
                        }
                    }
                }
            }

            try
            {
                ddlUpsellTypes.SelectedValue = (UpsellTypeID != null ? UpsellTypeID.ToString() : "");
            }
            catch { }
        }
    }
}