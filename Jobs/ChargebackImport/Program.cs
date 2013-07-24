using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;

namespace ChargebackImport
{
    class Program
    {
        private const string TRANSACTION_ID = "O";
        private const string CONTROL_NUMBER = "L";
        private const string STATUS = "M";
        private const string POSTING_DATE = "C";
        private const string REASON_CODE = "I";

        static void Main(string[] args)
        {
            try
            {
                string fileName = string.Empty;
                if (args.Length < 1)
                {
                    //throw new Exception("File name is not specified");
                    fileName = "all_chargbacks.xlsx";
                }
                else
                {
                    fileName = args[0];
                }

                if (!File.Exists(fileName))
                {
                    throw new Exception(string.Format("File ({0}) does not exist", fileName));
                }

                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileName, false);
                Worksheet sheet = spreadsheetDocument.WorkbookPart.WorksheetParts.First().Worksheet;
                SharedStringTablePart stringTable = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                MySqlCommand q = new MySqlCommand("select * from ChargebackStatusType");
                IList<ChargebackStatusType> statusList = dao.Load<ChargebackStatusType>(q);

                q = new MySqlCommand("select * from ChargebackReasonCode");
                IList<ChargebackReasonCode> reasonList = dao.Load<ChargebackReasonCode>(q);

                Console.WriteLine("BEGIN PROCESSING");
                foreach (Row row in sheet.Descendants<Row>())
                {
                    ProcessRow(row, stringTable, dao, statusList, reasonList);
                }
                Console.WriteLine("END PROCESSING");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ProcessRow(Row row, SharedStringTablePart stringTable, IDao dao, IList<ChargebackStatusType> statusList, IList<ChargebackReasonCode> reasonList)
        {
            try
            {
                string sTransactionID = GetCellValue(row, TRANSACTION_ID, stringTable);
                string sControloNumber = GetCellValue(row, CONTROL_NUMBER, stringTable);
                string sReasonCode = GetCellValue(row, REASON_CODE, stringTable);
                string sStatus = GetCellValue(row, STATUS, stringTable);
                string sDate = GetCellValue(row, POSTING_DATE, stringTable);
                
                if (string.IsNullOrEmpty(sTransactionID))
                    throw new Exception("Merchant Transaction ID is empty");

                if (string.IsNullOrEmpty(sStatus))
                    throw new Exception("Status is null or empty");

                if (string.IsNullOrEmpty(sDate))
                    throw new Exception("Posting Date is null or empty");

                if (string.IsNullOrEmpty(sReasonCode))
                    throw new Exception("Reason Code is null or empty");

                int transctionID = 0;
                if (!int.TryParse(sTransactionID, out transctionID))
                    throw new Exception(string.Format("Can't parse Merchant Transaction ID({0})", sTransactionID));
                
                string sStatusEnd = sStatus.Split('-').Last().Trim().ToLower();
                ChargebackStatusType status = statusList.Where(i => i.DisplayName.ToLower() == sStatusEnd).FirstOrDefault();
                if (status == null)
                    throw new Exception(string.Format("Can't determine Status({0})", sStatus));

                int iReasonCode = 0;
                if (!int.TryParse(sReasonCode, out iReasonCode))
                    throw new Exception(string.Format("Can't parse Reason Code({0})", sTransactionID));

                double dDate = 0;
                if (!double.TryParse(sDate, out dDate))
                    throw new Exception(string.Format("Can't parse Posting Date({0})", sDate));
                DateTime date = DateTime.FromOADate(dDate);

                MySqlCommand q = new MySqlCommand("Select bs.*, c.Amount From Billing b " +
                    "inner join BillingSubscription bs on b.BillingID=bs.BillingID " +
                    "inner join BillingSale bsale on bsale.BillingSubscriptionID=bs.BillingSubscriptionID " +
                    "inner join ChargeHistoryEx c on bsale.ChargeHistoryID=c.ChargeHistoryID " +
                    "where bsale.SaleID=@saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int32).Value = transctionID;
                BillingSubscription bs = dao.Load<BillingSubscription>(q).FirstOrDefault();
                
                if (bs == null)
                    throw new Exception(string.Format("Can't find BillingSale for SaleID({0})", transctionID));

                ChargebackReasonCode reasonCode = reasonList.Where(i => i.ReasonCode == iReasonCode).FirstOrDefault();
                if (reasonCode == null)
                    reasonCode = reasonList.Where(i => i.ReasonCode == iReasonCode + 4800).FirstOrDefault();
                if (reasonCode == null)
                    throw new Exception(string.Format("Can't determine Reason Code({0})", iReasonCode));

                try
                {
                    dao.BeginTransaction();

                    bs.StatusTID = BillingSubscriptionStatusEnum.Inactive;
                    dao.Save<BillingSubscription>(bs);

                    SaleChargeback sc = new SaleChargeback();
                    sc.BillingID = (int)bs.BillingID.Value;
                    sc.SaleID = transctionID;
                    sc.ChargebackStatusTID = status.ChargebackStatusTypeID;
                    sc.CaseNumber = sControloNumber;
                    sc.ChargebackReasonCodeID = reasonCode.ChargebackReasonCodeID;
                    sc.CreateDT = date;
                    dao.Save(sc);

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dao.RollbackTransaction();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Row({0})-----------------------------------------------------------------", row.RowIndex));
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                Console.WriteLine("-------------------------------------------------------------------------");
            }
        }

        private static string GetCellValue(Row row, string column, SharedStringTablePart stringTable)
        {
            string res = null;

            Cell theCell = row.Elements<Cell>().Where(
                c => c.CellReference.Value.ToUpper() == column + row.RowIndex.Value.ToString()).
                FirstOrDefault();

            if (theCell != null)
            {
                res = theCell.InnerText;

                if (theCell.DataType != null)
                {
                    switch (theCell.DataType.Value)
                    {
                        case CellValues.SharedString:
                            if (stringTable != null)
                            {
                                res = stringTable.SharedStringTable.ElementAt(int.Parse(res)).InnerText;
                            }
                            break;

                        case CellValues.Boolean:
                            switch (res)
                            {
                                case "0":
                                    res = "false";
                                    break;
                                default:
                                    res = "true";
                                    break;
                            }
                            break;
                    }
                }
            }

            return res;
        }
    }
}
