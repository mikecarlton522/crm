//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml;

//namespace TrimFuel.Business.Gateways.Assertigy
//{
//    public class AssertigyGatewayResponseParams : IGatewayResponseParams
//    {
//        public AssertigyGatewayResponseParams(string response)
//        {
//            Response = response;
//        }

//        public string Response { get; set; }
    
//        #region IGatewayResponseParams Members

//        public string GetParam(string paramName)
//        {
//            if (string.IsNullOrEmpty(Response) || string.IsNullOrEmpty(paramName))
//            {
//                return null;
//            }

//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(Response);
//            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
//            nsmgr.AddNamespace("x", "http://www.assertigy.com/creditcard/xmlschema/v1");
//            XmlNode node = doc.SelectSingleNode(string.Format("//x:{0}", paramName), nsmgr);
//            if (node != null)
//            {
//                return node.InnerText;
//            }
//            return null;
//        }

//        #endregion
//    }
//}
