using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Helper Class for Janrain Engage
    /// </summary>
    public class JanRain
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public JanRain(string apiKey, string baseUrl)
        {
            while (baseUrl.EndsWith("/"))
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            _apiKey = apiKey;
            _baseUrl = baseUrl;
        }

        public string GetApiKey()
        {
            return _apiKey;
        }

        public string GetBaseUrl()
        {
            return _baseUrl;
        }

        public XmlElement AuthInfo(string token)
        {
            var query = new Dictionary<string, string> {{"token", token}};
            return ApiCall("auth_info", query);
        }

        public List<string> Mappings(string primaryKey)
        {
            var query = new Dictionary<string, string> {{"primaryKey", primaryKey}};
            var rsp = ApiCall("mappings", query);
            var oids = (XmlElement) rsp.FirstChild;
            var result = new List<string>();

            for (var i = 0; i < oids.ChildNodes.Count; i++)
                result.Add(oids.ChildNodes[i].InnerText);
            
            return result;
        }

        public Dictionary<string, ArrayList> AllMappings()
        {
            var query = new Dictionary<string, string>();
            var rsp = ApiCall("all_mappings", query);
            var result = new Dictionary<string, ArrayList>();            
            var nav = rsp.CreateNavigator();

            var mappings = (XPathNodeIterator) nav.Evaluate("/rsp/mappings/mapping");
            if (mappings != null)
                foreach (XPathNavigator m in mappings)
                {
                    var remoteKey = GetContents("./primaryKey/text()", m);
                    var identNodes = (XPathNodeIterator) m.Evaluate("./identifiers/identifier");
                    var identifiers = new ArrayList();
                    
                    if (identNodes != null)
                        foreach (XPathNavigator i in identNodes)
                            identifiers.Add(i.ToString());

                    result.Add(remoteKey, identifiers);
                }

            return result;
        }

        private string GetContents(string xpathExpr, XPathNavigator nav)
        {
            var rkNodes = (XPathNodeIterator) nav.Evaluate(xpathExpr);

            while (rkNodes != null && rkNodes.MoveNext())
                if (rkNodes.Current != null) return rkNodes.Current.ToString();

            return null;
        }

        public void Map(string identifier, string primaryKey)
        {
            var query = new Dictionary<string, string> {{"identifier", identifier}, {"primaryKey", primaryKey}};
            ApiCall("map", query);
        }

        public void Unmap(string identifier, string primaryKey)
        {
            var query = new Dictionary<string, string> {{"identifier", identifier}, {"primaryKey", primaryKey}};
            ApiCall("unmap", query);
        }

        private XmlElement ApiCall(string methodName, Dictionary<string, string> partialQuery)
        {
            var query = new Dictionary<string, string>(partialQuery) {{"format", "xml"}, {"apiKey", _apiKey}};
            var sb = new StringBuilder();

            foreach (var e in query)
            {
                if (sb.Length > 0) sb.Append('&');

                sb.Append(HttpUtility.UrlEncode(e.Key, Encoding.UTF8));
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncode(e.Value, Encoding.UTF8));
            }
            
            var data = sb.ToString();
            var url = new Uri(_baseUrl + "/api/v2/" + methodName);
            
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            // Write the request
            var stOut = new StreamWriter(request.GetRequestStream(),
                                         Encoding.ASCII);
            stOut.Write(data);
            stOut.Close();
            
            var response = (HttpWebResponse) request.GetResponse();
            var dataStream = response.GetResponseStream();
            var doc = new XmlDocument {PreserveWhitespace = false};
            
            if (dataStream != null) doc.Load(dataStream);
            
            var resp = doc.DocumentElement;

            if (resp == null || !resp.GetAttribute("stat").Equals("ok"))
                throw new Exception("Unexpected API error");
            
            return resp;
        }

    }
}