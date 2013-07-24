using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Encrypting
{
    public class InvalidDecryptionException : CreditCardEncryptingException
    {
        private const string EXCEPTION_FORMAT = "Credit card number({0}) was failed to be decrypted by version({1}). Invalid result({2}) of decryption.";

        public InvalidDecryptionException(string creditCard, string decryptionVersion, string creditCardDecryptionResult)
        {
            this.CreditCard = creditCard;
            this.DecryptionVersion = decryptionVersion;
            this.CreditCardDecryptionResult = creditCardDecryptionResult;
        }

        public string DecryptionVersion { get; set; }
        public string CreditCardDecryptionResult { get; set; }

        public override string Message
        {
            get
            {
                return string.Format(EXCEPTION_FORMAT, CreditCard, DecryptionVersion, CreditCardDecryptionResult);
            }
        }
    }
}
