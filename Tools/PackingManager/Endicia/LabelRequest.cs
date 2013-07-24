using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TrimFuel.Tools.PackingManager.Endicia
{
    public partial class LabelRequest
    {
        [XmlIgnore] public long[] SaleIDList { get; set; }
        [XmlIgnore] public long BillingID { get; set; }
        [XmlIgnore] public DateTime Date { get; set; }        
        [XmlIgnore] public string[] SKUCodes { get; set; }
        [XmlIgnore] public int[] SKUQuantities { get; set; }
        [XmlIgnore] public string[] SKUDescriptions { get; set; }
        [XmlIgnore] public string ProductName { get; set; }
        [XmlIgnore] public int CurrentIndex { get; set; }

        [XmlIgnore]
        public string SKUCodesDisplayValue
        {
            get
            {
                string displayValue = string.Empty;

                for (int i = 0; i < SKUCodes.Length; i++)
                {
                    displayValue += string.Format("{0} ({1}){2}", SKUCodes[i], SKUQuantities[i], i < SKUCodes.Length - 1 ? ", " : string.Empty);
                }

                return displayValue;
            }
        }
    }
}
