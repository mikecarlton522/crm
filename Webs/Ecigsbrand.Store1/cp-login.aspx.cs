using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecigsbrand.Store1.Logic;
using TrimFuel.Business.Utils;
using System.Text.RegularExpressions;
using TrimFuel.Business;
using TrimFuel.Model;

namespace Ecigsbrand.Store1
{
    public partial class cp_login : PageX
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
            HideErrors();
        }

        #region Validation

        private bool ValidateLogin()
        {
            if (Utility.TryGetStr(tbLoginUsername.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbLoginUsername.Text), "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$") &&
                Utility.TryGetStr(tbLoginPassword.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbLoginPassword.Text), "^[a-zA-Z0-9]+$"))
            {
                return true;
            }
            ShowLoginError("Please verify your login information and try again.");
            return false;
        }

        private bool ValidateRegistration()
        {
            if (Utility.TryGetStr(tbUsername.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbUsername.Text), "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$") &&
                Utility.TryGetStr(tbPassword.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbPassword.Text), "^[a-zA-Z0-9]+$") &&
                Utility.TryGetStr(tbPassword.Text) == Utility.TryGetStr(tbPasswordConfirm.Text) &&
                Utility.TryGetStr(tbFirstName.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbFirstName.Text), "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbLastName.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbLastName.Text), "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbAddress1.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbAddress1.Text), "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbCity.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbCity.Text), "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                (ddlCountry.SelectedValue != RegistrationService.DEFAULT_COUNTRY || Utility.TryGetStr(tbZip.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbZip.Text), "^\\d{5}$")) &&
                Utility.TryGetStr(tbRefererCode.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbRefererCode.Text), "^[a-zA-Z0-9]+$"))
            {
                return true;
            }
            ShowRegistrationError("Please verify your registration information and try again.");
            return false;
        }

        private void HideErrors()
        {
            phLoginError.Visible = false;
            phRegistrationError.Visible = false;
        }

        private void ShowLoginError(string error)
        {
            lLoginError.Text = error;
            phLoginError.Visible = true;
        }

        private void ShowRegistrationError(string error)
        {
            lRegistrationError.Text = error;
            phRegistrationError.Visible = true;
        }

        #endregion

        protected void bLogin_Click(object sender, EventArgs e)
        {
            if (ValidateLogin())
            {
                Referer referer = Membership.ValidateReferer(Utility.TryGetStr(tbLoginUsername.Text), Utility.TryGetStr(tbLoginPassword.Text));
                if (referer != null)
                {
                    Membership.LoginReferer(referer);
                    Response.Redirect("~/account/cp-home.aspx");
                }
                else
                {
                    ShowLoginError("Invalid login or password.");
                }
            }
        }

        protected void bRegister_Click(object sender, EventArgs e)
        {
            if (ValidateRegistration())
            {
                BusinessError<Referer> refererRes = null;
                if (ddlCountry.SelectedValue == RegistrationService.DEFAULT_COUNTRY)
                {
                    refererRes = refererService.CreateReferer(Utility.TryGetStr(tbFirstName.Text), Utility.TryGetStr(tbLastName.Text), null,
                        Utility.TryGetStr(tbAddress1.Text), Utility.TryGetStr(tbAddress2.Text), Utility.TryGetStr(tbCity.Text), Utility.TryGetStr(ddlState.SelectedValue), Utility.TryGetStr(tbZip.Text), Utility.TryGetStr(ddlCountry.SelectedValue),
                        Utility.TryGetStr(tbRefererCode.Text), Utility.TryGetStr(tbParentRefererCode.Text),
                        Utility.TryGetStr(tbUsername.Text), Utility.TryGetStr(tbPassword.Text));
                }
                else
                {
                    refererRes = refererService.CreateReferer(Utility.TryGetStr(tbFirstName.Text), Utility.TryGetStr(tbLastName.Text), null,
                        Utility.TryGetStr(tbAddress1.Text), Utility.TryGetStr(tbAddress2.Text), Utility.TryGetStr(tbCity.Text), Utility.TryGetStr(tbStateEx.Text), Utility.TryGetStr(tbZipEx.Text), Utility.TryGetStr(ddlCountry.SelectedValue),
                        Utility.TryGetStr(tbRefererCode.Text), Utility.TryGetStr(tbParentRefererCode.Text),
                        Utility.TryGetStr(tbUsername.Text), Utility.TryGetStr(tbPassword.Text));
                }
                if (refererRes.State == BusinessErrorState.Success)
                {
                    Membership.LoginReferer(refererRes.ReturnValue);
                    Response.Redirect("~/account/cp-home.aspx");
                }
                else
                {
                    ShowRegistrationError(refererRes.ErrorMessage);
                }
            }
        }
    }
}
