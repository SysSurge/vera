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
    public class Solution {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Solution() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VeraWAF.WebPages.Bll.Resources.Solution", typeof(Solution).Assembly);
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
        ///   Looks up a localized string similar to Anonymous.
        /// </summary>
        public static string AnonymousUserName {
            get {
                return ResourceManager.GetString("AnonymousUserName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;access-policy&gt;
        ///  &lt;cross-domain-access&gt;
        ///    &lt;policy&gt;
        ///      &lt;allow-from http-methods=&quot;*&quot; http-request-headers=&quot;*&quot;&gt;
        ///        &lt;domain uri=&quot;*&quot; /&gt;
        ///        &lt;domain uri=&quot;http://*&quot; /&gt;
        ///      &lt;/allow-from&gt;
        ///      &lt;grant-to&gt;
        ///        &lt;resource path=&quot;/&quot; include-subpaths=&quot;true&quot; /&gt;
        ///      &lt;/grant-to&gt;
        ///    &lt;/policy&gt;
        ///  &lt;/cross-domain-access&gt;
        ///&lt;/access-policy&gt;.
        /// </summary>
        public static string clientaccesspolicy_xml {
            get {
                return ResourceManager.GetString("clientaccesspolicy_xml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to System.
        /// </summary>
        public static string SystemUserName {
            get {
                return ResourceManager.GetString("SystemUserName", resourceCulture);
            }
        }
    }
}