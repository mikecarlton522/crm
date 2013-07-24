using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FitDiet.Store1.Logic;

namespace FitDiet.Store1.Controls
{
    public class StepChangeEventArgs : EventArgs
    {
        public StepChangeEventArgs(PageCartExt.CartState toState)
        {
            ToState = toState;
        }

        public PageCartExt.CartState ToState { get; set; }
    }
}
