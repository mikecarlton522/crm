using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    //TODO: implement IList<object>
    public class Set<T1, T2>
        where T1 : Entity
        where T2 : Entity
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
    }

    public class Set<T1, T2, T3> : Set<T1, T2>
        where T1 : Entity
        where T2 : Entity
        where T3 : Entity
    {
        public T3 Value3 { get; set; }
    }

    public class Set<T1, T2, T3, T4> : Set<T1, T2, T3>
        where T1 : Entity
        where T2 : Entity
        where T3 : Entity
        where T4 : Entity
    {
        public T4 Value4 { get; set; }
    }

    public class Set<T1, T2, T3, T4, T5> : Set<T1, T2, T3, T4>
        where T1 : Entity
        where T2 : Entity
        where T3 : Entity
        where T4 : Entity
        where T5 : Entity
    {
        public T5 Value5 { get; set; }
    }
}
