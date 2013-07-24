using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace BillingAPIDirectPOSTRequest
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
                soapResponse = HttpPOSTRequest(textBox1.Text, soapRequest);
            }
            catch (Exception ex)
            {
                soapResponse = ex.ToString();
            }
            textBox3.Text = soapResponse;
        }

        private static string HttpPOSTRequest(string url, string body)
        {
            string res = null;

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            res = webClient.UploadString(url, body);

            return res;
        }
    }
}
