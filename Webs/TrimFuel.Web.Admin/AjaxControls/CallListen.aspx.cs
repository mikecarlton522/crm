using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class CallListen : System.Web.UI.Page
    {
        const string USERNAME = "TriangleClient";
        const string PASSWORD = "R3cording$4U";
        const string LOGINPAGE = "http://rhylquality.com/ClientPages/TriangleMedia/TriangleMediaLogin.aspx";
        const string LOADINGRECORDPAGE = "http://rhylquality.com/ClientPages/TriangleMedia/LoadRecording.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            string contactID = Request.QueryString["ContactID"];
            if (string.IsNullOrEmpty(contactID))
            {
                Response.Write("Invalid URL.");
                return;
            }

            CallManager callManager = new CallManager(
                USERNAME,
                PASSWORD,
                LOGINPAGE,
                LOADINGRECORDPAGE);
            byte[] outputbytes = callManager.GetResponseByContactID(contactID);
            if (outputbytes == null)
            {
                Response.Write("Error loading call.");
            }
            else
            {
                Response.ContentType = "audio/x-wav";
                Response.OutputStream.Write(outputbytes, 0, (int)outputbytes.Length);
            }
        }
    }

    public class GetRequest
    {
        /// <summary>
        /// Send current request.
        /// </summary>
        /// <returns>WebReaponse</returns>
        public HttpWebResponse Send(string targetURL, string sCookies)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(targetURL);
            request.Method = "GET";
            if (!string.IsNullOrEmpty(sCookies))
            {
                request.Headers.Add(HttpRequestHeader.Cookie, sCookies);
            }
            else
            {
                CookieContainer cookies = new CookieContainer();
                request.CookieContainer = cookies;
            }
            Stream requestStream = null;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (requestStream != null)
                    requestStream.Close();
            }
        }
    }

    public class PostRequest
    {
        /// <summary>
        /// Send current request.
        /// </summary>
        /// <returns>WebReaponse</returns>
        public HttpWebResponse Send(string content, string targetURL, string sCookies)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(targetURL);
            request.AllowAutoRedirect = false;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(sCookies))
            {
                request.Headers.Add(HttpRequestHeader.Cookie, sCookies);
            }
            else
            {
                CookieContainer cookies = new CookieContainer();
                request.CookieContainer = cookies;
            }
            Stream requestStream = null;
            try
            {
                var bytes = ASCIIEncoding.Default.GetBytes(content.ToCharArray());
                request.ContentLength = bytes.Length;
                requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (requestStream != null)
                    requestStream.Close();
            }
        }
    }

    public class CallManager
    {
        string _userName;
        string _password;
        string _loginPageAddress;
        string _loadRecordingPageAddress;
        public string LoadRecordingPageAddress
        {
            get
            {
                return _loadRecordingPageAddress;
            }
        }
        public CallManager(string userName, string password, string loginPageAddress, string loadingRecordingPageAddress)
        {
            _userName = userName;
            _password = password;
            _loadRecordingPageAddress = loadingRecordingPageAddress;
            _loginPageAddress = loginPageAddress;
        }
        public byte[] GetResponseByContactID(string contactID)
        {
            GetRequest getRequest = null;
            PostRequest postRequest = null;
            HttpWebResponse webResponse = null;
            int count = 0;
            try
            {
                //get login page
                getRequest = new GetRequest();
                webResponse = getRequest.Send(_loginPageAddress, "");
                string sCookies = string.IsNullOrEmpty(webResponse.Headers["Set-Cookie"]) ? "" : webResponse.Headers["Set-Cookie"];

                //get html code of login page
                var formsBytes = new byte[webResponse.ContentLength];
                webResponse.GetResponseStream().Read(formsBytes, 0, (int)webResponse.ContentLength);
                string response = ASCIIEncoding.Default.GetString(formsBytes);

                Regex regex = null;
                Match match = null;
                //get __VIEWSTATE value from form
                regex = new Regex("id=\"__VIEWSTATE\" value=\"(?<viewstate>[^\"]+)\"");
                match = regex.Match(response);
                string viewstate = match.Groups["viewstate"].Value;

                //get __EVENTVALIDATION value from form
                regex = new Regex("id=\"__EVENTVALIDATION\" value=\"(?<eventvalidation>[^\"]+)\"");
                match = regex.Match(response);
                string eventvalidation = match.Groups["eventvalidation"].Value;

                //create qyery string to authenticate
                StringBuilder queryString = new StringBuilder();
                queryString.Append("__VIEWSTATE=" + HttpUtility.UrlEncode(viewstate));
                queryString.Append("&txtUsername=TriangleClient");
                queryString.Append("&txtPassword=R3cording$4U");
                queryString.Append("&btnLogin=Login");
                queryString.Append("&__EVENTVALIDATION=" + HttpUtility.UrlEncode(eventvalidation));

                //sent POST request to authentication
                postRequest = new PostRequest();
                webResponse = postRequest.Send(
                    queryString.ToString(),
                    _loginPageAddress,
                    "");
                sCookies = string.IsNullOrEmpty(webResponse.Headers["Set-Cookie"]) ? "" : webResponse.Headers["Set-Cookie"];

                //get audio record
                getRequest = new GetRequest();
                webResponse = getRequest.Send(_loadRecordingPageAddress + "?ContactID=" + contactID, sCookies);
                if (webResponse != null)
                {
                    var bytes = new byte[webResponse.ContentLength];
                    int length = (int)webResponse.ContentLength;
                    while (count != length)
                    {
                        count += webResponse.GetResponseStream().Read(bytes, count, length - count);
                    }
                    return bytes;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
