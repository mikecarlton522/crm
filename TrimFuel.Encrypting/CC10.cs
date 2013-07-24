using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;

namespace TrimFuel.Encrypting
{
    public class CC10 : CC
    {
        private const string KEY_FILE_PATH = @"c:\Key\cc.key";
        private const string VERSION_PREFIX = "EncV10";
        private const string FORMAT = "^" + VERSION_PREFIX + @"[0-9a-zA-Z\+\/\=]{13,}$";

        protected override string VersionNumber
        {
            get
            {
                return "V.1.0";
            }
        }

        protected override bool ValidateFormat(string encryptedCreditCard)
        {
            if (string.IsNullOrEmpty(encryptedCreditCard))
            {
                return false;
            }
            return Regex.IsMatch(encryptedCreditCard, FORMAT);
        }

        private string ReadKey()
        {
            string res = string.Empty;
            try
            {
                res = File.ReadAllText(KEY_FILE_PATH, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(ex);
            }
            return res;            
        }

        private TripleDES CreateAlg()
        {
            TripleDES alg = new TripleDESCryptoServiceProvider();
            alg.Mode = CipherMode.CBC;
            alg.Padding = PaddingMode.PKCS7;
            alg.BlockSize = 64;
            alg.FeedbackSize = 8;
            alg.KeySize = 192;
            alg.Key = Convert.FromBase64String(ReadKey());
            return alg;
        }

        protected override string Encrypt(string decryptedCreditCard)
        {
            TripleDES alg = CreateAlg();

            //Generate IV
            alg.GenerateIV();
            byte[] iv = alg.IV;

            byte[] creditCard = Encoding.ASCII.GetBytes(decryptedCreditCard);

            byte[] encryptedCreditCard = null;
            //Encrypt credit card
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, alg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cStream.Write(creditCard, 0, creditCard.Length);
                    cStream.FlushFinalBlock();
                    encryptedCreditCard = mStream.ToArray();
                }
            }

            byte[] encryptedCreditCardWithIV = new byte[8 + encryptedCreditCard.Length];
            //Copy IV to final buffer
            Buffer.BlockCopy(iv, 0, encryptedCreditCardWithIV, 0, 8);
            //Copy encrypted credit card to final buffer
            Buffer.BlockCopy(encryptedCreditCard, 0, encryptedCreditCardWithIV, 8, encryptedCreditCard.Length);
            //Encode to Base64 encoding and add version prefix
            return VERSION_PREFIX + Convert.ToBase64String(encryptedCreditCardWithIV);
        }

        protected override string Decrypt(string encyptedCreditCard)
        {
            //Remove version prefix
            encyptedCreditCard = encyptedCreditCard.Substring(VERSION_PREFIX.Length);
            //Convert from Base64
            byte[] creditCardWithIV = Convert.FromBase64String(encyptedCreditCard);
            //First 8 bytes is IV
            byte[] iv = new byte[8];
            Buffer.BlockCopy(creditCardWithIV, 0, iv, 0, 8);
            //Other bytes - encrypted credit card
            byte[] creditCard = new byte[creditCardWithIV.Length - 8];
            Buffer.BlockCopy(creditCardWithIV, 8, creditCard, 0, creditCardWithIV.Length - 8);

            TripleDES alg = CreateAlg();
            alg.IV = iv;

            byte[] decryptedCreditCard = null;
            //Decrypt credit card
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, alg.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cStream.Write(creditCard, 0, creditCard.Length);
                    cStream.FlushFinalBlock();
                    decryptedCreditCard = mStream.ToArray();
                }
            }

            return Encoding.ASCII.GetString(decryptedCreditCard);
        }
    }
}
