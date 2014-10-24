using System.Text;

namespace VeraWAF.WebPages.Bll
{
    public class TextUtilities
    {
        int GetTextClipStartPosition(int maxNumberOfLettersInText, int textCenterPosition)
        {
            var sub = maxNumberOfLettersInText / 2;
            var clipStartPosition = textCenterPosition > sub ? textCenterPosition - sub : 0;

            return clipStartPosition;
        }

        int GetTextClipEndPosition(string text, int clipStartPosition, int maxNumberOfLettersInText)
        {
            var clipEndPosition = clipStartPosition + maxNumberOfLettersInText;
            return clipEndPosition > text.Length ? text.Length : clipEndPosition;            
        }

        /// <summary>
        /// Clips a text and avoids cliping whole words
        /// </summary>
        /// <param name="text">String to be clipped</param>
        /// <param name="maxNumberOfLettersInText">Maximum number of characters allowed in the clipped text. 
        /// The resulting clipped text does not follow this number literally.</param>
        /// <param name="textCenterPosition">What you consider the center of the string, if -1 then the center will be calculated</param>
        /// <returns>Text clipped where the a word is not clipped</returns>
        public string ClipText(string text, int maxNumberOfLettersInText, int textCenterPosition = -1)
        {
            var clippedText = new StringBuilder();

            if (text.Length > maxNumberOfLettersInText)
            {
                var endWordClipped = false;

                if (textCenterPosition == -1) textCenterPosition = text.Length / 2;

                var clipStartPosition = GetTextClipStartPosition(maxNumberOfLettersInText, textCenterPosition);

                var endOfFirstWord = text.IndexOf(' ', clipStartPosition);
                if (endOfFirstWord == -1) {
                    endOfFirstWord = clipStartPosition;
                    clippedText.Append("...");
                }

                var clipEndPosition = GetTextClipEndPosition(text, clipStartPosition, maxNumberOfLettersInText);

                var endOfLastWord = text.IndexOf(' ', clipEndPosition);
                if (endOfLastWord == -1)
                {
                    endOfLastWord = clipEndPosition;
                    endWordClipped = true;
                }

                var length = endOfLastWord - endOfFirstWord;

                clippedText.AppendFormat(text.Substring(endOfFirstWord, length));

                if (endWordClipped) clippedText.Append("...");

            } else clippedText.Append(text);

            return clippedText.ToString();

        }


        /// <summary>
        /// Clips a text
        /// </summary>
        /// <param name="text">String to be clipped</param>
        /// <param name="maxNumberOfLettersInText">Maximum number of characters allowed in the clipped text. 
        /// The resulting clipped text does not follow this number literally.</param>
        /// <returns>Text clipped</returns>
        public string ClipLeft(string text, int maxNumberOfLettersInText)
        {
            if (text != null && text.Length > maxNumberOfLettersInText && maxNumberOfLettersInText > 3)
                return string.Format("{0}...", text.Substring(0, maxNumberOfLettersInText - 3));
            return text;
        }
    }
}
