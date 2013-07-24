using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Encrypting
{
    public abstract class CreditCardEncryptingException : Exception
    {
        public string CreditCard { get; set; }
    }
}
