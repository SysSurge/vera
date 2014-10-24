using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace VeraWAF.WebPages.Bll {
    public class Interchange {
        public string JsonSerialize(object userProfile) {
            var serializer = new DataContractJsonSerializer(userProfile.GetType());
            var memoryStream = new MemoryStream();

            serializer.WriteObject(memoryStream, userProfile);

            return Encoding.Default.GetString(memoryStream.ToArray());
        }
    }
}
