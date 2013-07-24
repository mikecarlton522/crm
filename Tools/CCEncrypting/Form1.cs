using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrimFuel.Encrypting;
using System.Text.RegularExpressions;

namespace CCEncrypting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private CC enc = new CC();

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                tbDecrypted.Text = enc.GetDecrypted(tbEncrypted.Text);
            }
            catch (Exception ex)
            {
                mtbLog.AppendText(ex.ToString() + System.Environment.NewLine);
                tbDecrypted.Text = tbEncrypted.Text;
            }

            mtbDecrypted.Text = Regex.Replace(mtbEncrypted.Text, @"(\S+)", new MatchEvaluator(DecryptMatch), RegexOptions.Multiline);
        }

        private string DecryptMatch(Match m)
        {
            string res = null;
            try
            {
                res = enc.GetDecrypted(m.Value);
            }
            catch (Exception ex)
            {
                mtbLog.AppendText(ex.ToString() + System.Environment.NewLine);
                res = m.Value;
            }
            return res;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                tbEncrypted.Text = enc.GetEncrypted(tbDecrypted.Text);
            }
            catch (Exception ex)
            {
                mtbLog.AppendText(ex.ToString() + System.Environment.NewLine);
                tbEncrypted.Text = tbDecrypted.Text;
            }

            mtbEncrypted.Text = Regex.Replace(mtbDecrypted.Text, @"(\S+)", new MatchEvaluator(EncryptMatch), RegexOptions.Multiline);
        }

        private string EncryptMatch(Match m)
        {
            string res = null;
            try
            {
                res = enc.GetEncrypted(m.Value);
            }
            catch (Exception ex)
            {
                mtbLog.AppendText(ex.ToString() + System.Environment.NewLine);
                res = m.Value;
            }
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            mtbLog.Clear();
        }
    }
}
