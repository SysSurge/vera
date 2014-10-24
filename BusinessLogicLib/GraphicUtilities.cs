using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;

namespace VeraWAF.WebPages.Bll
{
    public class GraphicUtilities
    {
        const int DefaultThumbnailPixelWidth = 64;
        const int DefaultThumbnailPixelHeight = 64;

        ImageCodecInfo GetEncoder(ImageFormat format) {
            var codecs = ImageCodecInfo.GetImageDecoders();

            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        /// <summary>
        /// Resizes an image
        /// </summary>
        /// <param name="sourceImage">Source image to process</param>
        /// <param name="keepAspectRatio">If true then image aspect ratio is respected, if false the image may be stretched to fit the given width and height</param>
        /// <param name="maxPixelWidth">Scale to image width</param>
        /// <param name="maxPixelHeight">Scale to image height</param>
        /// <param name="newWidth">New width in pixels</param>
        /// <param name="newHeight">New height in pixel</param>
        /// <param name="imageFormat">Image format, if null then jpeg is used</param>
        /// <param name="compressionLevel">A JPEG quality level of 0 corresponds to the greatest compression, and a quality level of 100 corresponds to the least compression. Defaults to 75</param>
        /// <param name="scaleDownOnly">If true then images smaller than maxPixelWidth & maxPixelHeight are not resized</param>
        /// <returns>The resized image</returns>
        public Stream ResizeImage(Stream sourceImage, bool keepAspectRatio, int maxPixelWidth, int maxPixelHeight, out int newWidth, 
            out int newHeight, ImageFormat imageFormat = null, long compressionLevel = 75L, bool scaleDownOnly = false)
        {
            var orig = new Bitmap(sourceImage);
            int width;
            int height;

            if (keepAspectRatio)
            {
                if (orig.Width > orig.Height)
                {
                    if (scaleDownOnly && orig.Width < maxPixelWidth)
                    {
                        width = orig.Width;
                        height =orig.Height;                            
                    }
                    else
                    {
                        width = maxPixelWidth;
                        height = maxPixelWidth * orig.Height / orig.Width;                        
                    }
                }
                else
                {
                    if (scaleDownOnly && orig.Height < maxPixelHeight) {
                        width = orig.Width;
                        height = orig.Height;
                    } else
                    {
                        height = maxPixelHeight;
                        width = maxPixelHeight*orig.Width/orig.Height;
                    }
                }
            }
            else
            {
                width = maxPixelWidth;
                height = maxPixelHeight;
            }

            newWidth = width;
            newHeight = height;

            var scaledImage = new Bitmap(width, height);

            var format = imageFormat ?? ImageFormat.Jpeg;

            using (var graphic = Graphics.FromImage(scaledImage))
            {
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphic.DrawImage(orig, 0, 0, width, height);

                var stream = new MemoryStream();

                if (format == ImageFormat.Jpeg)
                {
                    // Set JPEG compression quality
                    var compressionQuality = Encoder.Quality;
                    var encoderParameter = new EncoderParameter(compressionQuality, compressionLevel);
                    var myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = encoderParameter;

                    var imageCodecInfo = GetEncoder(format);
                    scaledImage.Save(stream, imageCodecInfo, myEncoderParameters);                    
                }
                else scaledImage.Save(stream, format);

                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }


        /// <summary>
        /// Creates a thumbnail image using the default thumbnail size. Image aspect ratio is respected
        /// </summary>
        /// <param name="sourceImage">Source image</param>
        /// <returns>The thumbnail image</returns>
        public Stream CreateThumbnail(Stream sourceImage)
        {
            int newWidth;
            int newHeight;
            return ResizeImage(sourceImage, true, DefaultThumbnailPixelWidth, DefaultThumbnailPixelHeight, out newWidth, out newHeight);
        }

        public string ChangeToJailImages(string markup) {
            var doc = new HtmlDocument();
            doc.LoadHtml(markup);

            var imageTags = doc.DocumentNode.SelectNodes("//img");
            if (imageTags != null)
                foreach (var img in imageTags)
                {
                    var src = img.Attributes["src"] == null ||
                                      String.IsNullOrWhiteSpace(img.Attributes["src"].Value)
                                          ? String.Empty
                                          : new CdnUtilities().GetCdnUrl(img.Attributes["src"].Value);

                    var cssClasses = img.Attributes["class"] == null ||
                                      String.IsNullOrWhiteSpace(img.Attributes["class"].Value)
                                          ? String.Empty
                                          : img.Attributes["class"].Value;

                    var altText = img.Attributes["alt"] == null ||
                                      String.IsNullOrWhiteSpace(img.Attributes["alt"].Value)
                                          ? "Unspecified"
                                          : img.Attributes["alt"].Value;

                    var width = img.Attributes["width"] == null ||
                                      String.IsNullOrWhiteSpace(img.Attributes["width"].Value)
                                          ? String.Empty
                                          : img.Attributes["width"].Value;

                    var height = img.Attributes["height"] == null ||
                                      String.IsNullOrWhiteSpace(img.Attributes["height"].Value)
                                          ? String.Empty
                                          : img.Attributes["height"].Value;

                    var style = img.Attributes["style"] == null ||
                                      String.IsNullOrWhiteSpace(img.Attributes["style"].Value)
                                          ? String.Empty
                                          : img.Attributes["style"].Value;


                    img.ParentNode.InnerHtml = String.Format(Bll.Resources.Controls.JailImageMarkup5,
                                                                src, width, height, cssClasses, altText, style);
                }

            return doc.DocumentNode.OuterHtml;
        }

    }
}