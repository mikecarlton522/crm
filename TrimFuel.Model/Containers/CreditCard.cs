using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using TrimFuel.Encrypting;
using System.Text.RegularExpressions;
using TrimFuel.Model.Enums;

namespace TrimFuel.Model.Containers
{
    public class CreditCard
    {
        public CreditCard(string creditCard)
        {
            originalCreditCard = creditCard;
        }

        private CC encryptService = null;
        public CC EncryptService 
        {
            get 
            {
                if (encryptService == null)
                {
                    encryptService = new CC();
                }
                return encryptService;
            }
        }

        private string originalCreditCard = null;

        private string decryptedCreditCard = null;
        public string DecryptedCreditCard 
        {
            get 
            {
                if (decryptedCreditCard == null)
                {
                    decryptedCreditCard = EncryptService.GetDecrypted(originalCreditCard);
                }
                return decryptedCreditCard;
            }
        }

        private string searchHash = null;
        public string SearchHash
        {
            get
            {
                if (searchHash == null)
                {
                    searchHash = EncryptService.GetSearchHash(originalCreditCard);
                }
                return searchHash;
            }
        }

        private string encryptedCreditCard = null;
        public string EncryptedCreditCard
        {
            get
            {
                if (encryptedCreditCard == null)
                {
                    encryptedCreditCard = EncryptService.GetEncrypted(originalCreditCard);
                }
                return encryptedCreditCard;
            }
        }

        private string searchHashCode = null;
        public string SearchHashCode
        {
            get
            {
                if (searchHashCode == null)
                {
                    searchHashCode = EncryptService.GetSearchHash(originalCreditCard);
                }
                return searchHashCode;
            }
        }

        public string MD5Hash
        {
            get
            {
                if (string.IsNullOrEmpty(DecryptedCreditCard))
                {
                    return null;
                }

                MD5 md5Hasher = MD5.Create();

                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(DecryptedCreditCard));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
        public string DecryptedCreditCardLeft6
        {
            get
            {
                string res = DecryptedCreditCard;
                if (string.IsNullOrEmpty(res) || res.Length < 6)
                {
                    return null;
                }
                return res.Substring(0, 6);
            }
        }

        public string DecryptedCreditCardRight4
        {
            get
            {
                string res = DecryptedCreditCard;
                if (string.IsNullOrEmpty(res) || res.Length < 4)
                {
                    return null;
                }
                return res.Substring(res.Length - 4, 4);
            }
        }

        public string CardTypeName
        {
            get
            {
                string res = DecryptedCreditCard;
                if (string.IsNullOrEmpty(res) || res.Length < 1)
                {
                    return null;
                }
                if (res.Substring(0, 1) == "4")
                {
                    return "Visa";
                }
                else if (res.Substring(0, 1) == "5")
                {
                    return "MasterCard";
                }
                return "";
            }
        }

        public int? TryGetCardType()
        {
            int? res = null;

            try
            {
                string dc = DecryptedCreditCard;
                if (!string.IsNullOrEmpty(dc))
                {
                    if (dc[0] == '4')
                    {
                        res = PaymentTypeEnum.Visa;
                    }
                    else if (dc[0] == '5')
                    {
                        res = PaymentTypeEnum.Mastercard;
                    }
                    else if 
                        ((dc[0] == '3' && dc[1] == '0' && (dc[2] == '0' || dc[2] == '1' || dc[2] == '2' || dc[2] == '3' || dc[2] == '4' || dc[2] == '5')) ||
                        (dc[0] == '3' && dc[1] == '6'))
                    {                    
                        res = PaymentTypeEnum.DinersClub;
                    }
                    else if (dc[0] == '3')
                    {
                        res = PaymentTypeEnum.AmericanExpress;
                    }
                    else if (dc[0] == '6')
                    {
                        res = PaymentTypeEnum.Discover;
                    }
                }
            }
            catch { }

            return res;
        }

        private const string ALLOWED_SYMBOLS = " -";
        private const string DECRYPTED_FORMAT = "^[0-9]{13,}$";

        private static string EscapeAllowedSymbols(string creditCard)
        {
            string res = creditCard;
            foreach (char escapeSymbol in ALLOWED_SYMBOLS)
            {
                res = res.Replace(escapeSymbol.ToString(), "");
            }
            return res;
        }

        private static bool IsValidDecryptedCard(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return false;
            }
            return Regex.IsMatch(creditCard, DECRYPTED_FORMAT);
        }

        public static bool ValidateDecryptedFormat(string decryptedCreditCard)
        {
            if (string.IsNullOrEmpty(decryptedCreditCard))
            {
                return false;
            }
            string cc = EscapeAllowedSymbols(decryptedCreditCard);
            if (string.IsNullOrEmpty(cc))
            {
                return false;
            }
            return IsValidDecryptedCard(cc);
        }

        public string TryGetCardName()
        {
            var typeID = TryGetCardType();
            if (typeID != null)
                return PaymentTypeEnum.Types[typeID];

            return string.Empty;
        }
    }

    //public class CreditCard
    //{
    //    //10 symbols, 0..9
    //    private const string ENCYPT_ALPHABET = @":.@#*-~!^$";
    //    private const string DECRYPT_ALPHABET = @"0123456789";

    //    public CreditCard(string creditCard)
    //    {
    //        if (!string.IsNullOrEmpty(creditCard))
    //        {
    //            if (IsEncrypted(creditCard))
    //            {
    //                this.DecryptedCreditCard = Decrypt(creditCard);
    //                this.EncryptedCreditCard = creditCard;
    //            }
    //            else if (IsDecrypted(creditCard))
    //            {
    //                this.DecryptedCreditCard = creditCard;
    //                this.EncryptedCreditCard = Encrypt(creditCard);
    //            }
    //            else
    //            {
    //                throw new Exception(string.Format("Credit Card Number ({0}) is not valid", creditCard));
    //            }
    //        }
    //    }

    //    public string DecryptedCreditCard { get; set; }
    //    public string EncryptedCreditCard { get; set; }
    //    public string MD5Hash 
    //    {
    //        get 
    //        {   
    //            if (string.IsNullOrEmpty(DecryptedCreditCard))
    //            {
    //                return null;
    //            }

    //            MD5 md5Hasher = MD5.Create();

    //            // Convert the input string to a byte array and compute the hash.
    //            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(DecryptedCreditCard));

    //            // Create a new Stringbuilder to collect the bytes
    //            // and create a string.
    //            StringBuilder sBuilder = new StringBuilder();

    //            // Loop through each byte of the hashed data 
    //            // and format each one as a hexadecimal string.
    //            for (int i = 0; i < data.Length; i++)
    //            {
    //                sBuilder.Append(data[i].ToString("x2"));
    //            }

    //            // Return the hexadecimal string.
    //            return sBuilder.ToString();
    //        }
    //    }
    //    public string DecryptedCreditCardLeft6
    //    {
    //        get
    //        {
    //            string res = DecryptedCreditCard;
    //            if (string.IsNullOrEmpty(res) || res.Length < 6)
    //            {
    //                return null;
    //            }
    //            return res.Substring(0, 6);
    //        }
    //    }

    //    private bool IsEncrypted(string creditCard)
    //    {
    //        foreach (char c in creditCard)
    //        {
    //            if (!ENCYPT_ALPHABET.Contains(c))
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    private bool IsDecrypted(string creditCard)
    //    {
    //        foreach (char c in creditCard)
    //        {
    //            if (!DECRYPT_ALPHABET.Contains(c))
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    private string Encrypt(string creditCard)
    //    {
    //        for (int i = 0; i < ENCYPT_ALPHABET.Length; i++)
    //        {
    //            creditCard = creditCard.Replace(i.ToString(), ENCYPT_ALPHABET[i].ToString());
    //        }
    //        return creditCard;
    //    }

    //    private string Decrypt(string encyptedCreditCard)
    //    {
    //        for (int i = 0; i < ENCYPT_ALPHABET.Length; i++)
    //        {
    //            encyptedCreditCard = encyptedCreditCard.Replace(ENCYPT_ALPHABET[i].ToString(), i.ToString());
    //        }
    //        return encyptedCreditCard;
    //    }
    //}
}
