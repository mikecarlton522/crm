using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FitDiet.Store1.Controls
{
    public interface IStepControl
    {
        void ShowError(string errorText);

        void HideError();
    }
}
