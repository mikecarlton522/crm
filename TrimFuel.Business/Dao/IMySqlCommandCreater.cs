using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao
{
    public interface IMySqlCommandCreater
    {
        MySqlCommand CreateCommand();
        MySqlCommand CreateCommand(string sql);
    }
}
