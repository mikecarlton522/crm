using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao
{
    public interface IDao
    {
        void Save<TEntity>(TEntity entity) where TEntity : Entity;
        TEntity Load<TEntity>(object key) where TEntity : Entity;
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        //TODO: out from interface when full LINQ is ready
        IList<TEntity> Load<TEntity>(MySqlCommand selectCommand) where TEntity : Entity;
        //IList<Set<TEntity1, TEntity2>> Load<TEntity1, TEntity2>(MySqlCommand selectCommand) where TEntity1 : Entity where TEntity2 : Entity;
        //IList<Set<TEntity1, TEntity2, TEntity3>> Load<TEntity1, TEntity2, TEntity3>(MySqlCommand selectCommand) where TEntity1 : Entity where TEntity2 : Entity where TEntity3 : Entity;
        Nullable<T> ExecuteScalar<T>(MySqlCommand selectCommand) where T : struct;
        void ExecuteNonQuery(MySqlCommand command);
    }
}
