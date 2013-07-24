using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public abstract class EntityView : Entity
    {
        protected override void ValidateFields(ValidateHelper v)
        {
            throw new Exception("EntityView is read only entity");
        }
    }
}
