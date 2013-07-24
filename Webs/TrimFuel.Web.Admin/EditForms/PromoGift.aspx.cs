using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MySql.Data.MySqlClient;

using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.EditForms
{
    public partial class _PromoGift : System.Web.UI.Page
    {
        protected string giftNumber;
        protected string giftValue;
        protected string remainingGiftValue;
        protected string statusMessage = string.Empty;
        protected string hide = string.Empty;

        private SaleService saleService = new SaleService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["number"]))
            {
                giftNumber = Request["number"];

                PromoGift = saleService.GetGiftCertificateByNumber(giftNumber);

                if (PromoGift != null)
                {
                    giftValue = ((decimal)PromoGift.Value).ToString("f2");
                    remainingGiftValue = ((decimal)PromoGift.RemainingValue).ToString("f2");
                }
            }
            
            if (!string.IsNullOrEmpty(Request["action"]))
            {
                string action = Request["button"];

                if (action == "Save")
                {

                    try
                    {
                        PromoGift.Value = Convert.ToDecimal(Request["price"]);

                        PromoGift.RemainingValue = Convert.ToDecimal(Request["remainingPrice"]);


                        giftValue = ((decimal)PromoGift.Value).ToString("f2");

                        remainingGiftValue = ((decimal)PromoGift.RemainingValue).ToString("f2");

                        Save((decimal)PromoGift.Value, (decimal)PromoGift.RemainingValue);
                    }
                    catch { }
                }
                else if(action == "Delete")
                {
                    Delete(PromoGift.GiftNumber);
                }
            }
        }

        public PromoGift PromoGift { get; set; }

        private void Save(decimal newGiftValue, decimal newGiftRemainingValue)
        {
            try
            {
                string query = "update PromoGift set Value = @Value, RemainingValue = @RemainingValue where GiftNumber = @GiftNumber";

                MySqlCommand cmd = new MySqlCommand(query);

                cmd.Parameters.AddWithValue("@Value", newGiftValue);
                cmd.Parameters.AddWithValue("@RemainingValue", newGiftRemainingValue);
                cmd.Parameters.AddWithValue("@GiftNumber", PromoGift.GiftNumber);

                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                dao.ExecuteNonQuery(cmd);

                dao.CommitTransaction();

                statusMessage = "Your action has been saved succesfully.";
                
            }
            catch { }
        }

        private void Delete(string giftNumber)
        {
            string query = "delete from PromoGift where GiftNumber = @GiftNumber limit 1;";

            MySqlCommand cmd = new MySqlCommand(query);
         
            cmd.Parameters.AddWithValue("@GiftNumber", giftNumber);

            IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

            dao.ExecuteNonQuery(cmd);

            dao.CommitTransaction();

            hide = " style=\"display:none\"";

            statusMessage = "Your action has been saved succesfully.";
        }
    }
}