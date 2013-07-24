using System;

using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.EditForms
{
    public partial class ErrorsLog_ : System.Web.UI.Page
    {
        private ErrorsLogService _els = new ErrorsLogService();

        public ErrorsLog ErrorsLog { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out id))
            {
                ErrorsLog = _els.Load<ErrorsLog>(id);

                if (ErrorsLog == null)
                {
                    Response.Clear();
                    Response.End();
                }
            }
            else
            {
                Response.Clear();
                Response.End();
            }
        }
    }
}