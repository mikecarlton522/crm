using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using System.Text.RegularExpressions;
using System.Web.UI;
using Ecigsbrand.Store1.Controls;
using System.IO;
using BeautyTruth.Store1.Logic;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;

namespace BeautyTruth.Store1.service
{
    /// <summary>
    /// Summary description for login
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class login : System.Web.Services.WebService
    {
        [WebMethod]
        public BusinessError<Set<Billing, Registration, RegistrationInfo>> Login(string username, string password)
        {
            //Shows standard message on error.
            BusinessError<Set<Billing, Registration, RegistrationInfo>> res = new BusinessError<Set<Billing, Registration, RegistrationInfo>>(null, BusinessErrorState.Error, "Invalid login or password.");
            if (ValidateLogin(username, password))
            {
                Referer referer = Membership.ValidateReferer(Utility.TryGetStr(username), Utility.TryGetStr(password));
                if (referer != null)
                {
                    res.ReturnValue = new Set<Billing, Registration, RegistrationInfo>();
                    res.State = BusinessErrorState.Success;
                    Membership.LoginReferer(referer);
                    res.ReturnValue.Value1 = GetBilling(referer);
                    res.ReturnValue.Value2 = GetRegistration(res.ReturnValue.Value1.RegistrationID);
                    res.ReturnValue.Value3 = GetRegistrationInfo(res.ReturnValue.Value1.RegistrationID);
                }
            }
            return res;
        }

        [WebMethod]
        public string GetLoginToken()
        {
            LoginToken control = LoadControl<LoginToken>("~/Controls/LoginToken.ascx");
            control.DataBind();

            StringWriter writer = new StringWriter();
            control.RenderControl(new Html32TextWriter(writer));

            return writer.ToString();
        }

        private bool ValidateLogin(string username, string password)
        {
            if (Utility.TryGetStr(username) != null && Regex.IsMatch(Utility.TryGetStr(username), "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$") &&
                Utility.TryGetStr(password) != null && Regex.IsMatch(Utility.TryGetStr(password), "^[a-zA-Z0-9]+$"))
            {
                return true;
            }
            return false;
        }

        private Billing GetBilling(Referer referer)
        {
            RefererService rs = new RefererService();
            IList<Billing> refererBillings = rs.GetBillingByReferer(referer.RefererID.Value);
            Billing refererLastBilling = null;
            if (refererBillings != null)
            {
                refererLastBilling = refererBillings.LastOrDefault();
            }

            Billing res = new Billing();
            if (refererLastBilling != null)
            {
                res.FirstName = refererLastBilling.FirstName;
                res.LastName = refererLastBilling.LastName;
                res.Address1 = refererLastBilling.Address1;
                res.Address2 = refererLastBilling.Address2;
                res.City = refererLastBilling.City;
                res.Country = refererLastBilling.Country;
                res.State = refererLastBilling.State;
                res.Zip = refererLastBilling.Zip;
                res.Phone = refererLastBilling.Phone;
                res.Email = refererLastBilling.Email;
                res.BillingID = refererLastBilling.BillingID;
                res.RegistrationID = refererLastBilling.RegistrationID;
            }

            return res;
        }

        private Registration GetRegistration(long? registrationID)
        {
            if (registrationID == null)
                return null;

            Registration res = new Registration();
            RefererService rs = new RefererService();
            res = rs.Load<Registration>(registrationID);

            return res;
        }

        private RegistrationInfo GetRegistrationInfo(long? registrationID)
        {
            if (registrationID == null)
                return null;

            var res = new RegistrationService().GetRegistrationInfo(registrationID.Value);
            return res;
        }

        private TControl LoadControl<TControl>(string path) where TControl : UserControl
        {
            UserControl u = new UserControl();
            return (TControl)u.LoadControl(path);
        }
    }
}
