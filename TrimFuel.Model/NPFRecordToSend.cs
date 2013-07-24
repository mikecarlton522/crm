﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class NPFRecordToSend : Entity
    {
        public int? NPFRecordToSendID { get; set; }
        public long? SaleID { get; set; }
        public long? RegID { get; set; }
        public string Request { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
        }
    }
}
