﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VeraWAF.WebPages.Bll.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Email {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Email() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VeraWAF.WebPages.Bll.Resources.Email", typeof(Email).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello, {0}.&lt;br /&gt;&lt;br /&gt;This e-mail was sent to confirm that your {1} account has been activated. &lt;a href=&quot;http://{1}/Account/Login.aspx&quot;&gt;Click here&lt;/a&gt; to sign in for the first time..
        /// </summary>
        public static string ActivatedBody {
            get {
                return ResourceManager.GetString("ActivatedBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account has been activated.
        /// </summary>
        public static string ActivatedHeader {
            get {
                return ResourceManager.GetString("ActivatedHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because someone activated an account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; that is registered to your e-mail address..
        /// </summary>
        public static string ActivatedReason {
            get {
                return ResourceManager.GetString("ActivatedReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account has been activated.
        /// </summary>
        public static string ActivatedSubject {
            get {
                return ResourceManager.GetString("ActivatedSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello, {0}.&lt;br /&gt;Your new password is: &lt;span class=&quot;password&quot;&gt;{1}&lt;/span&gt;.
        /// </summary>
        public static string ChangePasswordBody {
            get {
                return ResourceManager.GetString("ChangePasswordBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account password has been changed.
        /// </summary>
        public static string ChangePasswordHeader {
            get {
                return ResourceManager.GetString("ChangePasswordHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because someone changed a password to an account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; registered to your e-mail address..
        /// </summary>
        public static string ChangePasswordReason {
            get {
                return ResourceManager.GetString("ChangePasswordReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account password has been changed.
        /// </summary>
        public static string ChangePasswordSubject {
            get {
                return ResourceManager.GetString("ChangePasswordSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;/font&gt;
        ///&lt;/td&gt;
        ///&lt;/tr&gt;
        ///&lt;/table&gt;
        ///&lt;p&gt;
        ///&lt;img alt=&quot;Eco&quot; align=&quot;absmiddle&quot; width=&quot;27&quot; height=&quot;27&quot; src=cid:recycle /&gt;
        ///&lt;font style=&quot;font-family: Verdana, sans-serif; font-size: 12px; color: #00aa00; font-weight: bold&quot;&gt;
        ///&amp;nbsp;Please consider the environment before printing this email. 
        ///&lt;/font&gt;
        ///&lt;/p&gt;
        ///&lt;font style=&quot;font-family: Verdana, sans-serif; font-size: 12px; color: #007da6; font-weight: bold&quot;&gt;
        ///Thank you,&lt;br /&gt;The Example Corp Team.
        ///&lt;/font&gt;
        ///&lt;br /&gt;
        ///&lt;br /&gt;
        ///&lt;/td&gt;
        ///&lt;/tr&gt;
        ///&lt;/table&gt;
        ///&lt;table width=&quot;624&quot; cellp [rest of string was truncated]&quot;;.
        /// </summary>
        public static string EmailFooter {
            get {
                return ResourceManager.GetString("EmailFooter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///&lt;head&gt;	
        ///&lt;title&gt;{0}&lt;/title&gt;
        ///&lt;/head&gt;
        ///&lt;body bgcolor=&quot;#ffffff&quot;&gt;
        ///&lt;table width=&quot;624&quot; height=&quot;36&quot; cellpadding=&quot;0&quot; cellspacing=&quot;0&quot; border=&quot;0&quot; align=&quot;center&quot; background=cid:bgheader&gt;
        ///&lt;tr&gt;
        ///&lt;td&gt;
        ///&lt;/td&gt;	
        ///&lt;/tr&gt;
        ///&lt;/table&gt;
        ///&lt;table width=&quot;624&quot; cellpadding=&quot;0&quot; cellspacing=&quot;0&quot; border=&quot;0&quot; align=&quot;center&quot; background=cid:bgcontent&gt;
        ///&lt;tr&gt;
        ///&lt;td&gt;
        ///&lt;table width=&quot;580&quot; cellpadding=&quot;10&quot; cellspacing=&quot;0&quot; border=&quot;0&quot; align=&quot;center&quot;&gt;	
        ///&lt;tr bgcolor=&quot;#ffffff&quot;&gt;		
        ///&lt;td width=&quot;380&quot;&gt;&lt;a href=&quot;http://www.example.com/&quot;&gt;Example Corp.&lt;/ [rest of string was truncated]&quot;;.
        /// </summary>
        public static string EmailHeader {
            get {
                return ResourceManager.GetString("EmailHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message from &lt;b&gt;{0}&lt;/b&gt;:&lt;br /&gt;&lt;br /&gt;{1}&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string InternalMessageBody1 {
            get {
                return ResourceManager.GetString("InternalMessageBody1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;a href=&apos;{0}&apos;&gt;Click here to see {1}&apos;s profile.&lt;/a&gt;&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string InternalMessageBody2 {
            get {
                return ResourceManager.GetString("InternalMessageBody2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} sent you a message via {1}..
        /// </summary>
        public static string InternalMessageHeader {
            get {
                return ResourceManager.GetString("InternalMessageHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because a user at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; using sent you a message via the web site. Your e-mail address &lt;em&gt;has not&lt;/em&gt; been disclosed..
        /// </summary>
        public static string InternalMessageReason {
            get {
                return ResourceManager.GetString("InternalMessageReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have recieved a new message via {0}.
        /// </summary>
        public static string InternalMessageSubject {
            get {
                return ResourceManager.GetString("InternalMessageSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello, {0}.&lt;br /&gt;Someone, possibly you, have {1} bad sign in attempts to your {2} account. Because of this your account was automatically locked out to prevent a possible account password hacking attempt..
        /// </summary>
        public static string LockedOutBody1 {
            get {
                return ResourceManager.GetString("LockedOutBody1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You will not be able to sign into {0} until your account has been unlocked. To unlock your {0} account again please click the link below:&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string LockedOutBody2 {
            get {
                return ResourceManager.GetString("LockedOutBody2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;a href=&apos;{0}&apos;&gt;{0}&lt;/a&gt;&lt;br /&gt;&lt;br /&gt;If your e-mail client does not allow links then copy &amp; paste the above url into your browser address bar..
        /// </summary>
        public static string LockedOutBody3 {
            get {
                return ResourceManager.GetString("LockedOutBody3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account has been locked out!.
        /// </summary>
        public static string LockedOutHeader {
            get {
                return ResourceManager.GetString("LockedOutHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent your user account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; was locked out because of too many bad sign attempt..
        /// </summary>
        public static string LockedOutReason {
            get {
                return ResourceManager.GetString("LockedOutReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unlock your {0} account.
        /// </summary>
        public static string LockedOutSubject {
            get {
                return ResourceManager.GetString("LockedOutSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A user has submitted the marketing research form at {0}.&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string MarketingResearchBody1 {
            get {
                return ResourceManager.GetString("MarketingResearchBody1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Company size: {0}.
        /// </summary>
        public static string MarketingResearchBody10 {
            get {
                return ResourceManager.GetString("MarketingResearchBody10", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Server OS: {0}.
        /// </summary>
        public static string MarketingResearchBody11 {
            get {
                return ResourceManager.GetString("MarketingResearchBody11", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Client OS: {0}.
        /// </summary>
        public static string MarketingResearchBody12 {
            get {
                return ResourceManager.GetString("MarketingResearchBody12", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;User name: {0}.
        /// </summary>
        public static string MarketingResearchBody2 {
            get {
                return ResourceManager.GetString("MarketingResearchBody2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Full name: {0}.
        /// </summary>
        public static string MarketingResearchBody3 {
            get {
                return ResourceManager.GetString("MarketingResearchBody3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;E-mail: &lt;a href=&quot;mailto:{0}&quot;&gt;{0}&lt;/a&gt;.
        /// </summary>
        public static string MarketingResearchBody4 {
            get {
                return ResourceManager.GetString("MarketingResearchBody4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;&lt;br /&gt;Filled date: {0}.
        /// </summary>
        public static string MarketingResearchBody5 {
            get {
                return ResourceManager.GetString("MarketingResearchBody5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;User IP address: {0}.
        /// </summary>
        public static string MarketingResearchBody6 {
            get {
                return ResourceManager.GetString("MarketingResearchBody6", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;&lt;br /&gt;Country: {0}.
        /// </summary>
        public static string MarketingResearchBody7 {
            get {
                return ResourceManager.GetString("MarketingResearchBody7", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Industry: {0}.
        /// </summary>
        public static string MarketingResearchBody8 {
            get {
                return ResourceManager.GetString("MarketingResearchBody8", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Job category: {0}.
        /// </summary>
        public static string MarketingResearchBody9 {
            get {
                return ResourceManager.GetString("MarketingResearchBody9", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Marketing form submitted.
        /// </summary>
        public static string MarketingResearchHeader {
            get {
                return ResourceManager.GetString("MarketingResearchHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent to inform you that someone submitted the marketing research form at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt;..
        /// </summary>
        public static string MarketingResearchReason {
            get {
                return ResourceManager.GetString("MarketingResearchReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} just submitted the marketing research form at {1}.
        /// </summary>
        public static string MarketingResearchSubject {
            get {
                return ResourceManager.GetString("MarketingResearchSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello, {0}.&lt;br /&gt;Please click the below link to activate your {1} account and complete your account registration:&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string NewAccountBody1 {
            get {
                return ResourceManager.GetString("NewAccountBody1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;a href=&apos;{0}&apos;&gt;{0}&lt;/a&gt;&lt;br /&gt;&lt;br /&gt;If your e-mail client does not allow links then copy &amp; paste the above url into your browser address bar..
        /// </summary>
        public static string NewAccountBody2 {
            get {
                return ResourceManager.GetString("NewAccountBody2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thank you for creating a {0} account!.
        /// </summary>
        public static string NewAccountHeader {
            get {
                return ResourceManager.GetString("NewAccountHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because someone registered an user account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; using this e-mail address..
        /// </summary>
        public static string NewAccountReason {
            get {
                return ResourceManager.GetString("NewAccountReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Activate your {0} account.
        /// </summary>
        public static string NewAccountSubject {
            get {
                return ResourceManager.GetString("NewAccountSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must specify a local virtual path, ex. &quot;/news/merger2014.aspx&quot;..
        /// </summary>
        public static string NewsletterBadPath {
            get {
                return ResourceManager.GetString("NewsletterBadPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;small&gt;&lt;a href=&quot;http://{1}{0}&quot;&gt;Click here if you have trouble reading the newsletter.&lt;/a&gt;&lt;/small&gt;&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string NewsletterBody {
            get {
                return ResourceManager.GetString("NewsletterBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No e-mail subscribers..
        /// </summary>
        public static string NewsletterNoSubscribers {
            get {
                return ResourceManager.GetString("NewsletterNoSubscribers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Page not found..
        /// </summary>
        public static string NewsletterPageNotFound {
            get {
                return ResourceManager.GetString("NewsletterPageNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because you are currently subscribing to newsletters from &lt;a href=&quot;http://{0}&quot;&gt;www.example.com&lt;/a&gt;, if you do not want to recieve any more newsletters then please alter your preferences at &lt;a href=&quot;http://{0}/Account/&quot;&gt;{0}/Account/&lt;/a&gt;..
        /// </summary>
        public static string NewsletterReason {
            get {
                return ResourceManager.GetString("NewsletterReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The newsletter has been successfully sent..
        /// </summary>
        public static string NewsletterSuccess1 {
            get {
                return ResourceManager.GetString("NewsletterSuccess1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please note that the e-mail worker processes the e-mail from an e-mail queue to prevent e-mail server hammering; some subscribers may not get their e-mail immediately if large lists are being processed..
        /// </summary>
        public static string NewsletterSuccess2 {
            get {
                return ResourceManager.GetString("NewsletterSuccess2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A new user has just registered at {0}.&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string NewUserNotifyBody1 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;User name: {0}.
        /// </summary>
        public static string NewUserNotifyBody2 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Full name: {0}.
        /// </summary>
        public static string NewUserNotifyBody3 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;E-mail: &lt;a href=&quot;mailto:{0}&quot;&gt;{0}&lt;/a&gt;.
        /// </summary>
        public static string NewUserNotifyBody4 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;&lt;br /&gt;Registration date: {0} UTC.
        /// </summary>
        public static string NewUserNotifyBody5 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;User IP address: {0}.
        /// </summary>
        public static string NewUserNotifyBody6 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody6", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Join beta: {0}.
        /// </summary>
        public static string NewUserNotifyBody7 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody7", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;Get newsletter: {0}.
        /// </summary>
        public static string NewUserNotifyBody8 {
            get {
                return ResourceManager.GetString("NewUserNotifyBody8", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New user registered!.
        /// </summary>
        public static string NewUserNotifyHeader {
            get {
                return ResourceManager.GetString("NewUserNotifyHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent to inform you that someone registered a new user account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt;..
        /// </summary>
        public static string NewUserNotifyReason {
            get {
                return ResourceManager.GetString("NewUserNotifyReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} just registered at {1}!.
        /// </summary>
        public static string NewUserNotifySubject {
            get {
                return ResourceManager.GetString("NewUserNotifySubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please click the below link to activate your {0} account and complete your account registration:&lt;br /&gt;&lt;br /&gt;.
        /// </summary>
        public static string RegisterBody1 {
            get {
                return ResourceManager.GetString("RegisterBody1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;a href=&apos;{0}&apos;&gt;{0}&lt;/a&gt;&lt;br /&gt;&lt;br /&gt;If your e-mail client does not allow links then copy &amp; paste the above url into your browser address bar..
        /// </summary>
        public static string RegisterBody2 {
            get {
                return ResourceManager.GetString("RegisterBody2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thank you for creating a {0} account!.
        /// </summary>
        public static string RegisterHeader {
            get {
                return ResourceManager.GetString("RegisterHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;br /&gt;&lt;br /&gt;User name: &lt;em&gt;{0}&lt;/em&gt;&lt;br /&gt;Password: &lt;em&gt;{1}&lt;/em&gt;.
        /// </summary>
        public static string RegisterHeader3rdPartyAuthentication {
            get {
                return ResourceManager.GetString("RegisterHeader3rdPartyAuthentication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because someone registered an user account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; using this e-mail address..
        /// </summary>
        public static string RegisterReason {
            get {
                return ResourceManager.GetString("RegisterReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Activate your {0} account.
        /// </summary>
        public static string RegisterSubject {
            get {
                return ResourceManager.GetString("RegisterSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello, {0}.&lt;br /&gt;Your new password is: &lt;span class=\&quot;password\&quot;&gt;{1}&lt;/span&gt;.
        /// </summary>
        public static string ResetPasswordBody {
            get {
                return ResourceManager.GetString("ResetPasswordBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your new {0} account password.
        /// </summary>
        public static string ResetPasswordHeader {
            get {
                return ResourceManager.GetString("ResetPasswordHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because someone reset a password to an account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; that is registered to your e-mail address..
        /// </summary>
        public static string ResetPasswordReason {
            get {
                return ResourceManager.GetString("ResetPasswordReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account password has been reset.
        /// </summary>
        public static string ResetPasswordSubject {
            get {
                return ResourceManager.GetString("ResetPasswordSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello, {0}.&lt;br /&gt;&lt;br /&gt;Your {1} account is now unlocked, &lt;a href=&quot;http://{1}/Account/Login.aspx&quot;&gt;click here&lt;/a&gt; to sign in..
        /// </summary>
        public static string UnlockAccountBody {
            get {
                return ResourceManager.GetString("UnlockAccountBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account has been unlocked.
        /// </summary>
        public static string UnlockAccountHeader {
            get {
                return ResourceManager.GetString("UnlockAccountHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This e-mail was sent because someone unlocked an account at &lt;a href=&quot;http://{0}&quot;&gt;{0}&lt;/a&gt; that is registered to your e-mail address..
        /// </summary>
        public static string UnlockAccountReason {
            get {
                return ResourceManager.GetString("UnlockAccountReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your {0} account has been unlocked.
        /// </summary>
        public static string UnlockAccountSubject {
            get {
                return ResourceManager.GetString("UnlockAccountSubject", resourceCulture);
            }
        }
    }
}