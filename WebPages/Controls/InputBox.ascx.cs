using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Input text control.
    /// </summary>
    public partial class InputBox : UserControl
    {
        /// <summary>
        /// Text box visual modes
        /// </summary>
        public enum ETextBoxMode
        {
            SingleLine,
            MultiLine,
            HtmlEditor
        }

        /// <summary>
        /// Azure table name where the data is located.
        /// Ex. "VeraUsers"
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Entity type. Ex. "UserEntity"
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Partition key that identifies the table row
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Name of Azure table column where the data is stored.
        /// Ex. "Username"
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Text box display mode
        /// </summary>
        public ETextBoxMode TextBoxMode { get; set; }

        /// <summary>
        /// Input quick access key.
        /// </summary>
        public char AccessKey { get; set; }

        /// <summary>
        /// Set to true to enable spellchecking while typing.
        /// Default value is false.
        /// </summary>
        public bool SpellCheck { get; set; }

        /// <summary>
        /// Set to true to enable autocompletion.
        /// Default value is false.
        /// </summary>
        public bool AutoComplete { get; set; }

        /// <summary>
        /// Control width 
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Control height 
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// Help text 
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Control text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Update REST API url
        /// </summary>
        public Uri UpdateUrl { get; set; }

        /// <summary>
        /// Update REST API OAuth signature
        /// </summary>
        public string UpdateOAuthSignature { get; set; }

        /// <summary>
        /// Update REST API OAuth parameters
        /// </summary>
        public string UpdateOAuthParameters { get; set; }
        
        /// <summary>
        /// Class contructor
        /// </summary>
        public InputBox()
        {
            // Disable access key
            AccessKey = Char.MinValue;

            // Disable spellchecking
            SpellCheck = false;

            // Disable autocompletion
            AutoComplete = false;
        }

        /// <summary>
        /// Set the edit field control's width and height
        /// </summary>
        /// <param name="textBox"></param>
        void SetEditControlSize(HtmlGenericControl htmlElement)
        {
            if (!String.IsNullOrEmpty(Width))
            {
                // Set width
                htmlElement.Attributes.CssStyle.Add("width", Width);
            }

            if (!String.IsNullOrEmpty(Height))
            {
                // Set height
                htmlElement.Attributes.CssStyle.Add("height", Height);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TextBox input;

            // Set edit field control size
            SetEditControlSize(editContainer);

            switch (TextBoxMode)
            {
                case InputBox.ETextBoxMode.HtmlEditor:
                    // Html editor
                    plTextOnly.Visible = false;
                    htmlInputField.Visible = true;
                    input = htmlInputField;
                    break;
                case InputBox.ETextBoxMode.MultiLine:
                    // Multiline
                    txtInputField.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                    input = txtInputField;
                    break;
                case InputBox.ETextBoxMode.SingleLine:
                    // Single line
                default:
                    // Unknown, default to single line
                    txtInputField.TextMode = System.Web.UI.WebControls.TextBoxMode.SingleLine;
                    input = txtInputField;
                    break;
            }

            // Add current markup to control
            input.Text = Text;

            // Add help text to a control that is visible when the field is empty
            litHelpText.Text = HelpText;

            // Add the help text as a tooltip on the input field control
            input.ToolTip = HelpText;

            // Quick access key
            if (AccessKey != Char.MinValue)
                input.Attributes.Add("accesskey", AccessKey.ToString());

            // Input autocompletion
            input.Attributes.Add("autocomplete", AutoComplete ? "on" : "off");

            // Spellchecking
            input.Attributes.Add("spellcheck", SpellCheck.ToString());

            // Add some information about how to access the REST API using OAuth
            if (UpdateUrl != null)
            {
                editContainer.Attributes.Add("vera_oauth_url_ns:update", UpdateUrl.ToString());

                // Add the OAuth signature to access the REST API
                editContainer.Attributes.Add("vera_oauth_sig_ns:update", UpdateOAuthSignature);

                // Add the OAuth signature to access the REST API
                editContainer.Attributes.Add("vera_oauth_param_ns:update", UpdateOAuthParameters);
            }

            // Add the Azure table name that the data is stored in
            editContainer.Attributes.Add("vera_table_ns:name", TableName);

            // Add the Azure table name that the data is stored in
            editContainer.Attributes.Add("vera_table_ns:entity", EntityType);

            // Add the Azure table row that identifies the table entity
            editContainer.Attributes.Add("vera_table_ns:partitionkey", PartitionKey);

            // Add the Azure table row property that identifies the table entity
            editContainer.Attributes.Add("vera_table_ns:property", PropertyName);
        }

    }
}