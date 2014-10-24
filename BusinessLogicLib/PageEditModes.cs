using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Edit page modes
    /// </summary>
    public enum EPageEditMode
    {
        /// <summary>
        /// Not in edit mode
        /// </summary>
        None = 0,

        /// <summary>
        /// Show the edit field control's own save button.
        /// Field data is stored using the field control's own internal logic.
        /// </summary>
        ShowSaveButton = 1,

        /// <summary>
        /// Hide the edit field control's own save button.
        /// Field data must be stored by some external logic.
        /// </summary>
        HideSaveButton = 2
    }
}
