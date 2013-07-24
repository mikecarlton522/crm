using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.NPFGateway
{
    public class NPFGateway
    {
        //private string GetResponse(string request)
        //{
        //    WebClient wc = new WebClient();
        //    wc.Credentials = new NetworkCredential("npf@coaction", "598#(25$%254coa?");
        //    byte[] responseArray = wc.UploadFile("ftp://114.141.197.192/Live_Orders_from_COM", request);
        //    return System.Text.Encoding.ASCII.GetString(responseArray);
        //}

        //public long? PostShipments(long saleID, Registration registration, string country, IList<InventoryView> inventories, out string request, out string response)
        //{
        //    //test
        //    request = @"C:\temp\20110204-225751_019.csv";
            
        //    response = GetResponse(request);

        //    return null;
        //}

        private const string NPF_URL = "ftp://114.141.197.192/Live_Orders_from_COM";

        public NPFGateway(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            this.config = config;
        }

        private IDictionary<ShipperConfig.ID, ShipperConfig> config = null;

        private const string FILE_TMP = "yyyyMMdd-HHmmss";
        public bool PostShipments(string content)
        {
            if (Config.Current.SHIPPING_TEST_MODE)
                return true;

            bool res = false;

            string path = (NPF_URL[NPF_URL.Length - 1] == '/' ? NPF_URL : NPF_URL + "/");
            path += DateTime.Now.ToString(FILE_TMP) + ".csv";
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(path);
            request.Credentials = new NetworkCredential(config[ShipperConfigEnum.NPF_Username].Value, config[ShipperConfigEnum.NPF_Password].Value);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;

            using(Stream cont = Utility.OpenStringAsStreamUTF8(content))
            {
                using (Stream ftpStream = request.GetRequestStream())
                {
                    Utility.CopyStream(cont, ftpStream);
                }
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            res = (response.StatusDescription.Contains("226"));
            response.Close();

            return res;
        }
    }
}
