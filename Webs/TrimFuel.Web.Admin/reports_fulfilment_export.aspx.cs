using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public partial class reports_fulfilment_export : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();

            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                var command = new MySqlCommand
                {
                    CommandText =
                        @"Select 
                        s.SaleID, 
                        b.BillingID, 
                        bi.CustomField1 as CPF, 
                        DATE_FORMAT(s.CreateDT, '%d/%m/%Y') as OrderDate, 
                        DATE_FORMAT(s.CreateDT, '%H:%i:%s') as OrderTime, 
                        b.FirstName, 
                        b.LastName, 
                        b.Address1, 
                        b.Address2, 
                        bi.CustomField2 as Numero, 
                        bi.CustomField3 as Complemento, 
                        ri.Neighborhood as Bairro,
                        b.City,
                        b.State,
                        b.Zip,
                        b.Country,
                        b.Email,
                        b.Phone,
                        'SM2' as PackageCode,
                        '' as Misc1,
                        '' as Misc2,
                        '' as Misc3,
                        (case when coalesce(isku.InventorySKU, '') <> '' then isku.InventorySKU else psku.ProductSKU end) as PackageDescription,
                        b.IP,
                        (t.ProductTotal + t.ProductShipping) as GrandTotal,
                        pcinf.Description as ProductDescription,
                        (case when coalesce(isku.InventorySKU, '') <> '' then isku.InventorySKU else psku.ProductSKU end) as ProductCode,
                        pci.Quantity,
                        t.ProductShipping  as ProductShipping,
                        t.ProductTotal as ProductTotal,
                        pcinf.Weight,
                        pcinf.RetailPrice as CustomsPrice,
                        pci.Quantity * t.Quantity as TotalQty,
                        c.HtmlSymbol
                        From 
                        (
	                        Select us.SaleID, u.ProductCode, u.Quantity, us.ChargeHistoryID, u.BillingID, 0 as ProductShipping, ut.Price as ProductTotal from Upsell u  
	                        inner join UpsellSale us on us.UpsellID = u.UpsellID
	                        inner join UpsellType ut on ut.UpsellTypeID = u.UpsellTypeID
	                        union all
	                        Select bs.SaleID, bs.ProductCode, bs.Quantity, bs.ChargeHistoryID, bsub.BillingID, sub.InitialShipping as ProductShipping, sub.InitialBillAmount as ProductTotal from BillingSale bs
	                        inner join BillingSubscription as bsub on bsub.BillingSubscriptionID = bs.BillingSubscriptionID
	                        inner join Subscription as sub on sub.SubscriptionID = bsub.SubscriptionID
	                        union all
	                        Select ss.SaleID, es.ProductCode, es.Quantity, null as ChargeHistoryID, es.BillingID, 0 as ProductShipping, 0 as ProductTotal from ExtraTrialShip es
	                        inner join ExtraTrialShipSale ss on ss.ExtraTrialShipID = es.ExtraTrialShipID 
                        ) t
                        inner join Sale as s on s.SaleID = t.SaleID
                        inner join Billing as b on b.BillingID = t.BillingID
                        left join BillingExternalInfo as bi on bi.BillingID = b.BillingID
                        left join Registration as r on r.RegistrationID = b.RegistrationID
                        left join RegistrationInfo as ri on ri.RegistrationID = r.RegistrationID
                        left join ProductCode as pc on pc.ProductCode = t.ProductCode
                        left join ProductCodeInventory as pci on pci.ProductCodeID = pc.ProductCodeID
                        left join ProductCodeInfo as pcinf on  pcinf.ProductCodeID = pc.ProductCodeID
                        left join ProductProductCode as ppc on ppc.ProductCodeID = pc.ProductCodeID 
                        left join ProductCurrency as pcur on pcur.ProductId = ppc.ProductID
                        left join Currency as c on c.CurrencyID = pcur.CurrencyID
                        left join Inventory as i on i.InventoryID = pci.InventoryID
                        left join ProductSKU as psku on psku.ProductSKU = i.SKU 
                        left join InventorySKU as isku on isku.ProductSKU = psku.ProductSKU 
                        inner join ChargeHistoryEx ch on ch.ChargeHistoryID = t.ChargeHistoryID 
                        left join ChargeHistoryExSale chs on chs.SaleID = t.SaleID 
                        Where s.NotShip <> 1 and (s.TrackingNumber = '' or IsNull(s.TrackingNumber)) and b.Email <> 'dtcoder@gmail.com' and b.Email <> 'rob@trianglemediacorp.com' and b.Email <> 'nikhil@trianglemediacorp.com' and 
                         ((s.CreateDT >= '" + DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "') AND (s.CreateDT <= '" + DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "')) " +
                        "order by TotalQty DESC" 
                        ,
                    Connection = connection
                };

                adapter.SelectCommand = command;
                adapter.Fill(dataSet);

                GvReport.DataSource = dataSet;
                GvReport.DataBind();

                if (GvReport.HeaderRow != null)
                {
                    GvReport.HeaderRow.Cells[0].Text = "Sale ID";
                    GvReport.HeaderRow.Cells[1].Text = "Customer ID";
                    GvReport.HeaderRow.Cells[2].Text = "CPF";
                    GvReport.HeaderRow.Cells[3].Text = "Order Date";
                    GvReport.HeaderRow.Cells[4].Text = "Order Time";

                    GvReport.HeaderRow.Cells[5].Text = "First Name";
                    GvReport.HeaderRow.Cells[6].Text = "Last Name";
                    GvReport.HeaderRow.Cells[7].Text = "Ship Address 1";
                    GvReport.HeaderRow.Cells[8].Text = "Ship Address 2";
                    GvReport.HeaderRow.Cells[9].Text = "Ship Numero";

                    GvReport.HeaderRow.Cells[10].Text = "Ship Complemento";
                    GvReport.HeaderRow.Cells[11].Text = "Ship City";
                    GvReport.HeaderRow.Cells[12].Text = "Ship State";
                    GvReport.HeaderRow.Cells[13].Text = "Ship Postal/Zip";
                    GvReport.HeaderRow.Cells[14].Text = "Ship Bairro";

                    GvReport.HeaderRow.Cells[15].Text = "Country";
                    GvReport.HeaderRow.Cells[16].Text = "Ship Telefone";
                    GvReport.HeaderRow.Cells[17].Text = "SKU";
                    GvReport.HeaderRow.Cells[18].Text = "Total Qty";
                    GvReport.HeaderRow.Cells[19].Text = "Order Net ";

                    GvReport.HeaderRow.Cells[20].Text = "Shipping (R$)";
                    GvReport.HeaderRow.Cells[21].Text = "Order Total (R$)";
                    GvReport.HeaderRow.Cells[22].Text = "Order Sequence";
                    GvReport.HeaderRow.Cells[23].Text = "Email";
                    GvReport.HeaderRow.Cells[24].Text = "IP";

                    GvReport.HeaderRow.Cells[25].Text = "Package Code";
                    GvReport.HeaderRow.Cells[26].Text = "Package Description";
                    GvReport.HeaderRow.Cells[27].Text = "Misc 1";
                    GvReport.HeaderRow.Cells[28].Text = "Misc 2";
                    GvReport.HeaderRow.Cells[29].Text = "Misc 3";

                    GvReport.HeaderRow.Cells[30].Text = "Product Description";
                    GvReport.HeaderRow.Cells[31].Text = "Customs Weight";
                    GvReport.HeaderRow.Cells[32].Text = "Customs Price";
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        public override string HeaderString
        {
            get { return "Fulfilment Export Report"; }
        }
    }
}