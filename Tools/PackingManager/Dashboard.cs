using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using LumenWorks.Framework.IO.Csv;

using TrimFuel.Tools.PackingManager.Endicia;
using TrimFuel.Tools.PackingManager.Logic;

namespace TrimFuel.Tools.PackingManager
{
    public partial class Dashboard : Form
    {
        private enum PrintMode { PrintSingle = 1, WriteSingle = 2, Append = 3 }

        private IList<LabelRequest> printQueue = new List<LabelRequest>();

        private IList<string> packageSlipList = new List<string>();

        #region General

        private const NumberStyles NUMBER_STYLE = System.Globalization.NumberStyles.AllowDecimalPoint;

        private readonly CultureInfo CULTURE = new CultureInfo("en-US");

        private const double DEFAULT_PACKAGE_WEIGHT = 0.3;

        private CheckBox selectAllBox;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            Rectangle rectangle = grid.GetCellDisplayRectangle(0, -1, true);

            selectAllBox = new CheckBox();

            selectAllBox.Size = new Size(18, 18);

            selectAllBox.BackColor = Color.Transparent;

            selectAllBox.Location = new Point(rectangle.Location.X + (rectangle.Width / 2) - 9, rectangle.Location.Y + (rectangle.Height / 2) - 9);

            selectAllBox.CheckedChanged += new EventHandler(selectAllBox_CheckedChanged);

            selectAllBox.Checked = true;

            grid.Controls.Add(selectAllBox);

            if (PrinterSettings.InstalledPrinters.Count > 0)
            {
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    labelPrinter.Items.Add(printer);

                    slipPrinter.Items.Add(printer);
                }

                labelPrinter.SelectedIndex = 0;

                selectedLabelPrinter = labelPrinter.SelectedItem.ToString();

                slipPrinter.SelectedIndex = 0;

                selectedSlipPrinter = slipPrinter.SelectedItem.ToString();

                selectedPackingWeight = (double)packageWeight.Value;
            }

            if (ConfigurationManager.ConnectionStrings.Count > 0)
            {
                foreach (ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings)
                {
                    if (!(string.IsNullOrEmpty(settings.ConnectionString) || !string.IsNullOrEmpty(settings.ProviderName)))
                    {
                        client.Items.Add(settings.Name);
                    }
                }
                
                client.SelectedIndex = 0;
                
                selectedClient = client.SelectedItem.ToString();
            }

            mailClass.SelectedIndex = 1;

            selectedMailClass = mailClass.SelectedItem.ToString();

            printMode.SelectedIndex = 1;

            selectedPrintMode = PrintMode.WriteSingle;

            RefreshSettingsGrid();
        }

        private void Dashboard_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rectangle = grid.GetCellDisplayRectangle(0, -1, true);

            selectAllBox.Location = new Point(rectangle.Location.X + (rectangle.Width / 2) - 9, rectangle.Location.Y + (rectangle.Height / 2) - 9);
        }

        private delegate void ShowMessageHandler(string message);

        private void ShowMessage(string message)
        {
            if (applicationLog.InvokeRequired)
            {
                ShowMessageHandler handler = new ShowMessageHandler(ShowMessage);

                Invoke(handler, new object[] { message });
            }
            else
            {
                applicationLog.AppendText(string.Format("[{0}] - {1}{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message, Environment.NewLine));
            }
        }

        private delegate void RefreshSettingsGridHandler();

        private void RefreshSettingsGrid()
        {
            if (settingsGrid.InvokeRequired)
            {
                RefreshSettingsGridHandler handler = new RefreshSettingsGridHandler(RefreshSettingsGrid);

                Invoke(handler);
            }
            else
            {
                settingsGrid.Rows.Add("Packing slip template path", Settings.TemplatePath);
                settingsGrid.Rows.Add("Archive path", Settings.ArchivePath);
                settingsGrid.Rows.Add("Print interval", string.Format("{0} seconds", Settings.Interval));
                settingsGrid.Rows.Add("Endicia passphrase", Settings.EndiciaPassPhrase);
                settingsGrid.Rows.Add("Endicia requester ID", Settings.EndiciaRequesterID);
                settingsGrid.Rows.Add("Endicia account ID", Settings.EndiciaAccountID);
                settingsGrid.Rows.Add("Endicia test mode", Settings.EndiciaTestMode);
            }
        }

        private void selectAllBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < grid.RowCount; i++)
            {
                grid[0, i].Value = selectAllBox.Checked;
            }

            grid.EndEdit();
        }

        private void labelPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLabelPrinter = labelPrinter.SelectedItem.ToString();
        }

        private void slipPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSlipPrinter = slipPrinter.SelectedItem.ToString();
        }

        private void packageWeight_ValueChanged(object sender, EventArgs e)
        {
            selectedPackingWeight = (double)packageWeight.Value;
        }

        private void mailClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMailClass = mailClass.SelectedItem.ToString();
        }

        private void printMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPrintMode = (PrintMode)(printMode.SelectedIndex + 1);
        }

        private void client_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedClient = client.SelectedItem.ToString();
        }

        #endregion

        #region Import

        private string importFile;

        private Thread importThread;

        private bool importComplete = true;

        private void open_Click(object sender, EventArgs e)
        {
            if (!importComplete)
                return;

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Choose a file",
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                printQueue.Clear();

                packageSlipList.Clear();

                grid.Rows.Clear();

                importFile = dialog.FileName;

                importThread = new Thread(ImportFile);

                importThread.IsBackground = true;

                importThread.Start();
            }
        }

        private void ImportFile()
        {
            importComplete = false;

            try
            {
                int count = 0;
                int successCount = 0;
                int recordIndex = 0;

                using (CachedCsvReader csv = new CachedCsvReader(new StreamReader(importFile), true, '\t'))
                {
                    count = csv.Count();

                    csv.MoveToStart();

                    ShowMessage(string.Format("(IMPORT) - Begin reading file \"{0}\" containing {1} records.", Path.GetFileName(importFile), count));

                    while (csv.ReadNextRecord())
                    {
                        recordIndex++;

                        try
                        {
                            ShowMessage(string.Format("(IMPORT) - Begin reading record {0} out of {1}.", recordIndex, count));

                            string customerName = csv["CustomerName"];
                            string referenceNum = csv["ReferenceNum"];
                            string totWeight = csv["TotWeight"];
                            string connectionString = ConfigurationManager.ConnectionStrings[selectedClient].ConnectionString;

                            if (string.IsNullOrEmpty(connectionString))
                                throw new Exception("Invalid ConnectionString");

                            long saleID = long.TryParse(referenceNum, out saleID) ? saleID : -1;

                            if (saleID < 0)
                                throw new Exception("Invalid ReferenceNum");

                            double weight = double.TryParse(totWeight, NUMBER_STYLE, CULTURE, out weight) ? weight : -1;

                            if (weight < 0)
                                throw new Exception("Invalid TotWeight");

                            weight = weight * 16;

                            weight = Math.Round(weight, 1);

                            LabelRequest labelRequest = TrimFuelService.FetchLabelRequest(connectionString, saleID, recordIndex);

                            if (labelRequest == null)
                                throw new Exception(string.Format("SaleID({0}) cancelled or not found.", saleID));

                            if (!TrimFuelService.IsValidUSZip(labelRequest.ToPostalCode))
                                throw new Exception("Invalid US zip code");

                            labelRequest.PartnerCustomerID = customerName;
                            labelRequest.PartnerTransactionID = referenceNum;
                            labelRequest.RubberStamp1 = string.Concat("Order #", referenceNum);
                            labelRequest.WeightOz = weight;
                            labelRequest.MailClass = selectedMailClass;
                            labelRequest.Services = new SpecialServices() { InsuredMail = "OFF", SignatureConfirmation = "OFF" };
                            labelRequest.MailpieceShape = "Parcel";
                            labelRequest.ResponseOptions = new ResponseOptions() { PostagePrice = "TRUE" };
                            labelRequest.LabelSize = "4x6";
                            labelRequest.FromCompany = "TLS";
                            labelRequest.FromCity = "Auburn Hills";
                            labelRequest.FromState = "MI";
                            labelRequest.FromPostalCode = "48326";
                            labelRequest.ReturnAddress1 = "1091 Centre Road, Suite #100";
                            labelRequest.ImageFormat = "JPEG";
                            labelRequest.PassPhrase = Settings.EndiciaPassPhrase;
                            labelRequest.RequesterID = Settings.EndiciaRequesterID;
                            labelRequest.AccountID = Settings.EndiciaAccountID;
                            labelRequest.Test = Settings.EndiciaTestMode;

                            AddRow(labelRequest);

                            ShowMessage(string.Format("(IMPORT) - Record #{0} added to grid (SaleID={1}).", recordIndex, saleID));

                            successCount++;

                        }
                        catch (Exception ex)
                        {
                            ShowMessage(string.Format("(IMPORT) - Record #{0}. An error occured! {1}", recordIndex, ex.Message));
                        }
                    }

                    ShowMessage(string.Format("(IMPORT) - End reading file \"{0}\". Successfully added {1} out of {2} records to grid", Path.GetFileName(importFile), successCount, recordIndex));
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

            importComplete = true;

            importThread.Join();
        }

        private delegate void AddRowHandler(LabelRequest labelRequest);

        private void AddRow(LabelRequest labelRequest)
        {
            if (grid.InvokeRequired)
            {
                AddRowHandler handler = new AddRowHandler(AddRow);

                Invoke(handler, new object[] { labelRequest });
            }
            else
            {
                grid.Rows.Add(selectAllBox.Checked, labelRequest.CurrentIndex,
                    selectedClient, labelRequest.PartnerTransactionID, labelRequest.SKUCodesDisplayValue,
                    labelRequest.ToName, labelRequest.ToAddress1, labelRequest.ToAddress2, labelRequest.ToCity, labelRequest.ToPostalCode, labelRequest.ToState, labelRequest.ToPhone,
                    labelRequest.WeightOz);

                grid.EndEdit();

                printQueue.Add(labelRequest);
            }
        }

        #endregion

        #region Print Packing Slips

        private Thread printPackageSlipThread;

        private string selectedSlipPrinter;

        private bool printPackageSlipComplete = true;

        private PrintMode selectedPrintMode;

        private void printPackingSlips_Click(object sender, EventArgs e)
        {
            if (!importComplete)
                return;

            if (printQueue.Count > 0)
            {
                printPackageSlipThread = new Thread(PrintPackageSlips);

                printPackageSlipThread.IsBackground = true;

                printPackageSlipThread.Start();
            }
        }        

        private void PrintPackageSlips()
        {
            printPackageSlipComplete = false;

            try
            {
                int count = printQueue.Count;
                int successCount = 0;
                int recordIndex = 0;

                while (recordIndex < count)
                {
                    if (Convert.ToBoolean(grid.Rows[recordIndex].Cells[0].Value))
                    {
                        LabelRequest request = printQueue[recordIndex];

                        recordIndex++;

                        try
                        {
                            ShowMessage(string.Format("(PACKING SLIP: {1}) - Record #{0}. Begin printing packing slip.", request.CurrentIndex, request.PartnerTransactionID));

                            if (selectedClient != "Triangle Media Corp")
                            {
                                if (TrimFuelService.QuotaExceeded(selectedClient))
                                    throw new Exception("Client does not have enough funds.");
                            }

                            PackageSlip packageSlip = new PackageSlip();

                            switch (selectedPrintMode)
                            {
                                case PrintMode.Append:
                                    {
                                        string target = Path.Combine(Settings.ArchivePath, string.Concat("TEMP/", request.PartnerTransactionID, ".docx"));
                                        
                                        packageSlip.Save(Path.Combine(Settings.TemplatePath, string.Concat(request.ProductName, ".docx")),
                                            request.PartnerTransactionID, request.ToName, request.ToAddress1, request.ToAddress2, request.ToCity, request.ToState, request.ToZIP4, request.ToCountry,
                                            request.Date.ToString("yyyy/MM/dd"), request.SKUQuantities, request.SKUCodes, request.SKUDescriptions, selectedMailClass,
                                            target, selectedClient);

                                        packageSlipList.Add(target);

                                        ShowMessage(string.Format("(PACKING SLIP: {1}) - Record #{0}. Packing slip prepared for appending.", request.CurrentIndex, request.PartnerTransactionID));
                                        
                                        break;
                                    }

                                case PrintMode.PrintSingle:
                                    {
                                        packageSlip.Print(Path.Combine(Settings.TemplatePath, string.Concat(request.ProductName, ".docx")), selectedSlipPrinter,
                                            request.PartnerTransactionID, request.ToName, request.ToAddress1, request.ToAddress2, request.ToCity, request.ToState, request.ToZIP4, request.ToCountry,
                                            request.Date.ToString("yyyy/MM/dd"), request.SKUQuantities, request.SKUCodes, request.SKUDescriptions, selectedMailClass,
                                            Path.Combine(Settings.ArchivePath, string.Concat(request.PartnerTransactionID, ".docx")), selectedClient);

                                        ShowMessage(string.Format("(PACKING SLIP: {2}) - Record #{0}. Packing slip sent to printer ({1}).", request.CurrentIndex, this.selectedSlipPrinter, request.PartnerTransactionID));
                                        
                                        ShowMessage(string.Format("(PACKING SLIP: {1}) - Waiting {0} seconds before sending next packing slip.", Settings.Interval, request.PartnerTransactionID));
                                        
                                        Thread.Sleep(Settings.Interval * 1000);
                                        
                                        break;
                                    }
                                case PrintMode.WriteSingle:
                                    {
                                        packageSlip.Save(Path.Combine(Settings.TemplatePath, string.Concat(request.ProductName, ".docx")),
                                            request.PartnerTransactionID, request.ToName, request.ToAddress1, request.ToAddress2, request.ToCity, request.ToState, request.ToZIP4, request.ToCountry,
                                            request.Date.ToString("yyyy/MM/dd"), request.SKUQuantities, request.SKUCodes, request.SKUDescriptions, selectedMailClass,
                                            Path.Combine(Settings.ArchivePath, string.Concat(request.PartnerTransactionID, ".docx")), selectedClient );

                                        ShowMessage(string.Format("(PACKING SLIP: {1}) - Record #{0}. Packing slip saved to archive folder.", request.CurrentIndex, request.PartnerTransactionID));

                                        break;
                                    }
                            }

                            successCount++;

                        }
                        catch (Exception ex)
                        {
                            ShowMessage(string.Format("(PACKING SLIP: {2}) - Record #{0}. An error occured! {1}", request.CurrentIndex, ex.Message, request.PartnerTransactionID));
                        }
                        finally
                        {
                            ShowMessage(string.Format("(PACKING SLIP: {1}) - Record #{0}. End printing packing slip.", request.CurrentIndex, request.PartnerTransactionID));
                        }
                    }
                    else
                    {
                        recordIndex++;
                    }
                }

                if (selectedPrintMode == PrintMode.Append)
                {
                    ShowMessage("(PACKING SLIP) - Start merging packing slips.");

                    try
                    {
                        PackageSlip packageSlipMerger = new PackageSlip();

                        string outputFile = Path.Combine(Settings.ArchivePath, string.Concat(DateTime.Now.ToString("yyyyMMddHHmmss"), "_PackingSlips.docx"));

                        packageSlipMerger.Merge(packageSlipList.ToArray(), outputFile, false);

                        ShowMessage("(PACKING SLIP) - Successfully merged all packing slips.");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(string.Format("(PACKING SLIP) - Error while merging packing slips. {0}", ex.Message));
                    }
                    finally
                    {
                        foreach (string file in Directory.GetFiles(Path.Combine(Settings.ArchivePath, "temp")))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

            printPackageSlipComplete = true;

            packageSlipList.Clear();

            printPackageSlipThread.Join();
        }        

        #endregion

        #region Print Labels

        private string selectedClient;   

        private string selectedMailClass;

        private double selectedPackingWeight;

        private Thread printLabelThread;

        private Image labelImage;

        private bool printLabelComplete = true;

        private string selectedLabelPrinter;

        private void printLabels_Click(object sender, EventArgs e)
        {
            if (importComplete && (this.printQueue.Count > 0))
            {
                printLabelThread = new Thread(new ThreadStart(this.PrintLabels));
                printLabelThread.IsBackground = true;
                printLabelThread.Start();
            }
        }

        private void PrintLabels()
        {
            printLabelComplete = false;

            try
            {
                int count = printQueue.Count;
                int successCount = 0;
                int recordIndex = 0;

                while (recordIndex < count)
                {
                    if (Convert.ToBoolean(grid.Rows[recordIndex].Cells[0].Value))
                    {
                        LabelRequest request = printQueue[recordIndex];

                        request.WeightOz += selectedPackingWeight;

                        recordIndex++;

                        try
                        {
                            ShowMessage(string.Format("(LABEL: {1}) - Record #{0}. Begin printing label.", request.CurrentIndex, request.PartnerTransactionID));

                            if (selectedClient != "Triangle Media Corp")
                            {
                                if (TrimFuelService.QuotaExceeded(selectedClient))
                                    throw new Exception("Client does not have enough funds.");
                            }

                            LabelRequestResponse response = null;

                            ShowMessage(string.Format("(LABEL: {1}) - Record #{0}. Requesting label from Endicia.", request.CurrentIndex, request.PartnerTransactionID));

                            using (EwsLabelServiceSoapClient client = new EwsLabelServiceSoapClient())
                            {
                                response = client.GetPostageLabel(request);
                            }

                            if (response != null && string.IsNullOrEmpty(response.ErrorMessage))
                            {
                                ShowMessage(string.Format("(LABEL: {4}) - Record #{0}. Label received from endicia. Weight: {1}. TrackingNumber: {2}. Postage costs: ${3}", request.CurrentIndex, request.WeightOz, response.TrackingNumber, response.FinalPostage.ToString("f2"), request.PartnerTransactionID));

                                ShowMessage(string.Format("(LABEL: {2}) - Record #{0}. Sending label to printer ({1}).", request.CurrentIndex, selectedLabelPrinter, request.PartnerTransactionID));

                                byte[] bytes = Convert.FromBase64String(response.Base64LabelImage);

                                MemoryStream stream = new MemoryStream(bytes);

                                labelImage = Image.FromStream(stream);

                                PrintDocument label = new PrintDocument();

                                label.DocumentName = "Label";

                                label.OriginAtMargins = true;

                                label.PrinterSettings.PrinterName = selectedLabelPrinter;

                                label.PrintPage += PrintLabel;

                                label.Print();

                                File.WriteAllBytes(Path.Combine(Settings.ArchivePath, string.Concat(request.PartnerTransactionID, ".jpg")), bytes);

                                ShowMessage(string.Format("(LABEL: {1}) - Record #{0}. Label saved to archive folder.", request.CurrentIndex, request.PartnerTransactionID));

                                string connectionString = ConfigurationManager.ConnectionStrings[selectedClient].ConnectionString;

                                ShowMessage(string.Format("(LABEL: {1}) - Record #{0}. Sending postage details to CRM.", request.CurrentIndex, request.PartnerTransactionID));

                                TrimFuelService.FinalizeTransaction(request, response, CULTURE);

                                if (selectedClient != "Triangle Media Corp")
                                    TrimFuelService.ReducePostageQuota(response.FinalPostage, selectedClient);

                                successCount++;
                            }
                            else
                            {
                                if (response != null)
                                    throw new Exception(response.ErrorMessage);

                                else
                                    throw new Exception("Endicia did not respond.");
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessage(string.Format("(LABEL: {2}) - Record #{0}. {1}", request.CurrentIndex, ex.Message, request.PartnerTransactionID));
                        }
                        finally
                        {
                            ShowMessage(string.Format("(LABEL: {1}) - Record #{0}. End printing label.", request.CurrentIndex, request.PartnerTransactionID));

                            request.WeightOz -= selectedPackingWeight;

                            request = null;
                        }
                    }
                    else
                    {
                        recordIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

            printLabelComplete = true;

            printLabelThread.Join();
        }

        private void PrintLabel(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(labelImage, e.Graphics.VisibleClipBounds);

            e.HasMorePages = false;
        }

        #endregion
        
        #region Daily Pick Ticket

        private Thread printDPTThread;

        private bool printDPTComplete = true;

        private void printDPT_Click(object sender, EventArgs e)
        {
            if (!importComplete)
                return;

            if (printQueue.Count > 0)
            {
                printDPTThread = new Thread(PrintDPT);

                printDPTThread.IsBackground = true;

                printDPTThread.Start();
            }
        }

        private void PrintDPT()
        {
            printDPTComplete = false;

            try
            {
                int count = printQueue.Count;
                int recordIndex = 0;

                ShowMessage("(DAILY PICK TICKET) - Collecting data for pick slip.");

                IList<PickSlipRow> rows = new List<PickSlipRow>();

                while (recordIndex < count)
                {
                    if (Convert.ToBoolean(grid.Rows[recordIndex].Cells[0].Value))
                    {
                        LabelRequest request = printQueue[recordIndex];

                        recordIndex++;

                        for (int i = 0; i < request.SKUCodes.Length; i++)
                        {
                            bool found = false;

                            foreach (PickSlipRow row in rows)
                            {
                                if (row.SKU == request.SKUCodes[i])
                                {
                                    row.Quantity += request.SKUQuantities[i];

                                    found = true;

                                    break;
                                }
                            }

                            if (!found)
                            {
                                PickSlipRow row = new PickSlipRow();

                                row.SKU = request.SKUCodes[i];
                                row.ItemDescription = request.SKUDescriptions[i];
                                row.Quantity = request.SKUQuantities[i];

                                rows.Add(row);
                            }
                        }
                    }
                    else
                    {
                        recordIndex++;
                    }
                }

                ShowMessage(string.Format("(DAILY PICK TICKET) - Sending pick slip to printer ({0})", selectedSlipPrinter));

                PickSlip pickSlip = new PickSlip(selectedSlipPrinter, rows);

                ShowMessage("(DAILY PICK TICKET) - Pick slip saved to archive folder.");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

            printDPTComplete = true;

            printDPTThread.Join();
        }

        #endregion

        #region Change Password

        Thread changePasswordThread;

        string newPassPhrase;

        private void changePassword_Click(object sender, EventArgs e)
        {
            if (!importComplete || !printLabelComplete || !printPackageSlipComplete || !printDPTComplete)
                return;

            if (newPassword.Text.Length <= 5)
                ShowMessage("(CHANGE PASSWORD) - Password must have at least 6 characters.");

            else if (newPassword.Text != confirmPassword.Text)
                ShowMessage("(CHANGE PASSWORD) - Passwords do not match.");

            else
            {
                newPassPhrase = newPassword.Text;

                changePasswordThread = new Thread(ChangePassword);

                changePasswordThread.IsBackground = true;

                changePasswordThread.Start();
            }
        }

        private void ChangePassword()
        {
            ShowMessage(string.Format("(CHANGE PASSWORD) - Requesting new password from Endicia."));

            try
            {
                ChangePassPhraseRequest request = new ChangePassPhraseRequest();

                request.CertifiedIntermediary = new CertifiedIntermediary() { AccountID = Settings.EndiciaAccountID, PassPhrase = Settings.EndiciaPassPhrase };
                request.NewPassPhrase = newPassPhrase;
                request.RequesterID = Settings.EndiciaRequesterID;
                request.RequestID = Guid.NewGuid().ToString();

                ChangePassPhraseRequestResponse response = null;

                using (EwsLabelServiceSoapClient client = new EwsLabelServiceSoapClient())
                {
                    response = client.ChangePassPhrase(request);
                }

                if (response == null)
                    throw new Exception("(CHANGE PASSWORD) - Endicia did not respond.");

                if (!string.IsNullOrEmpty(response.ErrorMessage))
                    throw new Exception(response.ErrorMessage);

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove("EndiciaPassPhrase");
                config.AppSettings.Settings.Add("EndiciaPassPhrase", newPassPhrase);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                RefreshSettingsGrid();

                ShowMessage(string.Format("(CHANGE PASSWORD) - Password changed successfully."));
            }
            catch (Exception ex)
            {
                ShowMessage(string.Format("(CHANGE PASSWORD) - Requesting new password failed. {0}", ex.Message));
            }
        }

        #endregion

        #region Dazzle

        private string dazzleFile;

        private Thread dazzleThread;

        private void dazzleImport_Click(object sender, EventArgs e)
        {
            if (((importComplete && printLabelComplete) && printPackageSlipComplete) && printDPTComplete)
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Choose a file",
                    Multiselect = false
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    dazzleFile = dialog.FileName;
                    dazzleThread = new Thread(new ThreadStart(ImportDazzle));
                    dazzleThread.IsBackground = true;
                    dazzleThread.Start();
                }
            }
        }

        private void ImportDazzle()
        {
            try
            {
                int recordCount = 0;
                int successCount = 0;
                int recordIndex = 0;

                using (CachedCsvReader reader = new CachedCsvReader(new StreamReader(dazzleFile), true, '\t'))
                {
                    recordCount = reader.Count();

                    reader.MoveToStart();

                    ShowMessage(string.Format("(DAZZLE) - Begin reading dazzle import file \"{0}\" containing {1} records.", Path.GetFileName(importFile), recordCount));

                    while (reader.ReadNextRecord())
                    {
                        recordIndex++;
                        try
                        {
                            try
                            {
                                string postageCostsStr = reader["Postage ($)"];
                                string weightStr = reader["Weight (OZ)"];
                                string mailClass = reader["Service_Class"];
                                string referenceID = reader["Reference ID"];
                                string trackingNumber = reader["Tracking_ID"];
                                string connectionString = ConfigurationManager.ConnectionStrings[selectedClient].ConnectionString;

                                if (string.IsNullOrEmpty(connectionString))
                                {
                                    throw new Exception("Invalid ConnectionString.");
                                }

                                long saleID = long.TryParse(referenceID, out saleID) ? saleID : -1;
                                if (saleID < 0)
                                {
                                    throw new Exception("Invalid Reference ID.");
                                }

                                double weight = double.TryParse(weightStr, NumberStyles.AllowDecimalPoint, CULTURE, out weight) ? weight : -1.0;
                                if (!(weight == -1.0))
                                {
                                    weight *= 16.0;
                                    weight = Math.Round(weight, 1);
                                }

                                double postageCosts = double.TryParse(postageCostsStr, NumberStyles.AllowDecimalPoint, CULTURE, out postageCosts) ? weight : -1.0;

                                ShowMessage(string.Format("(DAZZLE: {4}) - Record #{0}. Sending postage details to CRM. Weight: {1}. TrackingNumber: {2}. Postage costs: ${3}", recordIndex, postageCosts, trackingNumber, weight.ToString("f2"), saleID));

                                TrimFuelService.UpdatePostageDetails(connectionString, saleID, trackingNumber, postageCosts, weight, mailClass, CULTURE);

                                ShowMessage(string.Format("(DAZZLE: {1}) - Record #{0}. Successfully updated postage details.", recordIndex, saleID));

                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                ShowMessage(string.Format("(DAZZLE) - Record #{0}. An error occured! {1}", recordIndex, ex.Message));
                            }
                        }
                        finally
                        {
                        }
                    }

                    ShowMessage(string.Format("(DAZZLE) - End reading file \"{0}\". Successfully processed {1} out of {2} records.", Path.GetFileName(dazzleFile), successCount, recordIndex));
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
            dazzleThread.Join();
        }

        #endregion

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter writer = new StreamWriter(DateTime.Now.ToString("yyyyMMddHHss") + "_log.txt");
            writer.WriteLine(applicationLog.Text);
            writer.Close();
        }       
    }
}
