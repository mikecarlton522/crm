using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class TermView : EntityView
    {
        public string CorporationBody { get; set; }
        public string CorporationName { get; set; }
        public string Domain { get; set; }
        public string Email { get; set; }
        public string MembershipTerms { get; set; }
        public string Phone { get; set; }
        public string ReturnAddressBody { get; set; }
        public string ReturnAddressName { get; set; }
        public string StraightSaleTerms { get; set; }
        public string Outline { get; set; }
        public string PrivacyPolicy { get; set; }
    }
}
