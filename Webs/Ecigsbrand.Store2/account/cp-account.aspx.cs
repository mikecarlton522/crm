using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecigsbrand.Store2.Logic;
using TrimFuel.Business.Utils;
using System.Text.RegularExpressions;
using TrimFuel.Business;
using TrimFuel.Model;

namespace Ecigsbrand.Store2.account
{
    public partial class cp_account : AccountPageX
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
            HideErrors();
        }

        #region Validation

        private bool ValidateLogin()
        {
            if (Utility.TryGetStr(tbLoginPassword.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbLoginPassword.Text), "^[a-zA-Z0-9]+$") &&
                Utility.TryGetStr(tbUsername.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbUsername.Text), "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$") &&
                Utility.TryGetStr(tbUsername.Text) == Utility.TryGetStr(tbUsernameConfirm.Text))
            {
                if (Membership.ValidateReferer(Membership.CurrentReferer.Username, Utility.TryGetStr(tbLoginPassword.Text)) != null)
                {
                    return true;
                }
                else
                {
                    ShowLoginError("Invalid password.");
                    return false;
                }                
            }
            ShowLoginError("Please verify your login information and try again.");
            return false;
        }

        private bool ValidatePassword()
        {
            if (Utility.TryGetStr(tbPasswordPassword.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbPasswordPassword.Text), "^[a-zA-Z0-9]+$") &&
                Utility.TryGetStr(tbPassword.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbPassword.Text), "^[a-zA-Z0-9]+$") &&
                Utility.TryGetStr(tbPassword.Text) == Utility.TryGetStr(tbPasswordConfirm.Text))
            {
                if (Membership.ValidateReferer(Membership.CurrentReferer.Username, Utility.TryGetStr(tbPasswordPassword.Text)) != null)
                {
                    return true;
                }
                else
                {
                    ShowPasswordError("Invalid password.");
                    return false;
                }
            }
            ShowPasswordError("Please verify your password try again.");
            return false;
        }

        private bool ValidateRegistration()
        {
            if (Utility.TryGetStr(tbRegistrationPassword.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbRegistrationPassword.Text), "^[a-zA-Z0-9]+$") &&
                Utility.TryGetStr(tbFirstName.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbFirstName.Text), "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbLastName.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbLastName.Text), "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbAddress1.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbAddress1.Text), "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Utility.TryGetStr(tbCity.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbCity.Text), "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                (ddlCountry.SelectedValue != RegistrationService.DEFAULT_COUNTRY || Utility.TryGetStr(tbZip.Text) != null && Regex.IsMatch(Utility.TryGetStr(tbZip.Text), "^\\d{5}$")))
            {
                if (Membership.ValidateReferer(Membership.CurrentReferer.Username, Utility.TryGetStr(tbRegistrationPassword.Text)) != null)
                {
                    return true;
                }
                else
                {
                    ShowRegistrationError("Invalid password.");
                    return false;
                }
            }
            ShowRegistrationError("Please verify your shipping address information and try again.");
            return false;
        }

        private void HideErrors()
        {
            phLoginError.Visible = false;
            phPasswordError.Visible = false;
            phRegistrationError.Visible = false;
            lLoginErrorClass.Text = "error";
            lPasswordErrorClass.Text = "error";
            lRegistrationErrorClass.Text = "error";
        }

        private void ShowLoginError(string error)
        {
            lLoginError.Text = error;
            phLoginError.Visible = true;
        }

        private void ShowPasswordError(string error)
        {
            lPasswordError.Text = error;
            phPasswordError.Visible = true;
        }

        private void ShowRegistrationError(string error)
        {
            lRegistrationError.Text = error;
            phRegistrationError.Visible = true;
        }

        private void ShowLoginSuccess()
        {
            ShowLoginError("Email (login) was successfuly updated.");
            lLoginErrorClass.Text = "success";
        }

        private void ShowPasswordSuccess()
        {
            ShowPasswordError("Password was successfuly updated.");
            lPasswordErrorClass.Text = "success";
        }

        private void ShowRegistrationSuccess()
        {
            ShowRegistrationError("Shipping address was successfuly updated.");
            lRegistrationErrorClass.Text = "success";
        }

        #endregion

        protected void bChangeLogin_Click(object sender, EventArgs e)
        {
            if (ValidateLogin())
            {
                BusinessError<Referer> referer = refererService.UpdateReferer(Referer.RefererID, Referer.FirstName, Referer.LastName, Referer.Company,
                    Referer.Address1, Referer.Address2, Referer.City, Referer.State, Referer.Zip, Referer.Country, 
                    Referer.RefererCode, Referer.ParentRefererID, Utility.TryGetStr(tbUsername.Text), Referer.Password);
                if (referer.State == BusinessErrorState.Success)
                {
                    Membership.LoginReferer(referer.ReturnValue);
                    ShowLoginSuccess();
                }
                else
                {
                    ShowLoginError(referer.ErrorMessage);
                }
            }
        }

        protected void bChangePassword_Click(object sender, EventArgs e)
        {
            if (ValidatePassword())
            {
                BusinessError<Referer> referer = refererService.UpdateReferer(Referer.RefererID, Referer.FirstName, Referer.LastName, Referer.Company,
                    Referer.Address1, Referer.Address2, Referer.City, Referer.State, Referer.Zip, Referer.Country,
                    Referer.RefererCode, Referer.ParentRefererID, Referer.Username, Utility.TryGetStr(tbPassword.Text));
                if (referer.State == BusinessErrorState.Success)
                {
                    ShowPasswordSuccess();
                }
                else
                {
                    ShowPasswordError(referer.ErrorMessage);
                }
            }
        }

        protected void bRegister_Click(object sender, EventArgs e)
        {
            if (ValidateRegistration())
            {
                BusinessError<Referer> referer = null;
                if (ddlCountry.SelectedValue == RegistrationService.DEFAULT_COUNTRY)
                {
                    referer = refererService.UpdateReferer(Referer.RefererID, Utility.TryGetStr(tbFirstName.Text), Utility.TryGetStr(tbLastName.Text), Referer.Company,
                        Utility.TryGetStr(tbAddress1.Text), Utility.TryGetStr(tbAddress2.Text), Utility.TryGetStr(tbCity.Text), Utility.TryGetStr(ddlState.SelectedValue), Utility.TryGetStr(tbZip.Text), Utility.TryGetStr(ddlCountry.SelectedValue),
                        Referer.RefererCode, Referer.ParentRefererID, Referer.Username, Referer.Password);
                }
                else
                {
                    referer = refererService.UpdateReferer(Referer.RefererID, Utility.TryGetStr(tbFirstName.Text), Utility.TryGetStr(tbLastName.Text), Referer.Company,
                        Utility.TryGetStr(tbAddress1.Text), Utility.TryGetStr(tbAddress2.Text), Utility.TryGetStr(tbCity.Text), Utility.TryGetStr(tbStateEx.Text), Utility.TryGetStr(tbZipEx.Text), Utility.TryGetStr(ddlCountry.SelectedValue),
                        Referer.RefererCode, Referer.ParentRefererID, Referer.Username, Referer.Password);
                }
                if (referer.State == BusinessErrorState.Success)
                {
                    ShowRegistrationSuccess();
                }
                else
                {
                    ShowRegistrationError(referer.ErrorMessage);
                }
            }
        }
    }
}
