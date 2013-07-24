using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace Securetrialoffers.ec07
{
    public partial class recommended : System.Web.UI.Page
    {
        public enum Action
        {
            Index,
            Upsell
        }

        protected Action currentAction = Action.Index;

        private const int CAMPAIGN_ID = 229;
        private const string ACTION_UPSELL = "upsell";

        protected int? campaignID = null;

        protected string affiliateCode = null;
        protected string subAffiliateCode = null;
        protected string clickID = null;

        protected long? billingID = null;
        protected string action = null;
        protected string upsellItems = null;

        private void LoadInput()
        {
            campaignID = CAMPAIGN_ID;

            affiliateCode = Utility.TryGetStr(Request.Params["aff"]);
            subAffiliateCode = Utility.TryGetStr(Request.Params["sub"]);
            clickID = Utility.TryGetStr(Request.Params["cid"]);

            billingID = Utility.TryGetInt(Request.Params["bid"]);
            action = Utility.TryGetStr(Request.Params["_action"]);
            upsellItems = Utility.TryGetStr(Request.Params["upsell"]);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            LoadInput();

            currentAction = DetermineAction();
            ProcessAction(currentAction);
        }

        private Action DetermineAction()
        {
            if (action == ACTION_UPSELL)
            {
                return Action.Upsell;
            }
            return Action.Index;
        }

        private void ProcessAction(Action action)
        {
            switch (action)
            {
                case Action.Upsell:
                    Upsell();
                    break;
                default:
                    Index();
                    break;
            }
        }

        private void Index()
        {
        }

        private void Upsell()
        {
            if (!string.IsNullOrEmpty(upsellItems))
            {
                foreach (string item in upsellItems.Split(','))
                {
                    int? upsellTypeID = Utility.TryGetInt(item);
                    if (upsellTypeID != null)
                    {
                    }
                }
            }
        }
    }
}
