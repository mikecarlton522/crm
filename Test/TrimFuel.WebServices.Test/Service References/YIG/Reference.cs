﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17020
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrimFuel.WebServices.Test.YIG {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BusinessErrorOfChargeHistory", Namespace="http://tempuri.org/")]
    [System.SerializableAttribute()]
    public partial class BusinessErrorOfChargeHistory : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private TrimFuel.WebServices.Test.YIG.ChargeHistory ReturnValueField;
        
        private TrimFuel.WebServices.Test.YIG.BusinessErrorState StateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public TrimFuel.WebServices.Test.YIG.ChargeHistory ReturnValue {
            get {
                return this.ReturnValueField;
            }
            set {
                if ((object.ReferenceEquals(this.ReturnValueField, value) != true)) {
                    this.ReturnValueField = value;
                    this.RaisePropertyChanged("ReturnValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public TrimFuel.WebServices.Test.YIG.BusinessErrorState State {
            get {
                return this.StateField;
            }
            set {
                if ((this.StateField.Equals(value) != true)) {
                    this.StateField = value;
                    this.RaisePropertyChanged("State");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string ErrorMessage {
            get {
                return this.ErrorMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorMessageField, value) != true)) {
                    this.ErrorMessageField = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ChargeHistory", Namespace="http://tempuri.org/")]
    [System.SerializableAttribute()]
    public partial class ChargeHistory : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private System.Nullable<long> ChargeHistoryIDField;
        
        private System.Nullable<decimal> AmountField;
        
        private System.Nullable<bool> SuccessField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MIDField;
        
        private System.Nullable<System.DateTime> DateField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Nullable<long> ChargeHistoryID {
            get {
                return this.ChargeHistoryIDField;
            }
            set {
                if ((this.ChargeHistoryIDField.Equals(value) != true)) {
                    this.ChargeHistoryIDField = value;
                    this.RaisePropertyChanged("ChargeHistoryID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=1)]
        public System.Nullable<decimal> Amount {
            get {
                return this.AmountField;
            }
            set {
                if ((this.AmountField.Equals(value) != true)) {
                    this.AmountField = value;
                    this.RaisePropertyChanged("Amount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=2)]
        public System.Nullable<bool> Success {
            get {
                return this.SuccessField;
            }
            set {
                if ((this.SuccessField.Equals(value) != true)) {
                    this.SuccessField = value;
                    this.RaisePropertyChanged("Success");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string MID {
            get {
                return this.MIDField;
            }
            set {
                if ((object.ReferenceEquals(this.MIDField, value) != true)) {
                    this.MIDField = value;
                    this.RaisePropertyChanged("MID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=4)]
        public System.Nullable<System.DateTime> Date {
            get {
                return this.DateField;
            }
            set {
                if ((this.DateField.Equals(value) != true)) {
                    this.DateField = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BusinessErrorState", Namespace="http://tempuri.org/")]
    public enum BusinessErrorState : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Error = 1,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="YIG.billing_apiSoap")]
    public interface billing_apiSoap {
        
        // CODEGEN: Generating message contract since element name VoidResult from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Void", ReplyAction="*")]
        TrimFuel.WebServices.Test.YIG.VoidResponse Void(TrimFuel.WebServices.Test.YIG.VoidRequest request);
        
        // CODEGEN: Generating message contract since element name RefundResult from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Refund", ReplyAction="*")]
        TrimFuel.WebServices.Test.YIG.RefundResponse Refund(TrimFuel.WebServices.Test.YIG.RefundRequest request);
        
        // CODEGEN: Generating message contract since element name firstName from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Charge", ReplyAction="*")]
        TrimFuel.WebServices.Test.YIG.ChargeResponse Charge(TrimFuel.WebServices.Test.YIG.ChargeRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class VoidRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Void", Namespace="http://tempuri.org/", Order=0)]
        public TrimFuel.WebServices.Test.YIG.VoidRequestBody Body;
        
        public VoidRequest() {
        }
        
        public VoidRequest(TrimFuel.WebServices.Test.YIG.VoidRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class VoidRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long chargeHistoryID;
        
        public VoidRequestBody() {
        }
        
        public VoidRequestBody(long chargeHistoryID) {
            this.chargeHistoryID = chargeHistoryID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class VoidResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="VoidResponse", Namespace="http://tempuri.org/", Order=0)]
        public TrimFuel.WebServices.Test.YIG.VoidResponseBody Body;
        
        public VoidResponse() {
        }
        
        public VoidResponse(TrimFuel.WebServices.Test.YIG.VoidResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class VoidResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory VoidResult;
        
        public VoidResponseBody() {
        }
        
        public VoidResponseBody(TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory VoidResult) {
            this.VoidResult = VoidResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RefundRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Refund", Namespace="http://tempuri.org/", Order=0)]
        public TrimFuel.WebServices.Test.YIG.RefundRequestBody Body;
        
        public RefundRequest() {
        }
        
        public RefundRequest(TrimFuel.WebServices.Test.YIG.RefundRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RefundRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long chargeHistoryID;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public decimal refundAmount;
        
        public RefundRequestBody() {
        }
        
        public RefundRequestBody(long chargeHistoryID, decimal refundAmount) {
            this.chargeHistoryID = chargeHistoryID;
            this.refundAmount = refundAmount;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RefundResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RefundResponse", Namespace="http://tempuri.org/", Order=0)]
        public TrimFuel.WebServices.Test.YIG.RefundResponseBody Body;
        
        public RefundResponse() {
        }
        
        public RefundResponse(TrimFuel.WebServices.Test.YIG.RefundResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RefundResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory RefundResult;
        
        public RefundResponseBody() {
        }
        
        public RefundResponseBody(TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory RefundResult) {
            this.RefundResult = RefundResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ChargeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Charge", Namespace="http://tempuri.org/", Order=0)]
        public TrimFuel.WebServices.Test.YIG.ChargeRequestBody Body;
        
        public ChargeRequest() {
        }
        
        public ChargeRequest(TrimFuel.WebServices.Test.YIG.ChargeRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ChargeRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public decimal amount;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string firstName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string lastName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string address1;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string address2;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string city;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string state;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string zip;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=8)]
        public string phone;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=9)]
        public string email;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=10)]
        public string ip;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=11)]
        public int paymentType;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=12)]
        public string creditCard;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=13)]
        public string cvv;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=14)]
        public int expMonth;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=15)]
        public int expYear;
        
        public ChargeRequestBody() {
        }
        
        public ChargeRequestBody(
                    decimal amount, 
                    string firstName, 
                    string lastName, 
                    string address1, 
                    string address2, 
                    string city, 
                    string state, 
                    string zip, 
                    string phone, 
                    string email, 
                    string ip, 
                    int paymentType, 
                    string creditCard, 
                    string cvv, 
                    int expMonth, 
                    int expYear) {
            this.amount = amount;
            this.firstName = firstName;
            this.lastName = lastName;
            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.state = state;
            this.zip = zip;
            this.phone = phone;
            this.email = email;
            this.ip = ip;
            this.paymentType = paymentType;
            this.creditCard = creditCard;
            this.cvv = cvv;
            this.expMonth = expMonth;
            this.expYear = expYear;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ChargeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ChargeResponse", Namespace="http://tempuri.org/", Order=0)]
        public TrimFuel.WebServices.Test.YIG.ChargeResponseBody Body;
        
        public ChargeResponse() {
        }
        
        public ChargeResponse(TrimFuel.WebServices.Test.YIG.ChargeResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ChargeResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory ChargeResult;
        
        public ChargeResponseBody() {
        }
        
        public ChargeResponseBody(TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory ChargeResult) {
            this.ChargeResult = ChargeResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface billing_apiSoapChannel : TrimFuel.WebServices.Test.YIG.billing_apiSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class billing_apiSoapClient : System.ServiceModel.ClientBase<TrimFuel.WebServices.Test.YIG.billing_apiSoap>, TrimFuel.WebServices.Test.YIG.billing_apiSoap {
        
        public billing_apiSoapClient() {
        }
        
        public billing_apiSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public billing_apiSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public billing_apiSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public billing_apiSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TrimFuel.WebServices.Test.YIG.VoidResponse TrimFuel.WebServices.Test.YIG.billing_apiSoap.Void(TrimFuel.WebServices.Test.YIG.VoidRequest request) {
            return base.Channel.Void(request);
        }
        
        public TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory Void(long chargeHistoryID) {
            TrimFuel.WebServices.Test.YIG.VoidRequest inValue = new TrimFuel.WebServices.Test.YIG.VoidRequest();
            inValue.Body = new TrimFuel.WebServices.Test.YIG.VoidRequestBody();
            inValue.Body.chargeHistoryID = chargeHistoryID;
            TrimFuel.WebServices.Test.YIG.VoidResponse retVal = ((TrimFuel.WebServices.Test.YIG.billing_apiSoap)(this)).Void(inValue);
            return retVal.Body.VoidResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TrimFuel.WebServices.Test.YIG.RefundResponse TrimFuel.WebServices.Test.YIG.billing_apiSoap.Refund(TrimFuel.WebServices.Test.YIG.RefundRequest request) {
            return base.Channel.Refund(request);
        }
        
        public TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory Refund(long chargeHistoryID, decimal refundAmount) {
            TrimFuel.WebServices.Test.YIG.RefundRequest inValue = new TrimFuel.WebServices.Test.YIG.RefundRequest();
            inValue.Body = new TrimFuel.WebServices.Test.YIG.RefundRequestBody();
            inValue.Body.chargeHistoryID = chargeHistoryID;
            inValue.Body.refundAmount = refundAmount;
            TrimFuel.WebServices.Test.YIG.RefundResponse retVal = ((TrimFuel.WebServices.Test.YIG.billing_apiSoap)(this)).Refund(inValue);
            return retVal.Body.RefundResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TrimFuel.WebServices.Test.YIG.ChargeResponse TrimFuel.WebServices.Test.YIG.billing_apiSoap.Charge(TrimFuel.WebServices.Test.YIG.ChargeRequest request) {
            return base.Channel.Charge(request);
        }
        
        public TrimFuel.WebServices.Test.YIG.BusinessErrorOfChargeHistory Charge(
                    decimal amount, 
                    string firstName, 
                    string lastName, 
                    string address1, 
                    string address2, 
                    string city, 
                    string state, 
                    string zip, 
                    string phone, 
                    string email, 
                    string ip, 
                    int paymentType, 
                    string creditCard, 
                    string cvv, 
                    int expMonth, 
                    int expYear) {
            TrimFuel.WebServices.Test.YIG.ChargeRequest inValue = new TrimFuel.WebServices.Test.YIG.ChargeRequest();
            inValue.Body = new TrimFuel.WebServices.Test.YIG.ChargeRequestBody();
            inValue.Body.amount = amount;
            inValue.Body.firstName = firstName;
            inValue.Body.lastName = lastName;
            inValue.Body.address1 = address1;
            inValue.Body.address2 = address2;
            inValue.Body.city = city;
            inValue.Body.state = state;
            inValue.Body.zip = zip;
            inValue.Body.phone = phone;
            inValue.Body.email = email;
            inValue.Body.ip = ip;
            inValue.Body.paymentType = paymentType;
            inValue.Body.creditCard = creditCard;
            inValue.Body.cvv = cvv;
            inValue.Body.expMonth = expMonth;
            inValue.Body.expYear = expYear;
            TrimFuel.WebServices.Test.YIG.ChargeResponse retVal = ((TrimFuel.WebServices.Test.YIG.billing_apiSoap)(this)).Charge(inValue);
            return retVal.Body.ChargeResult;
        }
    }
}
