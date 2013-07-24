using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class Paging : System.Web.UI.UserControl
    {
        public event EventHandler<EventArgs> GoToPageClicked;
        protected void OnGoToPageClicked()
        {
            if (GoToPageClicked != null)
            {
                GoToPageClicked(this, EventArgs.Empty);
            }
        }

        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            OnGoToPageClicked();
        }

        public int TotalNumberOfRecords
        {
            get;
            set;
        }

        int currentPage = -1;
        public int CurrentPage
        {
            get
            {
                if (currentPage == -1)
                    return Utility.TryGetInt(Request["hdnCurrentPage"]) ?? 1;
                else
                    return currentPage;
            }
            set
            {
                currentPage = value;
            }
        }

        public int CountOnPage
        {
            get;
            set;
        }

        protected Dictionary<int, bool> Pages
        {
            get
            {
                var pagesCount = Math.Ceiling((double)TotalNumberOfRecords / CountOnPage);
                var res = new Dictionary<int, bool>();
                for (int i = 1; i <= pagesCount; i++)
                    res.Add(i, i != CurrentPage);
                return res;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (Pages.Count > 1)
                rPaging.Visible = true;
            else
                rPaging.Visible = false;
        }
    }
}