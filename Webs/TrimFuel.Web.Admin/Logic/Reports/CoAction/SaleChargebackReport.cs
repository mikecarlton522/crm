using System.Xml.Linq;
using TrimFuel.Business.Gateways.UspsGateway;

using DocumentFormat.OpenXml.Packaging;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;
using M = DocumentFormat.OpenXml.Math;
using Ovml = DocumentFormat.OpenXml.Vml.Office;
using V = DocumentFormat.OpenXml.Vml;
using TrimFuel.Model;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.Logic.Reports.CoAction
{
    public class SaleChargebackReport
    {
        public SaleChargebackReport(string caseNumber, string arnNumber, string companyName, string companyAddress1,
            string assertigyMIDID, ChargeHistoryEx ch, ChargeHistoryEx trialCh, Billing b,
            decimal chAmount, string chCurrency, decimal trialChAmount, string trialChCurrency, string trialTrackingNumber, string rebillTrackingNumber)
        {
            CaseNumber = caseNumber;
            ARNNumber = arnNumber;
            CompanyName = companyName;
            CompanyAddress1 = companyAddress1;
            AssertigyMIDID = assertigyMIDID;
            ChargeHistory = ch;
            TrialChargeHistory = trialCh;
            Billing = b;
            ChAmount = chAmount;
            ChCurrency = chCurrency;
            TrialChAmount = trialChAmount;
            TrialChCurrency = trialChCurrency;
            TrialTrackingNumber = trialTrackingNumber;
            RebillTrackingNumber = rebillTrackingNumber;

            TrialUspsGatewayResponse = GetTrackingInfo(trialTrackingNumber);
            if (trialTrackingNumber == rebillTrackingNumber)
                RebillUspsGatewayResponse = TrialUspsGatewayResponse;
            else
                RebillUspsGatewayResponse = GetTrackingInfo(rebillTrackingNumber);
        }

        // Creates a WordprocessingDocument.
        public void Create(System.IO.Stream outputStream)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/MidImages/" + TrimFuel.Business.Config.Current.APPLICATION_ID + "/terms_{0}.png", AssertigyMIDID));
            using (System.IO.FileStream imgFile1 = System.IO.File.OpenRead(path))
            {
                ImageData1 = new byte[imgFile1.Length];
                imgFile1.Read(ImageData1, 0, (int)imgFile1.Length);
                Image1 = new System.Drawing.Bitmap(new System.IO.MemoryStream(ImageData1));
            }

            path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/MidImages/" + TrimFuel.Business.Config.Current.APPLICATION_ID + "/order_form_{0}.png", AssertigyMIDID));
            using (System.IO.FileStream imgFile2 = System.IO.File.OpenRead(path))
            {
                ImageData2 = new byte[imgFile2.Length];
                imgFile2.Read(ImageData2, 0, (int)imgFile2.Length);
                Image2 = new System.Drawing.Bitmap(new System.IO.MemoryStream(ImageData2));
            }

            using (WordprocessingDocument package = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document))
            {
                CreateParts(package);
            }
        }

        public string CaseNumber { get; set; }
        public string ARNNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress1 { get; set; }
        public string AssertigyMIDID { get; set; }
        public decimal ChAmount { get; set; }
        public string ChCurrency { get; set; }
        public decimal TrialChAmount { get; set; }
        public string TrialChCurrency { get; set; }
        public string TrialTrackingNumber { get; set; }
        public string RebillTrackingNumber { get; set; }

        public ChargeHistoryEx ChargeHistory { get; set; }
        public ChargeHistoryEx TrialChargeHistory { get; set; }
        public Billing Billing { get; set; }
        private UspsGatewayResponse TrialUspsGatewayResponse { get; set; }
        private UspsGatewayResponse RebillUspsGatewayResponse { get; set; }


        public byte[] ImageData1 = null;
        public byte[] ImageData2 = null;
        public System.Drawing.Bitmap Image1 { get; set; }
        public System.Drawing.Bitmap Image2 { get; set; }

        // Adds child parts and generates content of the specified part.
        private void CreateParts(WordprocessingDocument document)
        {
            ExtendedFilePropertiesPart extendedFilePropertiesPart1 = document.AddNewPart<ExtendedFilePropertiesPart>("rId3");
            GenerateExtendedFilePropertiesPart1Content(extendedFilePropertiesPart1);

            MainDocumentPart mainDocumentPart1 = document.AddMainDocumentPart();
            GenerateMainDocumentPart1Content(mainDocumentPart1);

            ThemePart themePart1 = mainDocumentPart1.AddNewPart<ThemePart>("rId8");
            GenerateThemePart1Content(themePart1);

            DocumentSettingsPart documentSettingsPart1 = mainDocumentPart1.AddNewPart<DocumentSettingsPart>("rId3");
            GenerateDocumentSettingsPart1Content(documentSettingsPart1);

            FontTablePart fontTablePart1 = mainDocumentPart1.AddNewPart<FontTablePart>("rId7");
            GenerateFontTablePart1Content(fontTablePart1);

            StyleDefinitionsPart styleDefinitionsPart1 = mainDocumentPart1.AddNewPart<StyleDefinitionsPart>("rId2");
            GenerateStyleDefinitionsPart1Content(styleDefinitionsPart1);

            NumberingDefinitionsPart numberingDefinitionsPart1 = mainDocumentPart1.AddNewPart<NumberingDefinitionsPart>("rId1");
            GenerateNumberingDefinitionsPart1Content(numberingDefinitionsPart1);

            ImagePart imagePart1 = mainDocumentPart1.AddNewPart<ImagePart>("image/png", "rId6");
            GenerateImagePart1Content(imagePart1);

            ImagePart imagePart2 = mainDocumentPart1.AddNewPart<ImagePart>("image/png", "rId5");
            GenerateImagePart2Content(imagePart2);

            WebSettingsPart webSettingsPart1 = mainDocumentPart1.AddNewPart<WebSettingsPart>("rId4");
            GenerateWebSettingsPart1Content(webSettingsPart1);

            SetPackageProperties(document);
        }

        // Generates content of extendedFilePropertiesPart1.
        private void GenerateExtendedFilePropertiesPart1Content(ExtendedFilePropertiesPart extendedFilePropertiesPart1)
        {
            Ap.Properties properties1 = new Ap.Properties();
            properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
            Ap.Template template1 = new Ap.Template();
            template1.Text = "Normal.dotm";
            Ap.TotalTime totalTime1 = new Ap.TotalTime();
            totalTime1.Text = "2";
            Ap.Pages pages1 = new Ap.Pages();
            pages1.Text = "3";
            Ap.Words words1 = new Ap.Words();
            words1.Text = "165";
            Ap.Characters characters1 = new Ap.Characters();
            characters1.Text = "942";
            Ap.Application application1 = new Ap.Application();
            application1.Text = "Microsoft Office Word";
            Ap.DocumentSecurity documentSecurity1 = new Ap.DocumentSecurity();
            documentSecurity1.Text = "0";
            Ap.Lines lines1 = new Ap.Lines();
            lines1.Text = "7";
            Ap.Paragraphs paragraphs1 = new Ap.Paragraphs();
            paragraphs1.Text = "2";
            Ap.ScaleCrop scaleCrop1 = new Ap.ScaleCrop();
            scaleCrop1.Text = "false";
            Ap.Company company1 = new Ap.Company();
            company1.Text = "";
            Ap.LinksUpToDate linksUpToDate1 = new Ap.LinksUpToDate();
            linksUpToDate1.Text = "false";
            Ap.CharactersWithSpaces charactersWithSpaces1 = new Ap.CharactersWithSpaces();
            charactersWithSpaces1.Text = "1105";
            Ap.SharedDocument sharedDocument1 = new Ap.SharedDocument();
            sharedDocument1.Text = "false";
            Ap.HyperlinksChanged hyperlinksChanged1 = new Ap.HyperlinksChanged();
            hyperlinksChanged1.Text = "false";
            Ap.ApplicationVersion applicationVersion1 = new Ap.ApplicationVersion();
            applicationVersion1.Text = "12.0000";

            properties1.Append(template1);
            properties1.Append(totalTime1);
            properties1.Append(pages1);
            properties1.Append(words1);
            properties1.Append(characters1);
            properties1.Append(application1);
            properties1.Append(documentSecurity1);
            properties1.Append(lines1);
            properties1.Append(paragraphs1);
            properties1.Append(scaleCrop1);
            properties1.Append(company1);
            properties1.Append(linksUpToDate1);
            properties1.Append(charactersWithSpaces1);
            properties1.Append(sharedDocument1);
            properties1.Append(hyperlinksChanged1);
            properties1.Append(applicationVersion1);

            extendedFilePropertiesPart1.Properties = properties1;
        }

        // Generates content of mainDocumentPart1.
        private void GenerateMainDocumentPart1Content(MainDocumentPart mainDocumentPart1)
        {
            Document document1 = new Document();
            document1.AddNamespaceDeclaration("ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            document1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            document1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            document1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            document1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            document1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            document1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            document1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            document1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");

            Body body1 = new Body();

            Table table1 = new Table();

            TableProperties tableProperties1 = new TableProperties();
            TableWidth tableWidth1 = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };
            TableCellSpacing tableCellSpacing1 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();
            TopMargin topMargin1 = new TopMargin() { Width = "15", Type = TableWidthUnitValues.Dxa };
            TableCellLeftMargin tableCellLeftMargin1 = new TableCellLeftMargin() { Width = 15, Type = TableWidthValues.Dxa };
            BottomMargin bottomMargin1 = new BottomMargin() { Width = "15", Type = TableWidthUnitValues.Dxa };
            TableCellRightMargin tableCellRightMargin1 = new TableCellRightMargin() { Width = 15, Type = TableWidthValues.Dxa };

            tableCellMarginDefault1.Append(topMargin1);
            tableCellMarginDefault1.Append(tableCellLeftMargin1);
            tableCellMarginDefault1.Append(bottomMargin1);
            tableCellMarginDefault1.Append(tableCellRightMargin1);
            TableLook tableLook1 = new TableLook() { Val = "04A0" };

            tableProperties1.Append(tableWidth1);
            tableProperties1.Append(tableCellSpacing1);
            tableProperties1.Append(tableCellMarginDefault1);
            tableProperties1.Append(tableLook1);

            TableGrid tableGrid1 = new TableGrid();
            GridColumn gridColumn1 = new GridColumn() { Width = "9445" };

            tableGrid1.Append(gridColumn1);

            TableRow tableRow1 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties1 = new TableRowProperties();
            TableCellSpacing tableCellSpacing2 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties1.Append(tableCellSpacing2);

            TableCell tableCell1 = new TableCell();

            TableCellProperties tableCellProperties1 = new TableCellProperties();
            TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment1 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark1 = new HideMark();

            tableCellProperties1.Append(tableCellWidth1);
            tableCellProperties1.Append(tableCellVerticalAlignment1);
            tableCellProperties1.Append(hideMark1);

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "NormalWeb" };

            paragraphProperties1.Append(paragraphStyleId1);

            Run run1 = new Run();
            Text text1 = new Text();
            text1.Text = System.DateTime.Now.ToLongDateString();

            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            tableCell1.Append(tableCellProperties1);
            tableCell1.Append(paragraph1);

            tableRow1.Append(tableRowProperties1);
            tableRow1.Append(tableCell1);

            TableRow tableRow2 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties2 = new TableRowProperties();
            TableCellSpacing tableCellSpacing3 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties2.Append(tableCellSpacing3);

            TableCell tableCell2 = new TableCell();

            TableCellProperties tableCellProperties2 = new TableCellProperties();
            TableCellWidth tableCellWidth2 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment2 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark2 = new HideMark();

            tableCellProperties2.Append(tableCellWidth2);
            tableCellProperties2.Append(tableCellVerticalAlignment2);
            tableCellProperties2.Append(hideMark2);

            Paragraph paragraph2 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties2 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties1.Append(runFonts1);

            paragraphProperties2.Append(paragraphMarkRunProperties1);

            paragraph2.Append(paragraphProperties2);

            tableCell2.Append(tableCellProperties2);
            tableCell2.Append(paragraph2);

            tableRow2.Append(tableRowProperties2);
            tableRow2.Append(tableCell2);

            TableRow tableRow3 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties3 = new TableRowProperties();
            TableCellSpacing tableCellSpacing4 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties3.Append(tableCellSpacing4);

            TableCell tableCell3 = new TableCell();

            TableCellProperties tableCellProperties3 = new TableCellProperties();
            TableCellWidth tableCellWidth3 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment3 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark3 = new HideMark();

            tableCellProperties3.Append(tableCellWidth3);
            tableCellProperties3.Append(tableCellVerticalAlignment3);
            tableCellProperties3.Append(hideMark3);

            Paragraph paragraph3 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties3 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties2 = new ParagraphMarkRunProperties();
            RunFonts runFonts2 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties2.Append(runFonts2);

            paragraphProperties3.Append(paragraphMarkRunProperties2);

            paragraph3.Append(paragraphProperties3);

            tableCell3.Append(tableCellProperties3);
            tableCell3.Append(paragraph3);

            tableRow3.Append(tableRowProperties3);
            tableRow3.Append(tableCell3);

            TableRow tableRow4 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties4 = new TableRowProperties();
            TableCellSpacing tableCellSpacing5 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties4.Append(tableCellSpacing5);

            TableCell tableCell4 = new TableCell();

            TableCellProperties tableCellProperties4 = new TableCellProperties();
            TableCellWidth tableCellWidth4 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment4 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark4 = new HideMark();

            tableCellProperties4.Append(tableCellWidth4);
            tableCellProperties4.Append(tableCellVerticalAlignment4);
            tableCellProperties4.Append(hideMark4);

            Paragraph paragraph4 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties4 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties3 = new ParagraphMarkRunProperties();
            RunFonts runFonts3 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties3.Append(runFonts3);

            paragraphProperties4.Append(paragraphMarkRunProperties3);

            paragraph4.Append(paragraphProperties4);

            tableCell4.Append(tableCellProperties4);
            tableCell4.Append(paragraph4);

            tableRow4.Append(tableRowProperties4);
            tableRow4.Append(tableCell4);

            TableRow tableRow5 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties5 = new TableRowProperties();
            TableCellSpacing tableCellSpacing6 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties5.Append(tableCellSpacing6);

            TableCell tableCell5 = new TableCell();

            TableCellProperties tableCellProperties5 = new TableCellProperties();
            TableCellWidth tableCellWidth5 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment5 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark5 = new HideMark();

            tableCellProperties5.Append(tableCellWidth5);
            tableCellProperties5.Append(tableCellVerticalAlignment5);
            tableCellProperties5.Append(hideMark5);

            Paragraph paragraph5 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties5 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties4 = new ParagraphMarkRunProperties();
            RunFonts runFonts4 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties4.Append(runFonts4);

            paragraphProperties5.Append(paragraphMarkRunProperties4);

            paragraph5.Append(paragraphProperties5);

            tableCell5.Append(tableCellProperties5);
            tableCell5.Append(paragraph5);

            tableRow5.Append(tableRowProperties5);
            tableRow5.Append(tableCell5);

            TableRow tableRow6 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties6 = new TableRowProperties();
            TableCellSpacing tableCellSpacing7 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties6.Append(tableCellSpacing7);

            TableCell tableCell6 = new TableCell();

            TableCellProperties tableCellProperties6 = new TableCellProperties();
            TableCellWidth tableCellWidth6 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment6 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark6 = new HideMark();

            tableCellProperties6.Append(tableCellWidth6);
            tableCellProperties6.Append(tableCellVerticalAlignment6);
            tableCellProperties6.Append(hideMark6);

            Paragraph paragraph6 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties6 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId2 = new ParagraphStyleId() { Val = "Heading3" };
            Justification justification1 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties5 = new ParagraphMarkRunProperties();
            RunFonts runFonts5 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties5.Append(runFonts5);

            paragraphProperties6.Append(paragraphStyleId2);
            paragraphProperties6.Append(justification1);
            paragraphProperties6.Append(paragraphMarkRunProperties5);

            Run run2 = new Run();

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts6 = new RunFonts() { EastAsia = "Times New Roman" };

            runProperties1.Append(runFonts6);
            Text text2 = new Text();
            text2.Text = "CHARGEBACK RESPONSE";

            run2.Append(runProperties1);
            run2.Append(text2);

            paragraph6.Append(paragraphProperties6);
            paragraph6.Append(run2);

            tableCell6.Append(tableCellProperties6);
            tableCell6.Append(paragraph6);

            tableRow6.Append(tableRowProperties6);
            tableRow6.Append(tableCell6);

            TableRow tableRow7 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties7 = new TableRowProperties();
            TableCellSpacing tableCellSpacing8 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties7.Append(tableCellSpacing8);

            TableCell tableCell7 = new TableCell();

            TableCellProperties tableCellProperties7 = new TableCellProperties();
            TableCellWidth tableCellWidth7 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment7 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark7 = new HideMark();

            tableCellProperties7.Append(tableCellWidth7);
            tableCellProperties7.Append(tableCellVerticalAlignment7);
            tableCellProperties7.Append(hideMark7);

            Paragraph paragraph7 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties7 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties6 = new ParagraphMarkRunProperties();
            RunFonts runFonts7 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties6.Append(runFonts7);

            paragraphProperties7.Append(paragraphMarkRunProperties6);

            paragraph7.Append(paragraphProperties7);

            tableCell7.Append(tableCellProperties7);
            tableCell7.Append(paragraph7);

            tableRow7.Append(tableRowProperties7);
            tableRow7.Append(tableCell7);

            TableRow tableRow8 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties8 = new TableRowProperties();
            TableCellSpacing tableCellSpacing9 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties8.Append(tableCellSpacing9);

            TableCell tableCell8 = new TableCell();

            TableCellProperties tableCellProperties8 = new TableCellProperties();
            TableCellWidth tableCellWidth8 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment8 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark8 = new HideMark();

            tableCellProperties8.Append(tableCellWidth8);
            tableCellProperties8.Append(tableCellVerticalAlignment8);
            tableCellProperties8.Append(hideMark8);

            Paragraph paragraph8 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties8 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties7 = new ParagraphMarkRunProperties();
            RunFonts runFonts8 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties7.Append(runFonts8);

            paragraphProperties8.Append(paragraphMarkRunProperties7);

            paragraph8.Append(paragraphProperties8);

            tableCell8.Append(tableCellProperties8);
            tableCell8.Append(paragraph8);

            tableRow8.Append(tableRowProperties8);
            tableRow8.Append(tableCell8);

            TableRow tableRow9 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties9 = new TableRowProperties();
            TableCellSpacing tableCellSpacing10 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties9.Append(tableCellSpacing10);

            TableCell tableCell9 = new TableCell();

            TableCellProperties tableCellProperties9 = new TableCellProperties();
            TableCellWidth tableCellWidth9 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment9 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark9 = new HideMark();

            tableCellProperties9.Append(tableCellWidth9);
            tableCellProperties9.Append(tableCellVerticalAlignment9);
            tableCellProperties9.Append(hideMark9);

            Paragraph paragraph9 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties9 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties8 = new ParagraphMarkRunProperties();
            RunFonts runFonts9 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties8.Append(runFonts9);

            paragraphProperties9.Append(paragraphMarkRunProperties8);

            paragraph9.Append(paragraphProperties9);

            tableCell9.Append(tableCellProperties9);
            tableCell9.Append(paragraph9);

            tableRow9.Append(tableRowProperties9);
            tableRow9.Append(tableCell9);

            TableRow tableRow10 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties10 = new TableRowProperties();
            TableCellSpacing tableCellSpacing11 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties10.Append(tableCellSpacing11);

            TableCell tableCell10 = new TableCell();

            TableCellProperties tableCellProperties10 = new TableCellProperties();
            TableCellWidth tableCellWidth10 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment10 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark10 = new HideMark();

            tableCellProperties10.Append(tableCellWidth10);
            tableCellProperties10.Append(tableCellVerticalAlignment10);
            tableCellProperties10.Append(hideMark10);

            Paragraph paragraph10 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties10 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties9 = new ParagraphMarkRunProperties();
            RunFonts runFonts10 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties9.Append(runFonts10);

            paragraphProperties10.Append(paragraphMarkRunProperties9);

            paragraph10.Append(paragraphProperties10);

            tableCell10.Append(tableCellProperties10);
            tableCell10.Append(paragraph10);

            tableRow10.Append(tableRowProperties10);
            tableRow10.Append(tableCell10);

            TableRow tableRow11 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties11 = new TableRowProperties();
            TableCellSpacing tableCellSpacing12 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties11.Append(tableCellSpacing12);

            TableCell tableCell11 = new TableCell();

            TableCellProperties tableCellProperties11 = new TableCellProperties();
            TableCellWidth tableCellWidth11 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment11 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark11 = new HideMark();

            tableCellProperties11.Append(tableCellWidth11);
            tableCellProperties11.Append(tableCellVerticalAlignment11);
            tableCellProperties11.Append(hideMark11);

            Paragraph paragraph11 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties11 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties10 = new ParagraphMarkRunProperties();
            RunFonts runFonts11 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties10.Append(runFonts11);

            paragraphProperties11.Append(paragraphMarkRunProperties10);

            paragraph11.Append(paragraphProperties11);

            tableCell11.Append(tableCellProperties11);
            tableCell11.Append(paragraph11);

            tableRow11.Append(tableRowProperties11);
            tableRow11.Append(tableCell11);

            TableRow tableRow12 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties12 = new TableRowProperties();
            TableCellSpacing tableCellSpacing13 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties12.Append(tableCellSpacing13);

            TableCell tableCell12 = new TableCell();

            TableCellProperties tableCellProperties12 = new TableCellProperties();
            TableCellWidth tableCellWidth12 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment12 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark12 = new HideMark();

            tableCellProperties12.Append(tableCellWidth12);
            tableCellProperties12.Append(tableCellVerticalAlignment12);
            tableCellProperties12.Append(hideMark12);

            Paragraph paragraph12 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties12 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties11 = new ParagraphMarkRunProperties();
            RunFonts runFonts12 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties11.Append(runFonts12);

            paragraphProperties12.Append(paragraphMarkRunProperties11);

            paragraph12.Append(paragraphProperties12);

            tableCell12.Append(tableCellProperties12);
            tableCell12.Append(paragraph12);

            tableRow12.Append(tableRowProperties12);
            tableRow12.Append(tableCell12);

            TableRow tableRow13 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties13 = new TableRowProperties();
            TableCellSpacing tableCellSpacing14 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties13.Append(tableCellSpacing14);

            TableCell tableCell13 = new TableCell();

            TableCellProperties tableCellProperties13 = new TableCellProperties();
            TableCellWidth tableCellWidth13 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment13 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark13 = new HideMark();

            tableCellProperties13.Append(tableCellWidth13);
            tableCellProperties13.Append(tableCellVerticalAlignment13);
            tableCellProperties13.Append(hideMark13);

            Paragraph paragraph13 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties13 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties12 = new ParagraphMarkRunProperties();
            RunFonts runFonts13 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties12.Append(runFonts13);

            paragraphProperties13.Append(paragraphMarkRunProperties12);

            paragraph13.Append(paragraphProperties13);

            tableCell13.Append(tableCellProperties13);
            tableCell13.Append(paragraph13);

            tableRow13.Append(tableRowProperties13);
            tableRow13.Append(tableCell13);

            TableRow tableRow14 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties14 = new TableRowProperties();
            TableCellSpacing tableCellSpacing15 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties14.Append(tableCellSpacing15);

            TableCell tableCell14 = new TableCell();

            TableCellProperties tableCellProperties14 = new TableCellProperties();
            TableCellWidth tableCellWidth14 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment14 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark14 = new HideMark();

            tableCellProperties14.Append(tableCellWidth14);
            tableCellProperties14.Append(tableCellVerticalAlignment14);
            tableCellProperties14.Append(hideMark14);

            Paragraph paragraph14 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties14 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties13 = new ParagraphMarkRunProperties();
            RunFonts runFonts14 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties13.Append(runFonts14);

            paragraphProperties14.Append(paragraphMarkRunProperties13);

            paragraph14.Append(paragraphProperties14);

            tableCell14.Append(tableCellProperties14);
            tableCell14.Append(paragraph14);

            tableRow14.Append(tableRowProperties14);
            tableRow14.Append(tableCell14);

            TableRow tableRow15 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties15 = new TableRowProperties();
            TableCellSpacing tableCellSpacing16 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties15.Append(tableCellSpacing16);

            TableCell tableCell15 = new TableCell();

            TableCellProperties tableCellProperties15 = new TableCellProperties();
            TableCellWidth tableCellWidth15 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment15 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark15 = new HideMark();

            tableCellProperties15.Append(tableCellWidth15);
            tableCellProperties15.Append(tableCellVerticalAlignment15);
            tableCellProperties15.Append(hideMark15);

            Paragraph paragraph15 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties15 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties14 = new ParagraphMarkRunProperties();
            RunFonts runFonts15 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties14.Append(runFonts15);

            paragraphProperties15.Append(paragraphMarkRunProperties14);

            paragraph15.Append(paragraphProperties15);

            tableCell15.Append(tableCellProperties15);
            tableCell15.Append(paragraph15);

            tableRow15.Append(tableRowProperties15);
            tableRow15.Append(tableCell15);

            TableRow tableRow16 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties16 = new TableRowProperties();
            TableCellSpacing tableCellSpacing17 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties16.Append(tableCellSpacing17);

            TableCell tableCell16 = new TableCell();

            TableCellProperties tableCellProperties16 = new TableCellProperties();
            TableCellWidth tableCellWidth16 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment16 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark16 = new HideMark();

            tableCellProperties16.Append(tableCellWidth16);
            tableCellProperties16.Append(tableCellVerticalAlignment16);
            tableCellProperties16.Append(hideMark16);

            Paragraph paragraph16 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties16 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId3 = new ParagraphStyleId() { Val = "NormalWeb" };

            paragraphProperties16.Append(paragraphStyleId3);

            Run run3 = new Run();
            Text text3 = new Text();
            text3.Text = CompanyAddress1;

            run3.Append(text3);

            paragraph16.Append(paragraphProperties16);
            paragraph16.Append(run3);

            tableCell16.Append(tableCellProperties16);
            tableCell16.Append(paragraph16);

            tableRow16.Append(tableRowProperties16);
            tableRow16.Append(tableCell16);

            TableRow tableRow17_1 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties17_1 = new TableRowProperties();
            TableCellSpacing tableCellSpacing18_1 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties17_1.Append(tableCellSpacing18_1);

            TableCell tableCell17_1 = new TableCell();

            TableCellProperties tableCellProperties17_1 = new TableCellProperties();
            TableCellWidth tableCellWidth17_1 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment17_1 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark17_1 = new HideMark();

            tableCellProperties17_1.Append(tableCellWidth17_1);
            tableCellProperties17_1.Append(tableCellVerticalAlignment17_1);
            tableCellProperties17_1.Append(hideMark17_1);

            Paragraph paragraph17_1 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties17_1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId4_1 = new ParagraphStyleId() { Val = "NormalWeb" };

            Justification justification17_1 = new Justification() { Val = JustificationValues.Right };

            paragraphProperties17_1.Append(paragraphStyleId4_1);
            paragraphProperties17_1.Append(justification17_1);

            Run run4_1 = new Run();
            Text text4_1 = new Text();
            text4_1.Text = string.Format("CASE NUMBER: {0}", CaseNumber);

            run4_1.Append(text4_1);

            paragraph17_1.Append(paragraphProperties17_1);
            paragraph17_1.Append(run4_1);

            tableCell17_1.Append(tableCellProperties17_1);
            tableCell17_1.Append(paragraph17_1);

            tableRow17_1.Append(tableRowProperties17_1);
            tableRow17_1.Append(tableCell17_1);



            TableRow tableRow17_2 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties17_2 = new TableRowProperties();

            TableCell tableCell17_2 = new TableCell();

            TableCellProperties tableCellProperties17_2 = new TableCellProperties();
            TableCellWidth tableCellWidth17_2 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment17_2 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark17_2 = new HideMark();

            tableCellProperties17_2.Append(tableCellWidth17_2);
            tableCellProperties17_2.Append(tableCellVerticalAlignment17_2);
            tableCellProperties17_2.Append(hideMark17_2);

            Paragraph paragraph17_2 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties17_2 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId4_2 = new ParagraphStyleId() { Val = "NormalWeb" };

            Justification justification17_2 = new Justification() { Val = JustificationValues.Right };

            paragraphProperties17_2.Append(paragraphStyleId4_2);
            paragraphProperties17_2.Append(justification17_2);

            Run run4_2 = new Run();
            Text text4_2 = new Text();
            text4_2.Text = string.Format("ARN NUMBER: {0}", ARNNumber);

            run4_2.Append(text4_2);

            paragraph17_2.Append(paragraphProperties17_2);
            paragraph17_2.Append(run4_2);

            tableCell17_2.Append(tableCellProperties17_2);
            tableCell17_2.Append(paragraph17_2);

            tableRow17_2.Append(tableRowProperties17_2);
            tableRow17_2.Append(tableCell17_2);



            TableRow tableRow18 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties18 = new TableRowProperties();
            TableCellSpacing tableCellSpacing19 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties18.Append(tableCellSpacing19);

            TableCell tableCell18 = new TableCell();

            TableCellProperties tableCellProperties18 = new TableCellProperties();
            TableCellWidth tableCellWidth18 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment18 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark18 = new HideMark();

            tableCellProperties18.Append(tableCellWidth18);
            tableCellProperties18.Append(tableCellVerticalAlignment18);
            tableCellProperties18.Append(hideMark18);

            Paragraph paragraph18 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties18 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties15 = new ParagraphMarkRunProperties();
            RunFonts runFonts16 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties15.Append(runFonts16);

            paragraphProperties18.Append(paragraphMarkRunProperties15);

            paragraph18.Append(paragraphProperties18);

            tableCell18.Append(tableCellProperties18);
            tableCell18.Append(paragraph18);

            tableRow18.Append(tableRowProperties18);
            tableRow18.Append(tableCell18);

            TableRow tableRow19 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties19 = new TableRowProperties();
            TableCellSpacing tableCellSpacing20 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties19.Append(tableCellSpacing20);

            TableCell tableCell19 = new TableCell();

            TableCellProperties tableCellProperties19 = new TableCellProperties();
            TableCellWidth tableCellWidth19 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment19 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark19 = new HideMark();

            tableCellProperties19.Append(tableCellWidth19);
            tableCellProperties19.Append(tableCellVerticalAlignment19);
            tableCellProperties19.Append(hideMark19);

            Paragraph paragraph19 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties19 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties16 = new ParagraphMarkRunProperties();
            RunFonts runFonts17 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties16.Append(runFonts17);

            paragraphProperties19.Append(paragraphMarkRunProperties16);

            paragraph19.Append(paragraphProperties19);

            tableCell19.Append(tableCellProperties19);
            tableCell19.Append(paragraph19);

            tableRow19.Append(tableRowProperties19);
            tableRow19.Append(tableCell19);

            TableRow tableRow20 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties20 = new TableRowProperties();
            TableCellSpacing tableCellSpacing21 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties20.Append(tableCellSpacing21);

            TableCell tableCell20 = new TableCell();

            TableCellProperties tableCellProperties20 = new TableCellProperties();
            TableCellWidth tableCellWidth20 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment20 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark20 = new HideMark();

            tableCellProperties20.Append(tableCellWidth20);
            tableCellProperties20.Append(tableCellVerticalAlignment20);
            tableCellProperties20.Append(hideMark20);

            Paragraph paragraph20 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties20 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties17 = new ParagraphMarkRunProperties();
            RunFonts runFonts18 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties17.Append(runFonts18);

            paragraphProperties20.Append(paragraphMarkRunProperties17);

            paragraph20.Append(paragraphProperties20);

            tableCell20.Append(tableCellProperties20);
            tableCell20.Append(paragraph20);

            tableRow20.Append(tableRowProperties20);
            tableRow20.Append(tableCell20);

            TableRow tableRow21 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties21 = new TableRowProperties();
            TableCellSpacing tableCellSpacing22 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties21.Append(tableCellSpacing22);

            TableCell tableCell21 = new TableCell();

            TableCellProperties tableCellProperties21 = new TableCellProperties();
            TableCellWidth tableCellWidth21 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment21 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark21 = new HideMark();

            tableCellProperties21.Append(tableCellWidth21);
            tableCellProperties21.Append(tableCellVerticalAlignment21);
            tableCellProperties21.Append(hideMark21);

            Paragraph paragraph21 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties21 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties18 = new ParagraphMarkRunProperties();
            RunFonts runFonts19 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties18.Append(runFonts19);

            paragraphProperties21.Append(paragraphMarkRunProperties18);

            paragraph21.Append(paragraphProperties21);

            tableCell21.Append(tableCellProperties21);
            tableCell21.Append(paragraph21);

            tableRow21.Append(tableRowProperties21);
            tableRow21.Append(tableCell21);

            TableRow tableRow22 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties22 = new TableRowProperties();
            TableCellSpacing tableCellSpacing23 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties22.Append(tableCellSpacing23);

            TableCell tableCell22 = new TableCell();

            TableCellProperties tableCellProperties22 = new TableCellProperties();
            TableCellWidth tableCellWidth22 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment22 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark22 = new HideMark();

            tableCellProperties22.Append(tableCellWidth22);
            tableCellProperties22.Append(tableCellVerticalAlignment22);
            tableCellProperties22.Append(hideMark22);

            Paragraph paragraph22 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties22 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties19 = new ParagraphMarkRunProperties();
            RunFonts runFonts20 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties19.Append(runFonts20);

            paragraphProperties22.Append(paragraphMarkRunProperties19);

            paragraph22.Append(paragraphProperties22);

            tableCell22.Append(tableCellProperties22);
            tableCell22.Append(paragraph22);

            tableRow22.Append(tableRowProperties22);
            tableRow22.Append(tableCell22);

            TableRow tableRow23 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties23 = new TableRowProperties();
            TableCellSpacing tableCellSpacing24 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties23.Append(tableCellSpacing24);

            TableCell tableCell23 = new TableCell();

            TableCellProperties tableCellProperties23 = new TableCellProperties();
            TableCellWidth tableCellWidth23 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment23 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark23 = new HideMark();

            tableCellProperties23.Append(tableCellWidth23);
            tableCellProperties23.Append(tableCellVerticalAlignment23);
            tableCellProperties23.Append(hideMark23);

            Paragraph paragraph23 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidParagraphProperties = "00F42003", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties23 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId5 = new ParagraphStyleId() { Val = "NormalWeb" };

            ParagraphMarkRunProperties paragraphMarkRunProperties20 = new ParagraphMarkRunProperties();
            Languages languages1 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties20.Append(languages1);

            paragraphProperties23.Append(paragraphStyleId5);
            paragraphProperties23.Append(paragraphMarkRunProperties20);

            Run run5 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties2 = new RunProperties();
            Languages languages2 = new Languages() { Val = "en-US" };

            runProperties2.Append(languages2);
            Text text5 = new Text();
            text5.Text = "We are writing to issue a dispute regarding this chargeback request, as per the terms of sale the customer ordered the risk-free trial product which was shipped and delivery confirmed. The customer has never requested cancellation, products were shipped af";

            run5.Append(runProperties2);
            run5.Append(text5);

            Run run6 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties3 = new RunProperties();
            Languages languages3 = new Languages() { Val = "en-US" };

            runProperties3.Append(languages3);
            Text text6 = new Text();
            text6.Text = "ter 15 days and delivery confirmed. The customer still has possession of the merchandise.";

            run6.Append(runProperties3);
            run6.Append(text6);

            Run run7 = new Run() { RsidRunAddition = "00F42003" };

            RunProperties runProperties4 = new RunProperties();
            Languages languages4 = new Languages() { Val = "en-US" };

            runProperties4.Append(languages4);
            Text text7 = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text7.Text = " P";

            run7.Append(runProperties4);
            run7.Append(text7);

            Run run8 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties5 = new RunProperties();
            Languages languages5 = new Languages() { Val = "en-US" };

            runProperties5.Append(languages5);
            Text text8 = new Text();
            text8.Text = "lease find attached the following items:";

            run8.Append(runProperties5);
            run8.Append(text8);

            paragraph23.Append(paragraphProperties23);
            paragraph23.Append(run5);
            paragraph23.Append(run6);
            paragraph23.Append(run7);
            paragraph23.Append(run8);

            tableCell23.Append(tableCellProperties23);
            tableCell23.Append(paragraph23);

            tableRow23.Append(tableRowProperties23);
            tableRow23.Append(tableCell23);

            TableRow tableRow24 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties24 = new TableRowProperties();
            TableCellSpacing tableCellSpacing25 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties24.Append(tableCellSpacing25);

            TableCell tableCell24 = new TableCell();

            TableCellProperties tableCellProperties24 = new TableCellProperties();
            TableCellWidth tableCellWidth24 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment24 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark24 = new HideMark();

            tableCellProperties24.Append(tableCellWidth24);
            tableCellProperties24.Append(tableCellVerticalAlignment24);
            tableCellProperties24.Append(hideMark24);

            Paragraph paragraph24 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties24 = new ParagraphProperties();

            NumberingProperties numberingProperties1 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference1 = new NumberingLevelReference() { Val = 0 };
            NumberingId numberingId1 = new NumberingId() { Val = 1 };

            numberingProperties1.Append(numberingLevelReference1);
            numberingProperties1.Append(numberingId1);
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };

            ParagraphMarkRunProperties paragraphMarkRunProperties21 = new ParagraphMarkRunProperties();
            RunFonts runFonts21 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages6 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties21.Append(runFonts21);
            paragraphMarkRunProperties21.Append(languages6);

            paragraphProperties24.Append(numberingProperties1);
            paragraphProperties24.Append(spacingBetweenLines1);
            paragraphProperties24.Append(paragraphMarkRunProperties21);

            Run run9 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties6 = new RunProperties();
            RunFonts runFonts22 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages7 = new Languages() { Val = "en-US" };

            runProperties6.Append(runFonts22);
            runProperties6.Append(languages7);
            Text text9 = new Text();
            text9.Text = "Customer\'s shipping & delivery confirmation of product ordered";

            run9.Append(runProperties6);
            run9.Append(text9);

            paragraph24.Append(paragraphProperties24);
            paragraph24.Append(run9);

            Paragraph paragraph25 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties25 = new ParagraphProperties();

            NumberingProperties numberingProperties2 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference2 = new NumberingLevelReference() { Val = 0 };
            NumberingId numberingId2 = new NumberingId() { Val = 1 };

            numberingProperties2.Append(numberingLevelReference2);
            numberingProperties2.Append(numberingId2);
            SpacingBetweenLines spacingBetweenLines2 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };

            ParagraphMarkRunProperties paragraphMarkRunProperties22 = new ParagraphMarkRunProperties();
            RunFonts runFonts23 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages8 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties22.Append(runFonts23);
            paragraphMarkRunProperties22.Append(languages8);

            paragraphProperties25.Append(numberingProperties2);
            paragraphProperties25.Append(spacingBetweenLines2);
            paragraphProperties25.Append(paragraphMarkRunProperties22);

            Run run10 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties7 = new RunProperties();
            RunFonts runFonts24 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages9 = new Languages() { Val = "en-US" };

            runProperties7.Append(runFonts24);
            runProperties7.Append(languages9);
            Text text10 = new Text();
            text10.Text = "Customer\'s original risk free trial purchase";

            run10.Append(runProperties7);
            run10.Append(text10);

            paragraph25.Append(paragraphProperties25);
            paragraph25.Append(run10);

            Paragraph paragraph26 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties26 = new ParagraphProperties();

            NumberingProperties numberingProperties3 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference3 = new NumberingLevelReference() { Val = 0 };
            NumberingId numberingId3 = new NumberingId() { Val = 1 };

            numberingProperties3.Append(numberingLevelReference3);
            numberingProperties3.Append(numberingId3);
            SpacingBetweenLines spacingBetweenLines3 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };

            ParagraphMarkRunProperties paragraphMarkRunProperties23 = new ParagraphMarkRunProperties();
            RunFonts runFonts25 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties23.Append(runFonts25);

            paragraphProperties26.Append(numberingProperties3);
            paragraphProperties26.Append(spacingBetweenLines3);
            paragraphProperties26.Append(paragraphMarkRunProperties23);

            Run run11 = new Run();

            RunProperties runProperties8 = new RunProperties();
            RunFonts runFonts26 = new RunFonts() { EastAsia = "Times New Roman" };

            runProperties8.Append(runFonts26);
            Text text11 = new Text();
            text11.Text = "Customer\'s tra";

            run11.Append(runProperties8);
            run11.Append(text11);

            Run run12 = new Run();

            RunProperties runProperties9 = new RunProperties();
            RunFonts runFonts27 = new RunFonts() { EastAsia = "Times New Roman" };

            runProperties9.Append(runFonts27);
            Text text12 = new Text();
            text12.Text = "nsaction authorization for order";

            run12.Append(runProperties9);
            run12.Append(text12);

            paragraph26.Append(paragraphProperties26);
            paragraph26.Append(run11);
            paragraph26.Append(run12);

            Paragraph paragraph27 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties27 = new ParagraphProperties();

            NumberingProperties numberingProperties4 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference4 = new NumberingLevelReference() { Val = 0 };
            NumberingId numberingId4 = new NumberingId() { Val = 1 };

            numberingProperties4.Append(numberingLevelReference4);
            numberingProperties4.Append(numberingId4);
            SpacingBetweenLines spacingBetweenLines4 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };

            ParagraphMarkRunProperties paragraphMarkRunProperties24 = new ParagraphMarkRunProperties();
            RunFonts runFonts28 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties24.Append(runFonts28);

            paragraphProperties27.Append(numberingProperties4);
            paragraphProperties27.Append(spacingBetweenLines4);
            paragraphProperties27.Append(paragraphMarkRunProperties24);

            Run run13 = new Run();

            RunProperties runProperties10 = new RunProperties();
            RunFonts runFonts29 = new RunFonts() { EastAsia = "Times New Roman" };

            runProperties10.Append(runFonts29);
            Text text13 = new Text();
            text13.Text = "Customer\' delivery confirmation pages";

            run13.Append(runProperties10);
            run13.Append(text13);

            paragraph27.Append(paragraphProperties27);
            paragraph27.Append(run13);

            Paragraph paragraph28 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties28 = new ParagraphProperties();

            NumberingProperties numberingProperties5 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference5 = new NumberingLevelReference() { Val = 0 };
            NumberingId numberingId5 = new NumberingId() { Val = 1 };

            numberingProperties5.Append(numberingLevelReference5);
            numberingProperties5.Append(numberingId5);
            SpacingBetweenLines spacingBetweenLines5 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };

            ParagraphMarkRunProperties paragraphMarkRunProperties25 = new ParagraphMarkRunProperties();
            RunFonts runFonts30 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages10 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties25.Append(runFonts30);
            paragraphMarkRunProperties25.Append(languages10);

            paragraphProperties28.Append(numberingProperties5);
            paragraphProperties28.Append(spacingBetweenLines5);
            paragraphProperties28.Append(paragraphMarkRunProperties25);

            Run run14 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties11 = new RunProperties();
            RunFonts runFonts31 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages11 = new Languages() { Val = "en-US" };

            runProperties11.Append(runFonts31);
            runProperties11.Append(languages11);
            Text text14 = new Text();
            text14.Text = "Order billing page which customer used to order product received";

            run14.Append(runProperties11);
            run14.Append(text14);

            paragraph28.Append(paragraphProperties28);
            paragraph28.Append(run14);

            Paragraph paragraph29 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties29 = new ParagraphProperties();

            NumberingProperties numberingProperties6 = new NumberingProperties();
            NumberingLevelReference numberingLevelReference6 = new NumberingLevelReference() { Val = 0 };
            NumberingId numberingId6 = new NumberingId() { Val = 1 };

            numberingProperties6.Append(numberingLevelReference6);
            numberingProperties6.Append(numberingId6);
            SpacingBetweenLines spacingBetweenLines6 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };

            ParagraphMarkRunProperties paragraphMarkRunProperties26 = new ParagraphMarkRunProperties();
            RunFonts runFonts32 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages12 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties26.Append(runFonts32);
            paragraphMarkRunProperties26.Append(languages12);

            paragraphProperties29.Append(numberingProperties6);
            paragraphProperties29.Append(spacingBetweenLines6);
            paragraphProperties29.Append(paragraphMarkRunProperties26);

            Run run15 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties12 = new RunProperties();
            RunFonts runFonts33 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages13 = new Languages() { Val = "en-US" };

            runProperties12.Append(runFonts33);
            runProperties12.Append(languages13);
            Text text15 = new Text();
            text15.Text = "Full size terms section displayed directly under order confirmation button";

            run15.Append(runProperties12);
            run15.Append(text15);

            paragraph29.Append(paragraphProperties29);
            paragraph29.Append(run15);

            tableCell24.Append(tableCellProperties24);
            tableCell24.Append(paragraph24);
            tableCell24.Append(paragraph25);
            tableCell24.Append(paragraph26);
            tableCell24.Append(paragraph27);
            tableCell24.Append(paragraph28);
            tableCell24.Append(paragraph29);

            tableRow24.Append(tableRowProperties24);
            tableRow24.Append(tableCell24);

            TableRow tableRow25 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties25 = new TableRowProperties();
            TableCellSpacing tableCellSpacing26 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties25.Append(tableCellSpacing26);

            TableCell tableCell25 = new TableCell();

            TableCellProperties tableCellProperties25 = new TableCellProperties();
            TableCellWidth tableCellWidth25 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment25 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark25 = new HideMark();

            tableCellProperties25.Append(tableCellWidth25);
            tableCellProperties25.Append(tableCellVerticalAlignment25);
            tableCellProperties25.Append(hideMark25);

            Paragraph paragraph30 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties30 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId6 = new ParagraphStyleId() { Val = "NormalWeb" };

            ParagraphMarkRunProperties paragraphMarkRunProperties27 = new ParagraphMarkRunProperties();
            Languages languages14 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties27.Append(languages14);

            paragraphProperties30.Append(paragraphStyleId6);
            paragraphProperties30.Append(paragraphMarkRunProperties27);

            Run run16 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties13 = new RunProperties();
            Languages languages15 = new Languages() { Val = "en-US" };

            runProperties13.Append(languages15);
            Text text16 = new Text();
            text16.Text = "We are constantly working with our custome";

            run16.Append(runProperties13);
            run16.Append(text16);

            Run run17 = new Run() { RsidRunProperties = "00F42003" };

            RunProperties runProperties14 = new RunProperties();
            Languages languages16 = new Languages() { Val = "en-US" };

            runProperties14.Append(languages16);
            Text text17 = new Text();
            text17.Text = "rs to make sure that we maintain a high level of responsiveness and customer support. Please let us know what other documentation you require to process this dispute.";

            run17.Append(runProperties14);
            run17.Append(text17);

            paragraph30.Append(paragraphProperties30);
            paragraph30.Append(run16);
            paragraph30.Append(run17);

            tableCell25.Append(tableCellProperties25);
            tableCell25.Append(paragraph30);

            tableRow25.Append(tableRowProperties25);
            tableRow25.Append(tableCell25);

            TableRow tableRow26 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties26 = new TableRowProperties();
            TableCellSpacing tableCellSpacing27 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties26.Append(tableCellSpacing27);

            TableCell tableCell26 = new TableCell();

            TableCellProperties tableCellProperties26 = new TableCellProperties();
            TableCellWidth tableCellWidth26 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment26 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark26 = new HideMark();

            tableCellProperties26.Append(tableCellWidth26);
            tableCellProperties26.Append(tableCellVerticalAlignment26);
            tableCellProperties26.Append(hideMark26);

            Paragraph paragraph31 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties31 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties28 = new ParagraphMarkRunProperties();
            RunFonts runFonts34 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages17 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties28.Append(runFonts34);
            paragraphMarkRunProperties28.Append(languages17);

            paragraphProperties31.Append(paragraphMarkRunProperties28);

            paragraph31.Append(paragraphProperties31);

            tableCell26.Append(tableCellProperties26);
            tableCell26.Append(paragraph31);

            tableRow26.Append(tableRowProperties26);
            tableRow26.Append(tableCell26);

            TableRow tableRow27 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties27 = new TableRowProperties();
            TableCellSpacing tableCellSpacing28 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties27.Append(tableCellSpacing28);

            TableCell tableCell27 = new TableCell();

            TableCellProperties tableCellProperties27 = new TableCellProperties();
            TableCellWidth tableCellWidth27 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment27 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark27 = new HideMark();

            tableCellProperties27.Append(tableCellWidth27);
            tableCellProperties27.Append(tableCellVerticalAlignment27);
            tableCellProperties27.Append(hideMark27);

            Paragraph paragraph32 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties32 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties29 = new ParagraphMarkRunProperties();
            RunFonts runFonts35 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages18 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties29.Append(runFonts35);
            paragraphMarkRunProperties29.Append(languages18);

            paragraphProperties32.Append(paragraphMarkRunProperties29);

            paragraph32.Append(paragraphProperties32);

            tableCell27.Append(tableCellProperties27);
            tableCell27.Append(paragraph32);

            tableRow27.Append(tableRowProperties27);
            tableRow27.Append(tableCell27);

            TableRow tableRow28 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties28 = new TableRowProperties();
            TableCellSpacing tableCellSpacing29 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties28.Append(tableCellSpacing29);

            TableCell tableCell28 = new TableCell();

            TableCellProperties tableCellProperties28 = new TableCellProperties();
            TableCellWidth tableCellWidth28 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment28 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark28 = new HideMark();

            tableCellProperties28.Append(tableCellWidth28);
            tableCellProperties28.Append(tableCellVerticalAlignment28);
            tableCellProperties28.Append(hideMark28);

            Paragraph paragraph33 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties33 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties30 = new ParagraphMarkRunProperties();
            RunFonts runFonts36 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages19 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties30.Append(runFonts36);
            paragraphMarkRunProperties30.Append(languages19);

            paragraphProperties33.Append(paragraphMarkRunProperties30);

            paragraph33.Append(paragraphProperties33);

            tableCell28.Append(tableCellProperties28);
            tableCell28.Append(paragraph33);

            tableRow28.Append(tableRowProperties28);
            tableRow28.Append(tableCell28);

            TableRow tableRow29 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties29 = new TableRowProperties();
            TableCellSpacing tableCellSpacing30 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties29.Append(tableCellSpacing30);

            TableCell tableCell29 = new TableCell();

            TableCellProperties tableCellProperties29 = new TableCellProperties();
            TableCellWidth tableCellWidth29 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment29 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark29 = new HideMark();

            tableCellProperties29.Append(tableCellWidth29);
            tableCellProperties29.Append(tableCellVerticalAlignment29);
            tableCellProperties29.Append(hideMark29);

            Paragraph paragraph34 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties34 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties31 = new ParagraphMarkRunProperties();
            RunFonts runFonts37 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages20 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties31.Append(runFonts37);
            paragraphMarkRunProperties31.Append(languages20);

            paragraphProperties34.Append(paragraphMarkRunProperties31);

            paragraph34.Append(paragraphProperties34);

            tableCell29.Append(tableCellProperties29);
            tableCell29.Append(paragraph34);

            tableRow29.Append(tableRowProperties29);
            tableRow29.Append(tableCell29);

            TableRow tableRow30 = new TableRow() { RsidTableRowMarkRevision = "00F42003", RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties30 = new TableRowProperties();
            TableCellSpacing tableCellSpacing31 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties30.Append(tableCellSpacing31);

            TableCell tableCell30 = new TableCell();

            TableCellProperties tableCellProperties30 = new TableCellProperties();
            TableCellWidth tableCellWidth30 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment30 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark30 = new HideMark();

            tableCellProperties30.Append(tableCellWidth30);
            tableCellProperties30.Append(tableCellVerticalAlignment30);
            tableCellProperties30.Append(hideMark30);

            Paragraph paragraph35 = new Paragraph() { RsidParagraphMarkRevision = "00F42003", RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties35 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties32 = new ParagraphMarkRunProperties();
            RunFonts runFonts38 = new RunFonts() { EastAsia = "Times New Roman" };
            Languages languages21 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties32.Append(runFonts38);
            paragraphMarkRunProperties32.Append(languages21);

            paragraphProperties35.Append(paragraphMarkRunProperties32);

            paragraph35.Append(paragraphProperties35);

            tableCell30.Append(tableCellProperties30);
            tableCell30.Append(paragraph35);

            tableRow30.Append(tableRowProperties30);
            tableRow30.Append(tableCell30);

            TableRow tableRow31 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties31 = new TableRowProperties();
            TableCellSpacing tableCellSpacing32 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties31.Append(tableCellSpacing32);

            TableCell tableCell31 = new TableCell();

            TableCellProperties tableCellProperties31 = new TableCellProperties();
            TableCellWidth tableCellWidth31 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment31 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark31 = new HideMark();

            tableCellProperties31.Append(tableCellWidth31);
            tableCellProperties31.Append(tableCellVerticalAlignment31);
            tableCellProperties31.Append(hideMark31);

            Paragraph paragraph36 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties36 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId7 = new ParagraphStyleId() { Val = "NormalWeb" };

            paragraphProperties36.Append(paragraphStyleId7);

            Run run18 = new Run();
            Text text18 = new Text();
            text18.Text = "Thanks,";

            run18.Append(text18);

            paragraph36.Append(paragraphProperties36);
            paragraph36.Append(run18);

            tableCell31.Append(tableCellProperties31);
            tableCell31.Append(paragraph36);

            tableRow31.Append(tableRowProperties31);
            tableRow31.Append(tableCell31);

            TableRow tableRow32 = new TableRow() { RsidTableRowAddition = "00000000" };

            TableRowProperties tableRowProperties32 = new TableRowProperties();
            TableCellSpacing tableCellSpacing33 = new TableCellSpacing() { Width = "15", Type = TableWidthUnitValues.Dxa };

            tableRowProperties32.Append(tableCellSpacing33);

            TableCell tableCell32 = new TableCell();

            TableCellProperties tableCellProperties32 = new TableCellProperties();
            TableCellWidth tableCellWidth32 = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            TableCellVerticalAlignment tableCellVerticalAlignment32 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            HideMark hideMark32 = new HideMark();

            tableCellProperties32.Append(tableCellWidth32);
            tableCellProperties32.Append(tableCellVerticalAlignment32);
            tableCellProperties32.Append(hideMark32);

            Paragraph paragraph37 = new Paragraph() { RsidParagraphAddition = "00000000", RsidRunAdditionDefault = "006160F6" };

            ParagraphProperties paragraphProperties37 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId8 = new ParagraphStyleId() { Val = "NormalWeb" };

            paragraphProperties37.Append(paragraphStyleId8);

            Run run19 = new Run();
            Text text19 = new Text();
            text19.Text = CompanyName;

            run19.Append(text19);

            paragraph37.Append(paragraphProperties37);
            paragraph37.Append(run19);

            tableCell32.Append(tableCellProperties32);
            tableCell32.Append(paragraph37);

            tableRow32.Append(tableRowProperties32);
            tableRow32.Append(tableCell32);

            table1.Append(tableProperties1);
            table1.Append(tableGrid1);
            table1.Append(tableRow1);
            table1.Append(tableRow2);
            table1.Append(tableRow3);
            table1.Append(tableRow4);
            table1.Append(tableRow5);
            table1.Append(tableRow6);
            table1.Append(tableRow7);
            table1.Append(tableRow8);
            table1.Append(tableRow9);
            table1.Append(tableRow10);
            table1.Append(tableRow11);
            table1.Append(tableRow12);
            table1.Append(tableRow13);
            table1.Append(tableRow14);
            table1.Append(tableRow15);
            table1.Append(tableRow16);
            table1.Append(tableRow17_1);
            table1.Append(tableRow17_2);
            table1.Append(tableRow18);
            table1.Append(tableRow19);
            table1.Append(tableRow20);
            table1.Append(tableRow21);
            table1.Append(tableRow22);
            table1.Append(tableRow23);
            table1.Append(tableRow24);
            table1.Append(tableRow25);
            table1.Append(tableRow26);
            table1.Append(tableRow27);
            table1.Append(tableRow28);
            table1.Append(tableRow29);
            table1.Append(tableRow30);
            table1.Append(tableRow31);
            table1.Append(tableRow32);

            Paragraph paragraph38 = new Paragraph() { RsidParagraphAddition = "006160F6", RsidRunAdditionDefault = "00F42003" };

            ParagraphProperties paragraphProperties38 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties33 = new ParagraphMarkRunProperties();
            RunFonts runFonts39 = new RunFonts() { EastAsia = "Times New Roman" };

            paragraphMarkRunProperties33.Append(runFonts39);

            paragraphProperties38.Append(paragraphMarkRunProperties33);

            Run run20 = new Run();

            RunProperties runProperties15 = new RunProperties();
            RunFonts runFonts40 = new RunFonts() { EastAsia = "Times New Roman" };
            NoProof noProof1 = new NoProof();

            runProperties15.Append(runFonts40);
            runProperties15.Append(noProof1);
            LastRenderedPageBreak lastRenderedPageBreak1 = new LastRenderedPageBreak();

            Drawing drawing1 = new Drawing();

            Wp.Inline inline1 = new Wp.Inline() { DistanceFromTop = (UInt32Value)0U, DistanceFromBottom = (UInt32Value)0U, DistanceFromLeft = (UInt32Value)0U, DistanceFromRight = (UInt32Value)0U };
            Wp.Extent extent1 = new Wp.Extent() { Cx = (long)((Image2.Width / Image2.HorizontalResolution) * 914400L), Cy = (long)((Image2.Height / Image2.VerticalResolution) * 914400L) };
            Wp.EffectExtent effectExtent1 = new Wp.EffectExtent() { LeftEdge = 19050L, TopEdge = 0L, RightEdge = 3175L, BottomEdge = 0L };
            Wp.DocProperties docProperties1 = new Wp.DocProperties() { Id = (UInt32Value)1U, Name = "Picture 0", Description = "order_form_10.png" };

            Wp.NonVisualGraphicFrameDrawingProperties nonVisualGraphicFrameDrawingProperties1 = new Wp.NonVisualGraphicFrameDrawingProperties();

            A.GraphicFrameLocks graphicFrameLocks1 = new A.GraphicFrameLocks() { NoChangeAspect = true };
            graphicFrameLocks1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            nonVisualGraphicFrameDrawingProperties1.Append(graphicFrameLocks1);

            A.Graphic graphic1 = new A.Graphic();
            graphic1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            A.GraphicData graphicData1 = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };

            Pic.Picture picture1 = new Pic.Picture();
            picture1.AddNamespaceDeclaration("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");

            Pic.NonVisualPictureProperties nonVisualPictureProperties1 = new Pic.NonVisualPictureProperties();
            Pic.NonVisualDrawingProperties nonVisualDrawingProperties1 = new Pic.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "order_form_10.png" };
            Pic.NonVisualPictureDrawingProperties nonVisualPictureDrawingProperties1 = new Pic.NonVisualPictureDrawingProperties();

            nonVisualPictureProperties1.Append(nonVisualDrawingProperties1);
            nonVisualPictureProperties1.Append(nonVisualPictureDrawingProperties1);

            Pic.BlipFill blipFill1 = new Pic.BlipFill();
            A.Blip blip1 = new A.Blip() { Embed = "rId5", CompressionState = A.BlipCompressionValues.Print };

            A.Stretch stretch1 = new A.Stretch();
            A.FillRectangle fillRectangle1 = new A.FillRectangle();

            stretch1.Append(fillRectangle1);

            blipFill1.Append(blip1);
            blipFill1.Append(stretch1);

            Pic.ShapeProperties shapeProperties1 = new Pic.ShapeProperties();

            A.Transform2D transform2D1 = new A.Transform2D();
            A.Offset offset1 = new A.Offset() { X = 0L, Y = 0L };
            A.Extents extents1 = new A.Extents() { Cx = (long)((Image2.Width / Image2.HorizontalResolution) * 914400L), Cy = (long)((Image2.Height / Image2.VerticalResolution) * 914400L) };

            transform2D1.Append(offset1);
            transform2D1.Append(extents1);

            A.PresetGeometry presetGeometry1 = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            A.AdjustValueList adjustValueList1 = new A.AdjustValueList();

            presetGeometry1.Append(adjustValueList1);

            shapeProperties1.Append(transform2D1);
            shapeProperties1.Append(presetGeometry1);

            picture1.Append(nonVisualPictureProperties1);
            picture1.Append(blipFill1);
            picture1.Append(shapeProperties1);

            graphicData1.Append(picture1);

            graphic1.Append(graphicData1);

            inline1.Append(extent1);
            inline1.Append(effectExtent1);
            inline1.Append(docProperties1);
            inline1.Append(nonVisualGraphicFrameDrawingProperties1);
            inline1.Append(graphic1);

            drawing1.Append(inline1);

            run20.Append(runProperties15);
            run20.Append(lastRenderedPageBreak1);
            run20.Append(drawing1);

            Run run21 = new Run();

            RunProperties runProperties16 = new RunProperties();
            RunFonts runFonts41 = new RunFonts() { EastAsia = "Times New Roman" };
            NoProof noProof2 = new NoProof();

            runProperties16.Append(runFonts41);
            runProperties16.Append(noProof2);
            LastRenderedPageBreak lastRenderedPageBreak2 = new LastRenderedPageBreak();

            Drawing drawing2 = new Drawing();

            Wp.Inline inline2 = new Wp.Inline() { DistanceFromTop = (UInt32Value)0U, DistanceFromBottom = (UInt32Value)0U, DistanceFromLeft = (UInt32Value)0U, DistanceFromRight = (UInt32Value)0U };
            Wp.Extent extent2 = new Wp.Extent() { Cx = (long)((Image1.Width / Image1.HorizontalResolution) * 914400L), Cy = (long)((Image1.Height / Image1.VerticalResolution) * 914400L) };
            Wp.EffectExtent effectExtent2 = new Wp.EffectExtent() { LeftEdge = 19050L, TopEdge = 0L, RightEdge = 3175L, BottomEdge = 0L };
            Wp.DocProperties docProperties2 = new Wp.DocProperties() { Id = (UInt32Value)2U, Name = "Picture 1", Description = "terms_10.png" };

            Wp.NonVisualGraphicFrameDrawingProperties nonVisualGraphicFrameDrawingProperties2 = new Wp.NonVisualGraphicFrameDrawingProperties();

            A.GraphicFrameLocks graphicFrameLocks2 = new A.GraphicFrameLocks() { NoChangeAspect = true };
            graphicFrameLocks2.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            nonVisualGraphicFrameDrawingProperties2.Append(graphicFrameLocks2);

            A.Graphic graphic2 = new A.Graphic();
            graphic2.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            A.GraphicData graphicData2 = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };

            Pic.Picture picture2 = new Pic.Picture();
            picture2.AddNamespaceDeclaration("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");

            Pic.NonVisualPictureProperties nonVisualPictureProperties2 = new Pic.NonVisualPictureProperties();
            Pic.NonVisualDrawingProperties nonVisualDrawingProperties2 = new Pic.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "terms_10.png" };
            Pic.NonVisualPictureDrawingProperties nonVisualPictureDrawingProperties2 = new Pic.NonVisualPictureDrawingProperties();

            nonVisualPictureProperties2.Append(nonVisualDrawingProperties2);
            nonVisualPictureProperties2.Append(nonVisualPictureDrawingProperties2);

            Pic.BlipFill blipFill2 = new Pic.BlipFill();
            A.Blip blip2 = new A.Blip() { Embed = "rId6", CompressionState = A.BlipCompressionValues.Print };

            A.Stretch stretch2 = new A.Stretch();
            A.FillRectangle fillRectangle2 = new A.FillRectangle();

            stretch2.Append(fillRectangle2);

            blipFill2.Append(blip2);
            blipFill2.Append(stretch2);

            Pic.ShapeProperties shapeProperties2 = new Pic.ShapeProperties();

            A.Transform2D transform2D2 = new A.Transform2D();
            A.Offset offset2 = new A.Offset() { X = 0L, Y = 0L };
            A.Extents extents2 = new A.Extents() { Cx = (long)((Image1.Width / Image1.HorizontalResolution) * 914400L), Cy = (long)((Image1.Height / Image1.VerticalResolution) * 914400L) };

            transform2D2.Append(offset2);
            transform2D2.Append(extents2);

            A.PresetGeometry presetGeometry2 = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            A.AdjustValueList adjustValueList2 = new A.AdjustValueList();

            presetGeometry2.Append(adjustValueList2);

            shapeProperties2.Append(transform2D2);
            shapeProperties2.Append(presetGeometry2);

            picture2.Append(nonVisualPictureProperties2);
            picture2.Append(blipFill2);
            picture2.Append(shapeProperties2);

            graphicData2.Append(picture2);

            graphic2.Append(graphicData2);

            inline2.Append(extent2);
            inline2.Append(effectExtent2);
            inline2.Append(docProperties2);
            inline2.Append(nonVisualGraphicFrameDrawingProperties2);
            inline2.Append(graphic2);

            drawing2.Append(inline2);

            run21.Append(runProperties16);
            run21.Append(lastRenderedPageBreak2);
            run21.Append(drawing2);

            paragraph38.Append(paragraphProperties38);
            paragraph38.Append(run20);
            paragraph38.Append(run21);

            SectionProperties sectionProperties1 = new SectionProperties() { RsidR = "006160F6" };
            PageSize pageSize1 = new PageSize() { Width = (UInt32Value)11906U, Height = (UInt32Value)16838U };
            PageMargin pageMargin1 = new PageMargin() { Top = 1134, Right = (UInt32Value)850U, Bottom = 1134, Left = (UInt32Value)1701U, Header = (UInt32Value)708U, Footer = (UInt32Value)708U, Gutter = (UInt32Value)0U };
            Columns columns1 = new Columns() { Space = "708" };
            DocGrid docGrid1 = new DocGrid() { LinePitch = 360 };

            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(docGrid1);

            body1.Append(table1);

            string cardEnding = "";
            string cardBin = "";
            string cardBrand = "N/A";
            string cardExpiryDate = "";
            string year = "";
            string decrCard = "";
            ChargeHistoryCard chargeHistoryCard;
            ICreditCardContainer creditCardContainer;

            // Information sections
            if (TrialChargeHistory != null)
            {
                body1.Append(CreateHeaderParagraph("Trial Transaction Information"));
                body1.Append(CreateParagraph(string.Format("Transaction Type: {0}", "Settlement")));
                body1.Append(CreateParagraph(string.Format("Transaction Status: {0}", "Completed")));
                body1.Append(CreateParagraph(string.Format("Confirmation Number: {0}", TrialChargeHistory.AuthorizationCode)));
                body1.Append(CreateParagraph(string.Format("Transaction ID: {0}", TrialChargeHistory.TransactionNumber)));
                body1.Append(CreateParagraph(string.Format("Settlement Amount: {0}", TrialChAmount.ToString("0.00"))));
                body1.Append(CreateParagraph(string.Format("Merchant Settlement Transaction ID: {0}", TrialChargeHistory.TransactionNumber)));
                body1.Append(CreateParagraph(string.Format("Transaction Frequency: {0}", "One Time")));

                body1.Append(CreateHeaderParagraph("Trial Authorization Information"));
                body1.Append(CreateParagraph(string.Format("Account Number: {0}", TrialChargeHistory.ChildMID)));
                body1.Append(CreateParagraph(string.Format("Authorization Confirmation Number: {0}", TrialChargeHistory.AuthorizationCode)));
                body1.Append(CreateParagraph(string.Format("Merchant Authorization Transaction ID: {0}", TrialChargeHistory.TransactionNumber)));
                body1.Append(CreateParagraph(string.Format("Remaining to Settle: {0}", "0.00")));
                body1.Append(CreateParagraph(string.Format("Account Currency: {0}", TrialChCurrency)));
                body1.Append(CreateParagraph(string.Format("Transaction Date: {0}", TrialChargeHistory.ChargeDate)));

                //getting card info from ChargeHistoryCard table
                chargeHistoryCard = new ReportService().Load<ChargeHistoryCard>(TrialChargeHistory.ChargeHistoryID);
                creditCardContainer = Billing;
                if (chargeHistoryCard != null)
                    creditCardContainer = chargeHistoryCard;

                cardBrand = "N/A";
                if (creditCardContainer.PaymentTypeID == 2)
                    cardBrand = "Visa";
                else if (creditCardContainer.PaymentTypeID == 3)
                    cardBrand = "Mastercard";

                body1.Append(CreateParagraph(string.Format("Card Brand: {0}", cardBrand)));
                body1.Append(CreateParagraph(string.Format("Card Brand Options: {0}", "")));

                cardExpiryDate = "";
                if (creditCardContainer.ExpMonth != null && creditCardContainer.ExpYear != null)
                {
                    year = "";
                    if (creditCardContainer.ExpYear > 1000)
                        year = creditCardContainer.ExpYear.Value.ToString().Substring(2);
                    else
                        year = creditCardContainer.ExpYear.Value.ToString("00");

                    cardExpiryDate = string.Format("{0}/{1}", creditCardContainer.ExpMonth.Value.ToString("00"), year);
                }

                body1.Append(CreateParagraph(string.Format("Card Expiry Date: {0}", cardExpiryDate)));
                body1.Append(CreateParagraph(string.Format("ECI Code: {0}", "")));

                body1.Append(CreateParagraph(string.Format("Card Ending: {0}", creditCardContainer.CreditCardRight4)));
                body1.Append(CreateParagraph(string.Format("Card Bin: {0}", creditCardContainer.CreditCardLeft6)));
                body1.Append(CreateParagraph(string.Format("Merchant IP Address: {0}", "75.102.12.26")));
                body1.Append(CreateParagraph(string.Format("Auth Code: {0}", TrialChargeHistory.AuthorizationCode)));
                body1.Append(CreateParagraph(string.Format("Auth Mode: {0}", "P")));
                body1.Append(CreateParagraph(string.Format("Security Options: {0}", "ZN")));
                body1.Append(CreateParagraph(string.Format("AVS Information: {0}", "Y")));
                body1.Append(CreateParagraph(string.Format("CVD Information: {0}", "M")));
            }

            if (this.TrialUspsGatewayResponse != null)
            {
                body1.Append(CreateHeaderParagraph("Trial Delivery Information"));
                body1.Append(CreateParagraph(string.Format("Tracking Number: {0}", TrialTrackingNumber)));
                body1.Append(CreateParagraph(string.Format("Status: {0}", TrialUspsGatewayResponse.Event)));
                body1.Append(CreateParagraph(string.Format("Date: {0}", TrialUspsGatewayResponse.EventDate)));
                body1.Append(CreateParagraph(string.Format("Time: {0}", TrialUspsGatewayResponse.EventTime)));
                body1.Append(CreateParagraph(string.Format("Name: {0}", string.Format("{0} {1}", Billing.FirstName, Billing.LastName))));
                body1.Append(CreateParagraph(string.Format("City: {0}", TrialUspsGatewayResponse.EventCity)));
                body1.Append(CreateParagraph(string.Format("State: {0}", TrialUspsGatewayResponse.EventState)));
                body1.Append(CreateParagraph(string.Format("ZIPCode: {0}", TrialUspsGatewayResponse.EventZIPCode)));
            }
            
            body1.Append(CreateHeaderParagraph("Transaction Information"));
            body1.Append(CreateParagraph(string.Format("Transaction Type: {0}", "Settlement")));
            body1.Append(CreateParagraph(string.Format("Transaction Status: {0}", "Completed")));
            body1.Append(CreateParagraph(string.Format("Confirmation Number: {0}", ChargeHistory.AuthorizationCode)));
            body1.Append(CreateParagraph(string.Format("Transaction ID: {0}", ChargeHistory.TransactionNumber)));
            body1.Append(CreateParagraph(string.Format("Settlement Amount: {0}", ChAmount.ToString("0.00"))));
            body1.Append(CreateParagraph(string.Format("Merchant Settlement Transaction ID: {0}", ChargeHistory.TransactionNumber)));
            body1.Append(CreateParagraph(string.Format("Transaction Frequency: {0}", "One Time")));
            
            body1.Append(CreateHeaderParagraph("Authorization Information"));
            body1.Append(CreateParagraph(string.Format("Account Number: {0}", ChargeHistory.ChildMID)));
            body1.Append(CreateParagraph(string.Format("Authorization Confirmation Number: {0}", ChargeHistory.AuthorizationCode)));
            body1.Append(CreateParagraph(string.Format("Merchant Authorization Transaction ID: {0}", ChargeHistory.TransactionNumber)));
            body1.Append(CreateParagraph(string.Format("Remaining to Settle: {0}", "0.00")));
            body1.Append(CreateParagraph(string.Format("Account Currency: {0}", ChCurrency)));
            body1.Append(CreateParagraph(string.Format("Transaction Date: {0}", ChargeHistory.ChargeDate)));

            //getting card info from ChargeHistoryCard table
            chargeHistoryCard = new ReportService().Load<ChargeHistoryCard>(ChargeHistory.ChargeHistoryID);
            creditCardContainer = Billing;
            if (chargeHistoryCard != null)
                creditCardContainer = chargeHistoryCard;

            cardBrand = "N/A";
            if (creditCardContainer.PaymentTypeID == 2)
                cardBrand = "Visa";
            else if (creditCardContainer.PaymentTypeID == 3)
                cardBrand = "Mastercard";

            body1.Append(CreateParagraph(string.Format("Card Brand: {0}", cardBrand)));
            body1.Append(CreateParagraph(string.Format("Card Brand Options: {0}", "")));

            cardExpiryDate = "";
            if (creditCardContainer.ExpMonth != null && creditCardContainer.ExpYear != null)
            {
                year = "";
                if (creditCardContainer.ExpYear > 1000)
                    year = creditCardContainer.ExpYear.Value.ToString().Substring(2);
                else
                    year = creditCardContainer.ExpYear.Value.ToString("00");

                cardExpiryDate = string.Format("{0}/{1}", creditCardContainer.ExpMonth.Value.ToString("00"), year);
            }

            body1.Append(CreateParagraph(string.Format("Card Expiry Date: {0}", cardExpiryDate)));
            body1.Append(CreateParagraph(string.Format("ECI Code: {0}", "")));

            body1.Append(CreateParagraph(string.Format("Card Ending: {0}", creditCardContainer.CreditCardRight4)));
            body1.Append(CreateParagraph(string.Format("Card Bin: {0}", creditCardContainer.CreditCardLeft6)));
            body1.Append(CreateParagraph(string.Format("Merchant IP Address: {0}", "75.102.12.26")));
            body1.Append(CreateParagraph(string.Format("Auth Code: {0}", ChargeHistory.AuthorizationCode)));
            body1.Append(CreateParagraph(string.Format("Auth Mode: {0}", "P")));
            body1.Append(CreateParagraph(string.Format("Security Options: {0}", "ZN")));
            body1.Append(CreateParagraph(string.Format("AVS Information: {0}", "Y")));
            body1.Append(CreateParagraph(string.Format("CVD Information: {0}", "M")));

            if (this.RebillUspsGatewayResponse != null)
            {
                body1.Append(CreateHeaderParagraph("Rebill Delivery Information"));
                body1.Append(CreateParagraph(string.Format("Tracking Number: {0}", RebillTrackingNumber)));
                body1.Append(CreateParagraph(string.Format("Status: {0}", RebillUspsGatewayResponse.Event)));
                body1.Append(CreateParagraph(string.Format("Date: {0}", RebillUspsGatewayResponse.EventDate)));
                body1.Append(CreateParagraph(string.Format("Time: {0}", RebillUspsGatewayResponse.EventTime)));
                body1.Append(CreateParagraph(string.Format("Name: {0}", string.Format("{0} {1}", Billing.FirstName, Billing.LastName))));
                body1.Append(CreateParagraph(string.Format("City: {0}", RebillUspsGatewayResponse.EventCity)));
                body1.Append(CreateParagraph(string.Format("State: {0}", RebillUspsGatewayResponse.EventState)));
                body1.Append(CreateParagraph(string.Format("ZIPCode: {0}", RebillUspsGatewayResponse.EventZIPCode)));
            }

            body1.Append(CreateHeaderParagraph("Customer Information"));
            body1.Append(CreateParagraph(string.Format("Name: {0}", string.Format("{0} {1}", Billing.FirstName, Billing.LastName))));
            body1.Append(CreateParagraph(string.Format("Address: {0}", string.Format("{0} {1}", Billing.Address1, Billing.Address2))));
            body1.Append(CreateParagraph(string.Format("City: {0}", Billing.City)));
            body1.Append(CreateParagraph(string.Format("Province/State: {0}", Billing.State)));
            body1.Append(CreateParagraph(string.Format("Country: {0}", Billing.Country)));
            body1.Append(CreateParagraph(string.Format("Postal/Zip Code: {0}", Billing.Zip)));
            body1.Append(CreateParagraph(string.Format("Email: {0}", Billing.Email)));
            body1.Append(CreateParagraph(string.Format("Phone: {0}", Billing.Phone)));

            //Images
            body1.Append(paragraph38);
            body1.Append(sectionProperties1);

            document1.Append(body1);

            mainDocumentPart1.Document = document1;
        }

        public Paragraph CreateHeaderParagraph(string text)
        {
            Paragraph paragraph1 = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Heading3" };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            Languages languages1 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties1.Append(languages1);

            paragraphProperties1.Append(paragraphStyleId1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run1 = new Run();

            RunProperties runProperties1 = new RunProperties();
            Languages languages2 = new Languages() { Val = "en-US" };

            runProperties1.Append(languages2);
            Text text1 = new Text();
            text1.Text = text;

            run1.Append(runProperties1);
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            return paragraph1;
        }

        public Paragraph CreateParagraph(string text)
        {
            Paragraph paragraph1 = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            Languages languages1 = new Languages() { Val = "en-US" };

            paragraphMarkRunProperties1.Append(languages1);

            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run1 = new Run();

            RunProperties runProperties1 = new RunProperties();
            Languages languages2 = new Languages() { Val = "en-US" };

            runProperties1.Append(languages2);
            Text text1 = new Text();
            text1.Text = text;

            run1.Append(runProperties1);
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            return paragraph1;
        }

        // Generates content of themePart1.
        private void GenerateThemePart1Content(ThemePart themePart1)
        {
            A.Theme theme1 = new A.Theme() { Name = "Office Theme" };
            theme1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            A.ThemeElements themeElements1 = new A.ThemeElements();

            A.ColorScheme colorScheme1 = new A.ColorScheme() { Name = "Office" };

            A.Dark1Color dark1Color1 = new A.Dark1Color();
            A.SystemColor systemColor1 = new A.SystemColor() { Val = A.SystemColorValues.WindowText, LastColor = "000000" };

            dark1Color1.Append(systemColor1);

            A.Light1Color light1Color1 = new A.Light1Color();
            A.SystemColor systemColor2 = new A.SystemColor() { Val = A.SystemColorValues.Window, LastColor = "FFFFFF" };

            light1Color1.Append(systemColor2);

            A.Dark2Color dark2Color1 = new A.Dark2Color();
            A.RgbColorModelHex rgbColorModelHex1 = new A.RgbColorModelHex() { Val = "1F497D" };

            dark2Color1.Append(rgbColorModelHex1);

            A.Light2Color light2Color1 = new A.Light2Color();
            A.RgbColorModelHex rgbColorModelHex2 = new A.RgbColorModelHex() { Val = "EEECE1" };

            light2Color1.Append(rgbColorModelHex2);

            A.Accent1Color accent1Color1 = new A.Accent1Color();
            A.RgbColorModelHex rgbColorModelHex3 = new A.RgbColorModelHex() { Val = "4F81BD" };

            accent1Color1.Append(rgbColorModelHex3);

            A.Accent2Color accent2Color1 = new A.Accent2Color();
            A.RgbColorModelHex rgbColorModelHex4 = new A.RgbColorModelHex() { Val = "C0504D" };

            accent2Color1.Append(rgbColorModelHex4);

            A.Accent3Color accent3Color1 = new A.Accent3Color();
            A.RgbColorModelHex rgbColorModelHex5 = new A.RgbColorModelHex() { Val = "9BBB59" };

            accent3Color1.Append(rgbColorModelHex5);

            A.Accent4Color accent4Color1 = new A.Accent4Color();
            A.RgbColorModelHex rgbColorModelHex6 = new A.RgbColorModelHex() { Val = "8064A2" };

            accent4Color1.Append(rgbColorModelHex6);

            A.Accent5Color accent5Color1 = new A.Accent5Color();
            A.RgbColorModelHex rgbColorModelHex7 = new A.RgbColorModelHex() { Val = "4BACC6" };

            accent5Color1.Append(rgbColorModelHex7);

            A.Accent6Color accent6Color1 = new A.Accent6Color();
            A.RgbColorModelHex rgbColorModelHex8 = new A.RgbColorModelHex() { Val = "F79646" };

            accent6Color1.Append(rgbColorModelHex8);

            A.Hyperlink hyperlink1 = new A.Hyperlink();
            A.RgbColorModelHex rgbColorModelHex9 = new A.RgbColorModelHex() { Val = "0000FF" };

            hyperlink1.Append(rgbColorModelHex9);

            A.FollowedHyperlinkColor followedHyperlinkColor1 = new A.FollowedHyperlinkColor();
            A.RgbColorModelHex rgbColorModelHex10 = new A.RgbColorModelHex() { Val = "800080" };

            followedHyperlinkColor1.Append(rgbColorModelHex10);

            colorScheme1.Append(dark1Color1);
            colorScheme1.Append(light1Color1);
            colorScheme1.Append(dark2Color1);
            colorScheme1.Append(light2Color1);
            colorScheme1.Append(accent1Color1);
            colorScheme1.Append(accent2Color1);
            colorScheme1.Append(accent3Color1);
            colorScheme1.Append(accent4Color1);
            colorScheme1.Append(accent5Color1);
            colorScheme1.Append(accent6Color1);
            colorScheme1.Append(hyperlink1);
            colorScheme1.Append(followedHyperlinkColor1);

            A.FontScheme fontScheme1 = new A.FontScheme() { Name = "Office" };

            A.MajorFont majorFont1 = new A.MajorFont();
            A.LatinFont latinFont1 = new A.LatinFont() { Typeface = "Cambria" };
            A.EastAsianFont eastAsianFont1 = new A.EastAsianFont() { Typeface = "" };
            A.ComplexScriptFont complexScriptFont1 = new A.ComplexScriptFont() { Typeface = "" };
            A.SupplementalFont supplementalFont1 = new A.SupplementalFont() { Script = "Jpan", Typeface = "ＭＳ ゴシック" };
            A.SupplementalFont supplementalFont2 = new A.SupplementalFont() { Script = "Hang", Typeface = "맑은 고딕" };
            A.SupplementalFont supplementalFont3 = new A.SupplementalFont() { Script = "Hans", Typeface = "宋体" };
            A.SupplementalFont supplementalFont4 = new A.SupplementalFont() { Script = "Hant", Typeface = "新細明體" };
            A.SupplementalFont supplementalFont5 = new A.SupplementalFont() { Script = "Arab", Typeface = "Times New Roman" };
            A.SupplementalFont supplementalFont6 = new A.SupplementalFont() { Script = "Hebr", Typeface = "Times New Roman" };
            A.SupplementalFont supplementalFont7 = new A.SupplementalFont() { Script = "Thai", Typeface = "Angsana New" };
            A.SupplementalFont supplementalFont8 = new A.SupplementalFont() { Script = "Ethi", Typeface = "Nyala" };
            A.SupplementalFont supplementalFont9 = new A.SupplementalFont() { Script = "Beng", Typeface = "Vrinda" };
            A.SupplementalFont supplementalFont10 = new A.SupplementalFont() { Script = "Gujr", Typeface = "Shruti" };
            A.SupplementalFont supplementalFont11 = new A.SupplementalFont() { Script = "Khmr", Typeface = "MoolBoran" };
            A.SupplementalFont supplementalFont12 = new A.SupplementalFont() { Script = "Knda", Typeface = "Tunga" };
            A.SupplementalFont supplementalFont13 = new A.SupplementalFont() { Script = "Guru", Typeface = "Raavi" };
            A.SupplementalFont supplementalFont14 = new A.SupplementalFont() { Script = "Cans", Typeface = "Euphemia" };
            A.SupplementalFont supplementalFont15 = new A.SupplementalFont() { Script = "Cher", Typeface = "Plantagenet Cherokee" };
            A.SupplementalFont supplementalFont16 = new A.SupplementalFont() { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
            A.SupplementalFont supplementalFont17 = new A.SupplementalFont() { Script = "Tibt", Typeface = "Microsoft Himalaya" };
            A.SupplementalFont supplementalFont18 = new A.SupplementalFont() { Script = "Thaa", Typeface = "MV Boli" };
            A.SupplementalFont supplementalFont19 = new A.SupplementalFont() { Script = "Deva", Typeface = "Mangal" };
            A.SupplementalFont supplementalFont20 = new A.SupplementalFont() { Script = "Telu", Typeface = "Gautami" };
            A.SupplementalFont supplementalFont21 = new A.SupplementalFont() { Script = "Taml", Typeface = "Latha" };
            A.SupplementalFont supplementalFont22 = new A.SupplementalFont() { Script = "Syrc", Typeface = "Estrangelo Edessa" };
            A.SupplementalFont supplementalFont23 = new A.SupplementalFont() { Script = "Orya", Typeface = "Kalinga" };
            A.SupplementalFont supplementalFont24 = new A.SupplementalFont() { Script = "Mlym", Typeface = "Kartika" };
            A.SupplementalFont supplementalFont25 = new A.SupplementalFont() { Script = "Laoo", Typeface = "DokChampa" };
            A.SupplementalFont supplementalFont26 = new A.SupplementalFont() { Script = "Sinh", Typeface = "Iskoola Pota" };
            A.SupplementalFont supplementalFont27 = new A.SupplementalFont() { Script = "Mong", Typeface = "Mongolian Baiti" };
            A.SupplementalFont supplementalFont28 = new A.SupplementalFont() { Script = "Viet", Typeface = "Times New Roman" };
            A.SupplementalFont supplementalFont29 = new A.SupplementalFont() { Script = "Uigh", Typeface = "Microsoft Uighur" };

            majorFont1.Append(latinFont1);
            majorFont1.Append(eastAsianFont1);
            majorFont1.Append(complexScriptFont1);
            majorFont1.Append(supplementalFont1);
            majorFont1.Append(supplementalFont2);
            majorFont1.Append(supplementalFont3);
            majorFont1.Append(supplementalFont4);
            majorFont1.Append(supplementalFont5);
            majorFont1.Append(supplementalFont6);
            majorFont1.Append(supplementalFont7);
            majorFont1.Append(supplementalFont8);
            majorFont1.Append(supplementalFont9);
            majorFont1.Append(supplementalFont10);
            majorFont1.Append(supplementalFont11);
            majorFont1.Append(supplementalFont12);
            majorFont1.Append(supplementalFont13);
            majorFont1.Append(supplementalFont14);
            majorFont1.Append(supplementalFont15);
            majorFont1.Append(supplementalFont16);
            majorFont1.Append(supplementalFont17);
            majorFont1.Append(supplementalFont18);
            majorFont1.Append(supplementalFont19);
            majorFont1.Append(supplementalFont20);
            majorFont1.Append(supplementalFont21);
            majorFont1.Append(supplementalFont22);
            majorFont1.Append(supplementalFont23);
            majorFont1.Append(supplementalFont24);
            majorFont1.Append(supplementalFont25);
            majorFont1.Append(supplementalFont26);
            majorFont1.Append(supplementalFont27);
            majorFont1.Append(supplementalFont28);
            majorFont1.Append(supplementalFont29);

            A.MinorFont minorFont1 = new A.MinorFont();
            A.LatinFont latinFont2 = new A.LatinFont() { Typeface = "Calibri" };
            A.EastAsianFont eastAsianFont2 = new A.EastAsianFont() { Typeface = "" };
            A.ComplexScriptFont complexScriptFont2 = new A.ComplexScriptFont() { Typeface = "" };
            A.SupplementalFont supplementalFont30 = new A.SupplementalFont() { Script = "Jpan", Typeface = "ＭＳ 明朝" };
            A.SupplementalFont supplementalFont31 = new A.SupplementalFont() { Script = "Hang", Typeface = "맑은 고딕" };
            A.SupplementalFont supplementalFont32 = new A.SupplementalFont() { Script = "Hans", Typeface = "宋体" };
            A.SupplementalFont supplementalFont33 = new A.SupplementalFont() { Script = "Hant", Typeface = "新細明體" };
            A.SupplementalFont supplementalFont34 = new A.SupplementalFont() { Script = "Arab", Typeface = "Arial" };
            A.SupplementalFont supplementalFont35 = new A.SupplementalFont() { Script = "Hebr", Typeface = "Arial" };
            A.SupplementalFont supplementalFont36 = new A.SupplementalFont() { Script = "Thai", Typeface = "Cordia New" };
            A.SupplementalFont supplementalFont37 = new A.SupplementalFont() { Script = "Ethi", Typeface = "Nyala" };
            A.SupplementalFont supplementalFont38 = new A.SupplementalFont() { Script = "Beng", Typeface = "Vrinda" };
            A.SupplementalFont supplementalFont39 = new A.SupplementalFont() { Script = "Gujr", Typeface = "Shruti" };
            A.SupplementalFont supplementalFont40 = new A.SupplementalFont() { Script = "Khmr", Typeface = "DaunPenh" };
            A.SupplementalFont supplementalFont41 = new A.SupplementalFont() { Script = "Knda", Typeface = "Tunga" };
            A.SupplementalFont supplementalFont42 = new A.SupplementalFont() { Script = "Guru", Typeface = "Raavi" };
            A.SupplementalFont supplementalFont43 = new A.SupplementalFont() { Script = "Cans", Typeface = "Euphemia" };
            A.SupplementalFont supplementalFont44 = new A.SupplementalFont() { Script = "Cher", Typeface = "Plantagenet Cherokee" };
            A.SupplementalFont supplementalFont45 = new A.SupplementalFont() { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
            A.SupplementalFont supplementalFont46 = new A.SupplementalFont() { Script = "Tibt", Typeface = "Microsoft Himalaya" };
            A.SupplementalFont supplementalFont47 = new A.SupplementalFont() { Script = "Thaa", Typeface = "MV Boli" };
            A.SupplementalFont supplementalFont48 = new A.SupplementalFont() { Script = "Deva", Typeface = "Mangal" };
            A.SupplementalFont supplementalFont49 = new A.SupplementalFont() { Script = "Telu", Typeface = "Gautami" };
            A.SupplementalFont supplementalFont50 = new A.SupplementalFont() { Script = "Taml", Typeface = "Latha" };
            A.SupplementalFont supplementalFont51 = new A.SupplementalFont() { Script = "Syrc", Typeface = "Estrangelo Edessa" };
            A.SupplementalFont supplementalFont52 = new A.SupplementalFont() { Script = "Orya", Typeface = "Kalinga" };
            A.SupplementalFont supplementalFont53 = new A.SupplementalFont() { Script = "Mlym", Typeface = "Kartika" };
            A.SupplementalFont supplementalFont54 = new A.SupplementalFont() { Script = "Laoo", Typeface = "DokChampa" };
            A.SupplementalFont supplementalFont55 = new A.SupplementalFont() { Script = "Sinh", Typeface = "Iskoola Pota" };
            A.SupplementalFont supplementalFont56 = new A.SupplementalFont() { Script = "Mong", Typeface = "Mongolian Baiti" };
            A.SupplementalFont supplementalFont57 = new A.SupplementalFont() { Script = "Viet", Typeface = "Arial" };
            A.SupplementalFont supplementalFont58 = new A.SupplementalFont() { Script = "Uigh", Typeface = "Microsoft Uighur" };

            minorFont1.Append(latinFont2);
            minorFont1.Append(eastAsianFont2);
            minorFont1.Append(complexScriptFont2);
            minorFont1.Append(supplementalFont30);
            minorFont1.Append(supplementalFont31);
            minorFont1.Append(supplementalFont32);
            minorFont1.Append(supplementalFont33);
            minorFont1.Append(supplementalFont34);
            minorFont1.Append(supplementalFont35);
            minorFont1.Append(supplementalFont36);
            minorFont1.Append(supplementalFont37);
            minorFont1.Append(supplementalFont38);
            minorFont1.Append(supplementalFont39);
            minorFont1.Append(supplementalFont40);
            minorFont1.Append(supplementalFont41);
            minorFont1.Append(supplementalFont42);
            minorFont1.Append(supplementalFont43);
            minorFont1.Append(supplementalFont44);
            minorFont1.Append(supplementalFont45);
            minorFont1.Append(supplementalFont46);
            minorFont1.Append(supplementalFont47);
            minorFont1.Append(supplementalFont48);
            minorFont1.Append(supplementalFont49);
            minorFont1.Append(supplementalFont50);
            minorFont1.Append(supplementalFont51);
            minorFont1.Append(supplementalFont52);
            minorFont1.Append(supplementalFont53);
            minorFont1.Append(supplementalFont54);
            minorFont1.Append(supplementalFont55);
            minorFont1.Append(supplementalFont56);
            minorFont1.Append(supplementalFont57);
            minorFont1.Append(supplementalFont58);

            fontScheme1.Append(majorFont1);
            fontScheme1.Append(minorFont1);

            A.FormatScheme formatScheme1 = new A.FormatScheme() { Name = "Office" };

            A.FillStyleList fillStyleList1 = new A.FillStyleList();

            A.SolidFill solidFill1 = new A.SolidFill();
            A.SchemeColor schemeColor1 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill1.Append(schemeColor1);

            A.GradientFill gradientFill1 = new A.GradientFill() { RotateWithShape = true };

            A.GradientStopList gradientStopList1 = new A.GradientStopList();

            A.GradientStop gradientStop1 = new A.GradientStop() { Position = 0 };

            A.SchemeColor schemeColor2 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint1 = new A.Tint() { Val = 50000 };
            A.SaturationModulation saturationModulation1 = new A.SaturationModulation() { Val = 300000 };

            schemeColor2.Append(tint1);
            schemeColor2.Append(saturationModulation1);

            gradientStop1.Append(schemeColor2);

            A.GradientStop gradientStop2 = new A.GradientStop() { Position = 35000 };

            A.SchemeColor schemeColor3 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint2 = new A.Tint() { Val = 37000 };
            A.SaturationModulation saturationModulation2 = new A.SaturationModulation() { Val = 300000 };

            schemeColor3.Append(tint2);
            schemeColor3.Append(saturationModulation2);

            gradientStop2.Append(schemeColor3);

            A.GradientStop gradientStop3 = new A.GradientStop() { Position = 100000 };

            A.SchemeColor schemeColor4 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint3 = new A.Tint() { Val = 15000 };
            A.SaturationModulation saturationModulation3 = new A.SaturationModulation() { Val = 350000 };

            schemeColor4.Append(tint3);
            schemeColor4.Append(saturationModulation3);

            gradientStop3.Append(schemeColor4);

            gradientStopList1.Append(gradientStop1);
            gradientStopList1.Append(gradientStop2);
            gradientStopList1.Append(gradientStop3);
            A.LinearGradientFill linearGradientFill1 = new A.LinearGradientFill() { Angle = 16200000, Scaled = true };

            gradientFill1.Append(gradientStopList1);
            gradientFill1.Append(linearGradientFill1);

            A.GradientFill gradientFill2 = new A.GradientFill() { RotateWithShape = true };

            A.GradientStopList gradientStopList2 = new A.GradientStopList();

            A.GradientStop gradientStop4 = new A.GradientStop() { Position = 0 };

            A.SchemeColor schemeColor5 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Shade shade1 = new A.Shade() { Val = 51000 };
            A.SaturationModulation saturationModulation4 = new A.SaturationModulation() { Val = 130000 };

            schemeColor5.Append(shade1);
            schemeColor5.Append(saturationModulation4);

            gradientStop4.Append(schemeColor5);

            A.GradientStop gradientStop5 = new A.GradientStop() { Position = 80000 };

            A.SchemeColor schemeColor6 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Shade shade2 = new A.Shade() { Val = 93000 };
            A.SaturationModulation saturationModulation5 = new A.SaturationModulation() { Val = 130000 };

            schemeColor6.Append(shade2);
            schemeColor6.Append(saturationModulation5);

            gradientStop5.Append(schemeColor6);

            A.GradientStop gradientStop6 = new A.GradientStop() { Position = 100000 };

            A.SchemeColor schemeColor7 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Shade shade3 = new A.Shade() { Val = 94000 };
            A.SaturationModulation saturationModulation6 = new A.SaturationModulation() { Val = 135000 };

            schemeColor7.Append(shade3);
            schemeColor7.Append(saturationModulation6);

            gradientStop6.Append(schemeColor7);

            gradientStopList2.Append(gradientStop4);
            gradientStopList2.Append(gradientStop5);
            gradientStopList2.Append(gradientStop6);
            A.LinearGradientFill linearGradientFill2 = new A.LinearGradientFill() { Angle = 16200000, Scaled = false };

            gradientFill2.Append(gradientStopList2);
            gradientFill2.Append(linearGradientFill2);

            fillStyleList1.Append(solidFill1);
            fillStyleList1.Append(gradientFill1);
            fillStyleList1.Append(gradientFill2);

            A.LineStyleList lineStyleList1 = new A.LineStyleList();

            A.Outline outline1 = new A.Outline() { Width = 9525, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

            A.SolidFill solidFill2 = new A.SolidFill();

            A.SchemeColor schemeColor8 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Shade shade4 = new A.Shade() { Val = 95000 };
            A.SaturationModulation saturationModulation7 = new A.SaturationModulation() { Val = 105000 };

            schemeColor8.Append(shade4);
            schemeColor8.Append(saturationModulation7);

            solidFill2.Append(schemeColor8);
            A.PresetDash presetDash1 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };

            outline1.Append(solidFill2);
            outline1.Append(presetDash1);

            A.Outline outline2 = new A.Outline() { Width = 25400, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

            A.SolidFill solidFill3 = new A.SolidFill();
            A.SchemeColor schemeColor9 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill3.Append(schemeColor9);
            A.PresetDash presetDash2 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };

            outline2.Append(solidFill3);
            outline2.Append(presetDash2);

            A.Outline outline3 = new A.Outline() { Width = 38100, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

            A.SolidFill solidFill4 = new A.SolidFill();
            A.SchemeColor schemeColor10 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill4.Append(schemeColor10);
            A.PresetDash presetDash3 = new A.PresetDash() { Val = A.PresetLineDashValues.Solid };

            outline3.Append(solidFill4);
            outline3.Append(presetDash3);

            lineStyleList1.Append(outline1);
            lineStyleList1.Append(outline2);
            lineStyleList1.Append(outline3);

            A.EffectStyleList effectStyleList1 = new A.EffectStyleList();

            A.EffectStyle effectStyle1 = new A.EffectStyle();

            A.EffectList effectList1 = new A.EffectList();

            A.OuterShadow outerShadow1 = new A.OuterShadow() { BlurRadius = 40000L, Distance = 20000L, Direction = 5400000, RotateWithShape = false };

            A.RgbColorModelHex rgbColorModelHex11 = new A.RgbColorModelHex() { Val = "000000" };
            A.Alpha alpha1 = new A.Alpha() { Val = 38000 };

            rgbColorModelHex11.Append(alpha1);

            outerShadow1.Append(rgbColorModelHex11);

            effectList1.Append(outerShadow1);

            effectStyle1.Append(effectList1);

            A.EffectStyle effectStyle2 = new A.EffectStyle();

            A.EffectList effectList2 = new A.EffectList();

            A.OuterShadow outerShadow2 = new A.OuterShadow() { BlurRadius = 40000L, Distance = 23000L, Direction = 5400000, RotateWithShape = false };

            A.RgbColorModelHex rgbColorModelHex12 = new A.RgbColorModelHex() { Val = "000000" };
            A.Alpha alpha2 = new A.Alpha() { Val = 35000 };

            rgbColorModelHex12.Append(alpha2);

            outerShadow2.Append(rgbColorModelHex12);

            effectList2.Append(outerShadow2);

            effectStyle2.Append(effectList2);

            A.EffectStyle effectStyle3 = new A.EffectStyle();

            A.EffectList effectList3 = new A.EffectList();

            A.OuterShadow outerShadow3 = new A.OuterShadow() { BlurRadius = 40000L, Distance = 23000L, Direction = 5400000, RotateWithShape = false };

            A.RgbColorModelHex rgbColorModelHex13 = new A.RgbColorModelHex() { Val = "000000" };
            A.Alpha alpha3 = new A.Alpha() { Val = 35000 };

            rgbColorModelHex13.Append(alpha3);

            outerShadow3.Append(rgbColorModelHex13);

            effectList3.Append(outerShadow3);

            A.Scene3DType scene3DType1 = new A.Scene3DType();

            A.Camera camera1 = new A.Camera() { Preset = A.PresetCameraValues.OrthographicFront };
            A.Rotation rotation1 = new A.Rotation() { Latitude = 0, Longitude = 0, Revolution = 0 };

            camera1.Append(rotation1);

            A.LightRig lightRig1 = new A.LightRig() { Rig = A.LightRigValues.ThreePoints, Direction = A.LightRigDirectionValues.Top };
            A.Rotation rotation2 = new A.Rotation() { Latitude = 0, Longitude = 0, Revolution = 1200000 };

            lightRig1.Append(rotation2);

            scene3DType1.Append(camera1);
            scene3DType1.Append(lightRig1);

            A.Shape3DType shape3DType1 = new A.Shape3DType();
            A.BevelTop bevelTop1 = new A.BevelTop() { Width = 63500L, Height = 25400L };

            shape3DType1.Append(bevelTop1);

            effectStyle3.Append(effectList3);
            effectStyle3.Append(scene3DType1);
            effectStyle3.Append(shape3DType1);

            effectStyleList1.Append(effectStyle1);
            effectStyleList1.Append(effectStyle2);
            effectStyleList1.Append(effectStyle3);

            A.BackgroundFillStyleList backgroundFillStyleList1 = new A.BackgroundFillStyleList();

            A.SolidFill solidFill5 = new A.SolidFill();
            A.SchemeColor schemeColor11 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };

            solidFill5.Append(schemeColor11);

            A.GradientFill gradientFill3 = new A.GradientFill() { RotateWithShape = true };

            A.GradientStopList gradientStopList3 = new A.GradientStopList();

            A.GradientStop gradientStop7 = new A.GradientStop() { Position = 0 };

            A.SchemeColor schemeColor12 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint4 = new A.Tint() { Val = 40000 };
            A.SaturationModulation saturationModulation8 = new A.SaturationModulation() { Val = 350000 };

            schemeColor12.Append(tint4);
            schemeColor12.Append(saturationModulation8);

            gradientStop7.Append(schemeColor12);

            A.GradientStop gradientStop8 = new A.GradientStop() { Position = 40000 };

            A.SchemeColor schemeColor13 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint5 = new A.Tint() { Val = 45000 };
            A.Shade shade5 = new A.Shade() { Val = 99000 };
            A.SaturationModulation saturationModulation9 = new A.SaturationModulation() { Val = 350000 };

            schemeColor13.Append(tint5);
            schemeColor13.Append(shade5);
            schemeColor13.Append(saturationModulation9);

            gradientStop8.Append(schemeColor13);

            A.GradientStop gradientStop9 = new A.GradientStop() { Position = 100000 };

            A.SchemeColor schemeColor14 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Shade shade6 = new A.Shade() { Val = 20000 };
            A.SaturationModulation saturationModulation10 = new A.SaturationModulation() { Val = 255000 };

            schemeColor14.Append(shade6);
            schemeColor14.Append(saturationModulation10);

            gradientStop9.Append(schemeColor14);

            gradientStopList3.Append(gradientStop7);
            gradientStopList3.Append(gradientStop8);
            gradientStopList3.Append(gradientStop9);

            A.PathGradientFill pathGradientFill1 = new A.PathGradientFill() { Path = A.PathShadeValues.Circle };
            A.FillToRectangle fillToRectangle1 = new A.FillToRectangle() { Left = 50000, Top = -80000, Right = 50000, Bottom = 180000 };

            pathGradientFill1.Append(fillToRectangle1);

            gradientFill3.Append(gradientStopList3);
            gradientFill3.Append(pathGradientFill1);

            A.GradientFill gradientFill4 = new A.GradientFill() { RotateWithShape = true };

            A.GradientStopList gradientStopList4 = new A.GradientStopList();

            A.GradientStop gradientStop10 = new A.GradientStop() { Position = 0 };

            A.SchemeColor schemeColor15 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Tint tint6 = new A.Tint() { Val = 80000 };
            A.SaturationModulation saturationModulation11 = new A.SaturationModulation() { Val = 300000 };

            schemeColor15.Append(tint6);
            schemeColor15.Append(saturationModulation11);

            gradientStop10.Append(schemeColor15);

            A.GradientStop gradientStop11 = new A.GradientStop() { Position = 100000 };

            A.SchemeColor schemeColor16 = new A.SchemeColor() { Val = A.SchemeColorValues.PhColor };
            A.Shade shade7 = new A.Shade() { Val = 30000 };
            A.SaturationModulation saturationModulation12 = new A.SaturationModulation() { Val = 200000 };

            schemeColor16.Append(shade7);
            schemeColor16.Append(saturationModulation12);

            gradientStop11.Append(schemeColor16);

            gradientStopList4.Append(gradientStop10);
            gradientStopList4.Append(gradientStop11);

            A.PathGradientFill pathGradientFill2 = new A.PathGradientFill() { Path = A.PathShadeValues.Circle };
            A.FillToRectangle fillToRectangle2 = new A.FillToRectangle() { Left = 50000, Top = 50000, Right = 50000, Bottom = 50000 };

            pathGradientFill2.Append(fillToRectangle2);

            gradientFill4.Append(gradientStopList4);
            gradientFill4.Append(pathGradientFill2);

            backgroundFillStyleList1.Append(solidFill5);
            backgroundFillStyleList1.Append(gradientFill3);
            backgroundFillStyleList1.Append(gradientFill4);

            formatScheme1.Append(fillStyleList1);
            formatScheme1.Append(lineStyleList1);
            formatScheme1.Append(effectStyleList1);
            formatScheme1.Append(backgroundFillStyleList1);

            themeElements1.Append(colorScheme1);
            themeElements1.Append(fontScheme1);
            themeElements1.Append(formatScheme1);
            A.ObjectDefaults objectDefaults1 = new A.ObjectDefaults();
            A.ExtraColorSchemeList extraColorSchemeList1 = new A.ExtraColorSchemeList();

            theme1.Append(themeElements1);
            theme1.Append(objectDefaults1);
            theme1.Append(extraColorSchemeList1);

            themePart1.Theme = theme1;
        }

        // Generates content of documentSettingsPart1.
        private void GenerateDocumentSettingsPart1Content(DocumentSettingsPart documentSettingsPart1)
        {
            Settings settings1 = new Settings();
            settings1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            settings1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            settings1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            settings1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            settings1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            settings1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            settings1.AddNamespaceDeclaration("sl", "http://schemas.openxmlformats.org/schemaLibrary/2006/main");
            Zoom zoom1 = new Zoom() { Percent = "100" };
            ProofState proofState1 = new ProofState() { Spelling = ProofingStateValues.Clean, Grammar = ProofingStateValues.Clean };
            DefaultTabStop defaultTabStop1 = new DefaultTabStop() { Val = 708 };
            NoPunctuationKerning noPunctuationKerning1 = new NoPunctuationKerning();
            CharacterSpacingControl characterSpacingControl1 = new CharacterSpacingControl() { Val = CharacterSpacingValues.DoNotCompress };
            Compatibility compatibility1 = new Compatibility();

            Rsids rsids1 = new Rsids();
            RsidRoot rsidRoot1 = new RsidRoot() { Val = "00F42003" };
            Rsid rsid1 = new Rsid() { Val = "006160F6" };
            Rsid rsid2 = new Rsid() { Val = "00F42003" };

            rsids1.Append(rsidRoot1);
            rsids1.Append(rsid1);
            rsids1.Append(rsid2);

            M.MathProperties mathProperties1 = new M.MathProperties();
            M.MathFont mathFont1 = new M.MathFont() { Val = "Cambria Math" };
            M.BreakBinary breakBinary1 = new M.BreakBinary() { Val = M.BreakBinaryOperatorValues.Before };
            M.BreakBinarySubtraction breakBinarySubtraction1 = new M.BreakBinarySubtraction() { Val = M.BreakBinarySubtractionValues.MinusMinus };
            M.SmallFraction smallFraction1 = new M.SmallFraction() { Val = M.BooleanValues.Off };
            M.DisplayDefaults displayDefaults1 = new M.DisplayDefaults();
            M.LeftMargin leftMargin1 = new M.LeftMargin() { Val = (UInt32Value)0U };
            M.RightMargin rightMargin1 = new M.RightMargin() { Val = (UInt32Value)0U };
            M.DefaultJustification defaultJustification1 = new M.DefaultJustification() { Val = M.JustificationValues.CenterGroup };
            M.WrapIndent wrapIndent1 = new M.WrapIndent() { Val = (UInt32Value)1440U };
            M.IntegralLimitLocation integralLimitLocation1 = new M.IntegralLimitLocation() { Val = M.LimitLocationValues.SubscriptSuperscript };
            M.NaryLimitLocation naryLimitLocation1 = new M.NaryLimitLocation() { Val = M.LimitLocationValues.UnderOver };

            mathProperties1.Append(mathFont1);
            mathProperties1.Append(breakBinary1);
            mathProperties1.Append(breakBinarySubtraction1);
            mathProperties1.Append(smallFraction1);
            mathProperties1.Append(displayDefaults1);
            mathProperties1.Append(leftMargin1);
            mathProperties1.Append(rightMargin1);
            mathProperties1.Append(defaultJustification1);
            mathProperties1.Append(wrapIndent1);
            mathProperties1.Append(integralLimitLocation1);
            mathProperties1.Append(naryLimitLocation1);
            ThemeFontLanguages themeFontLanguages1 = new ThemeFontLanguages() { Val = "ru-RU" };
            ColorSchemeMapping colorSchemeMapping1 = new ColorSchemeMapping() { Background1 = ColorSchemeIndexValues.Light1, Text1 = ColorSchemeIndexValues.Dark1, Background2 = ColorSchemeIndexValues.Light2, Text2 = ColorSchemeIndexValues.Dark2, Accent1 = ColorSchemeIndexValues.Accent1, Accent2 = ColorSchemeIndexValues.Accent2, Accent3 = ColorSchemeIndexValues.Accent3, Accent4 = ColorSchemeIndexValues.Accent4, Accent5 = ColorSchemeIndexValues.Accent5, Accent6 = ColorSchemeIndexValues.Accent6, Hyperlink = ColorSchemeIndexValues.Hyperlink, FollowedHyperlink = ColorSchemeIndexValues.FollowedHyperlink };
            DoNotIncludeSubdocsInStats doNotIncludeSubdocsInStats1 = new DoNotIncludeSubdocsInStats();

            ShapeDefaults shapeDefaults1 = new ShapeDefaults();
            Ovml.ShapeDefaults shapeDefaults2 = new Ovml.ShapeDefaults() { Extension = V.ExtensionHandlingBehaviorValues.Edit, MaxShapeId = 2050 };

            Ovml.ShapeLayout shapeLayout1 = new Ovml.ShapeLayout() { Extension = V.ExtensionHandlingBehaviorValues.Edit };
            Ovml.ShapeIdMap shapeIdMap1 = new Ovml.ShapeIdMap() { Extension = V.ExtensionHandlingBehaviorValues.Edit, Data = "1" };

            shapeLayout1.Append(shapeIdMap1);

            shapeDefaults1.Append(shapeDefaults2);
            shapeDefaults1.Append(shapeLayout1);
            DecimalSymbol decimalSymbol1 = new DecimalSymbol() { Val = "," };
            ListSeparator listSeparator1 = new ListSeparator() { Val = ";" };

            settings1.Append(zoom1);
            settings1.Append(proofState1);
            settings1.Append(defaultTabStop1);
            settings1.Append(noPunctuationKerning1);
            settings1.Append(characterSpacingControl1);
            settings1.Append(compatibility1);
            settings1.Append(rsids1);
            settings1.Append(mathProperties1);
            settings1.Append(themeFontLanguages1);
            settings1.Append(colorSchemeMapping1);
            settings1.Append(doNotIncludeSubdocsInStats1);
            settings1.Append(shapeDefaults1);
            settings1.Append(decimalSymbol1);
            settings1.Append(listSeparator1);

            documentSettingsPart1.Settings = settings1;
        }

        // Generates content of fontTablePart1.
        private void GenerateFontTablePart1Content(FontTablePart fontTablePart1)
        {
            Fonts fonts1 = new Fonts();
            fonts1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            fonts1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            Font font1 = new Font() { Name = "Symbol" };
            Panose1Number panose1Number1 = new Panose1Number() { Val = "05050102010706020507" };
            FontCharSet fontCharSet1 = new FontCharSet() { Val = "02" };
            FontFamily fontFamily1 = new FontFamily() { Val = FontFamilyValues.Roman };
            Pitch pitch1 = new Pitch() { Val = FontPitchValues.Variable };
            FontSignature fontSignature1 = new FontSignature() { UnicodeSignature0 = "00000000", UnicodeSignature1 = "10000000", UnicodeSignature2 = "00000000", UnicodeSignature3 = "00000000", CodePageSignature0 = "80000000", CodePageSignature1 = "00000000" };

            font1.Append(panose1Number1);
            font1.Append(fontCharSet1);
            font1.Append(fontFamily1);
            font1.Append(pitch1);
            font1.Append(fontSignature1);

            Font font2 = new Font() { Name = "Times New Roman" };
            Panose1Number panose1Number2 = new Panose1Number() { Val = "02020603050405020304" };
            FontCharSet fontCharSet2 = new FontCharSet() { Val = "CC" };
            FontFamily fontFamily2 = new FontFamily() { Val = FontFamilyValues.Roman };
            Pitch pitch2 = new Pitch() { Val = FontPitchValues.Variable };
            FontSignature fontSignature2 = new FontSignature() { UnicodeSignature0 = "20002A87", UnicodeSignature1 = "80000000", UnicodeSignature2 = "00000008", UnicodeSignature3 = "00000000", CodePageSignature0 = "000001FF", CodePageSignature1 = "00000000" };

            font2.Append(panose1Number2);
            font2.Append(fontCharSet2);
            font2.Append(fontFamily2);
            font2.Append(pitch2);
            font2.Append(fontSignature2);

            Font font3 = new Font() { Name = "Courier New" };
            Panose1Number panose1Number3 = new Panose1Number() { Val = "02070309020205020404" };
            FontCharSet fontCharSet3 = new FontCharSet() { Val = "CC" };
            FontFamily fontFamily3 = new FontFamily() { Val = FontFamilyValues.Modern };
            Pitch pitch3 = new Pitch() { Val = FontPitchValues.Fixed };
            FontSignature fontSignature3 = new FontSignature() { UnicodeSignature0 = "20002A87", UnicodeSignature1 = "80000000", UnicodeSignature2 = "00000008", UnicodeSignature3 = "00000000", CodePageSignature0 = "000001FF", CodePageSignature1 = "00000000" };

            font3.Append(panose1Number3);
            font3.Append(fontCharSet3);
            font3.Append(fontFamily3);
            font3.Append(pitch3);
            font3.Append(fontSignature3);

            Font font4 = new Font() { Name = "Wingdings" };
            Panose1Number panose1Number4 = new Panose1Number() { Val = "05000000000000000000" };
            FontCharSet fontCharSet4 = new FontCharSet() { Val = "02" };
            FontFamily fontFamily4 = new FontFamily() { Val = FontFamilyValues.Auto };
            Pitch pitch4 = new Pitch() { Val = FontPitchValues.Variable };
            FontSignature fontSignature4 = new FontSignature() { UnicodeSignature0 = "00000000", UnicodeSignature1 = "10000000", UnicodeSignature2 = "00000000", UnicodeSignature3 = "00000000", CodePageSignature0 = "80000000", CodePageSignature1 = "00000000" };

            font4.Append(panose1Number4);
            font4.Append(fontCharSet4);
            font4.Append(fontFamily4);
            font4.Append(pitch4);
            font4.Append(fontSignature4);

            Font font5 = new Font() { Name = "Cambria" };
            Panose1Number panose1Number5 = new Panose1Number() { Val = "02040503050406030204" };
            FontCharSet fontCharSet5 = new FontCharSet() { Val = "CC" };
            FontFamily fontFamily5 = new FontFamily() { Val = FontFamilyValues.Roman };
            Pitch pitch5 = new Pitch() { Val = FontPitchValues.Variable };
            FontSignature fontSignature5 = new FontSignature() { UnicodeSignature0 = "A00002EF", UnicodeSignature1 = "4000004B", UnicodeSignature2 = "00000000", UnicodeSignature3 = "00000000", CodePageSignature0 = "0000009F", CodePageSignature1 = "00000000" };

            font5.Append(panose1Number5);
            font5.Append(fontCharSet5);
            font5.Append(fontFamily5);
            font5.Append(pitch5);
            font5.Append(fontSignature5);

            Font font6 = new Font() { Name = "Tahoma" };
            Panose1Number panose1Number6 = new Panose1Number() { Val = "020B0604030504040204" };
            FontCharSet fontCharSet6 = new FontCharSet() { Val = "CC" };
            FontFamily fontFamily6 = new FontFamily() { Val = FontFamilyValues.Swiss };
            Pitch pitch6 = new Pitch() { Val = FontPitchValues.Variable };
            FontSignature fontSignature6 = new FontSignature() { UnicodeSignature0 = "61002A87", UnicodeSignature1 = "80000000", UnicodeSignature2 = "00000008", UnicodeSignature3 = "00000000", CodePageSignature0 = "000101FF", CodePageSignature1 = "00000000" };

            font6.Append(panose1Number6);
            font6.Append(fontCharSet6);
            font6.Append(fontFamily6);
            font6.Append(pitch6);
            font6.Append(fontSignature6);

            Font font7 = new Font() { Name = "Calibri" };
            Panose1Number panose1Number7 = new Panose1Number() { Val = "020F0502020204030204" };
            FontCharSet fontCharSet7 = new FontCharSet() { Val = "CC" };
            FontFamily fontFamily7 = new FontFamily() { Val = FontFamilyValues.Swiss };
            Pitch pitch7 = new Pitch() { Val = FontPitchValues.Variable };
            FontSignature fontSignature7 = new FontSignature() { UnicodeSignature0 = "A00002EF", UnicodeSignature1 = "4000207B", UnicodeSignature2 = "00000000", UnicodeSignature3 = "00000000", CodePageSignature0 = "0000009F", CodePageSignature1 = "00000000" };

            font7.Append(panose1Number7);
            font7.Append(fontCharSet7);
            font7.Append(fontFamily7);
            font7.Append(pitch7);
            font7.Append(fontSignature7);

            fonts1.Append(font1);
            fonts1.Append(font2);
            fonts1.Append(font3);
            fonts1.Append(font4);
            fonts1.Append(font5);
            fonts1.Append(font6);
            fonts1.Append(font7);

            fontTablePart1.Fonts = fonts1;
        }

        // Generates content of styleDefinitionsPart1.
        private void GenerateStyleDefinitionsPart1Content(StyleDefinitionsPart styleDefinitionsPart1)
        {
            Styles styles1 = new Styles();
            styles1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            styles1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            DocDefaults docDefaults1 = new DocDefaults();

            RunPropertiesDefault runPropertiesDefault1 = new RunPropertiesDefault();

            RunPropertiesBaseStyle runPropertiesBaseStyle1 = new RunPropertiesBaseStyle();
            RunFonts runFonts42 = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "Times New Roman", ComplexScript = "Times New Roman" };
            Languages languages22 = new Languages() { Val = "ru-RU", EastAsia = "ru-RU", Bidi = "ar-SA" };

            runPropertiesBaseStyle1.Append(runFonts42);
            runPropertiesBaseStyle1.Append(languages22);

            runPropertiesDefault1.Append(runPropertiesBaseStyle1);
            ParagraphPropertiesDefault paragraphPropertiesDefault1 = new ParagraphPropertiesDefault();

            docDefaults1.Append(runPropertiesDefault1);
            docDefaults1.Append(paragraphPropertiesDefault1);

            LatentStyles latentStyles1 = new LatentStyles() { DefaultLockedState = false, DefaultUiPriority = 99, DefaultSemiHidden = true, DefaultUnhideWhenUsed = true, DefaultPrimaryStyle = false, Count = 267 };
            LatentStyleExceptionInfo latentStyleExceptionInfo1 = new LatentStyleExceptionInfo() { Name = "Normal", UiPriority = 0, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo2 = new LatentStyleExceptionInfo() { Name = "heading 1", UiPriority = 9, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo3 = new LatentStyleExceptionInfo() { Name = "heading 2", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo4 = new LatentStyleExceptionInfo() { Name = "heading 3", UiPriority = 9, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo5 = new LatentStyleExceptionInfo() { Name = "heading 4", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo6 = new LatentStyleExceptionInfo() { Name = "heading 5", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo7 = new LatentStyleExceptionInfo() { Name = "heading 6", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo8 = new LatentStyleExceptionInfo() { Name = "heading 7", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo9 = new LatentStyleExceptionInfo() { Name = "heading 8", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo10 = new LatentStyleExceptionInfo() { Name = "heading 9", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo11 = new LatentStyleExceptionInfo() { Name = "toc 1", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo12 = new LatentStyleExceptionInfo() { Name = "toc 2", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo13 = new LatentStyleExceptionInfo() { Name = "toc 3", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo14 = new LatentStyleExceptionInfo() { Name = "toc 4", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo15 = new LatentStyleExceptionInfo() { Name = "toc 5", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo16 = new LatentStyleExceptionInfo() { Name = "toc 6", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo17 = new LatentStyleExceptionInfo() { Name = "toc 7", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo18 = new LatentStyleExceptionInfo() { Name = "toc 8", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo19 = new LatentStyleExceptionInfo() { Name = "toc 9", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo20 = new LatentStyleExceptionInfo() { Name = "caption", UiPriority = 35, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo21 = new LatentStyleExceptionInfo() { Name = "Title", UiPriority = 10, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo22 = new LatentStyleExceptionInfo() { Name = "Default Paragraph Font", UiPriority = 1 };
            LatentStyleExceptionInfo latentStyleExceptionInfo23 = new LatentStyleExceptionInfo() { Name = "Subtitle", UiPriority = 11, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo24 = new LatentStyleExceptionInfo() { Name = "Strong", UiPriority = 22, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo25 = new LatentStyleExceptionInfo() { Name = "Emphasis", UiPriority = 20, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo26 = new LatentStyleExceptionInfo() { Name = "Table Grid", UiPriority = 59, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo27 = new LatentStyleExceptionInfo() { Name = "Placeholder Text", UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo28 = new LatentStyleExceptionInfo() { Name = "No Spacing", UiPriority = 1, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo29 = new LatentStyleExceptionInfo() { Name = "Light Shading", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo30 = new LatentStyleExceptionInfo() { Name = "Light List", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo31 = new LatentStyleExceptionInfo() { Name = "Light Grid", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo32 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo33 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo34 = new LatentStyleExceptionInfo() { Name = "Medium List 1", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo35 = new LatentStyleExceptionInfo() { Name = "Medium List 2", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo36 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo37 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo38 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo39 = new LatentStyleExceptionInfo() { Name = "Dark List", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo40 = new LatentStyleExceptionInfo() { Name = "Colorful Shading", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo41 = new LatentStyleExceptionInfo() { Name = "Colorful List", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo42 = new LatentStyleExceptionInfo() { Name = "Colorful Grid", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo43 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 1", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo44 = new LatentStyleExceptionInfo() { Name = "Light List Accent 1", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo45 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 1", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo46 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 1", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo47 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 1", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo48 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 1", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo49 = new LatentStyleExceptionInfo() { Name = "Revision", UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo50 = new LatentStyleExceptionInfo() { Name = "List Paragraph", UiPriority = 34, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo51 = new LatentStyleExceptionInfo() { Name = "Quote", UiPriority = 29, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo52 = new LatentStyleExceptionInfo() { Name = "Intense Quote", UiPriority = 30, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo53 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 1", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo54 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 1", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo55 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 1", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo56 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 1", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo57 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 1", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo58 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 1", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo59 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 1", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo60 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 1", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo61 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 2", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo62 = new LatentStyleExceptionInfo() { Name = "Light List Accent 2", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo63 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 2", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo64 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 2", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo65 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 2", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo66 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 2", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo67 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 2", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo68 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 2", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo69 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 2", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo70 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 2", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo71 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 2", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo72 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 2", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo73 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 2", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo74 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 2", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo75 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 3", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo76 = new LatentStyleExceptionInfo() { Name = "Light List Accent 3", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo77 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 3", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo78 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 3", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo79 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 3", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo80 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 3", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo81 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 3", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo82 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 3", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo83 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 3", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo84 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 3", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo85 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 3", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo86 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 3", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo87 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 3", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo88 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 3", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo89 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 4", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo90 = new LatentStyleExceptionInfo() { Name = "Light List Accent 4", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo91 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 4", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo92 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 4", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo93 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 4", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo94 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 4", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo95 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 4", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo96 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 4", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo97 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 4", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo98 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 4", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo99 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 4", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo100 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 4", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo101 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 4", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo102 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 4", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo103 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 5", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo104 = new LatentStyleExceptionInfo() { Name = "Light List Accent 5", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo105 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 5", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo106 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 5", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo107 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 5", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo108 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 5", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo109 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 5", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo110 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 5", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo111 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 5", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo112 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 5", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo113 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 5", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo114 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 5", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo115 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 5", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo116 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 5", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo117 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 6", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo118 = new LatentStyleExceptionInfo() { Name = "Light List Accent 6", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo119 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 6", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo120 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 6", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo121 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 6", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo122 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 6", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo123 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 6", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo124 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 6", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo125 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 6", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo126 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 6", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo127 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 6", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo128 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 6", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo129 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 6", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo130 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 6", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo131 = new LatentStyleExceptionInfo() { Name = "Subtle Emphasis", UiPriority = 19, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo132 = new LatentStyleExceptionInfo() { Name = "Intense Emphasis", UiPriority = 21, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo133 = new LatentStyleExceptionInfo() { Name = "Subtle Reference", UiPriority = 31, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo134 = new LatentStyleExceptionInfo() { Name = "Intense Reference", UiPriority = 32, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo135 = new LatentStyleExceptionInfo() { Name = "Book Title", UiPriority = 33, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo136 = new LatentStyleExceptionInfo() { Name = "Bibliography", UiPriority = 37 };
            LatentStyleExceptionInfo latentStyleExceptionInfo137 = new LatentStyleExceptionInfo() { Name = "TOC Heading", UiPriority = 39, PrimaryStyle = true };

            latentStyles1.Append(latentStyleExceptionInfo1);
            latentStyles1.Append(latentStyleExceptionInfo2);
            latentStyles1.Append(latentStyleExceptionInfo3);
            latentStyles1.Append(latentStyleExceptionInfo4);
            latentStyles1.Append(latentStyleExceptionInfo5);
            latentStyles1.Append(latentStyleExceptionInfo6);
            latentStyles1.Append(latentStyleExceptionInfo7);
            latentStyles1.Append(latentStyleExceptionInfo8);
            latentStyles1.Append(latentStyleExceptionInfo9);
            latentStyles1.Append(latentStyleExceptionInfo10);
            latentStyles1.Append(latentStyleExceptionInfo11);
            latentStyles1.Append(latentStyleExceptionInfo12);
            latentStyles1.Append(latentStyleExceptionInfo13);
            latentStyles1.Append(latentStyleExceptionInfo14);
            latentStyles1.Append(latentStyleExceptionInfo15);
            latentStyles1.Append(latentStyleExceptionInfo16);
            latentStyles1.Append(latentStyleExceptionInfo17);
            latentStyles1.Append(latentStyleExceptionInfo18);
            latentStyles1.Append(latentStyleExceptionInfo19);
            latentStyles1.Append(latentStyleExceptionInfo20);
            latentStyles1.Append(latentStyleExceptionInfo21);
            latentStyles1.Append(latentStyleExceptionInfo22);
            latentStyles1.Append(latentStyleExceptionInfo23);
            latentStyles1.Append(latentStyleExceptionInfo24);
            latentStyles1.Append(latentStyleExceptionInfo25);
            latentStyles1.Append(latentStyleExceptionInfo26);
            latentStyles1.Append(latentStyleExceptionInfo27);
            latentStyles1.Append(latentStyleExceptionInfo28);
            latentStyles1.Append(latentStyleExceptionInfo29);
            latentStyles1.Append(latentStyleExceptionInfo30);
            latentStyles1.Append(latentStyleExceptionInfo31);
            latentStyles1.Append(latentStyleExceptionInfo32);
            latentStyles1.Append(latentStyleExceptionInfo33);
            latentStyles1.Append(latentStyleExceptionInfo34);
            latentStyles1.Append(latentStyleExceptionInfo35);
            latentStyles1.Append(latentStyleExceptionInfo36);
            latentStyles1.Append(latentStyleExceptionInfo37);
            latentStyles1.Append(latentStyleExceptionInfo38);
            latentStyles1.Append(latentStyleExceptionInfo39);
            latentStyles1.Append(latentStyleExceptionInfo40);
            latentStyles1.Append(latentStyleExceptionInfo41);
            latentStyles1.Append(latentStyleExceptionInfo42);
            latentStyles1.Append(latentStyleExceptionInfo43);
            latentStyles1.Append(latentStyleExceptionInfo44);
            latentStyles1.Append(latentStyleExceptionInfo45);
            latentStyles1.Append(latentStyleExceptionInfo46);
            latentStyles1.Append(latentStyleExceptionInfo47);
            latentStyles1.Append(latentStyleExceptionInfo48);
            latentStyles1.Append(latentStyleExceptionInfo49);
            latentStyles1.Append(latentStyleExceptionInfo50);
            latentStyles1.Append(latentStyleExceptionInfo51);
            latentStyles1.Append(latentStyleExceptionInfo52);
            latentStyles1.Append(latentStyleExceptionInfo53);
            latentStyles1.Append(latentStyleExceptionInfo54);
            latentStyles1.Append(latentStyleExceptionInfo55);
            latentStyles1.Append(latentStyleExceptionInfo56);
            latentStyles1.Append(latentStyleExceptionInfo57);
            latentStyles1.Append(latentStyleExceptionInfo58);
            latentStyles1.Append(latentStyleExceptionInfo59);
            latentStyles1.Append(latentStyleExceptionInfo60);
            latentStyles1.Append(latentStyleExceptionInfo61);
            latentStyles1.Append(latentStyleExceptionInfo62);
            latentStyles1.Append(latentStyleExceptionInfo63);
            latentStyles1.Append(latentStyleExceptionInfo64);
            latentStyles1.Append(latentStyleExceptionInfo65);
            latentStyles1.Append(latentStyleExceptionInfo66);
            latentStyles1.Append(latentStyleExceptionInfo67);
            latentStyles1.Append(latentStyleExceptionInfo68);
            latentStyles1.Append(latentStyleExceptionInfo69);
            latentStyles1.Append(latentStyleExceptionInfo70);
            latentStyles1.Append(latentStyleExceptionInfo71);
            latentStyles1.Append(latentStyleExceptionInfo72);
            latentStyles1.Append(latentStyleExceptionInfo73);
            latentStyles1.Append(latentStyleExceptionInfo74);
            latentStyles1.Append(latentStyleExceptionInfo75);
            latentStyles1.Append(latentStyleExceptionInfo76);
            latentStyles1.Append(latentStyleExceptionInfo77);
            latentStyles1.Append(latentStyleExceptionInfo78);
            latentStyles1.Append(latentStyleExceptionInfo79);
            latentStyles1.Append(latentStyleExceptionInfo80);
            latentStyles1.Append(latentStyleExceptionInfo81);
            latentStyles1.Append(latentStyleExceptionInfo82);
            latentStyles1.Append(latentStyleExceptionInfo83);
            latentStyles1.Append(latentStyleExceptionInfo84);
            latentStyles1.Append(latentStyleExceptionInfo85);
            latentStyles1.Append(latentStyleExceptionInfo86);
            latentStyles1.Append(latentStyleExceptionInfo87);
            latentStyles1.Append(latentStyleExceptionInfo88);
            latentStyles1.Append(latentStyleExceptionInfo89);
            latentStyles1.Append(latentStyleExceptionInfo90);
            latentStyles1.Append(latentStyleExceptionInfo91);
            latentStyles1.Append(latentStyleExceptionInfo92);
            latentStyles1.Append(latentStyleExceptionInfo93);
            latentStyles1.Append(latentStyleExceptionInfo94);
            latentStyles1.Append(latentStyleExceptionInfo95);
            latentStyles1.Append(latentStyleExceptionInfo96);
            latentStyles1.Append(latentStyleExceptionInfo97);
            latentStyles1.Append(latentStyleExceptionInfo98);
            latentStyles1.Append(latentStyleExceptionInfo99);
            latentStyles1.Append(latentStyleExceptionInfo100);
            latentStyles1.Append(latentStyleExceptionInfo101);
            latentStyles1.Append(latentStyleExceptionInfo102);
            latentStyles1.Append(latentStyleExceptionInfo103);
            latentStyles1.Append(latentStyleExceptionInfo104);
            latentStyles1.Append(latentStyleExceptionInfo105);
            latentStyles1.Append(latentStyleExceptionInfo106);
            latentStyles1.Append(latentStyleExceptionInfo107);
            latentStyles1.Append(latentStyleExceptionInfo108);
            latentStyles1.Append(latentStyleExceptionInfo109);
            latentStyles1.Append(latentStyleExceptionInfo110);
            latentStyles1.Append(latentStyleExceptionInfo111);
            latentStyles1.Append(latentStyleExceptionInfo112);
            latentStyles1.Append(latentStyleExceptionInfo113);
            latentStyles1.Append(latentStyleExceptionInfo114);
            latentStyles1.Append(latentStyleExceptionInfo115);
            latentStyles1.Append(latentStyleExceptionInfo116);
            latentStyles1.Append(latentStyleExceptionInfo117);
            latentStyles1.Append(latentStyleExceptionInfo118);
            latentStyles1.Append(latentStyleExceptionInfo119);
            latentStyles1.Append(latentStyleExceptionInfo120);
            latentStyles1.Append(latentStyleExceptionInfo121);
            latentStyles1.Append(latentStyleExceptionInfo122);
            latentStyles1.Append(latentStyleExceptionInfo123);
            latentStyles1.Append(latentStyleExceptionInfo124);
            latentStyles1.Append(latentStyleExceptionInfo125);
            latentStyles1.Append(latentStyleExceptionInfo126);
            latentStyles1.Append(latentStyleExceptionInfo127);
            latentStyles1.Append(latentStyleExceptionInfo128);
            latentStyles1.Append(latentStyleExceptionInfo129);
            latentStyles1.Append(latentStyleExceptionInfo130);
            latentStyles1.Append(latentStyleExceptionInfo131);
            latentStyles1.Append(latentStyleExceptionInfo132);
            latentStyles1.Append(latentStyleExceptionInfo133);
            latentStyles1.Append(latentStyleExceptionInfo134);
            latentStyles1.Append(latentStyleExceptionInfo135);
            latentStyles1.Append(latentStyleExceptionInfo136);
            latentStyles1.Append(latentStyleExceptionInfo137);

            Style style1 = new Style() { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true };
            StyleName styleName1 = new StyleName() { Val = "Normal" };
            PrimaryStyle primaryStyle1 = new PrimaryStyle();

            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
            RunFonts runFonts43 = new RunFonts() { EastAsiaTheme = ThemeFontValues.MinorEastAsia };
            FontSize fontSize1 = new FontSize() { Val = "24" };
            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "24" };

            styleRunProperties1.Append(runFonts43);
            styleRunProperties1.Append(fontSize1);
            styleRunProperties1.Append(fontSizeComplexScript1);

            style1.Append(styleName1);
            style1.Append(primaryStyle1);
            style1.Append(styleRunProperties1);

            Style style2 = new Style() { Type = StyleValues.Paragraph, StyleId = "Heading3" };
            StyleName styleName2 = new StyleName() { Val = "heading 3" };
            BasedOn basedOn1 = new BasedOn() { Val = "Normal" };
            LinkedStyle linkedStyle1 = new LinkedStyle() { Val = "Heading3Char" };
            UIPriority uIPriority1 = new UIPriority() { Val = 9 };
            PrimaryStyle primaryStyle2 = new PrimaryStyle();

            StyleParagraphProperties styleParagraphProperties1 = new StyleParagraphProperties();
            SpacingBetweenLines spacingBetweenLines7 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };
            OutlineLevel outlineLevel1 = new OutlineLevel() { Val = 2 };

            styleParagraphProperties1.Append(spacingBetweenLines7);
            styleParagraphProperties1.Append(outlineLevel1);

            StyleRunProperties styleRunProperties2 = new StyleRunProperties();
            Bold bold1 = new Bold();
            BoldComplexScript boldComplexScript1 = new BoldComplexScript();
            FontSize fontSize2 = new FontSize() { Val = "27" };
            FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "27" };

            styleRunProperties2.Append(bold1);
            styleRunProperties2.Append(boldComplexScript1);
            styleRunProperties2.Append(fontSize2);
            styleRunProperties2.Append(fontSizeComplexScript2);

            style2.Append(styleName2);
            style2.Append(basedOn1);
            style2.Append(linkedStyle1);
            style2.Append(uIPriority1);
            style2.Append(primaryStyle2);
            style2.Append(styleParagraphProperties1);
            style2.Append(styleRunProperties2);

            Style style3 = new Style() { Type = StyleValues.Character, StyleId = "DefaultParagraphFont", Default = true };
            StyleName styleName3 = new StyleName() { Val = "Default Paragraph Font" };
            UIPriority uIPriority2 = new UIPriority() { Val = 1 };
            SemiHidden semiHidden1 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed1 = new UnhideWhenUsed();

            style3.Append(styleName3);
            style3.Append(uIPriority2);
            style3.Append(semiHidden1);
            style3.Append(unhideWhenUsed1);

            Style style4 = new Style() { Type = StyleValues.Table, StyleId = "TableNormal", Default = true };
            StyleName styleName4 = new StyleName() { Val = "Normal Table" };
            UIPriority uIPriority3 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden2 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed2 = new UnhideWhenUsed();
            PrimaryStyle primaryStyle3 = new PrimaryStyle();

            StyleTableProperties styleTableProperties1 = new StyleTableProperties();
            TableIndentation tableIndentation1 = new TableIndentation() { Width = 0, Type = TableWidthUnitValues.Dxa };

            TableCellMarginDefault tableCellMarginDefault2 = new TableCellMarginDefault();
            TopMargin topMargin2 = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            TableCellLeftMargin tableCellLeftMargin2 = new TableCellLeftMargin() { Width = 108, Type = TableWidthValues.Dxa };
            BottomMargin bottomMargin2 = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            TableCellRightMargin tableCellRightMargin2 = new TableCellRightMargin() { Width = 108, Type = TableWidthValues.Dxa };

            tableCellMarginDefault2.Append(topMargin2);
            tableCellMarginDefault2.Append(tableCellLeftMargin2);
            tableCellMarginDefault2.Append(bottomMargin2);
            tableCellMarginDefault2.Append(tableCellRightMargin2);

            styleTableProperties1.Append(tableIndentation1);
            styleTableProperties1.Append(tableCellMarginDefault2);

            style4.Append(styleName4);
            style4.Append(uIPriority3);
            style4.Append(semiHidden2);
            style4.Append(unhideWhenUsed2);
            style4.Append(primaryStyle3);
            style4.Append(styleTableProperties1);

            Style style5 = new Style() { Type = StyleValues.Numbering, StyleId = "NoList", Default = true };
            StyleName styleName5 = new StyleName() { Val = "No List" };
            UIPriority uIPriority4 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden3 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed3 = new UnhideWhenUsed();

            style5.Append(styleName5);
            style5.Append(uIPriority4);
            style5.Append(semiHidden3);
            style5.Append(unhideWhenUsed3);

            Style style6 = new Style() { Type = StyleValues.Paragraph, StyleId = "NormalWeb" };
            StyleName styleName6 = new StyleName() { Val = "Normal (Web)" };
            BasedOn basedOn2 = new BasedOn() { Val = "Normal" };
            UIPriority uIPriority5 = new UIPriority() { Val = 99 };
            UnhideWhenUsed unhideWhenUsed4 = new UnhideWhenUsed();

            StyleParagraphProperties styleParagraphProperties2 = new StyleParagraphProperties();
            SpacingBetweenLines spacingBetweenLines8 = new SpacingBetweenLines() { Before = "100", BeforeAutoSpacing = true, After = "100", AfterAutoSpacing = true };

            styleParagraphProperties2.Append(spacingBetweenLines8);

            style6.Append(styleName6);
            style6.Append(basedOn2);
            style6.Append(uIPriority5);
            style6.Append(unhideWhenUsed4);
            style6.Append(styleParagraphProperties2);

            Style style7 = new Style() { Type = StyleValues.Character, StyleId = "Heading3Char", CustomStyle = true };
            StyleName styleName7 = new StyleName() { Val = "Heading 3 Char" };
            BasedOn basedOn3 = new BasedOn() { Val = "DefaultParagraphFont" };
            LinkedStyle linkedStyle2 = new LinkedStyle() { Val = "Heading3" };
            UIPriority uIPriority6 = new UIPriority() { Val = 9 };

            StyleRunProperties styleRunProperties3 = new StyleRunProperties();
            RunFonts runFonts44 = new RunFonts() { AsciiTheme = ThemeFontValues.MajorHighAnsi, HighAnsiTheme = ThemeFontValues.MajorHighAnsi, EastAsiaTheme = ThemeFontValues.MajorEastAsia, ComplexScriptTheme = ThemeFontValues.MajorBidi };
            Bold bold2 = new Bold();
            BoldComplexScript boldComplexScript2 = new BoldComplexScript();
            Color color1 = new Color() { Val = "4F81BD", ThemeColor = ThemeColorValues.Accent1 };
            FontSize fontSize3 = new FontSize() { Val = "24" };
            FontSizeComplexScript fontSizeComplexScript3 = new FontSizeComplexScript() { Val = "24" };

            styleRunProperties3.Append(runFonts44);
            styleRunProperties3.Append(bold2);
            styleRunProperties3.Append(boldComplexScript2);
            styleRunProperties3.Append(color1);
            styleRunProperties3.Append(fontSize3);
            styleRunProperties3.Append(fontSizeComplexScript3);

            style7.Append(styleName7);
            style7.Append(basedOn3);
            style7.Append(linkedStyle2);
            style7.Append(uIPriority6);
            style7.Append(styleRunProperties3);

            Style style8 = new Style() { Type = StyleValues.Paragraph, StyleId = "BalloonText" };
            StyleName styleName8 = new StyleName() { Val = "Balloon Text" };
            BasedOn basedOn4 = new BasedOn() { Val = "Normal" };
            LinkedStyle linkedStyle3 = new LinkedStyle() { Val = "BalloonTextChar" };
            UIPriority uIPriority7 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden4 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed5 = new UnhideWhenUsed();
            Rsid rsid3 = new Rsid() { Val = "00F42003" };

            StyleRunProperties styleRunProperties4 = new StyleRunProperties();
            RunFonts runFonts45 = new RunFonts() { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma" };
            FontSize fontSize4 = new FontSize() { Val = "16" };
            FontSizeComplexScript fontSizeComplexScript4 = new FontSizeComplexScript() { Val = "16" };

            styleRunProperties4.Append(runFonts45);
            styleRunProperties4.Append(fontSize4);
            styleRunProperties4.Append(fontSizeComplexScript4);

            style8.Append(styleName8);
            style8.Append(basedOn4);
            style8.Append(linkedStyle3);
            style8.Append(uIPriority7);
            style8.Append(semiHidden4);
            style8.Append(unhideWhenUsed5);
            style8.Append(rsid3);
            style8.Append(styleRunProperties4);

            Style style9 = new Style() { Type = StyleValues.Character, StyleId = "BalloonTextChar", CustomStyle = true };
            StyleName styleName9 = new StyleName() { Val = "Balloon Text Char" };
            BasedOn basedOn5 = new BasedOn() { Val = "DefaultParagraphFont" };
            LinkedStyle linkedStyle4 = new LinkedStyle() { Val = "BalloonText" };
            UIPriority uIPriority8 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden5 = new SemiHidden();
            Rsid rsid4 = new Rsid() { Val = "00F42003" };

            StyleRunProperties styleRunProperties5 = new StyleRunProperties();
            RunFonts runFonts46 = new RunFonts() { Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma", EastAsiaTheme = ThemeFontValues.MinorEastAsia };
            FontSize fontSize5 = new FontSize() { Val = "16" };
            FontSizeComplexScript fontSizeComplexScript5 = new FontSizeComplexScript() { Val = "16" };

            styleRunProperties5.Append(runFonts46);
            styleRunProperties5.Append(fontSize5);
            styleRunProperties5.Append(fontSizeComplexScript5);

            style9.Append(styleName9);
            style9.Append(basedOn5);
            style9.Append(linkedStyle4);
            style9.Append(uIPriority8);
            style9.Append(semiHidden5);
            style9.Append(rsid4);
            style9.Append(styleRunProperties5);

            styles1.Append(docDefaults1);
            styles1.Append(latentStyles1);
            styles1.Append(style1);
            styles1.Append(style2);
            styles1.Append(style3);
            styles1.Append(style4);
            styles1.Append(style5);
            styles1.Append(style6);
            styles1.Append(style7);
            styles1.Append(style8);
            styles1.Append(style9);

            styleDefinitionsPart1.Styles = styles1;
        }

        // Generates content of numberingDefinitionsPart1.
        private void GenerateNumberingDefinitionsPart1Content(NumberingDefinitionsPart numberingDefinitionsPart1)
        {
            Numbering numbering1 = new Numbering();
            numbering1.AddNamespaceDeclaration("ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            numbering1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            numbering1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            numbering1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            numbering1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            numbering1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            numbering1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            numbering1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            numbering1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");

            AbstractNum abstractNum1 = new AbstractNum() { AbstractNumberId = 0 };
            Nsid nsid1 = new Nsid() { Val = "39E00DB9" };
            MultiLevelType multiLevelType1 = new MultiLevelType() { Val = MultiLevelValues.Multilevel };
            TemplateCode templateCode1 = new TemplateCode() { Val = "124093C6" };

            Level level1 = new Level() { LevelIndex = 0 };
            StartNumberingValue startNumberingValue1 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat1 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText1 = new LevelText() { Val = "·" };
            LevelJustification levelJustification1 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties1 = new PreviousParagraphProperties();

            Tabs tabs1 = new Tabs();
            TabStop tabStop1 = new TabStop() { Val = TabStopValues.Number, Position = 720 };

            tabs1.Append(tabStop1);
            Indentation indentation1 = new Indentation() { Left = "720", Hanging = "360" };

            previousParagraphProperties1.Append(tabs1);
            previousParagraphProperties1.Append(indentation1);

            NumberingSymbolRunProperties numberingSymbolRunProperties1 = new NumberingSymbolRunProperties();
            RunFonts runFonts47 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Symbol", HighAnsi = "Symbol" };
            FontSize fontSize6 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties1.Append(runFonts47);
            numberingSymbolRunProperties1.Append(fontSize6);

            level1.Append(startNumberingValue1);
            level1.Append(numberingFormat1);
            level1.Append(levelText1);
            level1.Append(levelJustification1);
            level1.Append(previousParagraphProperties1);
            level1.Append(numberingSymbolRunProperties1);

            Level level2 = new Level() { LevelIndex = 1, Tentative = true };
            StartNumberingValue startNumberingValue2 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat2 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText2 = new LevelText() { Val = "o" };
            LevelJustification levelJustification2 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties2 = new PreviousParagraphProperties();

            Tabs tabs2 = new Tabs();
            TabStop tabStop2 = new TabStop() { Val = TabStopValues.Number, Position = 1440 };

            tabs2.Append(tabStop2);
            Indentation indentation2 = new Indentation() { Left = "1440", Hanging = "360" };

            previousParagraphProperties2.Append(tabs2);
            previousParagraphProperties2.Append(indentation2);

            NumberingSymbolRunProperties numberingSymbolRunProperties2 = new NumberingSymbolRunProperties();
            RunFonts runFonts48 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Courier New", HighAnsi = "Courier New" };
            FontSize fontSize7 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties2.Append(runFonts48);
            numberingSymbolRunProperties2.Append(fontSize7);

            level2.Append(startNumberingValue2);
            level2.Append(numberingFormat2);
            level2.Append(levelText2);
            level2.Append(levelJustification2);
            level2.Append(previousParagraphProperties2);
            level2.Append(numberingSymbolRunProperties2);

            Level level3 = new Level() { LevelIndex = 2, Tentative = true };
            StartNumberingValue startNumberingValue3 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat3 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText3 = new LevelText() { Val = "§" };
            LevelJustification levelJustification3 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties3 = new PreviousParagraphProperties();

            Tabs tabs3 = new Tabs();
            TabStop tabStop3 = new TabStop() { Val = TabStopValues.Number, Position = 2160 };

            tabs3.Append(tabStop3);
            Indentation indentation3 = new Indentation() { Left = "2160", Hanging = "360" };

            previousParagraphProperties3.Append(tabs3);
            previousParagraphProperties3.Append(indentation3);

            NumberingSymbolRunProperties numberingSymbolRunProperties3 = new NumberingSymbolRunProperties();
            RunFonts runFonts49 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Wingdings", HighAnsi = "Wingdings" };
            FontSize fontSize8 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties3.Append(runFonts49);
            numberingSymbolRunProperties3.Append(fontSize8);

            level3.Append(startNumberingValue3);
            level3.Append(numberingFormat3);
            level3.Append(levelText3);
            level3.Append(levelJustification3);
            level3.Append(previousParagraphProperties3);
            level3.Append(numberingSymbolRunProperties3);

            Level level4 = new Level() { LevelIndex = 3, Tentative = true };
            StartNumberingValue startNumberingValue4 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat4 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText4 = new LevelText() { Val = "§" };
            LevelJustification levelJustification4 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties4 = new PreviousParagraphProperties();

            Tabs tabs4 = new Tabs();
            TabStop tabStop4 = new TabStop() { Val = TabStopValues.Number, Position = 2880 };

            tabs4.Append(tabStop4);
            Indentation indentation4 = new Indentation() { Left = "2880", Hanging = "360" };

            previousParagraphProperties4.Append(tabs4);
            previousParagraphProperties4.Append(indentation4);

            NumberingSymbolRunProperties numberingSymbolRunProperties4 = new NumberingSymbolRunProperties();
            RunFonts runFonts50 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Wingdings", HighAnsi = "Wingdings" };
            FontSize fontSize9 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties4.Append(runFonts50);
            numberingSymbolRunProperties4.Append(fontSize9);

            level4.Append(startNumberingValue4);
            level4.Append(numberingFormat4);
            level4.Append(levelText4);
            level4.Append(levelJustification4);
            level4.Append(previousParagraphProperties4);
            level4.Append(numberingSymbolRunProperties4);

            Level level5 = new Level() { LevelIndex = 4, Tentative = true };
            StartNumberingValue startNumberingValue5 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat5 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText5 = new LevelText() { Val = "§" };
            LevelJustification levelJustification5 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties5 = new PreviousParagraphProperties();

            Tabs tabs5 = new Tabs();
            TabStop tabStop5 = new TabStop() { Val = TabStopValues.Number, Position = 3600 };

            tabs5.Append(tabStop5);
            Indentation indentation5 = new Indentation() { Left = "3600", Hanging = "360" };

            previousParagraphProperties5.Append(tabs5);
            previousParagraphProperties5.Append(indentation5);

            NumberingSymbolRunProperties numberingSymbolRunProperties5 = new NumberingSymbolRunProperties();
            RunFonts runFonts51 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Wingdings", HighAnsi = "Wingdings" };
            FontSize fontSize10 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties5.Append(runFonts51);
            numberingSymbolRunProperties5.Append(fontSize10);

            level5.Append(startNumberingValue5);
            level5.Append(numberingFormat5);
            level5.Append(levelText5);
            level5.Append(levelJustification5);
            level5.Append(previousParagraphProperties5);
            level5.Append(numberingSymbolRunProperties5);

            Level level6 = new Level() { LevelIndex = 5, Tentative = true };
            StartNumberingValue startNumberingValue6 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat6 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText6 = new LevelText() { Val = "§" };
            LevelJustification levelJustification6 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties6 = new PreviousParagraphProperties();

            Tabs tabs6 = new Tabs();
            TabStop tabStop6 = new TabStop() { Val = TabStopValues.Number, Position = 4320 };

            tabs6.Append(tabStop6);
            Indentation indentation6 = new Indentation() { Left = "4320", Hanging = "360" };

            previousParagraphProperties6.Append(tabs6);
            previousParagraphProperties6.Append(indentation6);

            NumberingSymbolRunProperties numberingSymbolRunProperties6 = new NumberingSymbolRunProperties();
            RunFonts runFonts52 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Wingdings", HighAnsi = "Wingdings" };
            FontSize fontSize11 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties6.Append(runFonts52);
            numberingSymbolRunProperties6.Append(fontSize11);

            level6.Append(startNumberingValue6);
            level6.Append(numberingFormat6);
            level6.Append(levelText6);
            level6.Append(levelJustification6);
            level6.Append(previousParagraphProperties6);
            level6.Append(numberingSymbolRunProperties6);

            Level level7 = new Level() { LevelIndex = 6, Tentative = true };
            StartNumberingValue startNumberingValue7 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat7 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText7 = new LevelText() { Val = "§" };
            LevelJustification levelJustification7 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties7 = new PreviousParagraphProperties();

            Tabs tabs7 = new Tabs();
            TabStop tabStop7 = new TabStop() { Val = TabStopValues.Number, Position = 5040 };

            tabs7.Append(tabStop7);
            Indentation indentation7 = new Indentation() { Left = "5040", Hanging = "360" };

            previousParagraphProperties7.Append(tabs7);
            previousParagraphProperties7.Append(indentation7);

            NumberingSymbolRunProperties numberingSymbolRunProperties7 = new NumberingSymbolRunProperties();
            RunFonts runFonts53 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Wingdings", HighAnsi = "Wingdings" };
            FontSize fontSize12 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties7.Append(runFonts53);
            numberingSymbolRunProperties7.Append(fontSize12);

            level7.Append(startNumberingValue7);
            level7.Append(numberingFormat7);
            level7.Append(levelText7);
            level7.Append(levelJustification7);
            level7.Append(previousParagraphProperties7);
            level7.Append(numberingSymbolRunProperties7);

            Level level8 = new Level() { LevelIndex = 7, Tentative = true };
            StartNumberingValue startNumberingValue8 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat8 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText8 = new LevelText() { Val = "§" };
            LevelJustification levelJustification8 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties8 = new PreviousParagraphProperties();

            Tabs tabs8 = new Tabs();
            TabStop tabStop8 = new TabStop() { Val = TabStopValues.Number, Position = 5760 };

            tabs8.Append(tabStop8);
            Indentation indentation8 = new Indentation() { Left = "5760", Hanging = "360" };

            previousParagraphProperties8.Append(tabs8);
            previousParagraphProperties8.Append(indentation8);

            NumberingSymbolRunProperties numberingSymbolRunProperties8 = new NumberingSymbolRunProperties();
            RunFonts runFonts54 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Wingdings", HighAnsi = "Wingdings" };
            FontSize fontSize13 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties8.Append(runFonts54);
            numberingSymbolRunProperties8.Append(fontSize13);

            level8.Append(startNumberingValue8);
            level8.Append(numberingFormat8);
            level8.Append(levelText8);
            level8.Append(levelJustification8);
            level8.Append(previousParagraphProperties8);
            level8.Append(numberingSymbolRunProperties8);

            Level level9 = new Level() { LevelIndex = 8, Tentative = true };
            StartNumberingValue startNumberingValue9 = new StartNumberingValue() { Val = 1 };
            NumberingFormat numberingFormat9 = new NumberingFormat() { Val = NumberFormatValues.Bullet };
            LevelText levelText9 = new LevelText() { Val = "§" };
            LevelJustification levelJustification9 = new LevelJustification() { Val = LevelJustificationValues.Left };

            PreviousParagraphProperties previousParagraphProperties9 = new PreviousParagraphProperties();

            Tabs tabs9 = new Tabs();
            TabStop tabStop9 = new TabStop() { Val = TabStopValues.Number, Position = 6480 };

            tabs9.Append(tabStop9);
            Indentation indentation9 = new Indentation() { Left = "6480", Hanging = "360" };

            previousParagraphProperties9.Append(tabs9);
            previousParagraphProperties9.Append(indentation9);

            NumberingSymbolRunProperties numberingSymbolRunProperties9 = new NumberingSymbolRunProperties();
            RunFonts runFonts55 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Wingdings", HighAnsi = "Wingdings" };
            FontSize fontSize14 = new FontSize() { Val = "20" };

            numberingSymbolRunProperties9.Append(runFonts55);
            numberingSymbolRunProperties9.Append(fontSize14);

            level9.Append(startNumberingValue9);
            level9.Append(numberingFormat9);
            level9.Append(levelText9);
            level9.Append(levelJustification9);
            level9.Append(previousParagraphProperties9);
            level9.Append(numberingSymbolRunProperties9);

            abstractNum1.Append(nsid1);
            abstractNum1.Append(multiLevelType1);
            abstractNum1.Append(templateCode1);
            abstractNum1.Append(level1);
            abstractNum1.Append(level2);
            abstractNum1.Append(level3);
            abstractNum1.Append(level4);
            abstractNum1.Append(level5);
            abstractNum1.Append(level6);
            abstractNum1.Append(level7);
            abstractNum1.Append(level8);
            abstractNum1.Append(level9);

            NumberingInstance numberingInstance1 = new NumberingInstance() { NumberID = 1 };
            AbstractNumId abstractNumId1 = new AbstractNumId() { Val = 0 };

            numberingInstance1.Append(abstractNumId1);

            numbering1.Append(abstractNum1);
            numbering1.Append(numberingInstance1);

            numberingDefinitionsPart1.Numbering = numbering1;
        }

        // Generates content of imagePart1.
        private void GenerateImagePart1Content(ImagePart imagePart1)
        {
            using (System.IO.Stream data = new System.IO.MemoryStream(ImageData1))
            {
                imagePart1.FeedData(data);
            }
        }

        // Generates content of imagePart2.
        private void GenerateImagePart2Content(ImagePart imagePart2)
        {
            using (System.IO.MemoryStream data = new System.IO.MemoryStream(ImageData2))
            {
                imagePart2.FeedData(data);
            }
        }

        // Generates content of webSettingsPart1.
        private void GenerateWebSettingsPart1Content(WebSettingsPart webSettingsPart1)
        {
            WebSettings webSettings1 = new WebSettings();
            webSettings1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            webSettings1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            WebPageEncoding webPageEncoding1 = new WebPageEncoding() { Val = "unicode" };

            webSettings1.Append(webPageEncoding1);

            webSettingsPart1.WebSettings = webSettings1;
        }

        private void SetPackageProperties(OpenXmlPackage document)
        {
            document.PackageProperties.Creator = "";
            document.PackageProperties.Title = "";
            document.PackageProperties.Subject = "";
            document.PackageProperties.Keywords = "";
            document.PackageProperties.Description = "";
            document.PackageProperties.Revision = "1";
            document.PackageProperties.LastModifiedBy = "";
            document.PackageProperties.Created = System.DateTime.Now;
            document.PackageProperties.Modified = System.DateTime.Now;
        }

        private UspsGatewayResponse GetTrackingInfo(string trackingNumber)
        {
            try
            {
                UspsGateway usps = new UspsGateway();

                string response = string.Empty;

                usps.GetTrackInfo(trackingNumber, false, out response);

                XElement xml = XElement.Parse(response);

                UspsGatewayResponse res = new UspsGatewayResponse();

                XElement ele = xml.Element("TrackInfo");

                res.EventTime = xml.Element("TrackInfo").Element("TrackSummary").Element("EventTime").Value;
                res.EventDate = xml.Element("TrackInfo").Element("TrackSummary").Element("EventDate").Value;
                res.Event = xml.Element("TrackInfo").Element("TrackSummary").Element("Event").Value;
                res.EventCity = xml.Element("TrackInfo").Element("TrackSummary").Element("EventCity").Value;
                res.EventState = xml.Element("TrackInfo").Element("TrackSummary").Element("EventState").Value;
                res.EventZIPCode = xml.Element("TrackInfo").Element("TrackSummary").Element("EventZIPCode").Value;

                return res;
            }
            catch
            {
                return null;
            }
        }

    }

    public class UspsGatewayResponse
    {
        public string TrackingNumber { get; set; }
        public string EventTime { get; set; }
        public string EventDate { get; set; }
        public string Event { get; set; }
        public string EventCity { get; set; }
        public string EventState { get; set; }
        public string EventZIPCode { get; set; }

    }

    
}
