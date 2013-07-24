using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace TrimFuel.Web.RapidApp.Logic
{
    public abstract class PageX : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public abstract string HeaderString { get; }
    }
}
