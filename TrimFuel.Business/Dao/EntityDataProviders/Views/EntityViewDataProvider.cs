using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public abstract class EntityViewDataProvider<TEntityView> : EntityDataProvider<TEntityView> where TEntityView : EntityView
    {
        public override void Save(TEntityView entity, IMySqlCommandCreater cmdCreater)
        {
            throw new Exception("EntityView is read only entity");
        }

        public override TEntityView Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new Exception("EntityView is read only entity");
        }
    }
}
