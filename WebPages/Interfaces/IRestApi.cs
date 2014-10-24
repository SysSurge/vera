using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Interfaces {
    [ServiceContract]
    public interface IRestApi {

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string Query(string queryTerms);

        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string PerformanceCounter(string apiKey, int samplingIntervalMs);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "CloudCommand/username/{username}/command/{command}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string CloudCommand(string username, string command);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "CloudCommandArgs/username/{username}/command/{command}/arguments/{args}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string CloudCommandArgs(string username, string command, string args);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetBlobUploadUrl/username/{username}/containerAddress/{containerAddress}/folder/{folder}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetBlobUploadUrl(string username, string containerAddress, string folder);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Update/username/{username}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string Update(string username, TablePropertyInfo fieldData);

    }

}
