using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Net;
using System.IO;

namespace EPacificLinkLeads
{
    class Program
    {
        private const string HISTORY_PATH = @"History\";
        private const string FILE_NAME_TMP = "Ecigs_{0}.csv";
        private const string TODAY_PATH = @"Today\";
        private const string TODAY_FILE_NAME = "Ecigs.csv";
        private const string FTP_PATH = "ftp://ftp.epacificlink.com/";
        private const string FTP_LOGIN = "cigg@epacificlink.com";
        private const string FTP_PASSWORD = "smartcig!12";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));

            try
            {
                if (File.Exists(TODAY_PATH + TODAY_FILE_NAME))
                {
                    FileInfo fileInfo = new FileInfo(TODAY_PATH + TODAY_FILE_NAME);
                    if (fileInfo.Length > 0)
                    {
                        string fileName = GenerateFileName();
                        if (UploadToFTP(TODAY_PATH + TODAY_FILE_NAME, fileName))
                        {
                            MoveToHistoryDirectory(fileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static string GenerateFileName()
        {
            int maxNumber = 1000;
            int i = 2;
            string name = string.Format(FILE_NAME_TMP, DateTime.Today.ToString("yyyyMMdd"));
            while (i < maxNumber && File.Exists(HISTORY_PATH + name))
            {
                name = string.Format(FILE_NAME_TMP, DateTime.Today.ToString("yyyyMMdd") + "_" + i.ToString());
                i++;
            }
            return name;
        }

        private static void MoveToHistoryDirectory(string fileName)
        {
            if (!Directory.Exists(HISTORY_PATH))
            {
                Directory.CreateDirectory(HISTORY_PATH);
            }
            File.Move(TODAY_PATH + TODAY_FILE_NAME, HISTORY_PATH + fileName);
        }

        public static bool UploadToFTP(string filePath, string fileName)
        {
            bool res = false;

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTP_PATH + fileName);
            request.Credentials = new NetworkCredential(FTP_LOGIN, FTP_PASSWORD);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;

            byte[] file = File.ReadAllBytes(filePath);
            using (Stream ftpStream = request.GetRequestStream())
            {
                ftpStream.Write(file, 0, file.Length);
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            res = (response.StatusDescription.Contains("226"));
            response.Close();

            return res;
        }
    }
}
