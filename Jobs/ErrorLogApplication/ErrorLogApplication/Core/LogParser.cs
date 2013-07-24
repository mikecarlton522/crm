using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErrorLogApplication.Model;
using System.IO;
using System.Text.RegularExpressions;

namespace ErrorLogApplication.Core
{
    public class LogParser
    {
        public List<ErrorLog> ParseLog(string filePath, string application, DateTime from, int max)
        {
            List<ErrorLog> logs = new List<ErrorLog>();
            try
            {
                string allText = ReadAllContent(filePath).Replace("----", "~");
                Regex regEx = new Regex(@"(?<date>([0-9]{4}-[0-9]{2}-[0-9]{2}\s[0-9]{2}:[0-9]{2}:[0-9]{2}))");//([^(A-Z)]+)(?<category>(ERROR|INFO))");
                var matchesDate = regEx.Matches(allText);
                regEx = new Regex(@"(?<category>(ERROR|INFO))");
                var matchesCategory = regEx.Matches(allText);
                regEx = new Regex(@"(-\s+)(?<class>(\S+))(\r\n)");
                var matchesClass = regEx.Matches(allText);
                regEx = new Regex(@"((ERROR|INFO)[^\r\n]+\r\n)(?<brief>([^\r\n]+))");
                var matchesBrief = regEx.Matches(allText);
                regEx = new Regex(@"(ERROR|INFO)\s+(?<text>([^~]+))");
                var matchesText = regEx.Matches(allText);
                regEx = new Regex(@"(ERROR|INFO)\s[a-zA-z\.]+\s\[(?<applicationID>(\S+))\]\s-");
                var matchesApplicationID = regEx.Matches(allText);
                for (int i = matchesDate.Count - 1; i >= Math.Max(matchesDate.Count - max, 0); i--)
                {
                    string date = matchesDate[i].Groups["date"].Value.Trim();
                    string className = matchesClass[i].Groups["class"].Value.Trim();
                    string text = matchesText[i].Groups["text"].Value.Trim();
                    string brief = matchesBrief[i].Groups["brief"].Value.Trim();
                    string applicationID = matchesApplicationID[i].Groups["applicationID"].Value.Trim();

                    DateTime errorDate = DateTime.Parse(date);

                    if (errorDate >= from)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            Category = "Error",
                            Application = application,
                            ApplicationID = applicationID,
                            BriefErrorText = brief,
                            ClassName = className,
                            ErrorDate = errorDate,
                            ErrorText = text
                        };
                        logs.Add(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.ReadKey();

                logs = new List<ErrorLog>();
            }

            return logs;
        }

        private string ReadAllContent(string filePath)
        {
            string content = string.Empty; ;

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }
    }
}
