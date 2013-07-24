using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kaboom.Store1.Logic;
using TrimFuel.Business.Utils;

namespace Kaboom.Store1
{
    public partial class _default : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HideErrors();
            if (Page.IsPostBack && Products == null)
            {
                LoadProducts();
            }
        }

        public IEnumerable<KeyValuePair<ShoppingCartProduct, int>> Products { get; set; }
        private void LoadProducts()
        {
            Dictionary<ShoppingCartProduct, int> loadProducts = new Dictionary<ShoppingCartProduct, int>();
            if (Utility.TryGetInt(DropDownList1.SelectedValue) > 0)
                loadProducts.Add(ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x2_30_Trial], Utility.TryGetInt(DropDownList1.SelectedValue).Value);
            if (Utility.TryGetInt(DropDownList2.SelectedValue) > 0)
                loadProducts.Add(ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x12_60_Upsell], Utility.TryGetInt(DropDownList2.SelectedValue).Value);
            if (Utility.TryGetInt(DropDownList3.SelectedValue) > 0)
                loadProducts.Add(ShoppingCart.KnownProducts[KnownProduct.KaboomStrips_1x12_Upsell], Utility.TryGetInt(DropDownList3.SelectedValue).Value);
            if (Utility.TryGetInt(DropDownList4.SelectedValue) > 0)
                loadProducts.Add(ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x60_Upsell], Utility.TryGetInt(DropDownList4.SelectedValue).Value);
            if (Utility.TryGetInt(DropDownList5.SelectedValue) > 0)
                loadProducts.Add(ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x30_Upsell], Utility.TryGetInt(DropDownList5.SelectedValue).Value);

            Products = loadProducts;
        }

        private void HideErrors()
        {
            phError.Visible = false;
        }

        private void ShowError(string error)
        {
            lError.Text = error;
            phError.Visible = true;
        }

        protected void lbCheckout_Click(object sender, EventArgs e)
        {
            if (ValidateProducts())
            {
                ShoppingCart.Products = Products;
                ShoppingCart.Save();
                Response.Redirect("order.aspx");
            }
        }

        private bool ValidateProducts()
        {
            if (Products.Count() == 0)
            {
                ShowError("No products have been chosen. Please select your desired product and try again.");
                return false;
            }

            return true;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            Products = ShoppingCart.Products;

            FillQuantityDDL(DropDownList1, 0, 1, ShoppingCart.GetProductQuantity(KnownProduct.KaboomCombo_1x2_30_Trial));
            FillQuantityDDL(DropDownList2, 0, 9, ShoppingCart.GetProductQuantity(KnownProduct.KaboomCombo_1x12_60_Upsell));
            FillQuantityDDL(DropDownList3, 0, 9, ShoppingCart.GetProductQuantity(KnownProduct.KaboomStrips_1x12_Upsell));
            FillQuantityDDL(DropDownList4, 0, 9, ShoppingCart.GetProductQuantity(KnownProduct.KaboomDaily_1x60_Upsell));
            FillQuantityDDL(DropDownList5, 0, 9, ShoppingCart.GetProductQuantity(KnownProduct.KaboomDaily_1x30_Upsell));
        }

        private void FillQuantityDDL(DropDownList ddl, int minQuantity, int maxQuantity, int? selectedQuantity)
        {
            ddl.Items.Clear();
            for (int i = minQuantity; i < maxQuantity + 1; i++)
			{			 
                ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
			}

            if (selectedQuantity != null)
            {
                if (selectedQuantity.Value < minQuantity)
                    selectedQuantity = minQuantity;
                if (selectedQuantity.Value > maxQuantity)
                    selectedQuantity = maxQuantity;
            }
            ddl.SelectedValue = ((selectedQuantity != null) ? selectedQuantity.Value : minQuantity).ToString();
        }
    }
}
