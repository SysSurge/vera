using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureQueue
{
    /// <summary>
    /// Data source for the Vera Azure queues
    /// </summary>
    public class AzureQueueDataSource : AzureQueueDataSourceExt
    {
        /// <summary>
        /// Azure queue context
        /// </summary>
        protected AzureQueueContext _queueContext;

        /// <summary>
        /// Get the Azure cloud queue client
        /// </summary>
        /// <returns>Azure cloud queue client object</returns>
        public CloudQueueClient GetCloudQueueClient()
        {
            return _queueContext.QueueClient;
        }

        /// <summary>
        /// Initiate the Azure queue context
        /// </summary>
        void InitServiceContext()
        {
            var connectionString = RoleEnvironment.GetConfigurationSettingValue("DataConnectionString");
            _queueContext = new AzureQueueContext(connectionString);
        }

        /// <summary>
        /// Create a queue
        /// </summary>
        /// <param name="queueName">Name of the queue</param>
        /// <returns>Return true on success, false if already exists, throw exception on error</returns>
        public bool CreateQueueIfNotExists(string queueName)
        {
            try
            {
                var queue = GetCloudQueueClient().GetQueueReference(queueName);
                return queue.CreateIfNotExist();
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 409) return false;
                throw;
            }
        }

        /// <summary>
        /// Create the queues
        /// </summary>
        public void CreateQueuesIfTheyDontExists()
        {
            CreateQueueIfNotExists(AzureQueueContext.EmailQueueName);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public AzureQueueDataSource()
        {
            InitServiceContext();
        }

        /// <summary>
        /// Create or update a blob
        /// </summary>
        /// <param name="queueName">Name of queue</param>
        /// <param name="message">Message to post in the queue</param>
        /// <returns>Return true on success, false if already exists, throw exception on error</returns>
        public bool PutMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                var queue = _queueContext.QueueClient.GetQueueReference(queueName);
                queue.AddMessage(message);
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404) return false;
                throw;
            }
        }

        /// <summary>
        /// Serializes a e-mail to Json
        /// </summary>
        /// <param name="from">Sender e-mail address</param>
        /// <param name="to">Recipient e-mail address</param>
        /// <param name="subject">E-mail subject text</param>
        /// <param name="body">E-mail body text</param>
        /// <returns>JSON serialized VeraWAF.AzureQueue.EmailEntity object</returns>
        string SerializeEmail(string from, string to, string subject, string body)
        {
            var email = new EmailEntity()
                            {
                                From = from,
                                To = to,
                                Subject = subject,
                                Body = body
                            };

            var stream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(EmailEntity));
            serializer.WriteObject(stream, email);
            var serializedEmail = Encoding.Default.GetString(stream.ToArray());
            stream.Close();
            return serializedEmail;
        }

        /// <summary>
        /// Puts a e-mail on the Azure queue to be sent
        /// </summary>
        /// <param name="from">Sender e-mail address</param>
        /// <param name="to">Recipient e-mail address</param>
        /// <param name="subject">E-mail subject text</param>
        /// <param name="body">E-mail body text</param>
        /// <returns>Return true on success, false if already exists, throw exception on error</returns>
        public bool SendEmail(string from, string to, string subject, string body)
        {
            var serializedEmail = SerializeEmail(from, to, subject, body);
            var message = new CloudQueueMessage(serializedEmail);
            return PutMessage(AzureQueueContext.EmailQueueName, message);
        }

        /// <summary>
        /// Retrieve the next message from a queue
        /// </summary>
        /// <param name="queueName">Name of queue</param>
        /// <param name="message">Message</param>
        /// <returns>Return true on success (message available), false if no message or no queue, throw exception on error</returns>
        bool GetMessage(string queueName, out CloudQueueMessage message)
        {
            message = null;

            try
            {
                var queue = GetCloudQueueClient().GetQueueReference(queueName);
                message = queue.GetMessage();
                return message != null;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404) return false;

                throw;
            }
        }

        /// <summary>
        /// Deserializes a e-mail message
        /// </summary>
        /// <param name="message">Message serialized in the Json inerchange format</param>
        /// <returns>Deserialized e-mail as a EmailEntity object</returns>
        EmailEntity DeserializeEmail(CloudQueueMessage message)
        {
            var stream = new MemoryStream(Encoding.Default.GetBytes(message.AsString));
            var ser = new DataContractJsonSerializer(typeof(EmailEntity));
            var email = (EmailEntity)ser.ReadObject(stream);
            stream.Close();
            return email;
        }

        /// <summary>
        /// Retrieve the next message from a queue
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <returns>Return true on success (message available), false if no message or no queue, throw exception on error</returns>
        public bool GetEmail(out EmailEntity email, out CloudQueueMessage message)
        {
            var result = GetMessage(AzureQueueContext.EmailQueueName, out message);
            email = result ? DeserializeEmail(message) : null;
            return result;
        }

        /// <summary>
        /// Delete a previously read message
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <param name="message">Message to delete</param>
        /// <returns>Return true on success, false if already exists, throw exception on error</returns>
        public bool DeleteMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                var queue = GetCloudQueueClient().GetQueueReference(queueName);
                if (message != null) queue.DeleteMessage(message);
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404) return false;

                throw;
            }
        }

        /// <summary>
        /// Delete a e-mail message from the queue
        /// </summary>
        /// <param name="eMail">E-mail to delete</param>
        /// <returns>Return true on success, false if already exists, throw exception on error</returns>
        public bool DeleteEmail(CloudQueueMessage eMail)
        {
            return DeleteMessage(AzureQueueContext.EmailQueueName, eMail);
        }

    }
}