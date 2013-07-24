using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Dao.EntityDataProviders;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using System.Collections;

namespace TrimFuel.Business
{
    public class ErrorsLogService : BaseService
    {
        private string _identifier;

        private enum ErrorsLogDeleteMode
        {
            Application,
            ApplicationID,
            BriefErrorText,
            ClassName,
        }

        public IList<ErrorsLog> GetAllErrors()
        {
            IList<ErrorsLog> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select el.* from ErrorsLog el");

                res = dao.Load<ErrorsLog>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void DeleteErrorsById(int id)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand cmd = new MySqlCommand("delete from ErrorsLog where ErrorsLogID = @ErrorsLogID");
                cmd.Parameters.AddWithValue("@ErrorsLogID", id);

                dao.ExecuteNonQuery(cmd);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);

                dao.RollbackTransaction();
            }
        }

        public void DeleteErrorsByApplication(string application)
        {
            _identifier = application;

            DeleteErrors(ErrorsLogDeleteMode.Application);
        }

        public void DeleteErrorsByApplicationID(string applicationID)
        {
            _identifier = applicationID;

            DeleteErrors(ErrorsLogDeleteMode.ApplicationID);
        }

        public void DeleteErrorsByBriefErrorText(string briefErrorText)
        {
            _identifier = briefErrorText;

            DeleteErrors(ErrorsLogDeleteMode.BriefErrorText);
        }

        public void DeleteErrorsByClassName(string className)
        {
            _identifier = className;

            DeleteErrors(ErrorsLogDeleteMode.ClassName);
        }        

        private void DeleteErrors(ErrorsLogDeleteMode mode)
        {
            string query = "delete from ErrorsLog where ##MODE## = @identifier;SELECT ROW_COUNT();";

            switch (mode)
            {
                case ErrorsLogDeleteMode.Application:
                    query = query.Replace("##MODE##", "Application");
                    break;
                case ErrorsLogDeleteMode.ApplicationID:
                    query = query.Replace("##MODE##", "ApplicationID");
                    break;
                case ErrorsLogDeleteMode.BriefErrorText:
                    query = query.Replace("##MODE##", "BriefErrorText");
                    break;
                case ErrorsLogDeleteMode.ClassName:
                    query = query.Replace("##MODE##", "ClassName");
                    break;
            }

            try
            {
                dao.BeginTransaction();

                MySqlCommand cmd = new MySqlCommand(query);
                cmd.Parameters.AddWithValue("@identifier", _identifier);

                dao.ExecuteNonQuery(cmd);
                
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                
                dao.RollbackTransaction();
            }
        }
    }
}
