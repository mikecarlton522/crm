using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.Screenshots
{
    public static class WebShot
    {
        public static string GenerateScreenshot(int campaignID, int width, int height, string path)
        {
            CampaignService campaignService = new CampaignService();

            Campaign campaign = campaignService.GetCampaignByID(campaignID);

            if (campaign != null)
            {
                if (!string.IsNullOrEmpty(campaign.URL))
                {
                    try
                    {
                        WebBrowser browser = new WebBrowser();

                        browser.ScrollBarsEnabled = false;

                        browser.ScriptErrorsSuppressed = true;

                        browser.Navigate(campaign.URL);

                        while (browser.ReadyState != WebBrowserReadyState.Complete)
                        {
                            System.Windows.Forms.Application.DoEvents();
                        }

                        browser.Width = browser.Document.Body.ScrollRectangle.Width;

                        browser.Height = browser.Document.Body.ScrollRectangle.Height;

                        Bitmap bitmap = new Bitmap(browser.Width, browser.Height);

                        browser.DrawToBitmap(bitmap, new Rectangle(0, 0, browser.Width, browser.Height));

                        browser.Dispose();

                        Bitmap resized = CropAndResizeImage(bitmap, new Size(width, height));

                        resized.Save(string.Format("{0}/thumbnail-{1}_{2}x{3}.jpg", path, campaignID, width, height), ImageFormat.Jpeg);

                        bitmap.Dispose();
                        resized.Dispose();

                        return "1";
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }

            return "No campaign found";
        }

        private static Bitmap CropAndResizeImage(Bitmap img, Size size)
        {
            //the cropped image origin is always point 0,0

            int sourceWidth = img.Width;
            int sourceHeight = img.Height;
            float sourceRatio = ((float)img.Width / (float)img.Height);

            int targetWidth = size.Width;
            int targetHeight = size.Height;
            float targetRatio = ((float)size.Width / (float)size.Height);

            int cropWidth = 0;
            int cropHeight = 0;

            if (targetRatio > 1)
            {
                cropWidth = sourceWidth;
                cropHeight = (int)(sourceWidth / targetRatio);

                if (cropHeight > sourceHeight)
                {
                    cropHeight = sourceHeight;
                    cropWidth = (int)(sourceHeight * targetRatio);
                }
            }
            else
            {
                cropWidth = (int)(sourceHeight * targetRatio);
                cropHeight = sourceHeight;

                if (cropWidth > sourceWidth)
                {
                    cropWidth = sourceWidth;
                    cropHeight = (int)(sourceWidth / targetRatio);
                }
            }

            Bitmap bmp = new Bitmap(img);

            Bitmap bmpCrop = bmp.Clone(new Rectangle(0, 0, cropWidth, cropHeight), bmp.PixelFormat);

            Bitmap bmpResize = new Bitmap(bmpCrop, targetWidth, targetHeight);

            using (Graphics graphics = Graphics.FromImage(bmpResize))
            {
                graphics.DrawImage(bmpCrop, 0, 0, targetWidth, targetHeight);
            }

            bmp.Dispose();
            bmpCrop.Dispose();

            return bmpResize;
        }
    }
}

