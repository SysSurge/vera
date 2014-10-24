using System;
using System.Security.Cryptography;
using System.Text;

namespace VeraWAF.WebPages.Ccc
{
    public class EncryptionUtilities
    {
        /// <summary>
        /// Makes a MD5 hash from a string
        /// </summary>
        /// <param name="inString">String to make a MD5 hash from</param>
        /// <returns>MD5 hash in the form of a Guid</returns>
        public Guid GetMd5Hash(string inString)
        {
            var unicodeEncoding = new UnicodeEncoding();
            var message = unicodeEncoding.GetBytes(inString);

            MD5 hashString = new MD5CryptoServiceProvider();

            return new Guid(hashString.ComputeHash(message));
        }
    }
}