using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace TrimFuel.WebServices.BillingAPI.Logic
{
    public class LoggingOutputFilter : MemoryStream
    {
        // Private members
        private Stream m_outputStream;
        private StringBuilder m_outputString;

        // Ctor
        public LoggingOutputFilter(Stream outputStream, StringBuilder outputString)
        {
            m_outputString = outputString;
            InitStream(outputStream);
        }
        private void InitStream(Stream outputStream)
        {
            m_outputStream = outputStream;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Gets a string out of input bytes
            try
            {
                Encoding enc = HttpContext.Current.Response.ContentEncoding;
                if (enc == null)
                {
                    enc = Encoding.UTF8;
                }
                string theText = enc.GetString(buffer, offset, count);
                m_outputString.Append(theText);
            }
            catch { }

            m_outputStream.Write(buffer, offset, count);
        }
    }
}
