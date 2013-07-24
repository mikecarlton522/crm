using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SalesAgrBaseReportView : EntityView
    {
        public int? AssertigyMIDID { get; set; }
        public string AssertigyDisplayName { get; set; }
        public string AssertigyMID { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? SaleCount { get; set; }

        #region Fill

        public void Fill(SalesAgrBaseReportView objToFill)
        {
            if (objToFill != null)
            {
                objToFill.AssertigyMIDID = AssertigyMIDID;
                objToFill.AssertigyMID = AssertigyMID;
                objToFill.AssertigyDisplayName = AssertigyDisplayName;
                objToFill.PaymentTypeID = PaymentTypeID;
                objToFill.SaleCount = SaleCount;
            }
        }

        #endregion
    }
}
