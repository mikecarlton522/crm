using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace BillingAPIDirectSoapRequest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string soapRequest = textBox2.Text;
            string soapResponse = null;
            try
            {
                soapResponse = HttpSOAPRequest(textBox1.Text, textBox4.Text, soapRequest);
            }
            catch (Exception ex)
            {
                soapResponse = ex.ToString();
            }
            textBox3.Text = soapResponse;
        }

        private static string HttpSOAPRequest(string url, string action, string body)
        {
            string res = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Headers.Add("SOAPAction", "\"http://trianglecrm.com/" + action + "\"");
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.Accept = "text/xml";
            httpRequest.Method = "POST";

            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
            strOut.Write(body);
            strOut.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
            res = strIn.ReadToEnd();
            strIn.Close();

            return res;
        }
    }
}
