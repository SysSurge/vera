using System.Text;

namespace VeraWAF.CrossCuttingConcerns {
    public class StringUtilities {
        public byte[] Utf8ToAscii(string utf8String)
        {
            var utf8Bytes = Encoding.UTF8.GetBytes(utf8String);
            return Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("windows-1252"), utf8Bytes);
        }

        public string ConvertToHex(string utf8String) {
            var hex = new StringBuilder();
            var asciiString = Utf8ToAscii(utf8String);

            foreach (var c in asciiString) hex.AppendFormat("{0:x2}", c);
            
            return hex.ToString();
        }
    }
}