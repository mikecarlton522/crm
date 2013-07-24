using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class View<TStruct> : EntityView where TStruct : struct
    {
        public TStruct? Value { get; set; }
    }
}
