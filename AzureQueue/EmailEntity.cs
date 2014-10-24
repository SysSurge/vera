using System.Runtime.Serialization;

namespace VeraWAF.AzureQueue
{
    /// <summary>
    /// E-mail entity as stored in the queue.
    /// </summary>
    [DataContract]
    public class EmailEntity
    {
        /// <summary>
        /// From e-mail
        /// </summary>
        [DataMember]
        public string From;

        /// <summary>
        /// To e-mail
        /// </summary>
        [DataMember]
        public string To;

        /// <summary>
        /// E-mail subject
        /// </summary>
        [DataMember]
        public string Subject;

        /// <summary>
        /// E-mail body
        /// </summary>
        [DataMember]
        public string Body;
    }
}
