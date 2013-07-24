using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using ErrorLogApplication.Model;

namespace ErrorLogApplication.Core
{
    public class ErrorLogService
    {
        readonly string INSERT_COMMAND = "INSERT INTO ErrorsLog (Application, ApplicationID, ErrorDate, ClassName, BriefErrorText, ErrorText, Category) VALUES(@Application, @ApplicationID, @ErrorDate, @ClassName, @BriefErrorText, @ErrorText, @Category)";
        readonly string SELECT_COMMAND = "SELECT COUNT(*) FROM ErrorsLog WHERE Application=@Application AND ApplicationID=@ApplicationID AND ErrorDate=@ErrorDate AND ClassName=@ClassName AND BriefErrorText=@BriefErrorText AND ErrorText=@ErrorText AND Category=@Category;";
        string connectionString = ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString;
        IDao dao = null;

        public ErrorLogService()
        {
            dao = new MySqlDao(connectionString);
        }

        public void WriteError(ErrorLog error)
        {
            MySqlCommand q = new MySqlCommand(SELECT_COMMAND);
            q.Parameters.Add("@Application", MySqlDbType.VarChar).Value = error.Application;
            q.Parameters.Add("@ApplicationID", MySqlDbType.VarChar).Value = error.ApplicationID;
            q.Parameters.Add("@ClassName", MySqlDbType.VarChar).Value = error.ClassName;
            q.Parameters.Add("@BriefErrorText", MySqlDbType.VarChar).Value = error.BriefErrorText;
            q.Parameters.Add("@ErrorText", MySqlDbType.VarChar).Value = error.ErrorText;
            q.Parameters.Add("@Category", MySqlDbType.VarChar).Value = error.Category;
            q.Parameters.Add("@ErrorDate", MySqlDbType.DateTime).Value = error.ErrorDate;
            int? count = dao.ExecuteScalar<int>(q);

            if (count == 0)
            {
                q = new MySqlCommand(INSERT_COMMAND);
                q.Parameters.Add("@Application", MySqlDbType.VarChar).Value = error.Application;
                q.Parameters.Add("@ApplicationID", MySqlDbType.VarChar).Value = error.ApplicationID;
                q.Parameters.Add("@ClassName", MySqlDbType.VarChar).Value = error.ClassName;
                q.Parameters.Add("@BriefErrorText", MySqlDbType.VarChar).Value = error.BriefErrorText;
                q.Parameters.Add("@ErrorText", MySqlDbType.VarChar).Value = error.ErrorText;
                q.Parameters.Add("@Category", MySqlDbType.VarChar).Value = error.Category;
                q.Parameters.Add("@ErrorDate", MySqlDbType.DateTime).Value = error.ErrorDate;
                error.ErrorLogID = dao.ExecuteScalar<int>(q);
            }
        }
    }
}
