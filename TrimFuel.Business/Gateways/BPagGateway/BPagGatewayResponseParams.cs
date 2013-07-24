using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business.Gateways.BPagGateway
{
    public class BPagGatewayResponseParams : DelimitedResponseParams
    {
        public BPagGatewayResponseParams(string response)
            : base(response, '&')
        {
            Response = response;
        }

        public string Response { get; set; }

        private XDocument _responseXMLDoc = null;
        private XDocument ResponseXMLDoc
        {
            get
            {
                if (_responseXMLDoc == null)
                    _responseXMLDoc = XDocument.Parse(Response);
                return _responseXMLDoc;
            }
        }

        #region IGatewayResponseParams Members

        public string GetParam(string paramName, string parentNodeName)
        {
            if (string.IsNullOrEmpty(Response) || string.IsNullOrEmpty(paramName))
                return null;

            string res = null;
            GetValue(paramName, parentNodeName, ResponseXMLDoc.Root, ref res);
            return res;
        }

        void GetValue(string paramName, string parentNodeName, XElement element, ref string res)
        {
            if (element == null)
                return;

            if (!string.IsNullOrEmpty(parentNodeName))
            {
                if (element.Parent != null && element.Parent.Name == parentNodeName && element.Name == paramName)
                    res = element.Value;
            }
            else
                if (element.Name == paramName && (element.Parent == null || element.Parent.Name.LocalName.Contains("Return")))
                    res = element.Value;

            foreach (var child in element.Elements())
                GetValue(paramName, parentNodeName, child, ref res);
        }

        #endregion
    }
}
