using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;

using MySql.Data.MySqlClient;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using System.Configuration;
using System.Web.Services;
using System.Net;
using System.IO;


namespace TrimFuel.Web.DynamicCampaign.Logic
{
    public class BaseDynamicPage : System.Web.UI.Page
    {
        protected const string US_COUNTRY = "US";
        protected const string UK_COUNTRY = "United Kingdom";
        protected const string CANADA_COUNTRY = "Canada";
        protected const string AUSTRALIA_COUNTRY = "Australia";
        protected const string ARGENTINA_COUNTRY = "Argentina";
        protected const string BRASIL_COUNTRY = "BR";
        protected const string OTHER_COUNTRY = "Other";

        private Dictionary<string, ControlNames> _billingStates = new Dictionary<string, ControlNames>() 
        { 
            {UK_COUNTRY, ControlNames.Billing_State_UK}, 
            {US_COUNTRY, ControlNames.Billing_State_US}, 
            {OTHER_COUNTRY, ControlNames.Billing_State_Other}, 
            {CANADA_COUNTRY, ControlNames.Billing_State_Canada },
            {AUSTRALIA_COUNTRY, ControlNames.Billing_State_Australia},
            {ARGENTINA_COUNTRY, ControlNames.Billing_State_Argentina},
            {BRASIL_COUNTRY, ControlNames.Billing_State_Brasil}
        };
        private Dictionary<string, ControlNames> _shippingStates = new Dictionary<string, ControlNames>() 
        { 
            {UK_COUNTRY,ControlNames.Shipping_State_UK},
            {US_COUNTRY, ControlNames.Shipping_State_US},
            {OTHER_COUNTRY, ControlNames.Shipping_State_Other}, 
            {CANADA_COUNTRY, ControlNames.Shipping_State_Canada},
            {AUSTRALIA_COUNTRY, ControlNames.Shipping_State_Australia},
            {ARGENTINA_COUNTRY, ControlNames.Shipping_State_Argentina},
            {BRASIL_COUNTRY, ControlNames.Shipping_State_Brasil}
        };

        #region Fields

        private IDao _dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

        private CampaignService _campaignService = new CampaignService();

        private SubscriptionService _subscriptionService = new SubscriptionService();

        private Campaign _campaign;

        private static int _campaignID = int.TryParse(HttpContext.Current.Request["campaignID"], out _campaignID) ? _campaignID : 0;

        private CampaignPage _campaignPage;

        private string _html;

        private int _pageTypeID = 0;

        private string _affiliate = HttpContext.Current.Request["aff"];

        private string _subAffiliate = HttpContext.Current.Request["sub"];

        private string _cid = HttpContext.Current.Request["cid"];

        private NameValueCollection _pageValues = new NameValueCollection(HttpContext.Current.Request.Form);

        private PageActions _pageAction;

        #endregion

        #region Properties

        protected IDao Dao
        {
            get
            {
                return _dao;
            }
        }

        protected int CampaignID
        {
            get
            {
                return _campaignID;
            }
        }

        protected Campaign Campaign
        {
            get
            {
                return _campaign;
            }
        }

        protected CampaignPage CampaignPage
        {
            get
            {
                return _campaignPage;
            }
        }

        protected string HTML
        {
            get
            {
                return _html;
            }
        }

        protected int PageTypeID
        {
            set
            {
                _pageTypeID = value;
            }
        }

        protected string Affiliate
        {
            get
            {
                return _affiliate;
            }
        }

        protected string SubAffiliate
        {
            get
            {
                return _subAffiliate;
            }
        }

        protected string CID
        {
            get
            {
                return _cid;
            }
        }

        protected NameValueCollection PageValues
        {
            get
            {
                return _pageValues;
            }
        }

        protected PageActions PageAction
        {
            set
            {
                _pageAction = value;
            }
        }

        protected int? SubscriptionID
        {
            get
            {
                return string.IsNullOrEmpty(Request["SubscriptionID"]) ? _campaign.SubscriptionID : Utility.TryGetInt(Request["SubscriptionID"]);
            }
        }

        protected bool UseHTTPS
        {
            get
            {
                return ConfigurationManager.AppSettings["UseHTTPS"] == "true";
            }
        }

        #endregion

        protected string ReplacePixelSavePrice(string pixelCode)
        {
            if (_campaign == null)
                return pixelCode;

            if (pixelCode.IndexOf("##SAVE_PRICE##") > -1)
            {
                Subscription subscription = _subscriptionService.Load<Subscription>(SubscriptionID.GetValueOrDefault(0));

                if (subscription != null)
                {
                    decimal? shipAmount = subscription.InitialShipping;

                    if (shipAmount.HasValue)
                        pixelCode.Replace("##SAVE_PRICE##", shipAmount.Value.ToString("0.##"));

                    else
                        pixelCode = pixelCode.Replace("##SAVE_PRICE##", string.Empty);
                }
            }

            return pixelCode;
        }

        protected string ReplaceLinkVariables(string code)
        {
            if (code.IndexOf("##LINK##") > -1)
            {
                string url = Request.Url.ToString();

                url = url.Replace(Request.Url.Query, "");

                url = url.ToLower().Replace("http://", "");

                url = url.ToLower().Replace("https://", "");

                if (!url.StartsWith("www."))
                    url = "www." + url;

                string affiliate = string.IsNullOrEmpty(_affiliate) ? string.Empty : "&aff=" + _affiliate;

                string subAffiliate = string.IsNullOrEmpty(_subAffiliate) ? string.Empty : "&sub=" + _subAffiliate;

                string cid = string.IsNullOrEmpty(_cid) ? string.Empty : "&cid=" + _cid;

                url = Uri.EscapeDataString(
                    string.Format("{0}?campaignid={1}{2}{3}{4}", url, _campaignID, affiliate, subAffiliate, cid));

                code = code.Replace("##LINK##", url);
            }

            if (code.IndexOf("##AFFILIATE##") > 0)
                code = code.Replace("##AFFILIATE##", !string.IsNullOrEmpty(_affiliate) ? _affiliate : string.Empty);

            if (code.IndexOf("##SUBAFFFILIATE##") > 0)
                code = code.Replace("##SUBAFFFILIATE##", !string.IsNullOrEmpty(_subAffiliate) ? _subAffiliate : string.Empty);

            if (code.IndexOf("##CID##") > 0)
                code = code.Replace("##CID##", !string.IsNullOrEmpty(_cid) ? _cid : string.Empty);

            return code;
        }

        protected string ReplaceTermVariables(string code)
        {
            CampaignService src = new CampaignService();
            TermView view = src.GetTerms(CampaignID);
            if (view == null)
                view = new TermView();

            if (code.IndexOf("##TERMS_MEMBERSHIP##") > -1)
            {
                code = code.Replace("##TERMS_MEMBERSHIP##", view.MembershipTerms);
            }
            if (code.IndexOf("##TERMS_STRAIGHTSALE##") > -1)
            {
                code = code.Replace("##TERMS_STRAIGHTSALE##", view.StraightSaleTerms);
            }
            if (code.IndexOf("##CORPORATION##") > -1)
            {
                code = code.Replace("##CORPORATION##", view.CorporationBody);
            }
            if (code.IndexOf("##RETURN##") > -1)
            {
                code = code.Replace("##RETURN##", view.ReturnAddressBody);
            }
            if (code.IndexOf("##RETURN_NAME##") > -1)
            {
                code = code.Replace("##RETURN_NAME##", view.ReturnAddressName);
            }
            if (code.IndexOf("##SUPPORTPHONE##") > -1)
            {
                code = code.Replace("##SUPPORTPHONE##", view.Phone);
            }
            if (code.IndexOf("##SUPPORTEMAIL##") > -1)
            {
                code = code.Replace("##SUPPORTEMAIL##", view.Email);
            }
            if (code.IndexOf("##DOMAIN##") > -1)
            {
                code = code.Replace("##DOMAIN##", view.Domain);
            }
            if (code.IndexOf("##PHONE##") > -1)
            {
                code = code.Replace("##PHONE##", view.Phone);
            }
            if ((code.IndexOf("##CORPORATION_NAME##") > -1) && view.CorporationName != null)
            {
                code = code.Replace("##CORPORATION_NAME##", view.CorporationName);
            }
            else
            {
                code = code.Replace("##CORPORATION_NAME## |", "").Replace("##CORPORATION_NAME##", "");
            }

            string termsDisplayValue = "Terms &amp; Conditions",
                   policyDisplayValue = "Privacy Policy",
                   contactDisplayValue = "Contact Us";
            if (Config.Current.APPLICATION_ID == ApplicationEnum.TwoBoxCampaign)
            {
                //only for 2BOX client!!
                termsDisplayValue = "Termos e Condições";
                policyDisplayValue = "Política de Privacidade";
                contactDisplayValue = "Contato";
            }

            if ((code.IndexOf("##TERMS##") > -1) && view.Outline != null)
            {
                code = code.Replace("##TERMS##",
                        string.Format("<a href=\"javascript: void(0)\" onClick=\"window.open('tc.aspx?id={0}&type=tc','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); return false;\">{1}</a>", this.CampaignID, termsDisplayValue));
            }
            else
            {
                code = code.Replace("##TERMS## |", "").Replace("##TERMS##", "");
            }
            if ((code.IndexOf("##PRIVACY##") > -1) && view.PrivacyPolicy != null)
            {
                code = code.Replace("##PRIVACY##",
                    string.Format("<a href=\"javascript: void(0)\" onClick=\"window.open('tc.aspx?id={0}&type=pp','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); return false;\">{1}</a>", this.CampaignID, policyDisplayValue));
            }
            else
            {
                code = code.Replace("##PRIVACY## |", "").Replace("##PRIVACY##", "");
            }
            if (code.IndexOf("##CONTACT##") > -1 && view.Phone != null)
            {
                code = code.Replace("##CONTACT##",
                    string.Format("<a href=\"javascript: void(0)\" onClick=\"window.open('tc.aspx?id={0}&type=contact','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); return false;\">{1}</a>", this.CampaignID, contactDisplayValue));
            }
            else
            {
                code = code.Replace("##CONTACT## |", "").Replace("##CONTACT##", "");
            }

            return code;
        }

        protected string ReplaceQuerystringVarialbes(string code)
        {
            Regex regex = new Regex(@"(##QueryString\((.+?)\)##)", RegexOptions.Compiled);

            return regex.Replace(code, new MatchEvaluator(ReplaceQueryString));
        }

        protected virtual string GetPixels()
        {
            return GetPixels(string.Empty);
        }

        protected virtual string GetPixels(string pixel)
        {
            //string pix = pixel + GetPixelCode(_affiliate, _pageTypeID);

            //string affiliate = string.IsNullOrEmpty(_affiliate) ? string.Empty : "&aff=" + _affiliate;

            //string subAffiliate = string.IsNullOrEmpty(_subAffiliate) ? string.Empty : "&sub=" + _subAffiliate;

            //string cid = string.IsNullOrEmpty(_cid) ? string.Empty : "&cid=" + _cid;

            //string registrationID = string.IsNullOrEmpty(Request["registrationID"]) ? string.Empty : "&registrationID=" + Request["registrationID"];

            //string queryString = string.Format("&QS={0}{1}{2}{3}", affiliate, subAffiliate, cid, registrationID);

            //string redirectUrl = string.Format("https://www.inclinehealthsecure.com/dynamic/SaveCampaign.aspx?ParentCampaignID={0}&QS={1}", _campaignID, queryString);

            //pix = pix.Replace("##REDIRECT_URL##", redirectUrl);

            //pix = ReplaceLinkVariables(pix);

            //return ReplacePixelSavePrice(pix);
            return GetPixelCode(_affiliate, _pageTypeID, _cid, ShoppingCart.BillingID);
        }

        private string GetPixelCode(string affiliate, int pageTypeID, string cid, long? billingID)
        {
            if (string.IsNullOrEmpty(affiliate))
                return string.Empty;

            if (pageTypeID == 3)
            {
                Session["ConfirmationPixel"] = new object();
                return HtmlHelper.Pixels(affiliate, new int[] { pageTypeID, 4 }, cid, billingID);
            }
            else
                if (pageTypeID == 4 && Session["ConfirmationPixel"] == null)
                    return HtmlHelper.Pixels(affiliate, pageTypeID, cid, billingID);
                else
                    return string.Empty;
        }

        protected override void OnInit(EventArgs e)
        {
            try
            {
                _campaignID = int.Parse(Request["campaignID"]);
            }
            catch
            {
                Stop("Campaign should be set");
            }

            _campaign = _campaignService.GetCampaignByID(_campaignID);

            if (_campaign == null)
            {
                Stop("Campaign should be set");
            }

            _campaignPage = _campaignService.GetCampaignPages(_campaignID).SingleOrDefault(cp => cp.PageTypeID == _pageTypeID);

            if (_campaignPage == null || _campaignPage.HTML == string.Empty)
            {
                if (_pageTypeID == PageTypeEnum.PreLander)
                {
                    Response.Redirect("Landing.aspx?CampaignID=" + CampaignID + GetQuery());
                }
                else if (_pageTypeID == PageTypeEnum.Upsell_1 || _pageTypeID == PageTypeEnum.Upsell_2 || _pageTypeID == PageTypeEnum.Upsell_3)
                {
                    Response.Redirect("Confirmation.aspx?CampaignID=" + CampaignID + GetQuery());
                }
                else if (_pageTypeID == PageTypeEnum.Landing)
                {
                    Response.Redirect("Billing.aspx?CampaignID=" + CampaignID + GetQuery());
                }
                else if (_pageTypeID != PageTypeEnum.Landing)
                {
                    Response.Redirect("PreLander.aspx?CampaignID=" + CampaignID + GetQuery());
                }
            }

            SetErrorMessage(string.Empty);

            base.OnInit(e);
        }

        protected void Stop(string message)
        {
            Response.End();
        }

        protected override void OnLoad(EventArgs e)
        {
            string act = Request.Form["__action"];

            if (string.IsNullOrEmpty(act))
                _pageAction = PageActions.NoAction;

            else
                _pageAction = Enum.Parse(typeof(PageActions), act) != null ? (PageActions)Enum.Parse(typeof(PageActions), act) : PageActions.NoAction;

            if (_pageAction == PageActions.NoAction)
            {
                if (CheckCookie())
                {
                    TempConversion tmpConversion = new TempConversion()
                    {
                        PageTypeID = CampaignPage.PageTypeID,
                        Affiliate = Affiliate,
                        SubAffiliate = SubAffiliate,
                        CampaignID = CampaignPage.CampaignID,
                        CreateDT = DateTime.Now
                    };

                    try
                    {
                        Dao.Save<TempConversion>(tmpConversion);
                    }
                    catch
                    {
                    }
                }
                FillDefaultPageValues();
            }
            else
            {
                _pageValues = ObtainPageValues();
                DoAction(_pageAction);
            }


            _html = GetHTML();

            base.OnLoad(e);
        }

        protected virtual void DoAction(PageActions action)
        {

        }

        protected virtual NameValueCollection ObtainPageValues()
        {
            NameValueCollection res = new NameValueCollection(Request.Form);

            return res;
        }

        protected virtual void FillDefaultPageValues()
        {
        }

        protected string GetHTML()
        {
            string html = ReplaceLinkVariables(_campaignPage.HTML);
            html = ReplaceTermVariables(html);
            html = ReplaceQuerystringVarialbes(html);

            Regex regex = new Regex(@"(#[a-zA-Z0-9_\[\]=,]{1,140}#)+", RegexOptions.Compiled);

            return regex.Replace(html, new MatchEvaluator(ReplaceControl));
        }

        private string GetControl(string name)
        {
            name = name.Trim(new char[] { '#' });
            string onlyTerm = string.Empty;
            CampaignControl campaignControl = null;
            string value;
            bool campaignControlLoaded = false;

            if (name.Contains("Country") && name.ToLower().Contains("[values="))
            {
                int firstIndex = name.ToLower().IndexOf("[values=");
                int firstIndex2 = firstIndex + 7;
                int lastIndex = name.IndexOf(']');
                try
                {
                    IList<string> listOfCountries = name.Substring(firstIndex2 + 1, lastIndex - firstIndex2 - 1).Split(',').Select(i => i.Trim().ToUpper()).ToList();
                    IList<Geo> fullcountryList = new GeoService().GetCountryList();

                    IList<string> htmlOptions = (
                        from code in listOfCountries
                        join c in fullcountryList on code equals c.Code into geo
                        from country in geo.DefaultIfEmpty(new Geo() { Code = code, Name = code })
                        select string.Format("<option value=\"{0}\">{1}</option>", country.Name.Replace("United States", "US"), country.Name)
                        ).ToList();

                    campaignControl = _campaignService.GetCampaignControl(name.Substring(0, firstIndex));
                    int firstOption = campaignControl.HTML.IndexOf("<option");
                    int lastOption = campaignControl.HTML.LastIndexOf("/option>") + 7;
                    campaignControl.HTML = campaignControl.HTML.Remove(firstOption, lastOption - firstOption + 1).Insert(firstOption,
                        string.Join("\n", htmlOptions.ToArray()));
                    campaignControlLoaded = true;
                }
                catch
                {
                    //in case of error process as ##Billing_Country##/##Shipping_Country##
                    //Response.Write("Error: " + ex.ToString());
                }
                finally
                {
                    name = name.Substring(0, firstIndex);
                }
            }
            else if (name.Contains("Country") && name.Contains('['))
            {
                int firstIndex = name.IndexOf('[');
                int lastIndex = name.IndexOf(']');
                onlyTerm = name.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
                name = name.Substring(0, firstIndex);
            }

            if (name == "Billing_State" || name == "Shipping_State")
            {
                string valueToReturn = string.Empty;

                Dictionary<string, ControlNames> states = _shippingStates;
                if (name == "Billing_State")
                    states = _billingStates;

                foreach (var state in states)
                {
                    campaignControl = _campaignService.GetCampaignControl(state.Value.ToString());

                    if (campaignControl != null)
                    {
                        value = _pageValues[state.Value.ToString()];

                        if (string.IsNullOrEmpty(value))
                            valueToReturn += campaignControl.HTML;
                        else
                            valueToReturn += DisplayWithSelectedValue(campaignControl, value);
                    }
                }
                return valueToReturn;
            }
            if (name.Contains("Tel"))
            {
                string pageType = name.Split('_')[0];
                campaignControl = _campaignService.GetCampaignControl(name);

                if (campaignControl == null)
                    return string.Empty;

                value = _pageValues[pageType + "_Home_Tel"];

                if (string.IsNullOrEmpty(value))
                    return campaignControl.HTML;
                else
                    return DisplayWithSelectedValue(campaignControl, value);
            }
            if (name.Contains("Next_URL"))
                return "#Next_URL#";

            if (!campaignControlLoaded)
                campaignControl = _campaignService.GetCampaignControl(name);
            if (campaignControl == null)
                return string.Empty;
            value = _pageValues[name];

            if (name.Contains("Country"))
            {
                if (string.IsNullOrEmpty(onlyTerm))
                {
                    campaignControl.HTML = campaignControl.HTML.Replace("#DISPLAY#", "");
                    campaignControl.HTML = campaignControl.HTML.Replace("#CLASS#", "");
                }
                else
                {
                    campaignControl.HTML = campaignControl.HTML.Replace("#DISPLAY#", "display:none;");
                    campaignControl.HTML = campaignControl.HTML.Replace("#CLASS#", onlyTerm);
                }
            }

            if (string.IsNullOrEmpty(value))
                return campaignControl.HTML;
            else
                return DisplayWithSelectedValue(campaignControl, value);
        }

        private string DisplayWithSelectedValue(CampaignControl campaignControl, string value)
        {
            int controlTypeID = DetermineControlType(campaignControl);

            string res = campaignControl.HTML;

            switch (controlTypeID)
            {
                case ControlTypeEnum.TextInput:
                    res = res.Remove(res.Length - 2, 2);
                    res += string.Format(" value='{0}' />", HTMLEncodeSpecialChars(value));
                    break;

                case ControlTypeEnum.Phone:
                    value = value.Replace(",", string.Empty);
                    if (value.Length == 10)
                    {
                        string[] arr = res.Split(new string[] { "> -" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 3)
                        {
                            arr[0] = arr[0].Replace("value=\"\"", "value=\"" + value.Substring(0, 3) + "\"");
                            arr[1] = arr[1].Replace("value=\"\"", "value=\"" + value.Substring(3, 3) + "\"");
                            arr[2] = arr[2].Replace("value=\"\"", "value=\"" + value.Substring(6, 4) + "\"");
                            res = string.Join("> -", arr);
                        }
                        else
                            res = res.Replace("value=\"\"", "value=\"" + value + "\"");
                    }
                    else
                        res = res.Replace("value=\"\"", "value=\"" + value + "\"");
                    break;

                case ControlTypeEnum.Zip:
                    value = value.Replace(",", string.Empty);
                    value = value.Replace("-", string.Empty);
                    //if (value.Length > 0)
                    //{
                    //    string[] arr = res.Split(new string[] { "<span" }, StringSplitOptions.RemoveEmptyEntries);
                    //    arr[0] = arr[0].Replace("value=\"\"", "value=\"" + value.Substring(0, 5) + "\"");
                    //    if (value.Length > 5)
                    //        arr[1] = arr[1].Replace("value=\"\"", "value=\"" + value.Substring(5, 4) + "\"");
                    //    res = string.Join("<span", arr);
                    //}
                    res = res.Replace("value=\"\"", "value=\"" + value + "\"");
                    break;

                case ControlTypeEnum.DropDown:
                    res = res.Replace(string.Format("<option value=\"{0}\"", value), string.Format("<option value=\"{0}\" selected", value));
                    break;

                case ControlTypeEnum.Text:
                    if (value == null || value == string.Empty)
                        res = string.Empty;
                    else
                        res = res.Insert(res.IndexOf("</span>"), HTMLEncodeSpecialChars(value));
                    break;

                case ControlTypeEnum.Pixel:
                    res = value;
                    break;

                case ControlTypeEnum.CheckBox:
                    res = value == "on" ? res.Insert(res.Length - 3, "checked=\"true\" ") : res;
                    break;
            }
            return res;
        }

        private int DetermineControlType(CampaignControl campaignControl)
        {
            if (campaignControl.HTML.Length == 0)
                return ControlTypeEnum.Pixel;

            if (campaignControl.HTML.StartsWith("<select"))
                return ControlTypeEnum.DropDown;

            else if (campaignControl.HTML.StartsWith("<input type=\"text"))
            {
                if (campaignControl.HTML.Contains("_Tel"))
                    return ControlTypeEnum.Phone;

                else if (campaignControl.HTML.Contains("_Zip"))
                    return ControlTypeEnum.Zip;

                return ControlTypeEnum.TextInput;
            }

            else if (campaignControl.HTML.StartsWith("<span"))
                return ControlTypeEnum.Text;

            else if (campaignControl.HTML.StartsWith("<input type=\"checkbox"))
                return ControlTypeEnum.CheckBox;


            return ControlTypeEnum.TextInput;
        }

        public static string HTMLEncodeSpecialChars(string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c > 127 || c == 39) // special chars
                    sb.Append(String.Format("&#{0};", (int)c));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private string ReplaceControl(Match m)
        {
            var res = GetControl(m.ToString());
            return res;
        }

        private string ReplaceQueryString(Match m)
        {
            var pattern = new Regex(@"(?<=\().*?(?=\))");
            return Request.QueryString[pattern.Match(m.Value).Value];
        }

        protected string GetValue(ControlNames name)
        {
            return (Request.Form[name.ToString()] ?? string.Empty).Replace(",", "");
        }

        protected string GetValueWithDefault(ControlNames name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Request.Form[name.ToString()]))
                return (defaultValue ?? string.Empty).Replace(",", "");
            return Request.Form[name.ToString()].Replace(",", "");
        }

        protected void SetValueWithDefault(ControlNames name, string value)
        {
            PageValues[name.ToString()] = value;
        }

        protected void SetErrorMessage(string err)
        {
            PageValues.Set(ControlNames.Error_Message.ToString(), err);
        }

        protected string GetQuery()
        {
            var q = new StringBuilder();
            if (_affiliate != null && _affiliate != string.Empty)
            {
                q.AppendFormat("&{0}={1}", "aff", Affiliate);
            }

            if (SubAffiliate != null && SubAffiliate != string.Empty)
            {
                q.AppendFormat("&{0}={1}", "sub", SubAffiliate);
            }

            if (CID != null && CID != string.Empty)
            {
                q.AppendFormat("&{0}={1}", "cid", CID);
            }

            return q.ToString();
        }

        protected void GoToUpsell()
        {
            string upsellUrl = "Upsell.aspx?campaignID={0}&Step={1}{2}";

            string confirmationUrl = "Confirmation.aspx?campaignID={0}";

            int step = 0;
            int next = 0;

            if (_pageTypeID == PageTypeEnum.Billing)
            {
                step = 1;
                next = PageTypeEnum.Upsell_1;
            }

            else if (_pageTypeID == PageTypeEnum.Upsell_1)
            {
                step = 2;
                next = PageTypeEnum.Upsell_2;
            }

            else if (_pageTypeID == PageTypeEnum.Upsell_2)
            {
                step = 3;
                next = PageTypeEnum.Upsell_3;
            }

            else if (_pageTypeID == PageTypeEnum.Upsell_3)
                step = 4;

            if (step < 4)
            {
                CampaignPage campaignPage = _campaignService.GetCampaignPages(_campaignID).FirstOrDefault(x => x.HTML != string.Empty && x.PageTypeID == next);
                upsellUrl = string.Format(upsellUrl, _campaignID, step, GetQuery());
                Response.Redirect(upsellUrl);
            }

            else
            {
                Response.Redirect(string.Format(confirmationUrl, _campaignID) + GetQuery());
            }

        }

        protected string GetUpsellUrl()
        {
            string upsellUrl = "Upsell.aspx?campaignID={0}&Step={1}{2}";

            string confirmationUrl = "Confirmation.aspx?campaignID={0}";

            int step = 0;
            int next = 0;

            if (_pageTypeID == PageTypeEnum.Billing)
            {
                step = 1;
                next = PageTypeEnum.Upsell_1;
            }

            else if (_pageTypeID == PageTypeEnum.Upsell_1)
            {
                step = 2;
                next = PageTypeEnum.Upsell_2;
            }

            else if (_pageTypeID == PageTypeEnum.Upsell_2)
            {
                step = 3;
                next = PageTypeEnum.Upsell_3;
            }

            else if (_pageTypeID == PageTypeEnum.Upsell_3)
                step = 4;

            if (step < 4)
            {
                CampaignPage campaignPage = _campaignService.GetCampaignPages(_campaignID).FirstOrDefault(x => x.HTML != string.Empty && x.PageTypeID == next);
                return string.Format(upsellUrl, _campaignID, step, GetQuery());
            }

            else
                return string.Format(confirmationUrl, _campaignID) + GetQuery();
        }

        protected string GetConfirmationUrl()
        {
            string confirmationUrl = "Confirmation.aspx?campaignID={0}";
            return string.Format(confirmationUrl, _campaignID) + GetQuery();
        }

        //protected void GoToPage(int pageType, string query)
        //{
        //    var url = "{0}://{1}{2}/{3}";
        //    var host = Request.Url.Host;
        //    //if (pageType == PageTypeEnum.Confirmation)
        //    //{
        //    //    if (Campaign.Redirect)
        //    //    {
        //    //        host = AppConfigSettings.HostName;
        //    //    }
        //    //    if (Campaign.IsSTO)
        //    //    {
        //    //        host = AppConfigSettings.STO_URL;
        //    //    }
        //    //}

        //    string pageTypeString = string.Empty;
        //    if (pageType == PageTypeEnum.Billing)
        //        pageTypeString = "Billing";

        //    else if (pageType == PageTypeEnum.Upsell_1)
        //        pageTypeString = "Upsell_1";

        //    else if (pageType == PageTypeEnum.Upsell_2)
        //    {
        //        pageTypeString = "Upsell_2";
        //    }
        //    else if (pageType == PageTypeEnum.Landing)
        //    {
        //        pageTypeString = "Landing";
        //    }
        //    else if (pageType == PageTypeEnum.Confirmation)
        //    {
        //        pageTypeString = "Confirmation";
        //    }

        //    url = string.Format(url,
        //         (pageType == PageTypeEnum.Landing || pageType == PageTypeEnum.PreLander || !AppConfigSettings.UseHTTPS) ? "http" : "https",
        //         host,
        //         host == AppConfigSettings.HostName || Campaign.IsSTO
        //             ? AppConfigSettings.RelativePath
        //             : string.Empty,
        //         pageTypeString + ".aspx?" + query + GetQuery());
        //    Response.Redirect(url);
        //}

        private bool CheckCookie()
        {
            HttpCookie cookie = Request.Cookies[Uri.EscapeDataString("HitsCookie" + _campaignID + "_" + _pageTypeID.ToString())];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddMinutes(2.0);
                return false;
            }
            else
            {
                cookie = new HttpCookie(Uri.EscapeDataString("HitsCookie" + _campaignID + "_" + _pageTypeID.ToString()), "hit");
                cookie.Expires = DateTime.Now.AddMinutes(2.0);
                Response.Cookies.Add(cookie);
                return true;
            }

        }

        //protected void Preview(Literal html)
        //{
        //    _pageValues = new NameValueCollection();

        //    Regex regex = new Regex(@"(#[a-zA-Z0-9_]{1,30}#)+", RegexOptions.Compiled);
        //    MySqlCommand q = new MySqlCommand("SELECT * FROM CampaignPage Where CampaignID=@CampaignID AND PageTypeID=@PageTypeID");
        //    q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = CampaignID;
        //    q.Parameters.Add("@PageTypeID", MySqlDbType.Int32).Value = PageTypeID;

        //    var preViewPage = Dao.Load<CampaignPage>(q).SingleOrDefault();

        //    if (preViewPage != null)
        //    {
        //        html.Text = regex.Replace(preViewPage.HTML, new MatchEvaluator(ReplaceControl));
        //        var header = Master.FindControl("head") as Literal;
        //        header.Text = preViewPage.Header;
        //    }
        //}

        protected string GetBillingState(string billingCountry, Billing existedBil, Registration existedReg)
        {
            string billingState = "";
            foreach (var state in _billingStates)
            {
                if (billingCountry == state.Key)
                    billingState = GetValueWithDefault(state.Value, existedBil.State ?? existedReg.State);
            }

            if (string.IsNullOrEmpty(billingState))
                billingState = GetValueWithDefault(ControlNames.Billing_State_Other, existedBil.State ?? existedReg.State);
            return billingState;
        }

        protected string GetBillingState(string billingCountry, Registration existedReg)
        {
            string billingState = "";
            foreach (var state in _billingStates)
            {
                if (billingCountry == state.Key)
                    billingState = GetValueWithDefault(state.Value, existedReg.State);
            }

            if (string.IsNullOrEmpty(billingState))
                billingState = GetValueWithDefault(ControlNames.Billing_State_Other, existedReg.State);
            return billingState;
        }

        protected string GetShippingState(string shippingCountry, string defaultState)
        {
            string shippingState = "";
            foreach (var state in _shippingStates)
            {
                if (shippingCountry == state.Key)
                    shippingState = GetValueWithDefault(state.Value, defaultState);
            }

            if (string.IsNullOrEmpty(shippingState))
                shippingState = GetValueWithDefault(ControlNames.Shipping_State_Other, defaultState);
            return shippingState;
        }

        protected Set<Registration, Billing> UpdateBillingRegistration()
        {
            Set<Registration, Billing> res = new Set<Registration, Billing>();

            try
            {
                #region Registration

                RegistrationService registrationService = new RegistrationService();
                Registration registration = ShoppingCart.Registration;

                if (registration == null)
                    if (!string.IsNullOrEmpty(Request["RegistrationID"]))
                    {
                        ShoppingCart.RegistrationID = Utility.TryGetLong(Request["RegistrationID"]);
                        registration = Dao.Load<Registration>(Request["RegistrationID"]);
                    }

                //would be use when RegistrationID is null
                string registrationCountry = GetValueWithDefault(ControlNames.Shipping_Country, "");
                registrationCountry = string.IsNullOrEmpty(registrationCountry) ? US_COUNTRY : registrationCountry;

                if (registration == null)
                {
                    registration = registrationService.CreateRegistrationFromDynamicCampaign(Campaign.CampaignID,
                    GetValueWithDefault(ControlNames.Shipping_First_Name, ""),
                    GetValueWithDefault(ControlNames.Shipping_Last_Name, ""),
                    GetValueWithDefault(ControlNames.Shipping_Address_1, ""),
                    GetValueWithDefault(ControlNames.Shipping_Address_2, ""),
                    GetValueWithDefault(ControlNames.Shipping_City, ""),
                    GetShippingState(registrationCountry, ""),
                    GetValueWithDefault(ControlNames.Shipping_Zip, ""),
                    registrationCountry,
                    GetValueWithDefault(ControlNames.Shipping_Email, ""),
                    GetValueWithDefault(ControlNames.Shipping_Home_Tel, ""),
                    DateTime.Now, Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);

                    ShoppingCart.Registration = registration;
                }
                else
                {
                    RegistrationInfo existedRegInfo = null;
                    if (ShoppingCart.Registration != null)
                        existedRegInfo = Dao.Load<RegistrationInfo>(
                            new MySql.Data.MySqlClient.MySqlCommand(
                                "SELECT * FROM RegistrationInfo Where RegistrationID=" + ShoppingCart.Registration.RegistrationID.ToString()))
                            .SingleOrDefault();
                    else
                        existedRegInfo = new RegistrationInfo();


                    registrationCountry = GetValueWithDefault(ControlNames.Shipping_Country, existedRegInfo != null ? existedRegInfo.Country : US_COUNTRY);
                    registrationCountry = string.IsNullOrEmpty(registrationCountry) ? US_COUNTRY : registrationCountry;

                    registrationService.UpdateRegistration(registration.RegistrationID.Value, Campaign.CampaignID,
                        GetValueWithDefault(ControlNames.Shipping_First_Name, registration.FirstName),
                        GetValueWithDefault(ControlNames.Shipping_Last_Name, registration.LastName),
                        GetValueWithDefault(ControlNames.Shipping_Address_1, registration.Address1),
                        GetValueWithDefault(ControlNames.Shipping_Address_2, registration.Address2),
                        GetValueWithDefault(ControlNames.Shipping_City, registration.City),
                        GetShippingState(registrationCountry, registration.State),
                        GetValueWithDefault(ControlNames.Shipping_Zip, registration.Zip),
                        registrationCountry,
                        GetValueWithDefault(ControlNames.Shipping_Email, registration.Email),
                        GetValueWithDefault(ControlNames.Shipping_Home_Tel, registration.Phone),
                        registration.CreateDT, Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);
                }
                UpdateRegistrationNeighborhoodAndNumeroAndComplemento(registration.RegistrationID);

                res.Value1 = registration;

                #endregion

                #region Billing

                Billing billing = ShoppingCart.Billing;
                if (billing == null)
                {
                    string country = GetValueWithDefault(ControlNames.Billing_Country, "");
                    country = string.IsNullOrEmpty(country) ? registrationCountry : country;

                    billing = registrationService.CreateBilling(Campaign.CampaignID, registration.RegistrationID,
                        GetValueWithDefault(ControlNames.Billing_First_Name, registration.FirstName),
                        GetValueWithDefault(ControlNames.Billing_First_Name, registration.LastName),
                        GetValue(ControlNames.CC_Number),
                        GetValue(ControlNames.CC_CVV),
                        Utility.TryGetInt(GetValue(ControlNames.CC_Type)),
                        Utility.TryGetInt(GetValue(ControlNames.CC_Month)),
                        Utility.TryGetInt(GetValue(ControlNames.CC_Year)),
                        GetValueWithDefault(ControlNames.Billing_Address_1, registration.Address1),
                        GetValueWithDefault(ControlNames.Billing_Address_2, registration.Address2),
                        GetValueWithDefault(ControlNames.Billing_City, registration.City),
                        GetBillingState(country, registration),
                        GetValueWithDefault(ControlNames.Billing_Zip, registration.Zip),
                        country,
                        GetValueWithDefault(ControlNames.Billing_Email, registration.Email),
                        GetValueWithDefault(ControlNames.Billing_Home_Tel, registration.Phone),
                        DateTime.Now,
                        Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);

                    ShoppingCart.Billing = billing;
                }
                else
                {
                    string country = GetValueWithDefault(ControlNames.Billing_Country, billing.Country);
                    country = string.IsNullOrEmpty(country) ? registrationCountry : country;

                    // removing special sybmols
                    var zip = GetValueWithDefault(ControlNames.Billing_Zip, registration.Zip);
                    zip = string.IsNullOrEmpty(zip) ? zip : zip.Replace("-", "").Replace(" ", "");

                    var phone = GetValueWithDefault(ControlNames.Billing_Home_Tel, registration.Phone);
                    phone = string.IsNullOrEmpty(phone) ? phone : phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace(".", "");

                    string cc = GetValue(ControlNames.CC_Number);
                    cc = string.IsNullOrEmpty(cc) ? cc : cc.Replace(" ", "");
                    if (!string.IsNullOrEmpty(billing.CreditCard))
                    {
                        cc = string.IsNullOrEmpty(cc) ? billing.CreditCardCnt.DecryptedCreditCard : cc;
                    }
                    // removing special sybmols

                    registrationService.UpdateBilling(billing.BillingID.Value,
                       Campaign.CampaignID, billing.RegistrationID,
                       GetValueWithDefault(ControlNames.Billing_First_Name, billing.FirstName),
                       GetValueWithDefault(ControlNames.Billing_First_Name, billing.LastName),
                       cc,
                       GetValueWithDefault(ControlNames.CC_CVV, billing.CVV),
                       Utility.TryGetInt(GetValue(ControlNames.CC_Type)) ?? billing.PaymentTypeID,
                       Utility.TryGetInt(GetValue(ControlNames.CC_Month)) ?? billing.ExpMonth,
                       Utility.TryGetInt(GetValue(ControlNames.CC_Year)) ?? billing.ExpYear,
                       GetValueWithDefault(ControlNames.Billing_Address_1, billing.Address1),
                       GetValueWithDefault(ControlNames.Billing_Address_2, billing.Address2),
                       GetValueWithDefault(ControlNames.Billing_City, billing.City),
                       GetBillingState(country, billing, registration),
                       zip,
                       country,
                       GetValueWithDefault(ControlNames.Billing_Email, billing.Email),
                       phone,
                       DateTime.Now,
                       Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);

                }

                UpdateBillingCPFAndNeighborhoodandAndNumeroAndComplementoAndCardHolder(billing.BillingID);
                res.Value2 = billing;

                #endregion
            }
            catch
            {
                res = null;
            }
            return res;
        }

        protected bool UpdateRegistrationNeighborhoodAndNumeroAndComplemento(long? registrationID)
        {
            if (registrationID == null)
                return false;

            var res = false;
            try
            {
                //Save Neighborhood+ fields
                if (!string.IsNullOrEmpty(GetValue(ControlNames.Shipping_Neighborhood)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Shipping_Numero)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Shipping_Complemento)))
                {
                    RegistrationInfo existedRegInfo = null;
                    existedRegInfo = Dao.Load<RegistrationInfo>(
                        new MySql.Data.MySqlClient.MySqlCommand(
                            "SELECT * FROM RegistrationInfo Where RegistrationID=" + registrationID.ToString()))
                        .SingleOrDefault();

                    if (existedRegInfo == null)
                    {
                        existedRegInfo = new RegistrationInfo()
                        {
                            RegistrationID = registrationID
                        };
                    }
                    existedRegInfo.Neighborhood = GetValueWithDefault(ControlNames.Shipping_Neighborhood, existedRegInfo.Neighborhood);
                    //CustomField1 - Numero
                    existedRegInfo.CustomField1 = GetValueWithDefault(ControlNames.Shipping_Numero, existedRegInfo.CustomField1);
                    //CustomField2 - Complemento
                    existedRegInfo.CustomField2 = GetValueWithDefault(ControlNames.Shipping_Complemento, existedRegInfo.CustomField2);
                    Dao.Save<RegistrationInfo>(existedRegInfo);
                    //----------------
                }
                res = true;
            }
            catch
            {
                res = false;
            }
            return res;
        }

        protected bool UpdateBillingCPFAndNeighborhoodandAndNumeroAndComplementoAndCardHolder(long? billingID)
        {
            if (billingID == null)
                return false;

            var res = false;
            try
            {
                //Save CPF+ fields
                if (!string.IsNullOrEmpty(GetValue(ControlNames.Billing_CPF)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Numero)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Complemento)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Billing_City_Code)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Card_Holder)))
                {
                    BillingExternalInfo existedBilInfo = null;
                    existedBilInfo = Dao.Load<BillingExternalInfo>(
                        new MySql.Data.MySqlClient.MySqlCommand(
                            "SELECT * FROM BillingExternalInfo Where BillingID=" + billingID.ToString()))
                        .SingleOrDefault();

                    RegistrationInfo existedRegInfo = null;
                    if (ShoppingCart.RegistrationID != null)
                        existedRegInfo = Dao.Load<RegistrationInfo>(
                            new MySql.Data.MySqlClient.MySqlCommand(
                                "SELECT * FROM RegistrationInfo Where RegistrationID=" + ShoppingCart.RegistrationID.ToString()))
                            .SingleOrDefault();
                    else
                        existedRegInfo = new RegistrationInfo();

                    if (existedBilInfo == null)
                    {
                        existedBilInfo = new BillingExternalInfo()
                        {
                            BillingID = billingID
                        };
                    }

                    // removing special sybmols
                    var cpf = GetValueWithDefault(ControlNames.Billing_CPF, existedBilInfo.CustomField1);
                    cpf = string.IsNullOrEmpty(cpf) ? cpf : cpf.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace(".", "");
                    // removing special sybmols

                    //CustomField1 - CPF
                    existedBilInfo.CustomField1 = cpf;
                    //CustomField2 - Numero
                    existedBilInfo.CustomField2 = GetValueWithDefault(ControlNames.Billing_Numero, existedBilInfo.CustomField2 ?? existedRegInfo.CustomField1);
                    //CustomField3 - Complemento
                    existedBilInfo.CustomField3 = GetValueWithDefault(ControlNames.Billing_Complemento, existedBilInfo.CustomField3 ?? existedRegInfo.CustomField2);
                    //CustomField4 - CardHolder
                    existedBilInfo.CustomField4 = GetValueWithDefault(ControlNames.Billing_Card_Holder, existedBilInfo.CustomField4);
                    //CustomField5 - CityCode
                    existedBilInfo.CustomField5 = GetValueWithDefault(ControlNames.Billing_City_Code, existedBilInfo.CustomField5);
                    Dao.Save<BillingExternalInfo>(existedBilInfo);
                    //----------------
                }
                res = true;
            }
            catch
            {
                res = false;
            }
            return res;
        }

        #region TwoBox Section

        protected Set<Registration, Billing> UpdateBillingRegistration_TwoBox()
        {
            Set<Registration, Billing> res = new Set<Registration, Billing>();

            try
            {
                #region Registration

                RegistrationService registrationService = new RegistrationService();
                Registration registration = ShoppingCart.Registration;

                if (registration == null)
                    if (!string.IsNullOrEmpty(Request["RegistrationID"]))
                    {
                        ShoppingCart.RegistrationID = Utility.TryGetLong(Request["RegistrationID"]);
                        registration = Dao.Load<Registration>(Request["RegistrationID"]);
                    }

                //would be use when RegistrationID is null
                string registrationCountry = GetValueWithDefault(ControlNames.Shipping_Country, "");
                registrationCountry = string.IsNullOrEmpty(registrationCountry) ? US_COUNTRY : registrationCountry;

                if (registration == null)
                {
                    // removing special sybmols
                    var zip = GetValueWithDefault(ControlNames.Shipping_Zip, "");
                    zip = string.IsNullOrEmpty(zip) ? zip : zip.Replace("-", "").Replace(" ", "");

                    var phone = GetValueWithDefault(ControlNames.Shipping_Home_Tel, "");
                    phone = string.IsNullOrEmpty(phone) ? phone : phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace(".", "");
                    // removing special sybmols

                    registration = registrationService.CreateRegistrationFromDynamicCampaign(Campaign.CampaignID,
                    GetValueWithDefault(ControlNames.Shipping_First_Name, ""),
                    GetValueWithDefault(ControlNames.Shipping_Last_Name, ""),
                    GetValueWithDefault(ControlNames.Shipping_Address_1, ""),
                    GetValueWithDefault(ControlNames.Shipping_Address_2, ""),
                    GetValueWithDefault(ControlNames.Shipping_City, ""),
                    GetShippingState(registrationCountry, ""),
                    zip,
                    registrationCountry,
                    GetValueWithDefault(ControlNames.Shipping_Email, ""),
                    phone,
                    DateTime.Now, Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);

                    ShoppingCart.Registration = registration;
                }
                else
                {
                    RegistrationInfo existedRegInfo = null;
                    if (ShoppingCart.Registration != null)
                        existedRegInfo = Dao.Load<RegistrationInfo>(
                            new MySql.Data.MySqlClient.MySqlCommand(
                                "SELECT * FROM RegistrationInfo Where RegistrationID=" + ShoppingCart.Registration.RegistrationID.ToString()))
                            .SingleOrDefault();
                    else
                        existedRegInfo = new RegistrationInfo();


                    registrationCountry = GetValueWithDefault(ControlNames.Shipping_Country, existedRegInfo != null ? existedRegInfo.Country : US_COUNTRY);
                    registrationCountry = string.IsNullOrEmpty(registrationCountry) ? US_COUNTRY : registrationCountry;

                    // removing special sybmols
                    var zip = GetValueWithDefault(ControlNames.Shipping_Zip, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Zip)) ? GetValue(ControlNames.Billing_Zip) : registration.Zip);
                    zip = string.IsNullOrEmpty(zip) ? zip : zip.Replace("-", "").Replace(" ", "");

                    var phone = GetValueWithDefault(ControlNames.Shipping_Home_Tel, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Home_Tel)) ? GetValue(ControlNames.Billing_Home_Tel) : registration.Phone);
                    phone = string.IsNullOrEmpty(phone) ? phone : phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace(".", "");
                    // removing special sybmols

                    registrationService.UpdateRegistration(registration.RegistrationID.Value, Campaign.CampaignID,
                        GetValueWithDefault(ControlNames.Shipping_First_Name, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_First_Name)) ? GetValue(ControlNames.Billing_First_Name) : registration.FirstName),
                        GetValueWithDefault(ControlNames.Shipping_Last_Name, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Last_Name)) ? GetValue(ControlNames.Billing_Last_Name) : registration.LastName),
                        GetValueWithDefault(ControlNames.Shipping_Address_1, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Address_1)) ? GetValue(ControlNames.Billing_Address_1) : registration.Address1),
                        GetValueWithDefault(ControlNames.Shipping_Address_2, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Address_2)) ? GetValue(ControlNames.Billing_Address_2) : registration.Address2),
                        GetValueWithDefault(ControlNames.Shipping_City, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_City)) ? GetValue(ControlNames.Billing_City) : registration.City),
                        GetShippingState(registrationCountry, !string.IsNullOrEmpty(GetBillingState(registrationCountry, registration)) ? GetBillingState(registrationCountry, registration) : registration.State),
                        zip,
                        registrationCountry,
                        GetValueWithDefault(ControlNames.Shipping_Email, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Email)) ? GetValue(ControlNames.Billing_Email) : registration.Email),
                        phone,
                        registration.CreateDT, Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);
                }
                UpdateRegistrationNeighborhoodAndNumeroAndComplemento_TwoBox(registration.RegistrationID);

                res.Value1 = registration;

                #endregion

                #region Billing

                Billing billing = ShoppingCart.Billing;
                if (billing == null)
                {
                    string country = GetValueWithDefault(ControlNames.Billing_Country, "");
                    country = string.IsNullOrEmpty(country) ? registrationCountry : country;

                    // removing special sybmols
                    var zip = GetValueWithDefault(ControlNames.Billing_Zip, registration.Zip);
                    zip = string.IsNullOrEmpty(zip) ? zip : zip.Replace("-", "").Replace(" ", "");

                    var phone = GetValueWithDefault(ControlNames.Billing_Home_Tel, registration.Phone);
                    phone = string.IsNullOrEmpty(phone) ? phone : phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace(".", "");

                    var cc = GetValue(ControlNames.CC_Number);
                    cc = string.IsNullOrEmpty(cc) ? cc : cc.Replace(" ", "");
                    // removing special sybmols

                    billing = registrationService.CreateBilling(Campaign.CampaignID, registration.RegistrationID,
                        GetValueWithDefault(ControlNames.Billing_First_Name, registration.FirstName),
                        GetValueWithDefault(ControlNames.Billing_First_Name, registration.LastName),
                        cc,
                        GetValue(ControlNames.CC_CVV),
                        Utility.TryGetInt(GetValue(ControlNames.CC_Type)),
                        Utility.TryGetInt(GetValue(ControlNames.CC_Month)),
                        Utility.TryGetInt(GetValue(ControlNames.CC_Year)),
                        GetValueWithDefault(ControlNames.Billing_Address_1, registration.Address1),
                        GetValueWithDefault(ControlNames.Billing_Address_2, registration.Address2),
                        GetValueWithDefault(ControlNames.Billing_City, registration.City),
                        GetBillingState(country, registration),
                        zip,
                        country,
                        GetValueWithDefault(ControlNames.Billing_Email, registration.Email),
                        phone,
                        DateTime.Now,
                        Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);

                    ShoppingCart.Billing = billing;
                }
                else
                {
                    string country = GetValueWithDefault(ControlNames.Billing_Country, billing.Country);
                    country = string.IsNullOrEmpty(country) ? registrationCountry : country;

                    // removing special sybmols
                    var zip = GetValueWithDefault(ControlNames.Billing_Zip, billing.Zip);
                    zip = string.IsNullOrEmpty(zip) ? zip : zip.Replace("-", "").Replace(" ", "");

                    var phone = GetValueWithDefault(ControlNames.Billing_Home_Tel, billing.Phone);
                    phone = string.IsNullOrEmpty(phone) ? phone : phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace(".", "");

                    string cc = GetValue(ControlNames.CC_Number);
                    cc = string.IsNullOrEmpty(cc) ? cc : cc.Replace(" ", "");
                    if (!string.IsNullOrEmpty(billing.CreditCard))
                    {
                        cc = string.IsNullOrEmpty(cc) ? billing.CreditCardCnt.DecryptedCreditCard : cc;
                    }
                    // removing special sybmols

                    registrationService.UpdateBilling(billing.BillingID.Value,
                       Campaign.CampaignID, billing.RegistrationID,
                       GetValueWithDefault(ControlNames.Billing_First_Name, billing.FirstName),
                       GetValueWithDefault(ControlNames.Billing_First_Name, billing.LastName),
                       cc,
                       GetValueWithDefault(ControlNames.CC_CVV, billing.CVV),
                       Utility.TryGetInt(GetValue(ControlNames.CC_Type)) ?? billing.PaymentTypeID,
                       Utility.TryGetInt(GetValue(ControlNames.CC_Month)) ?? billing.ExpMonth,
                       Utility.TryGetInt(GetValue(ControlNames.CC_Year)) ?? billing.ExpYear,
                       GetValueWithDefault(ControlNames.Billing_Address_1, billing.Address1),
                       GetValueWithDefault(ControlNames.Billing_Address_2, billing.Address2),
                       GetValueWithDefault(ControlNames.Billing_City, billing.City),
                       GetBillingState(country, billing, registration),
                       zip,
                       country,
                       GetValueWithDefault(ControlNames.Billing_Email, billing.Email),
                       phone,
                       DateTime.Now,
                       Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);
                }

                UpdateBillingCPFAndNeighborhoodandAndNumeroAndComplementoAndCardHolder(billing.BillingID);
                res.Value2 = billing;

                #endregion
            }
            catch
            {
                res = null;
            }
            return res;
        }

        protected bool UpdateRegistrationNeighborhoodAndNumeroAndComplemento_TwoBox(long? registrationID)
        {
            if (registrationID == null)
                return false;

            var res = false;
            try
            {
                //Save Neighborhood+ fields
                if (!string.IsNullOrEmpty(GetValue(ControlNames.Shipping_Neighborhood)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Shipping_Numero)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Shipping_City_Code)) ||
                    !string.IsNullOrEmpty(GetValue(ControlNames.Shipping_Complemento)))
                {
                    RegistrationInfo existedRegInfo = null;
                    existedRegInfo = Dao.Load<RegistrationInfo>(
                        new MySql.Data.MySqlClient.MySqlCommand(
                            "SELECT * FROM RegistrationInfo Where RegistrationID=" + registrationID.ToString()))
                        .SingleOrDefault();

                    if (existedRegInfo == null)
                    {
                        existedRegInfo = new RegistrationInfo()
                        {
                            RegistrationID = registrationID
                        };
                    }
                    existedRegInfo.Neighborhood = GetValueWithDefault(ControlNames.Shipping_Neighborhood, existedRegInfo.Neighborhood);
                    //CustomField1 - Numero
                    existedRegInfo.CustomField1 = GetValueWithDefault(ControlNames.Shipping_Numero, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Numero)) ? GetValue(ControlNames.Billing_Numero) : existedRegInfo.CustomField1);
                    //CustomField2 - Complemento
                    existedRegInfo.CustomField2 = GetValueWithDefault(ControlNames.Shipping_Complemento, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_Complemento)) ? GetValue(ControlNames.Billing_Complemento) : existedRegInfo.CustomField2);
                    //CustomField3 - CityCode
                    existedRegInfo.CustomField3 = GetValueWithDefault(ControlNames.Shipping_City_Code, !string.IsNullOrEmpty(GetValue(ControlNames.Billing_City_Code)) ? GetValue(ControlNames.Billing_City_Code) : existedRegInfo.CustomField3);
                    Dao.Save<RegistrationInfo>(existedRegInfo);
                    //----------------
                }
                res = true;
            }
            catch
            {
                res = false;
            }
            return res;
        }

        #endregion

        [WebMethod]
        public static string DoCrossDomainCall(string url)
        {
            string res = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var encoding = !string.IsNullOrEmpty(response.CharacterSet) ?
                    Encoding.GetEncoding(response.CharacterSet.Replace("\"", "")) :
                    Encoding.UTF8;

                using (var stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream, encoding))
                    {
                        res = sr.ReadToEnd();
                    }
                }
            }

            return res;
        }
    }

    public enum PageActions
    {
        Registration,
        Billing,
        BillingAsync,
        Upsell,
        Confirmation,
        NoAction,
        RegistrationBilling,
        Coupon,
        Debit
    }

    public enum ControlNames
    {
        Billing_First_Name,
        Billing_Last_Name,
        Billing_Address_1,
        Billing_Address_2,
        Billing_State_US,
        Billing_State_UK,
        Billing_State_Canada,
        Billing_State_Other,
        Billing_State_Australia,
        Billing_State_Brasil,
        Billing_City,
        Billing_Zip,
        Billing_Home_Tel,
        Billing_Email,
        Billing_State_Argentina,
        Billing_CPF,
        Billing_Card_Holder,
        Billing_Numero,
        Billing_Complemento,
        Billing_City_Code,
        CC_Number,
        CC_Month,
        CC_Year,
        CC_CVV,
        CC_Type,
        Error_Message,
        Confirmation_First_Name,
        Confirmation_Last_Name,
        Confirmation_Address_1,
        Confirmation_Address_2,
        Confirmation_State,
        Confirmation_City,
        Confirmation_Zip,
        Confirmation_Home_Tel,
        Confirmation_Email,
        Confirmation_Order_ID,
        Confirmation_Friendly_Name,
        Confirmation_Neighborhood,
        Confirmation_Numero,
        Confirmation_Complemento,
        Confirmation_Card_Holder,
        Confirmation_CPF,
        Shipping_First_Name,
        Shipping_Last_Name,
        Shipping_Address_1,
        Shipping_Address_2,
        Shipping_State_US,
        Shipping_State_UK,
        Shipping_State_France,
        Shipping_State_Other,
        Shipping_State_Canada,
        Shipping_State_Australia,
        Shipping_State_Argentina,
        Shipping_State_Brasil,
        Shipping_City,
        Shipping_Zip,
        Shipping_Home_Tel,
        Shipping_Email,
        Shipping_Neighborhood,
        Shipping_Numero,
        Shipping_Complemento,
        Shipping_City_Code,
        Ship_To_Different_Address,
        Billing_Shipping_Price,
        Billing_Product_Price,
        Gift_Card,
        Billing_Country,
        Shipping_Country,
        Confirmation_Country,
        Coupon_Code,
        Billing_Total_Price
    }
}
