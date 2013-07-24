﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4961
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.4961.
// 
#pragma warning disable 1591

namespace TrimFuel.Business.MBApi {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="MBAPISoap", Namespace="http://api.moldingbox.com/")]
    public partial class MBAPI : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback Post_ShipmentOperationCompleted;
        
        private System.Threading.SendOrPostCallback Retrieve_Shipment_StatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback Retrieve_Shipping_MethodsOperationCompleted;
        
        private System.Threading.SendOrPostCallback Retrieve_Shipment_Status_TypesOperationCompleted;
        
        private System.Threading.SendOrPostCallback Cancel_ShipmentOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public MBAPI() {
            this.Url = global::TrimFuel.Business.Properties.Settings.Default.TrimFuel_Business_MBApi_MBAPI;
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
        public event Post_ShipmentCompletedEventHandler Post_ShipmentCompleted;
        
        /// <remarks/>
        public event Retrieve_Shipment_StatusCompletedEventHandler Retrieve_Shipment_StatusCompleted;
        
        /// <remarks/>
        public event Retrieve_Shipping_MethodsCompletedEventHandler Retrieve_Shipping_MethodsCompleted;
        
        /// <remarks/>
        public event Retrieve_Shipment_Status_TypesCompletedEventHandler Retrieve_Shipment_Status_TypesCompleted;
        
        /// <remarks/>
        public event Cancel_ShipmentCompletedEventHandler Cancel_ShipmentCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://api.moldingbox.com/Post_Shipment", RequestNamespace="http://api.moldingbox.com/", ResponseNamespace="http://api.moldingbox.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Response[] Post_Shipment(string ApiKey, Shipment[] Shipments) {
            object[] results = this.Invoke("Post_Shipment", new object[] {
                        ApiKey,
                        Shipments});
            return ((Response[])(results[0]));
        }
        
        /// <remarks/>
        public void Post_ShipmentAsync(string ApiKey, Shipment[] Shipments) {
            this.Post_ShipmentAsync(ApiKey, Shipments, null);
        }
        
        /// <remarks/>
        public void Post_ShipmentAsync(string ApiKey, Shipment[] Shipments, object userState) {
            if ((this.Post_ShipmentOperationCompleted == null)) {
                this.Post_ShipmentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPost_ShipmentOperationCompleted);
            }
            this.InvokeAsync("Post_Shipment", new object[] {
                        ApiKey,
                        Shipments}, this.Post_ShipmentOperationCompleted, userState);
        }
        
        private void OnPost_ShipmentOperationCompleted(object arg) {
            if ((this.Post_ShipmentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Post_ShipmentCompleted(this, new Post_ShipmentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://api.moldingbox.com/Retrieve_Shipment_Status", RequestNamespace="http://api.moldingbox.com/", ResponseNamespace="http://api.moldingbox.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public StatusResponse[] Retrieve_Shipment_Status(string ApiKey, int[] MBShipmentIDs) {
            object[] results = this.Invoke("Retrieve_Shipment_Status", new object[] {
                        ApiKey,
                        MBShipmentIDs});
            return ((StatusResponse[])(results[0]));
        }
        
        /// <remarks/>
        public void Retrieve_Shipment_StatusAsync(string ApiKey, int[] MBShipmentIDs) {
            this.Retrieve_Shipment_StatusAsync(ApiKey, MBShipmentIDs, null);
        }
        
        /// <remarks/>
        public void Retrieve_Shipment_StatusAsync(string ApiKey, int[] MBShipmentIDs, object userState) {
            if ((this.Retrieve_Shipment_StatusOperationCompleted == null)) {
                this.Retrieve_Shipment_StatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRetrieve_Shipment_StatusOperationCompleted);
            }
            this.InvokeAsync("Retrieve_Shipment_Status", new object[] {
                        ApiKey,
                        MBShipmentIDs}, this.Retrieve_Shipment_StatusOperationCompleted, userState);
        }
        
        private void OnRetrieve_Shipment_StatusOperationCompleted(object arg) {
            if ((this.Retrieve_Shipment_StatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Retrieve_Shipment_StatusCompleted(this, new Retrieve_Shipment_StatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://api.moldingbox.com/Retrieve_Shipping_Methods", RequestNamespace="http://api.moldingbox.com/", ResponseNamespace="http://api.moldingbox.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ShippingMethod[] Retrieve_Shipping_Methods() {
            object[] results = this.Invoke("Retrieve_Shipping_Methods", new object[0]);
            return ((ShippingMethod[])(results[0]));
        }
        
        /// <remarks/>
        public void Retrieve_Shipping_MethodsAsync() {
            this.Retrieve_Shipping_MethodsAsync(null);
        }
        
        /// <remarks/>
        public void Retrieve_Shipping_MethodsAsync(object userState) {
            if ((this.Retrieve_Shipping_MethodsOperationCompleted == null)) {
                this.Retrieve_Shipping_MethodsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRetrieve_Shipping_MethodsOperationCompleted);
            }
            this.InvokeAsync("Retrieve_Shipping_Methods", new object[0], this.Retrieve_Shipping_MethodsOperationCompleted, userState);
        }
        
        private void OnRetrieve_Shipping_MethodsOperationCompleted(object arg) {
            if ((this.Retrieve_Shipping_MethodsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Retrieve_Shipping_MethodsCompleted(this, new Retrieve_Shipping_MethodsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://api.moldingbox.com/Retrieve_Shipment_Status_Types", RequestNamespace="http://api.moldingbox.com/", ResponseNamespace="http://api.moldingbox.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Status[] Retrieve_Shipment_Status_Types() {
            object[] results = this.Invoke("Retrieve_Shipment_Status_Types", new object[0]);
            return ((Status[])(results[0]));
        }
        
        /// <remarks/>
        public void Retrieve_Shipment_Status_TypesAsync() {
            this.Retrieve_Shipment_Status_TypesAsync(null);
        }
        
        /// <remarks/>
        public void Retrieve_Shipment_Status_TypesAsync(object userState) {
            if ((this.Retrieve_Shipment_Status_TypesOperationCompleted == null)) {
                this.Retrieve_Shipment_Status_TypesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRetrieve_Shipment_Status_TypesOperationCompleted);
            }
            this.InvokeAsync("Retrieve_Shipment_Status_Types", new object[0], this.Retrieve_Shipment_Status_TypesOperationCompleted, userState);
        }
        
        private void OnRetrieve_Shipment_Status_TypesOperationCompleted(object arg) {
            if ((this.Retrieve_Shipment_Status_TypesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Retrieve_Shipment_Status_TypesCompleted(this, new Retrieve_Shipment_Status_TypesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://api.moldingbox.com/Cancel_Shipment", RequestNamespace="http://api.moldingbox.com/", ResponseNamespace="http://api.moldingbox.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public StatusResponse[] Cancel_Shipment(string ApiKey, int[] MBShipmentIDs) {
            object[] results = this.Invoke("Cancel_Shipment", new object[] {
                        ApiKey,
                        MBShipmentIDs});
            return ((StatusResponse[])(results[0]));
        }
        
        /// <remarks/>
        public void Cancel_ShipmentAsync(string ApiKey, int[] MBShipmentIDs) {
            this.Cancel_ShipmentAsync(ApiKey, MBShipmentIDs, null);
        }
        
        /// <remarks/>
        public void Cancel_ShipmentAsync(string ApiKey, int[] MBShipmentIDs, object userState) {
            if ((this.Cancel_ShipmentOperationCompleted == null)) {
                this.Cancel_ShipmentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancel_ShipmentOperationCompleted);
            }
            this.InvokeAsync("Cancel_Shipment", new object[] {
                        ApiKey,
                        MBShipmentIDs}, this.Cancel_ShipmentOperationCompleted, userState);
        }
        
        private void OnCancel_ShipmentOperationCompleted(object arg) {
            if ((this.Cancel_ShipmentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Cancel_ShipmentCompleted(this, new Cancel_ShipmentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.moldingbox.com/")]
    public partial class Shipment {
        
        private string orderIDField;
        
        private System.DateTime orderdateField;
        
        private string companyField;
        
        private string firstNameField;
        
        private string lastNameField;
        
        private string address1Field;
        
        private string address2Field;
        
        private string cityField;
        
        private string stateField;
        
        private string zipField;
        
        private string countryField;
        
        private string emailField;
        
        private string phoneField;
        
        private int shippingMethodIDField;
        
        private double cODAmountField;
        
        private string custom1Field;
        
        private string custom2Field;
        
        private string custom3Field;
        
        private string custom4Field;
        
        private string custom5Field;
        
        private string custom6Field;
        
        private Item[] itemsField;
        
        /// <remarks/>
        public string OrderID {
            get {
                return this.orderIDField;
            }
            set {
                this.orderIDField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime Orderdate {
            get {
                return this.orderdateField;
            }
            set {
                this.orderdateField = value;
            }
        }
        
        /// <remarks/>
        public string Company {
            get {
                return this.companyField;
            }
            set {
                this.companyField = value;
            }
        }
        
        /// <remarks/>
        public string FirstName {
            get {
                return this.firstNameField;
            }
            set {
                this.firstNameField = value;
            }
        }
        
        /// <remarks/>
        public string LastName {
            get {
                return this.lastNameField;
            }
            set {
                this.lastNameField = value;
            }
        }
        
        /// <remarks/>
        public string Address1 {
            get {
                return this.address1Field;
            }
            set {
                this.address1Field = value;
            }
        }
        
        /// <remarks/>
        public string Address2 {
            get {
                return this.address2Field;
            }
            set {
                this.address2Field = value;
            }
        }
        
        /// <remarks/>
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
            }
        }
        
        /// <remarks/>
        public string State {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
            }
        }
        
        /// <remarks/>
        public string Zip {
            get {
                return this.zipField;
            }
            set {
                this.zipField = value;
            }
        }
        
        /// <remarks/>
        public string Country {
            get {
                return this.countryField;
            }
            set {
                this.countryField = value;
            }
        }
        
        /// <remarks/>
        public string Email {
            get {
                return this.emailField;
            }
            set {
                this.emailField = value;
            }
        }
        
        /// <remarks/>
        public string Phone {
            get {
                return this.phoneField;
            }
            set {
                this.phoneField = value;
            }
        }
        
        /// <remarks/>
        public int ShippingMethodID {
            get {
                return this.shippingMethodIDField;
            }
            set {
                this.shippingMethodIDField = value;
            }
        }
        
        /// <remarks/>
        public double CODAmount {
            get {
                return this.cODAmountField;
            }
            set {
                this.cODAmountField = value;
            }
        }
        
        /// <remarks/>
        public string Custom1 {
            get {
                return this.custom1Field;
            }
            set {
                this.custom1Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom2 {
            get {
                return this.custom2Field;
            }
            set {
                this.custom2Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom3 {
            get {
                return this.custom3Field;
            }
            set {
                this.custom3Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom4 {
            get {
                return this.custom4Field;
            }
            set {
                this.custom4Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom5 {
            get {
                return this.custom5Field;
            }
            set {
                this.custom5Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom6 {
            get {
                return this.custom6Field;
            }
            set {
                this.custom6Field = value;
            }
        }
        
        /// <remarks/>
        public Item[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.moldingbox.com/")]
    public partial class Item {
        
        private string sKUField;
        
        private string descriptionField;
        
        private int quantityField;
        
        private string custom1Field;
        
        private string custom2Field;
        
        private string custom3Field;
        
        private string custom4Field;
        
        private string custom5Field;
        
        private string custom6Field;
        
        /// <remarks/>
        public string SKU {
            get {
                return this.sKUField;
            }
            set {
                this.sKUField = value;
            }
        }
        
        /// <remarks/>
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public int Quantity {
            get {
                return this.quantityField;
            }
            set {
                this.quantityField = value;
            }
        }
        
        /// <remarks/>
        public string Custom1 {
            get {
                return this.custom1Field;
            }
            set {
                this.custom1Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom2 {
            get {
                return this.custom2Field;
            }
            set {
                this.custom2Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom3 {
            get {
                return this.custom3Field;
            }
            set {
                this.custom3Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom4 {
            get {
                return this.custom4Field;
            }
            set {
                this.custom4Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom5 {
            get {
                return this.custom5Field;
            }
            set {
                this.custom5Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom6 {
            get {
                return this.custom6Field;
            }
            set {
                this.custom6Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.moldingbox.com/")]
    public partial class Status {
        
        private int idField;
        
        private string statusNameField;
        
        private string statusDescriptionField;
        
        /// <remarks/>
        public int ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string StatusName {
            get {
                return this.statusNameField;
            }
            set {
                this.statusNameField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.moldingbox.com/")]
    public partial class ShippingMethod {
        
        private int idField;
        
        private string carrierField;
        
        private string methodField;
        
        private string trackingURLField;
        
        /// <remarks/>
        public int ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string Carrier {
            get {
                return this.carrierField;
            }
            set {
                this.carrierField = value;
            }
        }
        
        /// <remarks/>
        public string Method {
            get {
                return this.methodField;
            }
            set {
                this.methodField = value;
            }
        }
        
        /// <remarks/>
        public string TrackingURL {
            get {
                return this.trackingURLField;
            }
            set {
                this.trackingURLField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.moldingbox.com/")]
    public partial class StatusResponse {
        
        private int mBShipmentIDField;
        
        private int shipmentStatusIDField;
        
        private string trackingNumberField;
        
        private string trackingURLField;
        
        private string lotNumberField;
        
        private bool requestSuccessfullyReceivedField;
        
        private bool shipmentExistsField;
        
        private bool shipmentCanceledField;
        
        private string errorMessageField;
        
        private string custom1Field;
        
        private string custom2Field;
        
        private string custom3Field;
        
        private string custom4Field;
        
        private string custom5Field;
        
        private string custom6Field;
        
        /// <remarks/>
        public int MBShipmentID {
            get {
                return this.mBShipmentIDField;
            }
            set {
                this.mBShipmentIDField = value;
            }
        }
        
        /// <remarks/>
        public int ShipmentStatusID {
            get {
                return this.shipmentStatusIDField;
            }
            set {
                this.shipmentStatusIDField = value;
            }
        }
        
        /// <remarks/>
        public string TrackingNumber {
            get {
                return this.trackingNumberField;
            }
            set {
                this.trackingNumberField = value;
            }
        }
        
        /// <remarks/>
        public string TrackingURL {
            get {
                return this.trackingURLField;
            }
            set {
                this.trackingURLField = value;
            }
        }
        
        /// <remarks/>
        public string LotNumber {
            get {
                return this.lotNumberField;
            }
            set {
                this.lotNumberField = value;
            }
        }
        
        /// <remarks/>
        public bool RequestSuccessfullyReceived {
            get {
                return this.requestSuccessfullyReceivedField;
            }
            set {
                this.requestSuccessfullyReceivedField = value;
            }
        }
        
        /// <remarks/>
        public bool ShipmentExists {
            get {
                return this.shipmentExistsField;
            }
            set {
                this.shipmentExistsField = value;
            }
        }
        
        /// <remarks/>
        public bool ShipmentCanceled {
            get {
                return this.shipmentCanceledField;
            }
            set {
                this.shipmentCanceledField = value;
            }
        }
        
        /// <remarks/>
        public string ErrorMessage {
            get {
                return this.errorMessageField;
            }
            set {
                this.errorMessageField = value;
            }
        }
        
        /// <remarks/>
        public string Custom1 {
            get {
                return this.custom1Field;
            }
            set {
                this.custom1Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom2 {
            get {
                return this.custom2Field;
            }
            set {
                this.custom2Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom3 {
            get {
                return this.custom3Field;
            }
            set {
                this.custom3Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom4 {
            get {
                return this.custom4Field;
            }
            set {
                this.custom4Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom5 {
            get {
                return this.custom5Field;
            }
            set {
                this.custom5Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom6 {
            get {
                return this.custom6Field;
            }
            set {
                this.custom6Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.moldingbox.com/")]
    public partial class Response {
        
        private int mBShipmentIDField;
        
        private Shipment shipmentDataField;
        
        private bool successfullyReceivedField;
        
        private string errorMessageField;
        
        private string custom1Field;
        
        private string custom2Field;
        
        private string custom3Field;
        
        private string custom4Field;
        
        private string custom5Field;
        
        private string custom6Field;
        
        /// <remarks/>
        public int MBShipmentID {
            get {
                return this.mBShipmentIDField;
            }
            set {
                this.mBShipmentIDField = value;
            }
        }
        
        /// <remarks/>
        public Shipment ShipmentData {
            get {
                return this.shipmentDataField;
            }
            set {
                this.shipmentDataField = value;
            }
        }
        
        /// <remarks/>
        public bool SuccessfullyReceived {
            get {
                return this.successfullyReceivedField;
            }
            set {
                this.successfullyReceivedField = value;
            }
        }
        
        /// <remarks/>
        public string ErrorMessage {
            get {
                return this.errorMessageField;
            }
            set {
                this.errorMessageField = value;
            }
        }
        
        /// <remarks/>
        public string Custom1 {
            get {
                return this.custom1Field;
            }
            set {
                this.custom1Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom2 {
            get {
                return this.custom2Field;
            }
            set {
                this.custom2Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom3 {
            get {
                return this.custom3Field;
            }
            set {
                this.custom3Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom4 {
            get {
                return this.custom4Field;
            }
            set {
                this.custom4Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom5 {
            get {
                return this.custom5Field;
            }
            set {
                this.custom5Field = value;
            }
        }
        
        /// <remarks/>
        public string Custom6 {
            get {
                return this.custom6Field;
            }
            set {
                this.custom6Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void Post_ShipmentCompletedEventHandler(object sender, Post_ShipmentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Post_ShipmentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Post_ShipmentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Response[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Response[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void Retrieve_Shipment_StatusCompletedEventHandler(object sender, Retrieve_Shipment_StatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Retrieve_Shipment_StatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Retrieve_Shipment_StatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public StatusResponse[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((StatusResponse[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void Retrieve_Shipping_MethodsCompletedEventHandler(object sender, Retrieve_Shipping_MethodsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Retrieve_Shipping_MethodsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Retrieve_Shipping_MethodsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ShippingMethod[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ShippingMethod[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void Retrieve_Shipment_Status_TypesCompletedEventHandler(object sender, Retrieve_Shipment_Status_TypesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Retrieve_Shipment_Status_TypesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Retrieve_Shipment_Status_TypesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Status[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Status[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void Cancel_ShipmentCompletedEventHandler(object sender, Cancel_ShipmentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Cancel_ShipmentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Cancel_ShipmentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public StatusResponse[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((StatusResponse[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591