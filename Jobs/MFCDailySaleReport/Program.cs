using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using System.IO;
using System.Globalization;
using TrimFuel.Model;
using log4net;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;
using System.Net;

namespace MFCDailySaleReport
{
    class Program
    {
        private const string HISTORY_PATH = @"History\";
        private const string HISTORY_SENT_PATH = @"Sent\";
        private const string FILE_NAME_TMP = "TRIOrders{0}.csv";
        private const string PUBLIC_PGP_KEY_PATH = @"c:\Key\TRIOrders_pub.asc";
        private const string FTP_PATH = "ftp://64.34.162.102/";
        private const string FTP_LOGIN = "DailyOrders";
        private const string FTP_PASSWORD = "_MZa8A12Fp";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));

            try
            {
                DateTime reportDate = DateTime.Today.AddDays(-1);

                ReportService reportService = new ReportService();
                IList<SaleBillingView> sales = reportService.CreateDailySaleReport(reportDate);

                string fileName = string.Format(FILE_NAME_TMP, reportDate.ToString("yyyyMMdd"));
                if (!Directory.Exists(HISTORY_PATH))
                {
                    Directory.CreateDirectory(HISTORY_PATH);
                }

                CultureInfo enUS = CultureInfo.GetCultureInfo("en-US");

                StringBuilder csv = new StringBuilder();
                using (TextWriter wr = new StringWriter(csv))
                {
                    WriteHeader(wr);
                    foreach (var sale in sales)
                    {
                        if (sale.Billing == null)
                        {
                            sale.Billing = new Billing();
                        }
                        WriteSale(sale, wr, enUS);
                    }
                }

                EncodeAndWriteFile(csv.ToString(), PUBLIC_PGP_KEY_PATH, fileName, HISTORY_PATH + fileName);
                SendFilesToFTP();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void WriteHeader(TextWriter wr)
        {
            wr.WriteLine("BillingID,OrderDate,First Name,Last Name,Address 1,Address 2,City,State,Zip,Phone,Credit Card Number,Exp Date,CVV,Total Order Amount,Shipping Amount,1,Total Order Amount,Order Data,0");
        }

        private static void WriteSale(SaleBillingView sale, TextWriter wr, CultureInfo loc)
        {
            wr.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
                sale.Billing.BillingID,
                FixCSVValue(sale.SaleDT),
                FixCSVValue(sale.Billing.FirstName),
                FixCSVValue(sale.Billing.LastName),
                FixCSVValue(sale.Billing.Address1),
                FixCSVValue(sale.Billing.Address2),
                FixCSVValue(sale.Billing.City),
                FixCSVValue(sale.Billing.State),
                FixCSVValue(sale.Billing.Zip),
                FixCSVValue(sale.Billing.Phone),
                FixCSVValue(sale.Billing.CreditCard),
                string.Format("{0}/{1}",
                    sale.Billing.ExpMonth != null ? sale.Billing.ExpMonth.Value.ToString("##") : "00",
                    sale.Billing.ExpYear != null ? sale.Billing.ExpYear >= 2000 ? (sale.Billing.ExpYear - 2000).Value.ToString("##") : sale.Billing.ExpYear.Value.ToString("##") : "00"),
                sale.Billing.CVV,
                FixCSVValue(sale.TotalAmount, loc),
                FixCSVValue(sale.ShippingAmount, loc),
                1,
                FixCSVValue(sale.TotalAmount, loc),
                FixCSVValue(sale.SaleDT),
                0);

        }

        private static string FixCSVValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "\"\"";
            }
            if (value.Contains("\"") || value.Contains(",") || value.Contains(System.Environment.NewLine))
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        private static string FixCSVValue(DateTime? value)
        {
            if (value == null)
            {
                return "\"\"";
            }
            return value.Value.ToString("yyyy-MM-dd");
        }

        private static string FixCSVValue(decimal? value, CultureInfo loc)
        {
            if (value == null)
            {
                return "\"\"";
            }
            return value.Value.ToString("0.00", loc);
        }

        private static void EncodeAndWriteFile(string text, string publicKeyPath, string fileName, string filePath)
        {
            PgpPublicKey publicKey = null;
            using(Stream publicKeyStream = File.OpenRead(publicKeyPath))
	        {
                publicKey = ReadPublicKey(publicKeyStream);
	        }

            byte[] clearText = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] compressedText = null;
            using(MemoryStream compressedStream = new MemoryStream())
            {
                PgpCompressedDataGenerator compresser = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Uncompressed);
                
                //PgpUtilities.WriteFileToLiteralData(compresser.Open(compressedStream), PgpLiteralData.Binary, new FileInfo(fileName));
                using (Stream compresserStream = compresser.Open(compressedStream))
                {
                    PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
                    using (Stream literalStream = lData.Open(compressedStream, PgpLiteralData.Binary, fileName, clearText.Length, DateTime.UtcNow))
                    {
                        literalStream.Write(clearText, 0, clearText.Length);
                    }
                }
                compressedText = compressedStream.ToArray();
            }

            using (Stream file = File.Create(filePath))
            {
                using (Stream outputStream = new ArmoredOutputStream(file))
                {
                    PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, new SecureRandom());
                    cPk.AddMethod(publicKey);
                    using (Stream encStream = cPk.Open(outputStream, compressedText.Length))
                    {
                        encStream.Write(compressedText, 0, compressedText.Length);
                    }
                }
            }
        }

        private static PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);

            //
            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            //

            //
            // iterate through the key rings.
            //

            foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey k in kRing.GetPublicKeys())
                {
                    if (k.IsEncryptionKey)
                    {
                        return k;
                    }
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        public static void SendFilesToFTP()
        {
            foreach (string filePath in Directory.GetFiles(HISTORY_PATH, "*.csv", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(filePath);
                if (UploadToFTPSite(HISTORY_PATH + fileName, fileName))
                {
                    MoveToSentDirectory(fileName);
                }
            }
        }

        private static void MoveToSentDirectory(string fileName)
        {
            if (!Directory.Exists(HISTORY_PATH + HISTORY_SENT_PATH))
            {
                Directory.CreateDirectory(HISTORY_PATH + HISTORY_SENT_PATH);
            }
            File.Move(HISTORY_PATH + fileName, HISTORY_PATH + HISTORY_SENT_PATH + fileName);
        }

        public static bool UploadToFTPSite(string filePath, string fileName)
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
