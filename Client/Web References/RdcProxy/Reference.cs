﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.586
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.586.
// 
#pragma warning disable 1591

namespace Client.RdcProxy {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="RdcServiceSoap", Namespace="http://tempuri.org/")]
    public partial class RdcService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetRdcVersionOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetSignatureManifestOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetSimilarityDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback TransferDataBlockOperationCompleted;
        
        private System.Threading.SendOrPostCallback FinializeOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public RdcService() {
            this.Url = global::Client.Properties.Settings.Default.Client_RdcProxy_RdcService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetRdcVersionCompletedEventHandler GetRdcVersionCompleted;
        
        /// <remarks/>
        public event GetSignatureManifestCompletedEventHandler GetSignatureManifestCompleted;
        
        /// <remarks/>
        public event GetSimilarityDataCompletedEventHandler GetSimilarityDataCompleted;
        
        /// <remarks/>
        public event TransferDataBlockCompletedEventHandler TransferDataBlockCompleted;
        
        /// <remarks/>
        public event FinializeCompletedEventHandler FinializeCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetRdcVersion", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public RdcVersion GetRdcVersion() {
            object[] results = this.Invoke("GetRdcVersion", new object[0]);
            return ((RdcVersion)(results[0]));
        }
        
        /// <remarks/>
        public void GetRdcVersionAsync() {
            this.GetRdcVersionAsync(null);
        }
        
        /// <remarks/>
        public void GetRdcVersionAsync(object userState) {
            if ((this.GetRdcVersionOperationCompleted == null)) {
                this.GetRdcVersionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetRdcVersionOperationCompleted);
            }
            this.InvokeAsync("GetRdcVersion", new object[0], this.GetRdcVersionOperationCompleted, userState);
        }
        
        private void OnGetRdcVersionOperationCompleted(object arg) {
            if ((this.GetRdcVersionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetRdcVersionCompleted(this, new GetRdcVersionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSignatureManifest", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public SignatureManifest GetSignatureManifest(string file) {
            object[] results = this.Invoke("GetSignatureManifest", new object[] {
                        file});
            return ((SignatureManifest)(results[0]));
        }
        
        /// <remarks/>
        public void GetSignatureManifestAsync(string file) {
            this.GetSignatureManifestAsync(file, null);
        }
        
        /// <remarks/>
        public void GetSignatureManifestAsync(string file, object userState) {
            if ((this.GetSignatureManifestOperationCompleted == null)) {
                this.GetSignatureManifestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetSignatureManifestOperationCompleted);
            }
            this.InvokeAsync("GetSignatureManifest", new object[] {
                        file}, this.GetSignatureManifestOperationCompleted, userState);
        }
        
        private void OnGetSignatureManifestOperationCompleted(object arg) {
            if ((this.GetSignatureManifestCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetSignatureManifestCompleted(this, new GetSignatureManifestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSimilarityData", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void GetSimilarityData() {
            this.Invoke("GetSimilarityData", new object[0]);
        }
        
        /// <remarks/>
        public void GetSimilarityDataAsync() {
            this.GetSimilarityDataAsync(null);
        }
        
        /// <remarks/>
        public void GetSimilarityDataAsync(object userState) {
            if ((this.GetSimilarityDataOperationCompleted == null)) {
                this.GetSimilarityDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetSimilarityDataOperationCompleted);
            }
            this.InvokeAsync("GetSimilarityData", new object[0], this.GetSimilarityDataOperationCompleted, userState);
        }
        
        private void OnGetSimilarityDataOperationCompleted(object arg) {
            if ((this.GetSimilarityDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetSimilarityDataCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/TransferDataBlock", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] TransferDataBlock(string file, int offset, int length) {
            object[] results = this.Invoke("TransferDataBlock", new object[] {
                        file,
                        offset,
                        length});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void TransferDataBlockAsync(string file, int offset, int length) {
            this.TransferDataBlockAsync(file, offset, length, null);
        }
        
        /// <remarks/>
        public void TransferDataBlockAsync(string file, int offset, int length, object userState) {
            if ((this.TransferDataBlockOperationCompleted == null)) {
                this.TransferDataBlockOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTransferDataBlockOperationCompleted);
            }
            this.InvokeAsync("TransferDataBlock", new object[] {
                        file,
                        offset,
                        length}, this.TransferDataBlockOperationCompleted, userState);
        }
        
        private void OnTransferDataBlockOperationCompleted(object arg) {
            if ((this.TransferDataBlockCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TransferDataBlockCompleted(this, new TransferDataBlockCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Finialize", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Finialize(SignatureManifest manifest) {
            this.Invoke("Finialize", new object[] {
                        manifest});
        }
        
        /// <remarks/>
        public void FinializeAsync(SignatureManifest manifest) {
            this.FinializeAsync(manifest, null);
        }
        
        /// <remarks/>
        public void FinializeAsync(SignatureManifest manifest, object userState) {
            if ((this.FinializeOperationCompleted == null)) {
                this.FinializeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnFinializeOperationCompleted);
            }
            this.InvokeAsync("Finialize", new object[] {
                        manifest}, this.FinializeOperationCompleted, userState);
        }
        
        private void OnFinializeOperationCompleted(object arg) {
            if ((this.FinializeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.FinializeCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class RdcVersion {
        
        private uint currentVersionField;
        
        private uint minimumCompatibleAppVersionField;
        
        /// <remarks/>
        public uint CurrentVersion {
            get {
                return this.currentVersionField;
            }
            set {
                this.currentVersionField = value;
            }
        }
        
        /// <remarks/>
        public uint MinimumCompatibleAppVersion {
            get {
                return this.minimumCompatibleAppVersionField;
            }
            set {
                this.minimumCompatibleAppVersionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class SignatureInfo {
        
        private int indexField;
        
        private string nameField;
        
        private string pathField;
        
        private long lengthField;
        
        /// <remarks/>
        public int Index {
            get {
                return this.indexField;
            }
            set {
                this.indexField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string Path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
        
        /// <remarks/>
        public long Length {
            get {
                return this.lengthField;
            }
            set {
                this.lengthField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class SignatureManifest {
        
        private string fileField;
        
        private SignatureInfo[] signaturesField;
        
        private long fileLengthField;
        
        /// <remarks/>
        public string File {
            get {
                return this.fileField;
            }
            set {
                this.fileField = value;
            }
        }
        
        /// <remarks/>
        public SignatureInfo[] Signatures {
            get {
                return this.signaturesField;
            }
            set {
                this.signaturesField = value;
            }
        }
        
        /// <remarks/>
        public long FileLength {
            get {
                return this.fileLengthField;
            }
            set {
                this.fileLengthField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetRdcVersionCompletedEventHandler(object sender, GetRdcVersionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetRdcVersionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetRdcVersionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public RdcVersion Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RdcVersion)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetSignatureManifestCompletedEventHandler(object sender, GetSignatureManifestCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetSignatureManifestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetSignatureManifestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public SignatureManifest Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((SignatureManifest)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetSimilarityDataCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void TransferDataBlockCompletedEventHandler(object sender, TransferDataBlockCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TransferDataBlockCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal TransferDataBlockCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void FinializeCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591