using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Dao.EntityDataProviders;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace TrimFuel.Business.Dao
{
    public class MySqlDao : IDao, IMySqlCommandCreater
    {
        public MySqlDao(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }

        #region IDao Members

        public void Save<TEntity>(TEntity entity) where TEntity : Entity
        {
            entity.SetDefaultValues();
            entity.ValidateEntity();

            OpenConnection();
            EntityDataProvider<TEntity>.CreateProvider().Save(entity, this);
            CloseConnection();
        }

        public TEntity Load<TEntity>(object key) where TEntity : Entity
        {
            TEntity res = null;

            if (key == null)
                throw new ArgumentNullException("key");

            OpenConnection();
            res = EntityDataProvider<TEntity>.CreateProvider().Load(key, this);
            CloseConnection();

            return res;
        }

        public IList<TEntity> Load<TEntity>(MySqlCommand selectCommand) where TEntity : Entity
        {
            IList<TEntity> res = null;

            selectCommand.Transaction = currentTransaction;
            selectCommand.Connection = OpenConnection();
            res = EntityDataProvider<TEntity>.CreateProvider().Load(selectCommand);
            CloseConnection();

            return res;
        }

        //public IList<Set<TEntity1, TEntity2>> Load<TEntity1, TEntity2>(MySqlCommand selectCommand) where TEntity1 : Entity where TEntity2 : Entity
        //{
        //    IList<Set<TEntity1, TEntity2>> res = null;

        //    selectCommand.Transaction = currentTransaction;
        //    selectCommand.Connection = OpenConnection();
        //    res = new Set<TEntity1, TEntity2>()
        //    {
        //        Value1 = EntityDataProvider<TEntity1>.CreateProvider().Load(selectCommand),
        //        Value2 = EntityDataProvider<TEntity2>.CreateProvider().Load(selectCommand)
        //    };
        //    CloseConnection();

        //    return res;
        //}

        public Nullable<T> ExecuteScalar<T>(MySqlCommand selectCommand) where T : struct
        {
            selectCommand.Transaction = currentTransaction;
            selectCommand.Connection = OpenConnection();
            object res = selectCommand.ExecuteScalar();
            CloseConnection();

            return (res != null) ? new Nullable<T>((T)Convert.ChangeType(res, typeof(T))) : null;
        }

        public void ExecuteNonQuery(MySqlCommand command)
        {
            command.Transaction = currentTransaction;
            command.Connection = OpenConnection();
            command.ExecuteNonQuery();
            CloseConnection();
        }

        #endregion

        #region Connections and transactions

        private MySqlConnection currentConnection = null;

        private MySqlConnection OpenConnection()
        {
            if (currentConnection == null)
            {
                currentConnection = new MySqlConnection(ConnectionString);
                currentConnection.Open();
            }            
            return currentConnection;
        }

        private void CloseConnection()
        {
            if (currentTransaction == null)
            {
                currentConnection.Close();
                currentConnection = null;
            }
        }

        private MySqlTransaction currentTransaction = null;

        private int transactionLevel = 0;

        public void BeginTransaction()
        {
            if (transactionLevel == 0)
            {
                MySqlConnection c = OpenConnection();
                currentTransaction = c.BeginTransaction();
            }
            transactionLevel++;
        }

        public void CommitTransaction()
        {
            transactionLevel--;
            if (transactionLevel == 0)
            {
                currentTransaction.Commit();
                currentTransaction = null;
                CloseConnection();
            }
        }

        public void RollbackTransaction()
        {
            transactionLevel--;
            if (transactionLevel == 0)
            {
                currentTransaction.Rollback();
                currentTransaction = null;
                CloseConnection();
            }
            else
            {
                throw new Exception("Inner transaction rollback");
            }
        }

        #endregion

        #region IMySqlCommandCreater Members

        public MySqlCommand CreateCommand()
        {
            MySqlCommand res = new MySqlCommand();
            res.CommandType = CommandType.Text;
            res.Connection = OpenConnection();
            res.Transaction = currentTransaction;
            return res;
        }

        public MySqlCommand CreateCommand(string sql)
        {
            MySqlCommand res = CreateCommand();
            res.CommandText = sql;
            return res;
        }

        #endregion
    }
}
