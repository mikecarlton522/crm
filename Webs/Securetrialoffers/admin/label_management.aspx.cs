using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;

namespace Securetrialoffers.admin
{
    public partial class label_management : System.Web.UI.Page
    {
        private TagService tagService = new TagService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {            
            base.OnDataBinding(e);

            //rTag.DataSource = tagService.GetTagList();
            //rGroup.DataSource = tagService.GetTagGroupList();
        }
    }
}
