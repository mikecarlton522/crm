using System;
using System.Collections.Generic;

namespace TrimFuel.Model.Enums
{
    //public static class AdminRestrictLevelEnum
    //{
    //    public const int Admin = 0;
    //    public const int FocusCallCenter = 1;
    //    public const int Authorise = 2;
    //    public const int _3SourceCallCenter = 3;
    //    public const int Mercatis = 4;
    //    public const int ReadOnlyAdmin = 5;
    //}

    //public static class AdminRestrictLevelNameEnum
    //{
    //    public static Dictionary<int, string> Values = new Dictionary<int, string> (){ 
    //        { AdminRestrictLevelEnum.Admin, "Admin" },
    //        { AdminRestrictLevelEnum.FocusCallCenter, "Focus Call Center" },
    //        { AdminRestrictLevelEnum.Authorise, "Authorise" },
    //        { AdminRestrictLevelEnum._3SourceCallCenter, "3Source Call Center" },
    //        { AdminRestrictLevelEnum.Mercatis, "Mercatis" },
    //        { AdminRestrictLevelEnum.ReadOnlyAdmin, "Read Only Admin" },
    //    };
    //}

    public static class ApplicationEnum
    {
        public const string LocalhostTriangleCRM = "trimfuel.localhost";
        public const string TriangleCRM = "dashboard.trianglecrm.com";
        public const string CoActionCRM = "coaction.trianglecrm.com";
        public const string OrangeStarCRM = "orangestar.trianglecrm.com";
        public const string TwoBoxCRM = "2box.trianglecrm.com";
        public const string TwoBoxCampaign = "c.2box.trianglecrm.com";
    }

    public static class AragonShoppingClubEnum
    {
        public const int EliteSavings = 250;
        public const int SuperShopper = 275;

        public static IDictionary<int, string> List
        {
            get
            {
                return new Dictionary<int, string>() { 
                    {EliteSavings, "Elite Savings"},
                    {SuperShopper, "Super Shopper"}
                };
            }
        }
    }

    public static class BillingBatchEmailTypeEnum
    {
        public const int InactiveUserReactivationEmail = 1;
    }

    public static class BillingSubscriptionEnum
    {
        public const int UpsellFake = 69;
    }

    public static class BillingSubscriptionStatusEnum
    {
        public const int Inactive = 0;
        public const int Active = 1;
        public const int PendingReturn = 3;
        public const int Returned = 4;
        public const int Declined = 5;
        public const int PreCancel = 6;
        public const int ReturnedNoRMA = 7;
        public const int Scrubbed = 8;
        public const int OrderCompleted = 10;
        public const int ChargebackRetrievalRequest = 11;
        public const int ChargebackPending = 12;
        public const int ChargebackDisputeWon = 13;
        public const int ChargebackDisputeLost = 14;
        public const int ChargebackDisputeFinal = 15;
    }

    public static class ChargeTypeEnum
    {
        public const int Charge = 1;
        public const int Refund = 2;
        public const int Void = 3;
        public const int Credit = 2;
        public const int AuthOnly = 4;
        public const int VoidAuthOnly = 5;
    }

    public static class ControlTypeEnum
    {
        public const int TextInput = 1;
        public const int DropDown = 2;
        public const int Text = 3;
        public const int Phone = 4;
        public const int Zip = 5;
        public const int Pixel = 6;
        public const int CheckBox = 7;
    }

    public static class DynamicEmailTypeEnum
    {
        public const int OrderConfirmation = 1;
        public const int Abandons = 2;
        public const int Shipment = 3;
        public const int RebillDecline = 4;
        public const int ActiveSubscribers = 5;
        public const int CancelledSubscribers = 6;
        public const int InactiveSubscribers = 7;
        public const int RefererPasswordRecovery = 8;
        public const int RefererPromotion1 = 9;
        public const int GiftCertificateGiven = 10;
        public const int Newsletter = 11;
        public const int RMA = 12;
        public const int Refund = 13;
    }

    public static class InventoryTypeEnum
    {
        public const int Inventory = 1;
        public const int Service = 2;
    }

    public static class GatewayNameEnum
    {
        public const string NMI = "NMI";
        public const string MSC = "MSC";
        public const string MPS = "MPS";
        public const string IPG = "IPG";
        public const string PrismPay = "PrismPay";
        public const string EMP = "EMP";
        public const string CBG = "CBG";
        public const string BPAG = "BPAG";
        public const string BPAGRedecard = "BPAGRedecard";
        public const string Pagador = "Pagador";
    }

    public static class GeoTypeEnum
    {
        public const int Region = 1;
        public const int Country = 2;

    }

    public static class LeadTypeEnum
    {
        public const int Abandons = 1;
        public const int OrderConfirmations = 2;
        public const int CancellationsDeclines = 3;
        public const int All = 4;
    }

    public static class LeadPartnerEnum
    {
        public const int Focus = 1;
        public const int REC = 2;
        public const int PopMarketing = 3;
        public const int CPAResponse = 4;
        public const int Aragon = 5;
        public const int ePacificLink = 6;
        public const int Five9 = 7;
        public const int REG = 8;
    }

    public static class PageTypeEnum
    {
        public const int Landing = 1;
        public const int Billing = 2;
        public const int Upsell_1 = 3;
        public const int Confirmation = 4;
        public const int PreLander = 5;
        public const int Upsell_2 = 6;
        public const int Upsell_3 = 7;
    }

    public static class PaymentTypeEnum
    {
        public const short AmericanExpress = 1;
        public const short Visa = 2;
        public const short Mastercard = 3;
        public const short Discover = 4;
        public const short DinersClub = 5;

        public static Dictionary<int?, string> Types = new Dictionary<int?,string>()
        {
         {1, "AmericanExpress"},
         {2, "Visa"},
         {3, "Mastercard"},
         {4, "Discover"},
         {5, "DinersClub"}
        };

    }

    public static class ProductEnum
    {
        public const int ECigarettes = 10;
    }

    public static class PromoGiftTypeEnum
    {
        public const int GiftCertificate = 1;
    }

    public static class RefererCommissionTypeEnum
    {
        public const int UseInStore = 1;
        public const int ConvertToCash = 2;
    }

    public static class ReturnProcessingActionEnum
    {
        public const byte ISSUE_REFUND = 1;
        public const byte CANCEL_ACCOUNT = 2;
        public const byte CHANGE_PLAN = 3;
        public const byte SHIP_FREE_ITEM = 4;
        public const byte BILL_AND_SHIP_ITEM = 5;
    }

    public static class SaleChargeTypeEnum
    {
        public const int Shipping = 1;
    }

    public static class SaleTypeEnum
    {
        public const short Billing = 1;
        public const short Rebill = 2;
        public const short Upsell = 3;
        public const short Extra = 4;
        public const short ExtraTrialShip = 5;
        public const short OrderSale = 10;
    }

    public static class ShipperEnum
    {
        public const short TSN = 2;
        public const short ABF = 4;
        public const short Keymail = 5;
        public const short AtLastFulfillment = 6;
        public const short GoFulfillment = 7;
        public const short NPF = 8;
        public const short MB = 9;
        public const short TF = 10;
        public const short CustomShipper = 11;

        public static Dictionary<int?, string> Shippers = new Dictionary<int?, string>()
        {
         {2, "TSN/Novaship"},
         {4, "ABF/USellWeShip"},
         {5, "Keymail"},
         {6, "At Last Fulfillment"},
         {7, "Go Fulfillment"},
         {8, "NPF"},
         {9, "Molding Box"},
         {10, "Triangle Fulfillment"},
         {11, "Custom Shipper"}
        };
    }

    public static class SubscriptionActionType
    {
        public const int Upsell = 1;
        public const int FreeProduct = 2;
    }

    public static class TPModeEnum
    {
        public const int WebService = 1;
        public const int PostGet = 2;
        public const int NMI_Emulation = 3;
        public const int AuthoriseNET_Emulation = 4;
    }

    public static class VeriSectPostTypeEnum
    {
        public const int Charge = 1;
        public const int Refund = 2;
        public const int Chargeback = 3;
    }

    public static class ShipperConfigEnum
    {
        public static ShipperConfig.ID ABF_ThreePLKey = new ShipperConfig.ID() { ShipperID = 4, Key = "ThreePLKey" };
        public static ShipperConfig.ID ABF_Login = new ShipperConfig.ID() { ShipperID = 4, Key = "Login" };
        public static ShipperConfig.ID ABF_Password = new ShipperConfig.ID() { ShipperID = 4, Key = "Password" };
        public static ShipperConfig.ID ABF_FacilityID = new ShipperConfig.ID() { ShipperID = 4, Key = "FacilityID" };
        public static ShipperConfig.ID ABF_ThreePLID = new ShipperConfig.ID() { ShipperID = 4, Key = "ThreePLID" };
        public static ShipperConfig.ID TSN_Username = new ShipperConfig.ID() { ShipperID = 2, Key = "Username" };
        public static ShipperConfig.ID TSN_Password = new ShipperConfig.ID() { ShipperID = 2, Key = "Password" };
        public static ShipperConfig.ID ALF_ApiKey = new ShipperConfig.ID() { ShipperID = 6, Key = "ApiKey" };
        public static ShipperConfig.ID GO_FULFILLMENT_UserId = new ShipperConfig.ID() { ShipperID = 7, Key = "UserId" };
        public static ShipperConfig.ID GO_FULFILLMENT_Password = new ShipperConfig.ID() { ShipperID = 7, Key = "Password" };
        public static ShipperConfig.ID GO_FULFILLMENT_OriginId = new ShipperConfig.ID() { ShipperID = 7, Key = "OriginId" };
        public static ShipperConfig.ID NPF_Username = new ShipperConfig.ID() { ShipperID = 8, Key = "Username" };
        public static ShipperConfig.ID NPF_Password = new ShipperConfig.ID() { ShipperID = 8, Key = "Password" };
        public static ShipperConfig.ID MB_ApiKey = new ShipperConfig.ID() { ShipperID = 9, Key = "ApiKey" };
        public static ShipperConfig.ID TF_ThreePLKey = new ShipperConfig.ID() { ShipperID = 10, Key = "ThreePLKey" };
        public static ShipperConfig.ID TF_Login = new ShipperConfig.ID() { ShipperID = 10, Key = "Login" };
        public static ShipperConfig.ID TF_Password = new ShipperConfig.ID() { ShipperID = 10, Key = "Password" };
        public static ShipperConfig.ID TF_FacilityID = new ShipperConfig.ID() { ShipperID = 10, Key = "FacilityID" };
        public static ShipperConfig.ID TF_ThreePLID = new ShipperConfig.ID() { ShipperID = 10, Key = "ThreePLID" };
        public static ShipperConfig.ID KEYMAIL_Emails = new ShipperConfig.ID() { ShipperID = 5, Key = "Emails" };
        public static ShipperConfig.ID CustomShipper_Username = new ShipperConfig.ID() { ShipperID =11, Key = "Username" };
        public static ShipperConfig.ID CustomShipper_Password = new ShipperConfig.ID() { ShipperID = 11, Key = "Password" };
        public static ShipperConfig.ID CustomShipper_Path = new ShipperConfig.ID() { ShipperID = 11, Key = "Path" };
    }

    public static class ShipperConfigList
    {
        public static List<ShipperConfig.ID> Values = new List<ShipperConfig.ID>()
            {
                ShipperConfigEnum.ABF_ThreePLKey,
                ShipperConfigEnum.ABF_FacilityID,
                ShipperConfigEnum.ABF_Login,
                ShipperConfigEnum.ABF_Password,
                ShipperConfigEnum.ABF_ThreePLID,
                ShipperConfigEnum.ALF_ApiKey,
                ShipperConfigEnum.GO_FULFILLMENT_OriginId,
                ShipperConfigEnum.GO_FULFILLMENT_Password,
                ShipperConfigEnum.GO_FULFILLMENT_UserId,
                ShipperConfigEnum.NPF_Password,
                ShipperConfigEnum.NPF_Username,
                ShipperConfigEnum.TSN_Password,
                ShipperConfigEnum.TSN_Username,
                ShipperConfigEnum.MB_ApiKey,
                ShipperConfigEnum.TF_ThreePLKey,
                ShipperConfigEnum.TF_FacilityID,
                ShipperConfigEnum.TF_Login,
                ShipperConfigEnum.TF_Password,
                ShipperConfigEnum.TF_ThreePLID,
                ShipperConfigEnum.KEYMAIL_Emails,
                ShipperConfigEnum.CustomShipper_Password,
                ShipperConfigEnum.CustomShipper_Username,
                ShipperConfigEnum.CustomShipper_Path
            };
    }


    public static class ShipperRoutingReasonEnum
    {
        public const int NotRouted = 0;
        public const int CoActionCanadaShipments = 1;
        public const int OrangeStarAllShipments = 2;
    }

    public static class LeadPartnerConfigEnum
    {
        public static LeadPartnerConfigValue.ID REC_Product = new LeadPartnerConfigValue.ID() { LeadPartnerID = 2, Key = "Product" };
        public static LeadPartnerConfigValue.ID CPAResponse_API_KEY = new LeadPartnerConfigValue.ID() { LeadPartnerID = 4, Key = "API_KEY" };
        public static LeadPartnerConfigValue.ID Aragon_CampaignID = new LeadPartnerConfigValue.ID() { LeadPartnerID = 5, Key = "CampaignID" };
        public static LeadPartnerConfigValue.ID Aragon_CompanyID = new LeadPartnerConfigValue.ID() { LeadPartnerID = 5, Key = "CompanyID" };
        public static LeadPartnerConfigValue.ID Aragon_Password = new LeadPartnerConfigValue.ID() { LeadPartnerID = 5, Key = "Password" };
        public static LeadPartnerConfigValue.ID FiveNine_Username = new LeadPartnerConfigValue.ID() { LeadPartnerID = 7, Key = "Username" };
        public static LeadPartnerConfigValue.ID FiveNine_Password = new LeadPartnerConfigValue.ID() { LeadPartnerID = 7, Key = "Password" };
        public static LeadPartnerConfigValue.ID FiveNine_DomainID = new LeadPartnerConfigValue.ID() { LeadPartnerID = 7, Key = "DomainID" };
        public static LeadPartnerConfigValue.ID FiveNine_ListName = new LeadPartnerConfigValue.ID() { LeadPartnerID = 7, Key = "ListName" };
        public static LeadPartnerConfigValue.ID REG_ClientID = new LeadPartnerConfigValue.ID() { LeadPartnerID = 8, Key = "ClientID" };
        public static LeadPartnerConfigValue.ID REG_ClientKey = new LeadPartnerConfigValue.ID() { LeadPartnerID = 8, Key = "ClientKey" };
        public static LeadPartnerConfigValue.ID REG_CampaignCode = new LeadPartnerConfigValue.ID() { LeadPartnerID = 8, Key = "CampaignCode" };
        public static LeadPartnerConfigValue.ID Focus_ClientCode = new LeadPartnerConfigValue.ID() { LeadPartnerID = 1, Key = "ClientCode" };
    }

    public static class LeadPartnerConfigList
    {
        public static List<LeadPartnerConfigValue.ID> Values = new List<LeadPartnerConfigValue.ID>()
        {
            LeadPartnerConfigEnum.REC_Product,
            LeadPartnerConfigEnum.CPAResponse_API_KEY,
            LeadPartnerConfigEnum.Aragon_CampaignID,
            LeadPartnerConfigEnum.Aragon_CompanyID,
            LeadPartnerConfigEnum.Aragon_Password,
            LeadPartnerConfigEnum.FiveNine_Username,
            LeadPartnerConfigEnum.FiveNine_Password,
            LeadPartnerConfigEnum.FiveNine_DomainID,
            LeadPartnerConfigEnum.FiveNine_ListName,
            LeadPartnerConfigEnum.Aragon_Password,
            LeadPartnerConfigEnum.REG_ClientID,
            LeadPartnerConfigEnum.REG_ClientKey,
            LeadPartnerConfigEnum.REG_CampaignCode,
            LeadPartnerConfigEnum.Focus_ClientCode
        };
    }

    public static class EventTypeEnum
    {
        public static int Registration = 1;
        public static int OrderConfirmation = 2;
    }

    public static class ShippingOptionEnum
    {
        public static int PriorityShipping = 1;
    }

    public static class OrderStatusEnum
    {
        public static int New = 0;
        public static int Active = 1;
    }

    public static class SaleStatusEnum
    {
        public static int New = 0;
        public static int Approved = 1;
        public static int Declined = 2;
    }

    public static class RecurringStatusEnum
    {
        public static int New = 0;
        public static int Active = 1;
        public static int Inactive = 2;
        public static int Completed = 3;
        public static int ReturnedNoRMA = 4;
        public static int Scrubbed = 5;
        public static int Declined = 6;

        public static IDictionary<int, string> Name = new Dictionary<int, string>()
        {
            {New, "New"},
            {Active, "Active"},
            {Inactive, "Inactive"},
            {Completed, "Completed"},
            {ReturnedNoRMA, "Returned No RMA"},
            {Scrubbed, "Scrubbed"},
            {Declined, "Declined"}
        };
    }

    public static class ShipmentStatusEnum
    {
        public static int Blocked = 0;
        public static int New = 10;
        public static int SubmitError = 11;
        public static int Submitted = 20;        
        public static int Shipped = 30;
        public static int Returned = 40;

        public static IDictionary<int, string> Name = new Dictionary<int, string>()
        {
            {Blocked, "Blocked"},
            {New, "New"},
            {SubmitError, "Submit Error"},
            {Submitted, "Submitted"},
            {Shipped, "Shipped"},
            {Returned, "Returned"}
        };
    }
    
    public static class InvoiceStatusEnum
    {
        public static int New = 0;
        public static int Paid = 1;
    }

    public static class OrderSaleTypeEnum
    {
        public static int Trial = 1;
        public static int Rebill = 2;
        public static int Upsell = 3;

        public static IDictionary<int, string> Name = new Dictionary<int, string>()
        {
            {Trial, "Trial"},
            {Rebill, "Rebill"},
            {Upsell, "Upsell"}
        };
    }
}