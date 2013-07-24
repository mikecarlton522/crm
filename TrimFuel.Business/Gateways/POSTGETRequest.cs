using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace TrimFuel.Business.Gateways
{
    public class POSTGETRequest
    {
        public class GetRequest
        {
            /// <summary>
            /// Send current request.
            /// </summary>
            /// <returns>WebReaponse</returns>
            public HttpWebResponse Send(string targetURL)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(targetURL);
                request.Method = "GET";
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
            public HttpWebResponse Send(string content, string targetURL)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(targetURL);
                request.AllowAutoRedirect = false;
                request.Method = "POST";
                request.ContentType = "text/xml";
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
    }
}
