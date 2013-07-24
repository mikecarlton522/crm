using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business
{
    public enum BusinessErrorState
    {
        Success = 1,
        Error = 2
    }

    public class BusinessError<Type>
    {
        public BusinessError()
        { }

        public BusinessError(Type returnValue, BusinessErrorState state, string errorMessage)
        {
            ReturnValue = returnValue;
            State = state;
            if (State == BusinessErrorState.Error)
            {
                ErrorMessage = errorMessage;
            }
        }

        public Type ReturnValue { get; set; }
        public BusinessErrorState State { get; set; }
        public string ErrorMessage { get; set; }
    }
}
