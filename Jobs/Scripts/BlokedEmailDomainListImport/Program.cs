using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using TrimFuel.Business;
using TrimFuel.Business.Dao;
using log4net;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace BlokedEmailDomainListImport
{
    class Program
    {
        private const string FILE_NAME = @"DomainNames.txt";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));

            try
            {
                var blockedEmailDomainList = ConvertTxtFileToBlockedEmailDomainList(FILE_NAME);
                ViewBlockedEmailDomainList(blockedEmailDomainList);
                Clear();
                Console.WriteLine("Please, press any key!!!");
                Console.ReadLine();
                Load(blockedEmailDomainList);
                Console.WriteLine("Done!!! Please, press any key!!!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public static void ViewBlockedEmailDomainList(List<BlockedEmailDomain> blockedEmailDomainList)
        {
            foreach (var blockedEmailDomain in blockedEmailDomainList)
            {
                Console.WriteLine(blockedEmailDomain.Name);
            }
        }

        public static BlockedEmailDomain ConvertStringToBlockedEmailDomain(string str)
        {
            var blockedEmailDomain = new BlockedEmailDomain();
            blockedEmailDomain.CreateDT = DateTime.Parse(str.Substring(0, str.IndexOf("^")));
            blockedEmailDomain.Name = str.Substring(str.IndexOf("^") + 1, str.Length - 1 - str.IndexOf("^"));

            return blockedEmailDomain;
        }

        public static List<BlockedEmailDomain> ConvertTxtFileToBlockedEmailDomainList(string pathFile)
        {
            var fileStream = new FileStream(pathFile, FileMode.Open);
            var streamReader = new StreamReader(fileStream);
            var blockedEmailDomainList = new List<BlockedEmailDomain>();

            while (!streamReader.EndOfStream)
                blockedEmailDomainList.Add(ConvertStringToBlockedEmailDomain(streamReader.ReadLine()));

            streamReader.Close();
            fileStream.Close();

            return blockedEmailDomainList;
        }

        public static DataTable ConvertBlockedEmailDomainListToDataTable(List<BlockedEmailDomain> blockedEmailDomainList)
        {
            var props = TypeDescriptor.GetProperties(typeof(BlockedEmailDomain));
            var table = new DataTable();

            table.Columns.Add("BlockedEmailDomainID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("CreateDT", typeof(DateTime));

            table.Columns["BlockedEmailDomainID"].AutoIncrement = true;
            table.Columns["BlockedEmailDomainID"].AutoIncrementSeed = 1;
            table.Columns["BlockedEmailDomainID"].AutoIncrementStep = 1;

            object[] values = new object[props.Count];
            foreach (var item in blockedEmailDomainList)
            {
                for (int i = 1; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            return table;
        }

        public static void Load(List<BlockedEmailDomain> blockedEmailDomainList)
        {
            var dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            try
            {
                dao.BeginTransaction();
                foreach (var blockedEmailDomain in blockedEmailDomainList)
                {
                    dao.Save<BlockedEmailDomain>(blockedEmailDomain);
                }
                dao.CommitTransaction();
            }
            catch
            {
                dao.RollbackTransaction();
                throw;
            }
        }

        public static void Clear()
        {
            string ConnectionString = Config.Current.CONNECTION_STRINGS["TrimFuel"]; // "Datasource=localhost;Database=trimfuellocal;uid=root;pwd=1;";

            string CommandText = "TRUNCATE TABLE BlockedEmailDomain";

            MySqlConnection myConnection = new MySqlConnection(ConnectionString);
            MySqlCommand myCommand = new MySqlCommand(CommandText, myConnection);

            myConnection.Open();
            myCommand.ExecuteNonQuery();
            myConnection.Close();
        }
    }
}
