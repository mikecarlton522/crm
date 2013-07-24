using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Enums;

namespace TrimFuel.Model.Views
{
    public class SaleFullInfo : EntityView
    {
        public ChargeHistoryEx Charge { get; set; }
        public AssertigyMID ChargeMID { get; set; }
        public ChargeHistoryExCurrency ChargeCurrency { get; set; }
        public ChargeHistoryExSale ChargeSalePart { get; set; }
        public IList<SaleChargeDetails> ChargeDetails { get; set; }
        public Referer Referer { get; set; }
        public SaleDetails Details { get; set; }

        #region Logic

        public decimal TotalAmount 
        {
            get 
            {
                decimal res = 0M;
                if (ChargeSalePart != null && ChargeSalePart.CurrencyAmount != null)
                {
                    res = ChargeSalePart.CurrencyAmount.Value;
                }
                else if (ChargeSalePart != null && ChargeSalePart.Amount != null)
                {
                    res = ChargeSalePart.Amount.Value;
                }
                else if (ChargeCurrency != null && ChargeCurrency.CurrencyAmount != null)
                {
                    res = ChargeCurrency.CurrencyAmount.Value;
                }
                else if (Charge != null && Charge.Amount != null)
                {
                    res = Charge.Amount.Value;
                }
                return res;
            }
        }

        public decimal ShippingAmount 
        {
            get 
            {
                decimal res = 0M;

                if (ChargeDetails != null && ChargeDetails.Count > 0)
                {
                    foreach (SaleChargeDetails d in ChargeDetails)
                    {
                        decimal amount = 0M;
                        if (d.SaleChargeTypeID == SaleChargeTypeEnum.Shipping && d.CurrencyAmount != null)
                        {
                            amount = d.CurrencyAmount.Value;
                        }
                        else if (d.SaleChargeTypeID == SaleChargeTypeEnum.Shipping && d.Amount != null)
                        {
                            amount = d.Amount.Value;
                        }
                        res += amount;
                    }                    
                }

                return res;
            }
        }

        public decimal ProductAmount 
        {
            get 
            {
                return TotalAmount - ShippingAmount;
            }
        }

        #endregion
    }
}
