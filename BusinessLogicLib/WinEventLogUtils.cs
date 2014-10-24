using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraWAF.WebPages.Bll
{
    public class WinEventLogUtils
    {
        /// <summary>
        /// Get Windows Event log item as HTML
        /// </summary>
        /// <param name="logItemType">Log item type</param>
        /// <returns></returns>
        public string GetLogItemTypeHtml(System.Int32 logItemType)
        {
            string literal;
            string color;

            switch (logItemType)
            {
                case 1:
                    literal = Bll.Resources.Template.LogCritical;
                    color = "red";
                    break;
                case 2:
                    literal = Bll.Resources.Template.LogError;
                    color = "red";
                    break;
                case 3:
                    literal = Bll.Resources.Template.LogWarning;
                    color = "orange";
                    break;
                case 4:
                    literal = Bll.Resources.Template.LogInformational;
                    color = "black";
                    break;
                case 5:
                    literal = Bll.Resources.Template.LogVerbose;
                    color = "black";
                    break;
                default:
                    // Custom log level
                    literal = String.Format(Bll.Resources.Template.LogCustom, logItemType);
                    color = "black";
                    break;
            }

            return String.Format("<span class=\"WinLogEntry\" style=\"color:{1}\">{0}</span>", literal, color);
        }

    }
}
