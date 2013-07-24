using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;

namespace TrimFuel.Encrypting
{
    public class CC
    {
        private const string TEST_CARD = "4111111111111111";
        private const string TEST_CARD_ENCRYPTED = "*...............";
        private const string SEARCH_HASH_PREFIX = "HASH#";
        private CC[] VERSION_LIST
        {
            get { return new CC[] { this, new CC01(), new CC10() }; }
        }

        private const string ALLOWED_SYMBOLS = " -";
        private const string DECRYPTED_FORMAT = "^[0-9]{13,}$";

        protected virtual string VersionNumber
        {
            get { return "V.0.0"; }
        }

        /// <summary>
        /// Returns encrypted CC number by the last method, no matter input string is encrypted by any known version or decrypted card.
        /// </summary>
        /// <param name="creditCard">Encryped by any known version or decrypted CC number, </param>
        /// <returns>Encrypted by the last version CC number, 
        /// empty string if parameter is null or empty,
        /// unchanged parameter if it was not recognized as decrypted or encrypted by any known version CC number
        /// </returns>
        public string GetEncrypted(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return string.Empty;
            }
            string decryptedCreditCard = GetDecrypted(creditCard);
            if (decryptedCreditCard == TEST_CARD)
            {
                return TEST_CARD_ENCRYPTED;
            }
            CC lastVersion = VERSION_LIST.Last();
            return lastVersion.Encrypt(decryptedCreditCard) + ComputeSearchHash(decryptedCreditCard);
        }

        public string GetDecrypted(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return string.Empty;
            }
            //remove search hash code
            int searchHashStart = creditCard.IndexOf(SEARCH_HASH_PREFIX);
            if (searchHashStart >= 0)
            {
                creditCard = creditCard.Remove(searchHashStart);
            }
            CC encVersion = DetermineEncryptionVersion(creditCard);
            if (encVersion == null)
            {
                throw new CreditCardFormatNotRecognizedException(creditCard);
            }
            string decryptedCreditCard = encVersion.Decrypt(creditCard);
            decryptedCreditCard = EscapeAllowedSymbols(decryptedCreditCard);
            if (!IsValidDecryptedCard(decryptedCreditCard))
            {
                throw new InvalidDecryptionException(creditCard, encVersion.VersionNumber, decryptedCreditCard);
            }
            return decryptedCreditCard;
        }

        public string GetSearchHash(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return string.Empty;
            }
            string decryptedCreditCard = GetDecrypted(creditCard);
            return ComputeSearchHash(decryptedCreditCard);
        }

        public string GetDecryptedLastFour(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return string.Empty;
            }
            string decryptedCC = GetDecrypted(creditCard);
            return decryptedCC.Substring(decryptedCC.Length - 4, 4);
        }

        public string GetDecryptedAsteriskEscaped(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return string.Empty;
            }
            string decryptedCC = GetDecrypted(creditCard);
            return decryptedCC.Substring(0, 8) + "****" + decryptedCC.Substring(12, decryptedCC.Length - 12);
        }

        //Prefix + MD5 on last 
        private string ComputeSearchHash(string creditCard)
        {
            string last6 = creditCard.Substring(creditCard.Length - 6);
            byte[] creditCardLast6 = Encoding.ASCII.GetBytes(last6);
            MD5 hashService = MD5.Create();
            byte[] hash = hashService.ComputeHash(creditCardLast6);
            return SEARCH_HASH_PREFIX + Convert.ToBase64String(hash);
        }

        private CC DetermineEncryptionVersion(string creditCard)
        {
            CC res = null;
            foreach (CC version in VERSION_LIST)
            {
                if (version.ValidateFormat(creditCard))
                {
                    res = version;
                    break;
                }
            }
            return res;
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

        public bool IsValidDecryptedCard(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return false;
            }
            return Regex.IsMatch(creditCard, DECRYPTED_FORMAT);
        }

        protected virtual bool ValidateFormat(string encryptedCreditCard)
        {
            if (string.IsNullOrEmpty(encryptedCreditCard))
            {
                return false;
            }
            string cc = EscapeAllowedSymbols(encryptedCreditCard);
            if (string.IsNullOrEmpty(cc))
            {
                return false;
            }
            return IsValidDecryptedCard(cc);
        }

        protected virtual string Encrypt(string decryptedCreditCard)
        {
            return EscapeAllowedSymbols(decryptedCreditCard);
        }

        protected virtual string Decrypt(string encyptedCreditCard)
        {
            return encyptedCreditCard;
        }
    }
}
