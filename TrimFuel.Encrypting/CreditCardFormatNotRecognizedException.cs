using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Encrypting
{
    public class CreditCardFormatNotRecognizedException : CreditCardEncryptingException
    {
        private const string EXCEPTION_FORMAT = "Credit card number({0}) was not recognized as decrypted or known format encrypted credit card.";

        public CreditCardFormatNotRecognizedException(string creditCard)
        {
            this.CreditCard = creditCard;
        }

        public override string Message
        {
            get
            {
                return string.Format(EXCEPTION_FORMAT, CreditCard);
            }
        }
    }
}
