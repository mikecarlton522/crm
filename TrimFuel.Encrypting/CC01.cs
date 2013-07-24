using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrimFuel.Encrypting
{
    public class CC01 : CC
    {
        private const string ALLOWED_SYMBOLS = " ";
        private const string FORMAT = @"^[\:\.\@\#\*\-\~\!\^\$]{13,}$";

        private const string ENCYPT_ALPHABET = @":.@#*-~!^$";
        private const string DECRYPT_ALPHABET = @"0123456789";

        protected override string VersionNumber
        {
            get { return "V.0.1"; }
        }

        private string EscapeAllowedSymbols(string creditCard)
        {
            string res = creditCard;
            foreach (char escapeSymbol in ALLOWED_SYMBOLS)
            {
                res = res.Replace(escapeSymbol.ToString(), "");
            }
            return res;
        }

        protected override bool ValidateFormat(string encryptedCreditCard)
        {
            if (string.IsNullOrEmpty(encryptedCreditCard))
            {
                return false;
            }
            string cc = EscapeAllowedSymbols(encryptedCreditCard);
            return Regex.IsMatch(cc, FORMAT);
        }

        protected override string Encrypt(string decryptedCreditCard)
        {
            for (int i = 0; i < ENCYPT_ALPHABET.Length; i++)
            {
                decryptedCreditCard = decryptedCreditCard.Replace(i.ToString(), ENCYPT_ALPHABET[i].ToString());
            }
            return decryptedCreditCard;
        }

        protected override string Decrypt(string encyptedCreditCard)
        {
            for (int i = 0; i < ENCYPT_ALPHABET.Length; i++)
            {
                encyptedCreditCard = encyptedCreditCard.Replace(ENCYPT_ALPHABET[i].ToString(), i.ToString());
            }
            return encyptedCreditCard;
        }
    }
}
