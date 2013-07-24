using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Business;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace WrongSHWtoCSV
{
    class Program
    {
        private const string WRONG_RESPONSE = "%Extended%";
        private const char DELIMITER = '|';

        static void Main(string[] args)
        {
            try
            {
                string fileName = string.Empty;
                if (args.Length < 1)
                {
                    fileName = "test.csv";
                    //throw new Exception("File name is not specified");
                }
                else
                {
                    fileName = args[0];
                }

                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                MySqlCommand q = new MySqlCommand("select shw.* from SHWRegistration shw " +
                    "where shw.Response like @wrongResponse " +
                    "order by shw.SHWRegistrationID asc");
                q.Parameters.Add("@wrongResponse", MySqlDbType.VarChar).Value = WRONG_RESPONSE;

                IList<SHWRegistration> shwList = dao.Load<SHWRegistration>(q);

                Console.WriteLine("BEGIN PROCESSING");
                IList<string> columns = new List<string>();
                IList<IDictionary<string, object>> rows = new List<IDictionary<string, object>>();
                foreach (SHWRegistration shw in shwList)
                {
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine(string.Format("SHWRegistrationID({0})", shw.SHWRegistrationID));

                    IDictionary<string, object> row = GetRow(shw);
                    try
                    {
                        Billing b = dao.Load<Billing>(row["BillingID"]);
                        if (b == null)
                            throw new Exception(string.Format("Billing({0}) was not found", row["BillingID"]));

                        row["strCCAddress1"] = b.Address1;
                        row["strCCAddress2"] = b.Address2;
                        row["strCCCity"] = b.City;
                        row["strCCState"] = b.State;
                        row["strCCZip"] = b.Zip;
                        row["strCCCountry"] = b.Country;
                    }
                    catch (Exception ex)
                    {
                        row = null;
                        Console.WriteLine(ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine(ex.InnerException.Message);
                    }

                    if (row != null)
                    {
                        rows.Add(row);
                        foreach (string key in row.Keys)
                        {
                            if (!columns.Contains(key))
                                columns.Add(key);
                        }
                    }
                    Console.WriteLine("------------------------------------------------");
                }
                WriteCSV(fileName, columns, rows);
                Console.WriteLine("END PROCESSING");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
            }
        }

        private static void WriteCSV(string fileName, IList<string> columns, IList<IDictionary<string, object>> rows)
        {
            using (StreamWriter csv = new StreamWriter(fileName, false, Encoding.ASCII))
            {
                string temp = string.Empty;
                foreach (string column in columns)
                {
                    temp += DELIMITER + column;
                }
                csv.WriteLine(temp.Substring(1));

                foreach (IDictionary<string, object> row in rows)
                {
                    temp = string.Empty;
                    foreach (string column in columns)
                    {
                        object val = null;
                        if (row.ContainsKey(column))
                            val = row[column];

                        temp += string.Format("{0}{1}", DELIMITER, val);
                    }
                    csv.WriteLine(temp.Substring(1));
                }
            }
        }

        private static string VAR_REG_EX = @"\<field name=""(?<varName>.+?)""\>(?<varValue>.+?)\</field\>";
        private static IDictionary<string, object> GetRow(SHWRegistration shw)
        {
            IDictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                res["SHWRegistrationID"] = shw.SHWRegistrationID;
                res["BillingID"] = shw.BillingID;
                if (shw.CreateDT != null)
                    res["CreateDT"] = shw.CreateDT.Value.ToString(CultureInfo.GetCultureInfo("en-US"));

                string[] vars = shw.Request.Split('&');
                foreach (string v in vars)
                {
                    if (!v.Contains("="))
                    {
                        throw new Exception(string.Format("Invalid variable occured ({0}), SHWRegistration skipped.", v));
                    }

                    string key = v.Substring(0, v.IndexOf('='));
                    string value = v.Substring(v.IndexOf('=')).TrimStart('=');

                    if (key.ToLower() != "strOptionalXML".ToLower())
                    {
                        res[key] = value;
                    }
                    else
                    {
                        MatchCollection matchList = Regex.Matches(value, VAR_REG_EX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        foreach (Match match in matchList)
                        {
                            res[match.Groups["varName"].Value] = match.Groups["varValue"].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = null;

                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
            }

            return res;
        }
    }
}
