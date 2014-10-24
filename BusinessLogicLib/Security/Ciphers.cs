using System;

namespace VeraWAF.WebPages.Bll.Security {
    public class Ciphers
    {
        /// <summary>
        /// XOR cipher.
        /// Simple and fast cipher that works both ways.
        /// </summary>
        /// <param name="arr">Data to encrypt/decrypt</param>
        /// <param name="key">64-bit cipher key</param>
        /// <returns>Returns cipher result with same length as \c arrLen</returns>
        public byte[] XorCipher(byte[] arr, UInt64 key)
        {
	        // Output buffer
            var outArr = new byte[arr.Length];
	
	        // Refer to the 64-bit key as a 8 byte byte array
	        var keyArr = BitConverter.GetBytes(key);

            for (int i = 0; i < arr.Length; i++)
                outArr[i] = (byte)(arr[i] ^ keyArr[i % keyArr.Length]);

	        return outArr;
        }

    }
}