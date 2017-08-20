


#region '---- Using Library ----'

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace Wrly.Utils
{
    /// <summary>
    /// This class is used to encrypt/decrypt data using encryption 64 / base 64 algorithm.
    /// Functions defined in this library will return following : 
    ///     1. Return encrypted string
    ///     2. Return decrypted string
    /// </summary>
    public static class ValueEncryptionHelper
    {
        #region '---- Members ----'

        private const string DEFAULT_KEY = "#kl?+@<z";

        #endregion

        #region '---- Methods ----'

        /// <summary>
        /// Encrypts string using key value.
        /// If key is not passed then it will take default key defined in this class.
        /// POSSIBLE PURPOSE            : Encrypt the provided string with the provided/default key
        /// EXAMPLE APPEARANCE          : ID=42
        /// INPUT                       : Simple one line text.
        /// OUTPUT                      : N4qsPJ3Z3fk=
        /// </summary>
        /// <param name="strStringToEncrypt">possible value of StringToEncrypt like ID=42</param>
        /// <param name="strKey">possible value of Key like 12345678912345678912345678</param>

        /// <Remarks>
        /// </Remarks>
        public static string Encrypt(string strStringToEncrypt, string strKey)
        {
            DESCryptoServiceProvider objDESCryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream;

            // Check whether the key is valid, otherwise make it valid
            CheckKey(ref strKey);

            objDESCryptoServiceProvider.Key = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);
            objDESCryptoServiceProvider.IV = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);
            byte[] inputBytes = Encoding.UTF8.GetBytes(strStringToEncrypt);

            objCryptoStream = new CryptoStream(objMemoryStream, objDESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
            objCryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(objMemoryStream.ToArray());
        }

        /// <summary>
        /// Encrypts array of bytes using key value.
        /// If key is not passed then it will take default key defined in this class.
        /// POSSIBLE PURPOSE            : Encrypt the provided array of bytes with the provided/default key
        /// EXAMPLE APPEARANCE          : Array of bytes.
        /// INPUT                       : Array of bytes.
        /// OUTPUT                      : N4qsPJ3Z3fk=
        /// </summary>
        /// <param name="arrBytesToEncrypt">possible array of bytes containing bytes to encrypt</param>
        /// <param name="strKey">possible value of Key like 12345678912345678912345678</param>

        /// <Remarks>
        /// </Remarks>
        public static byte[] Encrypt(byte[] arrBytesToEncrypt, string strKey)
        {
            DESCryptoServiceProvider objDESCryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream;

            // Check whether the key is valid, otherwise make it valid
            CheckKey(ref strKey);

            objDESCryptoServiceProvider.Key = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);
            objDESCryptoServiceProvider.IV = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);

            objCryptoStream = new CryptoStream(objMemoryStream, objDESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            objCryptoStream.Write(arrBytesToEncrypt, 0, arrBytesToEncrypt.Length);
            objCryptoStream.FlushFinalBlock();

            return objMemoryStream.ToArray();
        }

        /// <summary>
        /// Decrypts the string using key.
        /// If key is not passed then it will take default key defined in this class.
        /// POSSIBLE PURPOSE            : Decrypt the provided string with the provided/default key
        /// EXAMPLE APPEARANCE          : h%6jhh^3
        /// INPUT                       : Simple one line text.
        /// OUTPUT                      : ID=42
        /// </summary>
        /// <param name="strStringToDecrypt">possible value of StringToDecrypt like N4qsPJ3Z3fk=</param>
        /// <param name="strKey">possible value of Key like h%6jhh^3</param>

        /// <Remarks>
        /// </Remarks>
        public static string Decrypt(string strStringToDecrypt, string strKey)
        {
            try
            {
                DESCryptoServiceProvider objDESCryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream objMemoryStream = new MemoryStream();
                CryptoStream objCryptoStream;

                // Check whether the key is valid, otherwise make it valid
                CheckKey(ref strKey);

                objDESCryptoServiceProvider.Key = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);
                objDESCryptoServiceProvider.IV = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);
                byte[] inputBytes = Convert.FromBase64String(strStringToDecrypt);

                objCryptoStream = new CryptoStream(objMemoryStream, objDESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
                objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                objCryptoStream.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;

                return encoding.GetString(objMemoryStream.ToArray());
            }
            catch (Exception)
            {
                // Note:- Earlier if query string tampered it was returning default value (means 0).
                // Now as over some pages we have conditions where we are checking that if value passed > 0 then do this else do that.
                // So to avoid conflict scenario we decided to return -1 if query string tampered.
                // MODIFIED DATE:- 10/10/2013
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypts encrypted array of bytes using key.
        /// If key is not passed then it will take default key defined in this class.
        /// POSSIBLE PURPOSE            : Decrypt the provided array of bytes with the provided/default key
        /// EXAMPLE APPEARANCE          : Array of bytes
        /// INPUT                       : Array of bytes
        /// OUTPUT                      : ID=42
        /// </summary>
        /// <param name="arrBytesToDecrypt">possible encrypted array of bytes which need to be decrypted</param>
        /// <param name="strKey">possible value of Key like h%6jhh^3</param>

        /// <Remarks>
        /// </Remarks>
        public static byte[] Decrypt(byte[] arrBytesToDecrypt, string strKey)
        {
            try
            {
                DESCryptoServiceProvider objDESCryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream objMemoryStream = new MemoryStream();
                CryptoStream objCryptoStream;

                // Check whether the key is valid, otherwise make it valid
                CheckKey(ref strKey);

                objDESCryptoServiceProvider.Key = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);
                objDESCryptoServiceProvider.IV = HashKey(strKey, objDESCryptoServiceProvider.KeySize / 8);

                objCryptoStream = new CryptoStream(objMemoryStream, objDESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
                objCryptoStream.Write(arrBytesToDecrypt, 0, arrBytesToDecrypt.Length);
                objCryptoStream.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;

                return objMemoryStream.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Validates that the used key has a length of exact eight 
        /// characters.
        /// </summary>
        /// <param name="strKeyToCheck">possible value of KeyToCheck like "ABC"</param>

        /// <Remarks>
        /// </Remarks>
        private static void CheckKey(ref string strKeyToCheck)
        {
            strKeyToCheck = strKeyToCheck.Length > 8 ? strKeyToCheck.Substring(0, 8) : strKeyToCheck;

            if (strKeyToCheck.Length < 8)
            {
                for (int i = strKeyToCheck.Length; i < 8; i++)
                {
                    strKeyToCheck += DEFAULT_KEY[i];
                }
            }
        }

        /// <summary>
        /// Gets the Hash Key.
        /// </summary>
        /// <param name="strKey">possible value of Key like "ABC"</param>
        /// <param name="intLength">possible value of Length like "10"</param>

        /// <Remarks>
        /// </Remarks>
        private static byte[] HashKey(string strKey, int intLength)
        {
            SHA1CryptoServiceProvider objSHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();

            // Hash the key
            byte[] arrKeyBytes = Encoding.UTF8.GetBytes(strKey);
            byte[] arrHash = objSHA1CryptoServiceProvider.ComputeHash(arrKeyBytes);

            // Truncate hash
            byte[] arrTruncatedHash = new byte[intLength];
            Array.Copy(arrHash, 0, arrTruncatedHash, 0, intLength);

            return arrTruncatedHash;
        }

        #endregion
    }
}
