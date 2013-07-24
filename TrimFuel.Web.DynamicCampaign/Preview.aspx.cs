using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.DynamicCampaign.Logic;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.DynamicCampaign
{
    public partial class Preview : BaseDynamicPage
    {
        protected override void OnInit(EventArgs e)
        {
            string[] arr = Request.Url.Host.Split(".".ToCharArray());

            base.PageTypeID = (Utility.TryGetInt(Request["pageTypeID"]) ?? new int?(TrimFuel.Model.Enums.PageTypeEnum.Landing)).Value;

            base.OnInit(e);

            Master.Head = base.CampaignPage.Header;

            Master.Title = base.CampaignPage.Title;

            Master.Pixel = base.GetPixels();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _html.Text = base.HTML;
        }
    }
}
