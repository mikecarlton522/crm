using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecigsbrand.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using System.Text.RegularExpressions;

namespace Ecigsbrand.Store1
{
    public partial class cp_lost_password : PageX
    {
        private RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
            HideErrors();
        }

        #region Validation

        private bool ValidateUsername()
        {
            if (Utility.TryGetStr(tbUsername.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbUsername.Text), "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$"))
            {
                Referer referer = refererService.GetByLogin(Utility.TryGetStr(tbUsername.Text));
                if (referer != null)
                {
                    return true;
                }
                else
                {
                    ShowUsernameError("No account found with that email address. Please try again.");
                }
            }
            else
            {
                ShowUsernameError("Email address is incorrect. Please try again.");
            }
            return false;
        }

        private bool ValidateInfo()
        {
            if (Utility.TryGetStr(tbFirstName.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbFirstName.Text), "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbLastName.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbLastName.Text), "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbCity.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbCity.Text), "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbZip.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbZip.Text), "^\\d{5}$"))
            {
                Referer referer = refererService.GetByRecoveryInfo(Utility.TryGetStr(tbUsername.Text),
                    Utility.TryGetStr(tbFirstName.Text), Utility.TryGetStr(tbLastName.Text), Utility.TryGetStr(tbCity.Text), Utility.TryGetStr(tbZip.Text));
                if (referer != null)
                {
                    return true;
                }
            }

            ShowInfoError("The answers are incorrect. Please try again.");
            return false;
        }

        private void HideErrors()
        {
            phUsernameError.Visible = false;
            phInfoError.Visible = false;
        }

        private void ShowUsernameError(string error)
        {
            lUsernameError.Text = error;
            phUsernameError.Visible = true;
        }

        private void ShowInfoError(string error)
        {
            lInfoError.Text = error;
            phInfoError.Visible = true;
        }

        #endregion

        public enum LostPasswordStep
        {
            Step1 = 1,
            Step2 = 2,
            Step3 = 3
        }

        public LostPasswordStep CurrentStep
        {
            get 
            {
                if (ViewState["CurrentStep"] == null)
                {
                    ViewState["CurrentStep"] = LostPasswordStep.Step1;
                }
                return (LostPasswordStep)ViewState["CurrentStep"]; 
            }
            set { ViewState["CurrentStep"] = value; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            CurrentStep = LostPasswordStep.Step1;
            UpdateCurrentView();
        }

        protected void btnNext1_Click(object sender, EventArgs e)
        {
            if (ValidateUsername())
            {
                CurrentStep = LostPasswordStep.Step2;
                UpdateCurrentView();
            }
        }

        protected void btnNext2_Click(object sender, EventArgs e)
        {
            if (ValidateInfo())
            {
                refererService.RecoverRefererPasswordEcigs(Utility.TryGetStr(tbUsername.Text));

                CurrentStep = LostPasswordStep.Step3;
                UpdateCurrentView();
            }
        }

        private void UpdateCurrentView()
        {
            mvSteps.ActiveViewIndex = (int)CurrentStep - 1;
        }
    }
}
