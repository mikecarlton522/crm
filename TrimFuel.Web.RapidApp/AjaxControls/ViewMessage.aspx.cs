using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ViewMessage : BaseControlPage
    {
        TPClientService tpSer = new TPClientService();

        protected string MessageType
        {
            get
            {
                return Request["type"].ToLower();
            }
        }

        protected int? ID
        {
            get
            {
                return Utility.TryGetInt(Request["ID"]);
            }
        }

        protected Dictionary<string, string> Rows = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            Rows = new Dictionary<string, string>();
            base.OnDataBinding(e);
            switch (MessageType)
            {
                case "mail":
                    var mail = tpSer.GetClientEmail(ID.Value);
                    Rows.Add("From", mail.From);
                    Rows.Add("To", mail.To);
                    Rows.Add("Subject", mail.Subject);
                    Rows.Add("Content", mail.Content);
                    Rows.Add("Create Date", mail.CreateDT.Value.ToShortDateString());
                    break;
                case "note":
                    var note = tpSer.GetClientNote(ID.Value);
                    Rows.Add("Content", note.Content);
                    Rows.Add("Create Date", note.CreateDT.Value.ToShortDateString());
                    break;
            }
        }
    }
}
