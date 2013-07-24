using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrimFuel.Web.UI
{
    [ToolboxData("<{0}:If Condition=\"true\" runat=\"server\"></{0}:If>")]
    public class If : PlaceHolder
    {
        private bool condition = false;
        public bool Condition
        {
            get
            {
                if (ViewState != null)
                {
                    if (ViewState["Condition"] == null)
                        ViewState["Condition"] = false;

                    condition = (bool)ViewState["Condition"];
                }
                return condition;
            }
            set
            {
                if (ViewState != null)
                {
                    ViewState["Condition"] = value;
                }
                condition = value;
            }
        }

        protected override void DataBindChildren()
        {
            if (Condition)
                base.DataBindChildren();
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            if (Condition)
                base.RenderChildren(writer);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (!Condition)
            {
                Controls.Clear();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Condition)
            {
                Controls.Clear();
            }
        }
    }
}
