using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.EnterpriseServices;
using TrimFuel.Encrypting;
using System.IO;

namespace TrimFuel.ComPlus
{
    [GuidAttribute("C2F0D295-BD5B-45e5-A83B-84E106B76777")]
    [ComVisible(true)]
    [ProgId("TrimFuel.Encrypting.1.0")]
    public class Encrypting : ServicedComponent
    {
        private const string LOG_FILE_DIRECTORY = @"c:\Logs\";
        private const string LOG_FILE_NAME = @"complus_encrypting.log";

        [ComVisible(true)]
        public string GetEncryptedCC(string cc)
        {
            string res = cc ?? string.Empty;
            try
            {
                res = (new CC()).GetEncrypted(cc);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return res;
        }

        [ComVisible(true)]
        public string GetDecryptedCC(string cc)
        {
            string res = cc ?? string.Empty;
            try
            {
                res = (new CC()).GetDecrypted(cc);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return res;
        }

        [ComVisible(true)]
        public string GetSearchHashCC(string cc)
        {
            string res = string.Empty;
            try
            {
                res = (new CC()).GetSearchHash(cc);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return res;
        }

        private void LogException(Exception ex)
        {
            try
            {
                if (!Directory.Exists(LOG_FILE_DIRECTORY))
                {
                    Directory.CreateDirectory(LOG_FILE_DIRECTORY);
                }
                File.AppendAllText(LOG_FILE_DIRECTORY + LOG_FILE_NAME, DateTime.Now.ToString() + "--------------------------------------" + Environment.NewLine + ex.ToString() + Environment.NewLine);
            }
            catch { }
        }
    }
}
