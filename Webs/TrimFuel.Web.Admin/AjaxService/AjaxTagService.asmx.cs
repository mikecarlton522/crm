using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Model;
using TrimFuel.Business;
using System.Web.UI;
using TrimFuel.Web.Admin.Controls;
using System.IO;

namespace TrimFuel.Web.Admin.AjaxService
{
    /// <summary>
    /// Summary description for AjaxTagService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class AjaxTagService : System.Web.Services.WebService
    {
        private TagService tagService = new TagService();

        private TControl LoadControl<TControl>(string path) where TControl : UserControl
        {
            UserControl u = new UserControl();
            return (TControl)u.LoadControl(path);
        }

        [WebMethod]
        public int SaveTag(int tagID, string tagValue)
        {
            Tag res = null;

            if (tagID != -1)
            {
                res = tagService.UpdateTag(tagID, tagValue);
            }
            else
            {
                res = tagService.AddTag(tagValue);
            }

            return (res != null) ? res.TagID.Value : -1;
        }

        [WebMethod]
        public bool RemoveTag(int tagID)
        {
            bool res = false;

            if (tagID != -1)
            {
                res = tagService.DeleteTag(tagID);
            }

            return res;
        }

        [WebMethod]
        public string LoadBillingTagLinks(long billingID)
        {
            TagList control = LoadControl<TagList>("Controls/TagList.ascx");
            control.BillingID = billingID;
            control.DataBind();

            StringWriter writer = new StringWriter();
            control.RenderControl(new Html32TextWriter(writer));

            return writer.ToString();
        }

        [WebMethod]
        public string LoadBillingTags(long billingID)
        {
            BillingTagList control = LoadControl<BillingTagList>("Controls/BillingTagList.ascx");
            control.BillingID = billingID;
            control.DataBind();

            StringWriter writer = new StringWriter();
            control.RenderControl(new Html32TextWriter(writer));

            return writer.ToString();
        }

        [WebMethod]
        public bool SaveBillingTags(long billingID, string tagIDList)
        {
            string[] tagIDarray = tagIDList.Split(',');
            IList<int> tagIDs = new List<int>();
            int tempInt = 0;
            foreach (string strTagID in tagIDarray)
            {
                if (int.TryParse(strTagID, out tempInt))
                {
                    tagIDs.Add(tempInt);
                }
            }
            tagService.SetTagListToBilling(billingID, tagIDs);
            return true;
        }
    }
}
