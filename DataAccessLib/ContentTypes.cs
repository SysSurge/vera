using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraWAF.WebPages.Dal
{
    /// <summary>
    /// Information about a content types
    /// </summary>
    public struct ContentTypeInfo
    {
        public ContentTypeInfo(EContentType internalType, string mimeType, string description, string[] suffixes)
        {
            InternalType = internalType;
            MimeType = mimeType;
            Description = description;
            Suffixes = suffixes;
        }

        /// <summary>
        /// Internal enumeration type that identifies the content type
        /// </summary>
        public EContentType InternalType;

        /// <summary>
        /// Mime type. Ex. "image/jpeg" for a JPEG image
        /// </summary>
        public string MimeType;

        /// <summary>
        /// Content type description.
        /// </summary>
        public string Description;

        /// <summary>
        /// Suffix for the mime type. Ex. "aspx" or "html"
        /// </summary>
        public string[] Suffixes;
    }

    /// <summary>
    /// Known content types
    /// </summary>
    public enum EContentType
    {
        Jpeg, Gif, Png, Css, Js, Html, Aspx
    }

    /// <summary>
    /// Handles served content types.
    /// This class is used by the Vera WAF virtual path provider to serve static files like *.CSS and 
    /// *.JS etc.
    /// </summary>
    /// <remarks>
    /// See the Web.Config file system.webServer->handlers to see which static files are served by
    /// the Vera WAF virtual path provider. It is not the intention that this class handles all known
    /// content types, but only those that are handled by the Vera virtual path provider.
    /// </remarks>
    public class ContentTypes
    {
        /// <summary>
        /// Index containing the content types using mimetypes as keys
        /// </summary>
        static Dictionary<string, ContentTypeInfo> _contentTypeIdxByMime;

        /// <summary>
        /// Index containing the content types using suffixes as keys
        /// </summary>
        static Dictionary<string, ContentTypeInfo> _contentTypeIdxBySuffix;

        /// <summary>
        /// Add a content type tot the index
        /// </summary>
        /// <param name="contentTypeIdxByMime">Index</param>
        /// <param name="internalType">Internal type</param>
        /// <param name="mimeType">Mime type</param>
        void AddContentType(Dictionary<string, ContentTypeInfo> contentTypeIdxByMime, 
            Dictionary<string, ContentTypeInfo> contentTypeIdxBySuffix, EContentType internalType,
            string mimeType, string description, string[] suffixes)
        {
            var contentTypeInfo = new ContentTypeInfo(internalType, mimeType, description, suffixes);

            // Add content type info to the index by mime type
            if (!contentTypeIdxByMime.ContainsKey(contentTypeInfo.MimeType))
                contentTypeIdxByMime.Add(contentTypeInfo.MimeType, contentTypeInfo);

            // Add content type info to the index by suffixes
            for (var i = 0; i < suffixes.Length; i++)
                _contentTypeIdxBySuffix.Add(suffixes[i], contentTypeInfo);
        }

        /// <summary>
        /// Load all the served content types
        /// </summary>
        /// <param name="contentTypeIdxByMime">Index</param>
        void LoadContentTypes(Dictionary<string, ContentTypeInfo> contentTypeIdxByMime,
            Dictionary<string, ContentTypeInfo> contentTypeIdxBySuffix)
        {
            AddContentType(contentTypeIdxByMime, contentTypeIdxBySuffix, EContentType.Aspx, "text/html", 
                Resources.Mimetypes.text_html, new string[] { "aspx" });
            AddContentType(contentTypeIdxByMime, contentTypeIdxBySuffix, EContentType.Css, "text/css", 
                Resources.Mimetypes.text_css, new string[] { "css" });
            AddContentType(contentTypeIdxByMime, contentTypeIdxBySuffix, EContentType.Gif, "image/gif", 
                Resources.Mimetypes.image_gif, new string[] { "gif" });
            AddContentType(contentTypeIdxByMime, contentTypeIdxBySuffix, EContentType.Html, "text/html", 
                Resources.Mimetypes.text_html, new string[] { "html", "htm" });
            AddContentType(contentTypeIdxByMime, contentTypeIdxBySuffix, EContentType.Jpeg, "image/jpeg", 
                Resources.Mimetypes.image_jpeg, new string[] { "jpeg", "jpg" });
            AddContentType(contentTypeIdxByMime, contentTypeIdxBySuffix, EContentType.Js, "application/javascript", 
                Resources.Mimetypes.application_javascript, new string[] { "js" });
            AddContentType(contentTypeIdxByMime, contentTypeIdxBySuffix, EContentType.Png, "image/png", 
                Resources.Mimetypes.image_png, new string[] { "png" });
        }

        /// <summary>
        /// Initiate content type indexes
        /// </summary>
        void InitIndexes()
        {
            // Create the content type index
            _contentTypeIdxByMime = new Dictionary<string, ContentTypeInfo>();
            _contentTypeIdxBySuffix = new Dictionary<string, ContentTypeInfo>();

            // Load the content types
            LoadContentTypes(_contentTypeIdxByMime, _contentTypeIdxBySuffix);
        }

        public ContentTypes()
        {
            // Load the indexes with their content type data
            InitIndexes();
        }

        /// <summary>
        /// Get information about a content type by its mime type
        /// </summary>
        /// <param name="mimetype">Mime type</param>
        /// <returns>Description or null if the mime type was not found</returns>
        public ContentTypeInfo GetInfoByMime(string mimetype) {
            return _contentTypeIdxByMime[mimetype];
        }


        /// <summary>
        /// Get information about a content type by its suffix
        /// </summary>
        /// <param name="suffix">Suffix. Ex. "html"</param>
        /// <returns>Description or null if the mime type was not found</returns>
        public ContentTypeInfo GetInfoBySuffix(string suffix) {
            return _contentTypeIdxBySuffix[suffix];
        }

        /// <summary>
        /// Get an index containing the content types using suffixes as keys
        /// </summary>
        public Dictionary<string, ContentTypeInfo> GetAllContentTypesOrderBySuffix() {
            return _contentTypeIdxBySuffix;
        }
    }
}
