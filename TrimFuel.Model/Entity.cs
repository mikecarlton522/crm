using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public abstract class Entity
    {
        public void ValidateEntity()
        {
            using (ValidateHelper v = new ValidateHelper())
            {
                ValidateFields(v);
            }
        }

        protected abstract void ValidateFields(ValidateHelper v);

        public virtual void SetDefaultValues()
        {
        }
    }
}
