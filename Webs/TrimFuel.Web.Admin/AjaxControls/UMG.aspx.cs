using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;
using Tamir.SharpSsh;
using log4net;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class UMG : System.Web.UI.Page
    {
        private const string HISTORY_PATH = @"c:\Web\dashboard\History\";
        private const string HISTORY_SENT_PATH = @"Sent\";
        private const string FILE_NAME_TMP = "E4.{0}.{1}.{2}.DAT.pgp";
        private const string PUBLIC_PGP_KEY_PATH = @"c:\Key\UMG_Key2011.asc";

        private const string SFTP_PATH = "ftp://ftp1.teleformix.com/";
        private const string SFTP_LOGIN = "directaction";
        private const string SFTP_PASSWORD = "d@ct10n!";
        private const string SFTP_FOLDER = "Input/";
        
        private const string FTP_PATH = "ftp://campaigns.coaction.trianglecrm.com/";
        private const string FTP_LOGIN = "chris";
        private const string FTP_PASSWORD = "$ap9-12k";
        private const string FTP_FOLDER = "input/";
        
        private const string VENDOR_NAME = @"directaction";
        private const string TEST_OFFER_CODE = @"BE999999";

        private const string MESSAGE_SUCCESS = @"File has been transmitted successfully";
        private const string MESSAGE_ERROR = @"File has not been transmitted";

        readonly string INSERT_COMMAND = "insert into EnrollInfo (VendorID, TxnID, TxnDate, Data) values (@VendorID, @TxnID, @TxnDate, @Data);";
        readonly string SELECT_COMMAND = "select coalesce(max(TxnID),0) as MaxTxnID from EnrollInfo where VendorID = @VendorID;";

        protected static string _connectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
        protected static string _errorMsg = "";

        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));

        protected void Page_Load(object sender, EventArgs e)
        {
            LabelResult.Text = MESSAGE_SUCCESS;
            try
            {
                // read input parameters
                Info info = new Info();

                info.VendorID = VENDOR_NAME;
                info.OfferCode = TEST_OFFER_CODE;

                info.BillingFirstName = Page.Request.QueryString["rfname"];
                info.BillingLastName = Page.Request.QueryString["rlname"];
                info.BillingAddress1 = Page.Request.QueryString["radd1"];
                info.BillingAddress2 = Page.Request.QueryString["radd2"];
                info.BillingCity = Page.Request.QueryString["rcity"];
                info.BillingState = Page.Request.QueryString["rstate"];
                info.BillingZip = Page.Request.QueryString["rzip"];
                info.BillingPhone = Page.Request.QueryString["rphone"];
                info.EmailAddress = Page.Request.QueryString["remail"];

                // Billing method
                int bMethod = string.IsNullOrEmpty(Page.Request.QueryString["paymenttid"]) ? 0 : Convert.ToInt32(Page.Request.QueryString["paymenttid"]);
                string BillingMethod = "";
                switch (bMethod)
                {
                    case 1:
                        BillingMethod = "0003"; // American Express
                        break;
                    case 2:
                        BillingMethod = "0005"; // VISA
                        break;
                    case 3:
                        BillingMethod = "0004"; // MasterCard
                        break;
                    case 4:
                        BillingMethod = "0012"; // Discover
                        break;
                }
                info.BillingMethod = BillingMethod;

                info.BillingCard = Page.Request.QueryString["creditcard"];
                string BillingExpMonth = Page.Request.QueryString["expmonth"];
                info.BillingExpMonth = string.IsNullOrEmpty(BillingExpMonth) ? 0 : Convert.ToInt32(BillingExpMonth);
                string BillingExpYear = Page.Request.QueryString["expyear"];
                info.BillingExpYear = string.IsNullOrEmpty(BillingExpYear) ? 0 : Convert.ToInt32(BillingExpYear);

                // get last txn ID related to the current Vendor
                int TxnID = GetLastTxnID(info.VendorID) + 1;

                // generate file name
                DateTime TxnDate = DateTime.Now;
                string strID = TxnID.ToString();
                strID = (new StringBuilder().Insert(0, "0", 6 - strID.Length)).ToString() + strID;
                string fileName = string.Format(FILE_NAME_TMP, info.VendorID, TxnDate.ToString("yyyyMMdd"), strID);

                // prepare output string
                StringBuilder csv = new StringBuilder();
                TextWriter wr = new StringWriter(csv);
                WriteData(info, wr);

                // encode and save data to the local file in History folder
                EncodeAndWriteFile(csv.ToString(), PUBLIC_PGP_KEY_PATH, fileName, HISTORY_PATH + fileName);

                // send file to FTP and move it to the Sent folder
                SendFilesToFTP();

                // save transaction info to the EnrollInfo table
                SaveTxnInfo(info.VendorID, TxnID, TxnDate, wr.ToString());

            }
            catch (Exception ex)
            {
                LabelResult.Text = ex.Message.ToString();
                logger.Error(ex);
            }
        }

        private int GetLastTxnID(string VendorID)
        {
            int lastTxnID = 0;
            MySqlConnection connection = new MySqlConnection();
            MySqlCommand command = new MySqlCommand();
            try
            {
                connection.ConnectionString = (_connectionString);
                connection.Open();
                command.Connection = connection;

                command.CommandText = SELECT_COMMAND;
                command.Parameters.AddWithValue("@VendorID", VendorID);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lastTxnID = Convert.ToInt32(reader["MaxTxnID"].ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if (connection != null)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }

            return lastTxnID;
        }

        private void SaveTxnInfo(string VendorID, int TxnID, DateTime TxnDate, string Data)
        {
            IDao dao = new MySqlDao(_connectionString);
            MySqlCommand q = new MySqlCommand(INSERT_COMMAND);
            q.Parameters.Add("@VendorID", MySqlDbType.String).Value = VendorID;
            q.Parameters.Add("@TxnID", MySqlDbType.Int32).Value = TxnID;
            q.Parameters.Add("@TxnDate", MySqlDbType.DateTime).Value = TxnDate;
            q.Parameters.Add("@Data", MySqlDbType.String).Value = Data;
            int? id = dao.ExecuteScalar<int>(q);
        }

        private void WriteData(Info info, TextWriter wr)
        {
            wr.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}{29}{30}{31}",
                WriteValue(info.OfferCode, 8),
                WriteValue(info.BillingFirstName, 30),
                WriteValue(info.BillingMiddleInitial, 1),
                WriteValue(info.BillingLastName, 30),
                WriteValue(info.BillingAddress1, 30),
                WriteValue(info.BillingAddress2, 30),
                WriteValue(info.BillingCity, 25),
                WriteValue(info.BillingState, 2),
                WriteValue(info.BillingZip, 5),
                WriteValue(info.BillingZip, 4),
                WriteValue(info.BillingPhone, 10),
                WriteValue(info.EmailAddress, 50),
                WriteValue(info.DateOfBirth, 8),
                WriteValue(info.VerifyingData, 30),
                WriteValue(info.SaleDate, 8),
                WriteValue(info.SaleDate, 6),
                WriteValue(info.BillingMethod, 4),
                info.BillingExpMonth > 0 ? info.BillingExpMonth.ToString("##") : "00",
                info.BillingExpYear > 0 ? info.BillingExpYear >= 2000 ? (info.BillingExpYear - 2000).ToString("##  ") : info.BillingExpYear.ToString("##  ") : "00  ",
                WriteValue(info.BankNumber, 9),
                WriteValue(info.VendorID, 10),
                WriteValue(info.VendorSiteId, 15),
                WriteValue(info.VendorDefined1, 15),
                WriteValue(info.VendorDefined2, 15),
                WriteValue(info.ProgramDefined1, 30),
                WriteValue(info.ProgramDefined2, 30),
                WriteValue(info.ProgramDefined3, 30),
                WriteValue(info.ProgramDefined4, 50),
                WriteValue(info.ProgramDefined5, 50),
                WriteValue(info.SaleImage, 50),
                string.IsNullOrEmpty(_errorMsg) ? "" : WriteValue(_errorMsg, 50),
                0
            );
        }

        private string WriteValue(string value, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = " ";
            }
            if (value.Contains(System.Environment.NewLine))
            {
                value = value.Replace(System.Environment.NewLine, " ");
            }

            if (value.Length > length)
            {
                value = value.Substring(0, length);
            }

            string spaces = (new StringBuilder().Insert(0," ",length - value.Length)).ToString();
            value = value + spaces;
            return value;
        }

        private string WriteValue(DateTime value, int length)
        {
            if (value == null)
            {
                return (new StringBuilder().Insert(0, " ", length)).ToString();
            }
            return value.ToString("mmddyyyy");
        }

        private string WriteValue(int value, int length)
        {
            string strValue = value.ToString();
            string spaces = (new StringBuilder().Insert(0, " ", length - strValue.Length)).ToString();
            return strValue + spaces;
        }

        private void EncodeAndWriteFile(string text, string publicKeyPath, string fileName, string filePath)
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

        private PgpPublicKey ReadPublicKey(Stream inputStream)
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

        private void SendFilesToFTP()
        {
            foreach (string filePath in Directory.GetFiles(HISTORY_PATH, "*.pgp", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(filePath);
                if (UploadToFTP(HISTORY_PATH + fileName, fileName))
                    MoveToSentDirectory(fileName);
                else
                    break;
            }
        }

        private void MoveToSentDirectory(string fileName)
        {
            if (!Directory.Exists(HISTORY_PATH + HISTORY_SENT_PATH))
            {
                Directory.CreateDirectory(HISTORY_PATH + HISTORY_SENT_PATH);
            }
            File.Move(HISTORY_PATH + fileName, HISTORY_PATH + HISTORY_SENT_PATH + fileName);
        }

        private bool UploadToSFTP(string filePath, string fileName)
        {
            bool res = false;

            Sftp sftp = new Sftp(SFTP_PATH, SFTP_LOGIN, SFTP_PASSWORD);
            try
            {
                sftp.Connect();
                sftp.Put(filePath, SFTP_FOLDER + "/" + fileName);
                res = true;
            }
            finally
            {
                sftp.Close();
            }

            return res;
        }

        private bool UploadToFTP(string filePath, string fileName)
        {
            bool res = false;

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(SFTP_PATH + SFTP_FOLDER + fileName);
            request.Credentials = new NetworkCredential(SFTP_LOGIN, SFTP_PASSWORD);
            request.KeepAlive = false;
            request.UseBinary = true;
            request.UsePassive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            byte[] file = File.ReadAllBytes(filePath);
            using (Stream ftpStream = request.GetRequestStream())
            {
                ftpStream.Write(file, 0, file.Length);
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            res = (response.StatusDescription.Contains("226"));
            response.Close();
            res = true;

            return res;
        }

        private class Info
        {
            public string OfferCode { get; set; }
            public string BillingFirstName { get; set; }
            public string BillingMiddleInitial { get; set; }
            public string BillingLastName { get; set; }
            public string BillingAddress1 { get; set; }
            public string BillingAddress2 { get; set; }
            public string BillingCity { get; set; }
            public string BillingState { get; set; }
            public string BillingZip { get; set; }
            public string BillingPhone { get; set; }
            public string EmailAddress { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string VerifyingData { get; set; }
            public DateTime SaleDate { get; set; }
            public string BillingMethod { get; set; }
            public string BillingCard { get; set; }
            public int BillingExpMonth { get; set; }
            public int BillingExpYear { get; set; }
            public string BankNumber { get; set; }
            public string VendorID { get; set; }
            public string VendorSiteId { get; set; }
            public string VendorDefined1 { get; set; }
            public string VendorDefined2 { get; set; }
            public string ProgramDefined1 { get; set; }
            public string ProgramDefined2 { get; set; }
            public string ProgramDefined3 { get; set; }
            public string ProgramDefined4 { get; set; }
            public string ProgramDefined5 { get; set; }
            public string SaleImage { get; set; }
        }
    }
}
