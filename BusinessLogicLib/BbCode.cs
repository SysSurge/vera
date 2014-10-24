using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Allows for BBCode formatting of input text.
    /// BBCode or Bulletin Board Code is a lightweight markup language used to format posts in many message boards. 
    /// The available tags are indicated by square brackets ([ ]) surrounding a keyword, and they are parsed by the 
    /// message board system before being translated into HTML markup language. BBCodes are safer than letting users
    /// writing HTML elements directly.
    /// </summary>
    /// <example>
    ///     var rawBBCodeText = "[b]Hello world![/b][i]italicized text[/i][url=http://example.com]Example[/url]";
    ///     var processedHtml = new BbCode().Format(rawBBCodeText);
    ///     // Outputs HTML
    ///     Console.Out(processedHtml);
    /// </example>
    public class BbCode
    {
        interface IHtmlFormatter
        {
            string Format(string data);
        }

        protected class RegexFormatter : IHtmlFormatter
        {
            private string _replace;
            private Regex _regex;

            public RegexFormatter(string pattern, string replace)
                : this(pattern, replace, true)
            {
            }

            public RegexFormatter(string pattern, string replace, bool ignoreCase)
            {
                RegexOptions options = RegexOptions.Compiled;

                if (ignoreCase)
                {
                    options |= RegexOptions.IgnoreCase;
                }

                _replace = replace;
                _regex = new Regex(pattern, options);
            }

            public string Format(string data)
            {
                return _regex.Replace(data, _replace);
            }
        }

        protected class SearchReplaceFormatter : IHtmlFormatter
        {
            private readonly string _pattern;
            private readonly string _replace;

            public SearchReplaceFormatter(string pattern, string replace)
            {
                _pattern = pattern;
                _replace = replace;
            }

            public string Format(string data)
            {
                return data.Replace(_pattern, _replace);
            }
        }

        private readonly List<IHtmlFormatter> _formatters;

        public BbCode()
        {
            _formatters = new List<IHtmlFormatter>
                              {
                                  new RegexFormatter(@"<(.|\n)*?>", string.Empty),
                                  new SearchReplaceFormatter("\r", ""),
                                  new SearchReplaceFormatter("\n\n", "</p><p>"),
                                  new SearchReplaceFormatter("\n", "<br />"),
                                  new RegexFormatter(@"\[code(?:\s*)\]((.|\n)*?)\[/code(?:\s*)\]", "<pre>$1</pre>"),
                                  new RegexFormatter(@"\[u(?:\s*)\]((.|\n)*?)\[/u(?:\s*)\]", "<u>$1</u>"),
                                  new RegexFormatter(@"\[b(?:\s*)\]((.|\n)*?)\[/b(?:\s*)\]", "<b>$1</b>"),
                                  new RegexFormatter(@"\[i(?:\s*)\]((.|\n)*?)\[/i(?:\s*)\]", "<i>$1</i>"),
                                  new RegexFormatter(@"\[s(?:\s*)\]((.|\n)*?)\[/s(?:\s*)\]", "<strike>$1</strike>"),
                                  new RegexFormatter(@"\[left(?:\s*)\]((.|\n)*?)\[/left(?:\s*)]",
                                                     "<div style=\"text-align:left\">$1</div>"),
                                  new RegexFormatter(@"\[center(?:\s*)\]((.|\n)*?)\[/center(?:\s*)]",
                                                     "<div style=\"text-align:center\">$1</div>"),
                                  new RegexFormatter(@"\[right(?:\s*)\]((.|\n)*?)\[/right(?:\s*)]",
                                                     "<div style=\"text-align:right\">$1</div>")
                              };

            const string quoteStart = "<blockquote><b>$1 said:</b></p><p>";
            const string quoteEmptyStart = "<blockquote>";
            const string quoteEnd = "</blockquote>";
            _formatters.Add(new RegexFormatter(@"\[quote=((.|\n)*?)(?:\s*)\]", quoteStart));
            _formatters.Add(new RegexFormatter(@"\[quote(?:\s*)\]", quoteEmptyStart));
            _formatters.Add(new RegexFormatter(@"\[/quote(?:\s*)\]", quoteEnd));

            _formatters.Add(new RegexFormatter(@"\[url(?:\s*)\]www\.(.*?)\[/url(?:\s*)\]",
                                               "<a class=\"bbcode-link\" href=\"http://www.$1\" target=\"_blank\" title=\"$1\">$1</a>"));
            _formatters.Add(new RegexFormatter(@"\[url(?:\s*)\]((.|\n)*?)\[/url(?:\s*)\]",
                                               "<a class=\"bbcode-link\" href=\"$1\" target=\"_blank\" title=\"$1\">$1</a>"));
            _formatters.Add(new RegexFormatter(@"\[url=""((.|\n)*?)(?:\s*)""\]((.|\n)*?)\[/url(?:\s*)\]",
                                               "<a class=\"bbcode-link\" href=\"$1\" target=\"_blank\" title=\"$1\">$3</a>"));
            _formatters.Add(new RegexFormatter(@"\[url=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/url(?:\s*)\]",
                                               "<a class=\"bbcode-link\" href=\"$1\" target=\"_blank\" title=\"$1\">$3</a>"));
            _formatters.Add(new RegexFormatter(@"\[link(?:\s*)\]((.|\n)*?)\[/link(?:\s*)\]",
                                               "<a class=\"bbcode-link\" href=\"$1\" target=\"_blank\" title=\"$1\">$1</a>"));
            _formatters.Add(new RegexFormatter(@"\[link=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/link(?:\s*)\]",
                                               "<a class=\"bbcode-link\" href=\"$1\" target=\"_blank\" title=\"$1\">$3</a>"));

            // Allow BBCode images?
            bool allowBBCodeImages;
            if (bool.TryParse(ConfigurationManager.AppSettings["EnableBBCodeImages"], out allowBBCodeImages) && allowBBCodeImages)
            {
                _formatters.Add(new RegexFormatter(@"\[img(?:\s*)\]((.|\n)*?)\[/img(?:\s*)\]",
                                                   "<img src=\"$1\" border=\"0\" alt=\"\" />"));
                _formatters.Add(new RegexFormatter(@"\[img align=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/img(?:\s*)\]",
                                                   "<img src=\"$3\" border=\"0\" align=\"$1\" alt=\"\" />"));
                _formatters.Add(new RegexFormatter(@"\[img=((.|\n)*?)x((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/img(?:\s*)\]",
                                                   "<img width=\"$1\" height=\"$3\" src=\"$5\" border=\"0\" alt=\"\" />"));

                _formatters.Add(new RegexFormatter(@"\[color=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/color(?:\s*)\]",
                                                   "<span style=\"color:$1;\">$3</span>"));

                _formatters.Add(new RegexFormatter(@"\[hr(?:\s*)\]", "<hr />"));


                _formatters.Add(new RegexFormatter(@"\[email(?:\s*)\]((.|\n)*?)\[/email(?:\s*)\]",
                                                   "<a href=\"mailto:$1\">$1</a>"));
            }

            // Allow BBCode embedded fonts?
            bool allowBBCodeFonts;
            if (bool.TryParse(ConfigurationManager.AppSettings["EnableBBCodeFonts"], out allowBBCodeFonts) && allowBBCodeFonts)
                _formatters.Add(new RegexFormatter(@"\[size=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/size(?:\s*)\]",
                                                   "<span style=\"font-size:$1\">$3</span>"));

            _formatters.Add(new RegexFormatter(@"\[font=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/font(?:\s*)\]",
                                               "<span style=\"font-family:$1;\">$3</span>"));
            _formatters.Add(new RegexFormatter(@"\[align=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/align(?:\s*)\]",
                                               "<span style=\"text-align:$1;\">$3</span>"));
            _formatters.Add(new RegexFormatter(@"\[float=((.|\n)*?)(?:\s*)\]((.|\n)*?)\[/float(?:\s*)\]",
                                               "<span style=\"float:$1;\">$3</div>"));

            const string sListFormat = "<ol class=\"bbcode-list\" style=\"list-style:{0};\">$1</ol>";

            _formatters.Add(new RegexFormatter(@"\[\*(?:\s*)]\s*([^\[]*)", "<li>$1</li>"));
            _formatters.Add(new RegexFormatter(@"\[list(?:\s*)\]((.|\n)*?)\[/list(?:\s*)\]",
                                               "<ul class=\"bbcode-list\">$1</ul>"));
            _formatters.Add(new RegexFormatter(@"\[list=1(?:\s*)\]((.|\n)*?)\[/list(?:\s*)\]",
                                               string.Format(sListFormat, "decimal"), false));
            _formatters.Add(new RegexFormatter(@"\[list=i(?:\s*)\]((.|\n)*?)\[/list(?:\s*)\]",
                                               string.Format(sListFormat, "lower-roman"), false));
            _formatters.Add(new RegexFormatter(@"\[list=I(?:\s*)\]((.|\n)*?)\[/list(?:\s*)\]",
                                               string.Format(sListFormat, "upper-roman"), false));
            _formatters.Add(new RegexFormatter(@"\[list=a(?:\s*)\]((.|\n)*?)\[/list(?:\s*)\]",
                                               string.Format(sListFormat, "lower-alpha"), false));
            _formatters.Add(new RegexFormatter(@"\[list=A(?:\s*)\]((.|\n)*?)\[/list(?:\s*)\]",
                                               string.Format(sListFormat, "upper-alpha"), false));
        }

        public string Format(string data)
        {
            return _formatters.Aggregate(data, (current, formatter) => formatter.Format(current));
        }
    }
}