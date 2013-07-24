using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrimFuel.Web.Admin.Logic.Reports.JMB
{
    [XmlRoot()]
    public class member
    {
        [XmlElement("registration_details")]
        public registration_details registration_details { get; set; }
        [XmlElement("login_history")]
        public login_history login_history { get; set; }
        [XmlElement("purchased_bidpacks")]
        public purchased_bidpacks purchased_bidpacks { get; set; }
        [XmlElement("auctions_bid_on")]
        public auctions_bid_on auctions_bid_on { get; set; }
    }

    public class registration_details
    {
        [XmlElement("username")]
        public string username { get; set; }
        [XmlElement("sex")]
        public string sex { get; set; }
        [XmlElement("registration_date")]
        public string registration_date { get; set; }
    }

    public class login_history
    {
        [XmlElement("login")]
        public List<login> loginList { get; set; }
    }

    public class login
    {
        [XmlElement("login_time")]
        public string login_time { get; set; }
        [XmlElement("logout_time")]
        public string logout_time { get; set; }
    }

    public class purchased_bidpacks
    {
        [XmlElement("bidpack")]
        public List<bidpack> bidpackList { get; set; }
    }

    public class bidpack
    {
        [XmlElement("date_purchased")]
        public string date_purchased { get; set; }
        [XmlElement("purchase_amount")]
        public string purchase_amount { get; set; }
        [XmlElement("bids_left")]
        public string bids_left { get; set; }
        [XmlElement("description")]
        public string description { get; set; }
    }

    public class auctions_bid_on
    {
        [XmlElement("auction")]
        public List<auction> auctionList { get; set; }
    }

    public class auction
    {
        [XmlElement("auction_details")]
        public auction_details auction_details { get; set; }
    }

    public class auction_details
    {
        [XmlElement("productname")]
        public string productname { get; set; }
        [XmlElement("startdate")]
        public string startdate { get; set; }
        [XmlElement("enddate")]
        public string enddate { get; set; }
        [XmlElement("auction_status")]
        public string auction_status { get; set; }
        [XmlElement("bidding_outcome")]
        public string bidding_outcome { get; set; }
        [XmlElement("total_bids_placed")]
        public string total_bids_placed { get; set; }
        [XmlElement("seller")]
        public string seller { get; set; }
    }
}
