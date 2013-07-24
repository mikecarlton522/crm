using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Office.Interop.Word;

namespace TrimFuel.Tools.PackingManager.Logic
{
    public class PickSlip
    {
        

        

        object missing = System.Reflection.Missing.Value;

        public PickSlip(string printer, IList<PickSlipRow> rows)
        {
            Application word = new Application() { Visible = false };

            Document document = new Document();

            object oFalse = false;

            object oTrue = true;

            object printRange = WdPrintOutRange.wdPrintAllDocument;

            object items = WdPrintOutItem.wdPrintDocumentContent;

            object pageType = WdPrintOutPages.wdPrintAllPages;

            object copies = "1";

            object pages = "";

            object oEndOfDoc = "\\endofdoc";

            try
            {
                document = word.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                Paragraph header1;
                header1 = document.Content.Paragraphs.Add(ref missing);
                header1.Range.Text = "DAILY PICK TICKET";
                header1.Format.SpaceAfter = 24;
                header1.Range.InsertParagraphAfter();

                Paragraph header2;
                object h2Range = document.Bookmarks.get_Item(ref oEndOfDoc).Range;
                header2 = document.Content.Paragraphs.Add(ref h2Range);
                header2.Range.Text = "SKU\tItem Description\tTotal Quantity Ordered for Batch";
                header2.Format.SpaceAfter = 6;
                header2.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                header2.Range.InsertParagraphAfter();

                foreach (PickSlipRow row in rows)
                {
                    Paragraph paragraph;
                    object pRange = document.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    paragraph = document.Content.Paragraphs.Add(ref pRange);
                    paragraph.Range.Text = string.Format("{0}\t{1}\t{2}", row.SKU, row.ItemDescription, row.Quantity);
                    paragraph.Format.SpaceAfter = 6;
                    paragraph.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    paragraph.Range.InsertParagraphAfter();
                }

                word.ActivePrinter = printer;

                document.PrintOut(ref oTrue, ref oFalse, ref printRange, ref missing, ref missing, ref missing,
                    ref items, ref copies, ref pages, ref pageType, ref oFalse, ref oTrue,
                    ref missing, ref oFalse, ref missing, ref missing, ref missing, ref missing);

                document.SaveAs(Path.Combine(Settings.ArchivePath, string.Concat(DateTime.Now.ToString("yyyyMMddHHmmss"), "_DPT.docx")));
            }
            catch
            {
                throw;
            }
            finally
            {
                document.Close(ref missing, ref missing, ref missing);

                while (word.BackgroundPrintingStatus > 0)
                {
                    System.Threading.Thread.Sleep(250);
                }

                word.Quit(ref missing, ref missing, ref missing);
            }
        }
    }

    public class PickSlipRow
    {
        public string SKU { get; set; }
        public string ItemDescription { get; set; }
        public int Quantity { get; set; }
    }
}
