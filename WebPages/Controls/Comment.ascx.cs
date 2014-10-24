using System;
using System.Globalization;
using System.Text;
using System.Web.UI;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls {
    public partial class Comment : UserControl {
        public DateTime Timestamp { get; set; }
        public object ProviderUserKey { get; set; }
        public string UserDisplayName { get; set; }
        public string VirtualPath { get; set; }
        public string PortraitUrl { get; set; }
        public string MainContent { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public bool AllowVoting { get; set; }
        public bool ShowSignature { get; set; }
        public string Signature { get; set; }
        public int UserSocialScore { get; set; }
        public DateTime UserJoinedDate { get; set; }
        public DateTime UserLastActiveDate { get; set; }
        public bool ShowUserMetaData { get; set; }
        public string[] UserRoles { get; set; }

        void InitMainContentControl()
        {
            lnkCommentLink.HRef = String.Format("{0}#{1}", VirtualPath, RowKey);
            lnkAnchor.Name = RowKey;

            litMainContent.Text = MainContent;            
        }

        void InitPortraitControl()
        {
            jailUserPortrait.alt = String.Format("Portrait of {0}", UserDisplayName);
            jailUserPortrait.src = PortraitUrl;
            figCaption.Text = UserDisplayName;            
        }

        void InitUserLinkControl()
        {
            lnkUser.HRef = lnkUserPortrait.HRef = String.Format("/Account/?id={0}", ProviderUserKey);
            lnkUser.InnerText = UserDisplayName;            
        }

        void InitPubDateControl()
        {
            var dateUtilities = new DateUtilities();
            timePubDate.Attributes["datetime"] = dateUtilities.GetCustomIso8601Date(Timestamp);
            timePubDate.InnerText = dateUtilities.GetReadableDateAndTime(Timestamp);            
        }

        void InitVoteControl()
        {
            vote.UserGettingVote = ProviderUserKey;
            vote.VoteItemId = PartitionKey + RowKey;
            vote.RedirectUrl = String.Format("{0}#{1}", VirtualPath, RowKey);
            vote.Visible = AllowVoting;            
        }

        void InitSignatureControl()
        {
            if (ShowSignature && !String.IsNullOrWhiteSpace(Signature))
                signature.Text = Signature;
            else divSignature.Visible = false;
        }

        void InitScoreControl()
        {
            score.InnerText = UserSocialScore.ToString(CultureInfo.InvariantCulture);
        }
        
        void InitUserJoinedControl() {
            var dateUtilities = new DateUtilities();
            joinedDate.Attributes["datetime"] = dateUtilities.GetCustomIso8601Date(UserJoinedDate);
            joinedDate.InnerText = dateUtilities.GetReadableDate(UserJoinedDate);
        }

        void InitUserLastActiveControl() {
            var dateUtilities = new DateUtilities();
            lastActiveDate.Attributes["datetime"] = dateUtilities.GetCustomIso8601Date(UserLastActiveDate);
            lastActiveDate.InnerText = dateUtilities.GetReadableDate(UserLastActiveDate);
        }

        void InitUserRolesControls()
        {
            if (UserRoles.Length > 0)
            {

                var userRolesText = new StringBuilder("Roles: ");
                var isFirst = true;

                foreach (var role in UserRoles)
                {
                    if (isFirst)
                        userRolesText.Append(role);
                    else
                        userRolesText.AppendFormat(", {0}", role);

                    isFirst = false;
                }

                userRolesText.Append(".");

                userRoles.InnerText = userRolesText.ToString();
            }
            else
                userRoles.Visible = false;
        }

        void InitControls()
        {
            InitUserLinkControl();
            InitPortraitControl();
            InitPubDateControl();
            InitMainContentControl();
            InitSignatureControl();
            InitScoreControl();
            InitVoteControl();
            InitUserJoinedControl();
            InitUserLastActiveControl();
            InitUserRolesControls();

            panUserMetaData.Visible = ShowUserMetaData;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();
        }
    }
}