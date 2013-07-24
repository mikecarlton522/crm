using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.Pagador
{
    public static class PagadorPaymentEnum
    {
        public const int One_Off = 0;
        public const int Installments_by_merchant = 1;
        public const int Installments_by_card_issuer = 2;
    }

    public static class PaymentMethodEnum
    {
        public const int ItauShopline = 12;
        public const int AmericanExpress = 18;
        public const int RedecardWS = 20;
        public const int VISA_TEF = 22;
        public const int Mastercard_TEF = 23;
        public const int Diners_TEF = 24;
        public const int AmericanExpress_TEF = 25;
        public const int Aura_TEF = 37;
        public const int Cielo_Visa_Captura = 71;
        public const int Banorte_Visa_USD = 112;
        public const int Banorte_Diners_USD = 114;
        public const int Banorte_Master_USD = 115;
        public const int Banorte_Master_Automatic_Capture_USD = 116;
        public const int Banorte_Diners_Automatic_Capture_USD = 117;
        public const int Banorte_Visa_Automatic_Capture_USD = 118;
        public const int Cielo_Mastercard_Captura = 120;
        public const int Cielo_Mastercard_PreAuth = 122;
        public const int Cielo_Elo = 126;
        public const int Cielo_Diners = 130;
        public const int PaymentMethodForAll = 998;
    }
}
