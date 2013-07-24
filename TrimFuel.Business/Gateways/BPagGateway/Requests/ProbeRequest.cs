using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.BPagGateway.Requests
{
    public class ProbeRequest
    {
        public string merch_ref { get; set; }
        public string id { get; set; }
        public string bpag_payment_id { get; set; }
    }
}
