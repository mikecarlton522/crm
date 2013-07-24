using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace Securetrialoffers.admin.EditForms
{
    public partial class Referer_ : System.Web.UI.Page
    {
        private RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out id))
            {
                Referer = (new BaseService()).Load<Referer>(id);
            }
            else
            {
                Referer = new Referer();
                Referer.CreateDT = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(Request["action"]))
            {
                Save();
            }
        }

        public Referer Referer { get; set; }

        private void Save()
        {
            if (Referer.RefererID != null)
            {
                BusinessError<Referer> updated = refererService.UpdateReferer(Referer.RefererID, Utility.TryGetStr(Request["firstname"]), Utility.TryGetStr(Request["lastname"]), Utility.TryGetStr(Request["company"]),
                    Utility.TryGetStr(Request["address1"]), Utility.TryGetStr(Request["address2"]), Utility.TryGetStr(Request["city"]), Utility.TryGetStr(Request["state"]), Utility.TryGetStr(Request["zip"]), Utility.TryGetStr(Request["country"]),
                    Utility.TryGetStr(Request["refererCode"]), Utility.TryGetInt(Request["parentReferer"]), Utility.TryGetStr(Request["username"]), Utility.TryGetStr(Request["password"]));
                if (updated.State == BusinessErrorState.Success)
                {
                    Referer = updated.ReturnValue;
                }
                else
                {
                    //Show error
                    //updated.ErrorMessage
                }
            }
            else
            {
                BusinessError<Referer> created = refererService.CreateReferer(Utility.TryGetStr(Request["firstname"]), Utility.TryGetStr(Request["lastname"]), Utility.TryGetStr(Request["company"]),
                    Utility.TryGetStr(Request["address1"]), Utility.TryGetStr(Request["address2"]), Utility.TryGetStr(Request["city"]), Utility.TryGetStr(Request["state"]), Utility.TryGetStr(Request["zip"]), Utility.TryGetStr(Request["country"]),
                    Utility.TryGetStr(Request["refererCode"]), Utility.TryGetInt(Request["parentReferer"]), Utility.TryGetStr(Request["username"]), Utility.TryGetStr(Request["password"]));
                if (created.State == BusinessErrorState.Success)
                {
                    Referer = created.ReturnValue;
                }
                else
                {
                    //Show error
                    //created.ErrorMessage
                }
            }
        }

        protected IList<KeyValuePair<string, string>> GetRefererList()
        {
            return refererService.GetRefererList().Select(item =>
                new KeyValuePair<string, string>(item.RefererID.Value.ToString(), string.Format("{0} {1}", item.FirstName, item.LastName)))
                .ToList();

        }
    }
}
