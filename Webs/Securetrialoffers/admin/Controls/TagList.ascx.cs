using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;

namespace Securetrialoffers.admin.Controls
{
    public partial class TagList : System.Web.UI.UserControl
    {
        private TagService tagService = new TagService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public long? BillingID { get; set; }
        public int? TagGroupID { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rTag.DataSource = tagService.GetTagList();
            if (BillingID != null)
            {
                GroupList = tagService.GetTagListByBilling(BillingID.Value);
            }
            else if (TagGroupID != null)
            {
                GroupList = tagService.GetTagListByTagGroup(TagGroupID.Value);
            }
        }

        private IList<Tag> groupList = new List<Tag>();
        protected IList<Tag> GroupList
        {
            get { return groupList; }
            set { groupList = value; }
        }

        protected bool IsInGroup(Tag tag)
        {
            return (GroupList.Where(item => item.TagID == tag.TagID).Count() > 0);
        }

        public bool IsGrouping 
        {
            get { return (BillingID != null || TagGroupID != null); }
        }
    }
}