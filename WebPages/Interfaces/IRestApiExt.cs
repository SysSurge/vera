using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;

namespace VeraWAF.WebPages.Interfaces
{
    [ServiceContract]
    public interface IRestApiExt
    {
        //[OperationContract]
        //[WebInvoke(Method = "GET",
        //    UriTemplate = "Custom1/username/{username}",
        //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
        //    RequestFormat = WebMessageFormat.Json,
        //    ResponseFormat = WebMessageFormat.Json)]
        //string Custom1(string username);
    }
}
