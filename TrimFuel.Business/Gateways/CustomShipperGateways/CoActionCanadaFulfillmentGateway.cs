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

namespace TrimFuel.Business.Gateways.CustomShipperGateways
{
    public class CoActionCanadaFulfillmentGateway
    {
        private const string CUSTOMSHIPPER_URL = "";

        public CoActionCanadaFulfillmentGateway(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            this.config = config;
        }

        private IDictionary<ShipperConfig.ID, ShipperConfig> config = null;

        public bool PostShipments(string content)
        {
            if (Config.Current.SHIPPING_TEST_MODE)
                return true;

            bool res = false;

            string path = string.Concat(config[ShipperConfigEnum.CustomShipper_Path].Value, DateTime.Now.ToString("yyyyMMddHHmmss"), ".csv");
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(path);
            request.Credentials = new NetworkCredential(config[ShipperConfigEnum.CustomShipper_Username].Value, config[ShipperConfigEnum.CustomShipper_Password].Value);
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
