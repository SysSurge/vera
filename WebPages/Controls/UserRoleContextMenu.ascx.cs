using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VeraWAF.WebPages.Controls
{
    public partial class UserRoleContextMenu : System.Web.UI.UserControl
    {
        /// <summary>
        /// Make the edit page buttons reflect the current edit mode state
        /// </summary>
        void HandlePageEditButtons()
        {
            // Get the requested edit mode
            var editModeArg = Request["Edit"];
            int editMode;
            var editModeEnabled = !String.IsNullOrEmpty(editModeArg) && int.TryParse(editModeArg, out editMode) && editMode > 0;

            // Set the admin role edit page controls
            var phAdminStartEdit = HeadLoginView.FindControl("phAdminStartEdit");
            if (phAdminStartEdit != null)
                phAdminStartEdit.Visible = !editModeEnabled;
            var phAdminStopEdit = HeadLoginView.FindControl("phAdminStopEdit");
            if (phAdminStopEdit != null)
                phAdminStopEdit.Visible = editModeEnabled;

            // Set the editor role edit page controls
            var phEditorStartEdit = HeadLoginView.FindControl("phEditorStartEdit");
            if (phEditorStartEdit != null)
                phEditorStartEdit.Visible = !editModeEnabled;
            var phEditorStopEdit = HeadLoginView.FindControl("phEditorStopEdit");
            if (phEditorStopEdit != null)
                phEditorStopEdit.Visible = editModeEnabled;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HandlePageEditButtons();
        }
    }
}