using System;
using System.IO;
using System.Reflection;
using System.Threading;

using Microsoft.Office.Interop.Word;

namespace TrimFuel.Tools.PackingManager.Logic
{
    public class PackageSlip
    {
        private void Replace(string template, string value, ref Document document)
        {
            object findText = Missing.Value;
            foreach (Range range in document.StoryRanges)
            {
                range.Find.Text = template;
                range.Find.Replacement.Text = value;
                range.Find.Wrap = WdFindWrap.wdFindContinue;
                object wdReplaceAll = WdReplace.wdReplaceAll;
                range.Find.Execute(ref findText, ref findText, ref findText, ref findText, ref findText, ref findText, ref findText, ref findText, ref findText, ref findText, ref wdReplaceAll, ref findText, ref findText, ref findText, ref findText);
            }
        }

        public void Save(string path, string saleID, string fullName, string address1, string address2, string city, string state, string zip, string country, string date, int[] quantity, string[] sku, string[] productDescription, string shipMethod, string target, string client)
        {
            Application application = new Application() { Visible = false };
            
            Document document = new DocumentClass();
            
            object newTemplate = Missing.Value;
            object template = path;
            
            try
            {
                document = application.Documents.Add(ref template, ref newTemplate, ref newTemplate, ref newTemplate);
                this.Replace("[OrderID]", saleID, ref document);
                this.Replace("[FullName]", fullName, ref document);
                this.Replace("[Address1]", address1, ref document);
                this.Replace("[Address2]", address2, ref document);
                this.Replace("[City]", city, ref document);
                this.Replace("[State]", state, ref document);
                this.Replace("[Zip]", zip, ref document);
                this.Replace("[Country]", country, ref document);
                this.Replace("[Date]", date, ref document);
                this.Replace("[XXXXXX]", saleID, ref document);
                if (client != "Coaction")
                {
                    for (int i = 0; i < sku.Length; i++)
                    {
                        this.AddRow(quantity[i], sku[i], productDescription[i], ref document);
                    }
                }
                else
                {
                    int totalQuantity = 0;

                    for (int i = 0; i < sku.Length; i++)
                    {
                        this.AddRowCoaction(date, shipMethod, saleID, quantity[i], sku[i], productDescription[i], ref document, i);
                        
                        totalQuantity += quantity[i];
                    }
                    this.Replace("[TotQuantity]", totalQuantity.ToString(), ref document);
                }
                object fileName = target;
                object fileFormat = Missing.Value;
                object lockComments = Missing.Value;
                object password = Missing.Value;
                object addToRecentFiles = Missing.Value;
                object writePassword = Missing.Value;
                object readOnlyRecommended = Missing.Value;
                object embedTrueTypeFonts = Missing.Value;
                object saveNativePictureFormat = Missing.Value;
                object saveFormsData = Missing.Value;
                object saveAsAOCELetter = Missing.Value;
                object encoding = Missing.Value;
                object insertLineBreaks = Missing.Value;
                object allowSubstitutions = Missing.Value;
                object lineEnding = Missing.Value;
                object addBiDiMarks = Missing.Value;
                document.SaveAs(ref fileName, ref fileFormat, ref lockComments, ref password, ref addToRecentFiles, ref writePassword, ref readOnlyRecommended, ref embedTrueTypeFonts, ref saveNativePictureFormat, ref saveFormsData, ref saveAsAOCELetter, ref encoding, ref insertLineBreaks, ref allowSubstitutions, ref lineEnding, ref addBiDiMarks);
            }
            catch
            {
                throw;
            }
            finally
            {
                document.Close(ref newTemplate, ref newTemplate, ref newTemplate);
                application.Quit(ref newTemplate, ref newTemplate, ref newTemplate);
                document = null;
                application = null;
            }
        }

        public void Print(string path, string printer, string saleID, string fullName, string address1, string address2, string city, string state, string zip, string country, string date, int[] quantity, string[] sku, string[] productDescription, string shipMethod, string target, string client)
        {
            Application application = new Application() { Visible = false };
           
            Document document = new DocumentClass();
            
            object newTemplate = Missing.Value;
            object template = path;
            object append = false;
            object background = true;
            object wdPrintAllDocument = WdPrintOutRange.wdPrintAllDocument;
            object wdPrintDocumentContent = WdPrintOutItem.wdPrintDocumentContent;
            object wdPrintAllPages = WdPrintOutPages.wdPrintAllPages;
            object copies = "1";
            object pages = "";
            
            try
            {
                document = application.Documents.Add(ref template, ref newTemplate, ref newTemplate, ref newTemplate);

                Replace("[OrderID]", saleID, ref document);
                Replace("[FullName]", fullName, ref document);
                Replace("[Address1]", address1, ref document);
                Replace("[Address2]", address2, ref document);
                Replace("[City]", city, ref document);
                Replace("[State]", state, ref document);
                Replace("[Zip]", zip, ref document);
                Replace("[Country]", country, ref document);
                Replace("[Date]", date, ref document);
                Replace("[XXXXXX]", saleID, ref document);

                if (client != "Coaction")
                {
                    for (int i = 0; i < sku.Length; i++)
                    {
                        AddRow(quantity[i], sku[i], productDescription[i], ref document);
                    }
                }
                else
                {
                    int totalQuantity = 0;

                    for (int i = 0; i < sku.Length; i++)
                    {
                        AddRowCoaction(date, shipMethod, saleID, quantity[i], sku[i], productDescription[i], ref document, i);
                        
                        totalQuantity += quantity[i];
                    }
                    
                    Replace("[TotQuantity]", totalQuantity.ToString(), ref document);
                }
                
                application.ActivePrinter = printer;
                
                object fileName = target;
                object fileFormat = Missing.Value;
                object lockComments = Missing.Value;
                object password = Missing.Value;
                object addToRecentFiles = Missing.Value;
                object writePassword = Missing.Value;
                object readOnlyRecommended = Missing.Value;
                object embedTrueTypeFonts = Missing.Value;
                object saveNativePictureFormat = Missing.Value;
                object saveFormsData = Missing.Value;
                object saveAsAOCELetter = Missing.Value;
                object encoding = Missing.Value;
                object insertLineBreaks = Missing.Value;
                object allowSubstitutions = Missing.Value;
                object lineEnding = Missing.Value;
                object addBiDiMarks = Missing.Value;
                
                document.SaveAs(ref fileName, ref fileFormat, ref lockComments, ref password, ref addToRecentFiles, ref writePassword, ref readOnlyRecommended, ref embedTrueTypeFonts, ref saveNativePictureFormat, ref saveFormsData, ref saveAsAOCELetter, ref encoding, ref insertLineBreaks, ref allowSubstitutions, ref lineEnding, ref addBiDiMarks);
                
                document.PrintOut(ref background, ref append, ref wdPrintAllDocument, ref newTemplate, ref newTemplate, ref newTemplate, ref wdPrintDocumentContent, ref copies, ref pages, ref wdPrintAllPages, ref append, ref background, ref newTemplate, ref append, ref newTemplate, ref newTemplate, ref newTemplate, ref newTemplate);
            }
            catch
            {
                throw;
            }
            finally
            {
                while (application.BackgroundPrintingStatus > 0)
                {
                    Thread.Sleep(250);
                }
                
                document.Close(ref newTemplate, ref newTemplate, ref newTemplate);
                
                application.Quit(ref newTemplate, ref newTemplate, ref newTemplate);
                
                document = null;
                
                application = null;
            }
        }

        public void Merge(string[] filesToMerge, string outputFileName, bool insertPageBreaks)
        {
            Application application = new Application() { Visible = false };
            
            Document document = new DocumentClass();
            
            object newTemplate = Missing.Value;
            int length = filesToMerge.Length;
            object template = filesToMerge[length - 1];
            object wdPageBreak = WdBreakType.wdPageBreak;

            try
            {
                document = application.Documents.Add(ref template, ref newTemplate, ref newTemplate, ref newTemplate);
                
                Selection selection = application.Selection;
                
                for (int i = 0; i < (length - 1); i++)
                {
                    selection.InsertFile(filesToMerge[i], ref newTemplate, ref newTemplate, ref newTemplate, ref newTemplate);
                    
                    if (insertPageBreaks)
                    {
                        selection.InsertBreak(ref wdPageBreak);
                    }
                }
                
                object fileName = outputFileName;
                object fileFormat = Missing.Value;
                object lockComments = Missing.Value;
                object password = Missing.Value;
                object addToRecentFiles = Missing.Value;
                object writePassword = Missing.Value;
                object readOnlyRecommended = Missing.Value;
                object embedTrueTypeFonts = Missing.Value;
                object saveNativePictureFormat = Missing.Value;
                object saveFormsData = Missing.Value;
                object saveAsAOCELetter = Missing.Value;
                object encoding = Missing.Value;
                object insertLineBreaks = Missing.Value;
                object allowSubstitutions = Missing.Value;
                object lineEnding = Missing.Value;
                object addBiDiMarks = Missing.Value;
                
                document.SaveAs(ref fileName, ref fileFormat, ref lockComments, ref password, ref addToRecentFiles, ref writePassword, ref readOnlyRecommended, ref embedTrueTypeFonts, ref saveNativePictureFormat, ref saveFormsData, ref saveAsAOCELetter, ref encoding, ref insertLineBreaks, ref allowSubstitutions, ref lineEnding, ref addBiDiMarks);
            }
            catch
            {
                throw;
            }
            finally
            {
                document.Close(ref newTemplate, ref newTemplate, ref newTemplate);
                
                application.Quit(ref newTemplate, ref newTemplate, ref newTemplate);
                
                document = null;

                application = null;
            }
        }

        private void AddRow(int quantity, string sku, string description, ref Document document)
        {
            object beforeRow = Missing.Value;
            
            Row row = document.Tables[1].Rows.Add(ref beforeRow);
            
            row.Cells.Shading.BackgroundPatternColor = WdColor.wdColorWhite;
            
            document.Tables[1].Rows[row.Index].Cells[1].Range.Text = quantity.ToString();
            document.Tables[1].Rows[row.Index].Cells[1].Range.Font.Color = WdColor.wdColorBlack;
            document.Tables[1].Rows[row.Index].Cells[1].Range.Font.Bold = 0;
            document.Tables[1].Rows[row.Index].Cells[2].Range.Text = sku;
            document.Tables[1].Rows[row.Index].Cells[2].Range.Font.Color = WdColor.wdColorBlack;
            document.Tables[1].Rows[row.Index].Cells[2].Range.Font.Bold = 0;
            document.Tables[1].Rows[row.Index].Cells[3].Range.Text = description;
            document.Tables[1].Rows[row.Index].Cells[3].Range.Font.Color = WdColor.wdColorBlack;
            document.Tables[1].Rows[row.Index].Cells[3].Range.Font.Bold = 0;
            document.Tables[1].Rows[row.Index].Cells[4].Range.Text = quantity.ToString();
            document.Tables[1].Rows[row.Index].Cells[4].Range.Font.Color = WdColor.wdColorBlack;
            document.Tables[1].Rows[row.Index].Cells[4].Range.Font.Bold = 0;
        }

        private void AddRowCoaction(string date, string shipmethod, string orderID, int quantity, string sku, string description, ref Document document, int i)
        {
            object beforeRow = Missing.Value;
            if (i == 0)
            {
                Row row = document.Tables[1].Rows[1].Cells[1].Tables[1].Rows.Add(ref beforeRow);
                document.Tables[1].Rows[1].Cells[1].Tables[1].Rows[row.Index].Cells[1].Range.Text = date;
                document.Tables[1].Rows[1].Cells[1].Tables[1].Rows[row.Index].Cells[2].Range.Text = DateTime.Now.ToString("yyyy/MM/dd");
                document.Tables[1].Rows[1].Cells[1].Tables[1].Rows[row.Index].Cells[3].Range.Text = shipmethod;
                document.Tables[1].Rows[1].Cells[1].Tables[1].Rows[row.Index].Cells[4].Range.Text = orderID;
            }
            
            Row row2 = document.Tables[1].Rows[1].Cells[1].Tables[2].Rows.Add(ref beforeRow);
            document.Tables[1].Rows[1].Cells[1].Tables[2].Rows[row2.Index].Cells[1].Range.Text = quantity.ToString();
            document.Tables[1].Rows[1].Cells[1].Tables[2].Rows[row2.Index].Cells[2].Range.Text = sku;
            document.Tables[1].Rows[1].Cells[1].Tables[2].Rows[row2.Index].Cells[3].Range.Text = description;
        }
    }
}
