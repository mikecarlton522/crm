using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace BeautyTruth.Store1.Controls
{
    public partial class CategoryPager : System.Web.UI.UserControl
    {
        public List<KeyValuePair<string, string>> Products { get; set; }
        public bool ViewAll { get; set; }
        public int CurrentPage { get; set; }
        public int CountOnPage { get; set; }
        public int SortType { get; set; }
        public int? CategoryID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (!ViewAll)
            {
                var totalPages = (int)Math.Ceiling(Products.Count / (double)CountOnPage);
                if (CurrentPage == 0)
                    CurrentPage = 1;
                toUppendDown.Controls.Add(ImplementPaging(totalPages));
                toUppendUp.Controls.Add(ImplementPaging(totalPages));

                Products = Products.Skip((CurrentPage - 1) * CountOnPage).Take(CountOnPage).ToList();
            }
            lvProducts.DataSource = Products;
            lvProducts.DataBind();
        }

        private Control ImplementPaging(int totalPages)
        {
            var ul = new HtmlGenericControl("ul");
            var li = new HtmlGenericControl("li");
            li.InnerHtml = string.Format("{0}-{1} of {2} Item(s) ", (CurrentPage - 1) * CurrentPage + 1, CurrentPage * CountOnPage > Products.Count ? Products.Count : CurrentPage * CountOnPage, Products.Count);
            ul.Controls.Add(li);
            for (int page = 1; page <= totalPages; page++)
            {
                var li1 = new HtmlGenericControl("li");
                var li2 = new HtmlGenericControl("li");
                if (page == CurrentPage)
                {
                    li1.Attributes.Add("class", "current");
                    li1.InnerHtml = page.ToString();
                }
                else
                {
                    var a = new HtmlGenericControl("a");
                    a.InnerHtml = page.ToString();
                    a.Attributes.Add("href", string.Format("category.aspx?page={0}&sort={1}&categoryID={2}", page, SortType, CategoryID));
                    li1.Controls.Add(a);
                }
                li2.InnerHtml = " | ";
                ul.Controls.Add(li1);
                ul.Controls.Add(li2);
            }

            if (totalPages != CurrentPage)
            {
                var li1 = new HtmlGenericControl("li");
                var li2 = new HtmlGenericControl("li");
                li1.Attributes.Add("class", "underline");
                var a = new HtmlGenericControl("a");
                a.InnerHtml = "Next";
                a.Attributes.Add("href", string.Format("category.aspx?page={0}&sort={1}&categoryID={2}", (CurrentPage + 1), SortType, CategoryID));
                li1.Controls.Add(a);
                li2.InnerHtml = " | ";
                ul.Controls.Add(li1);
                ul.Controls.Add(li2);
            }

            var liAll1 = new HtmlGenericControl("li");
            var liAll2 = new HtmlGenericControl("li");
            liAll1.Attributes.Add("class", "underline");
            var aAll = new HtmlGenericControl("a");
            aAll.InnerHtml = "View All";
            aAll.Attributes.Add("href", string.Format("category.aspx?viewAll=true&sort={0}&categoryID={1}", SortType, CategoryID));
            liAll1.Controls.Add(aAll);
            ul.Controls.Add(liAll1);
            ul.Controls.Add(liAll2);

            return ul;
        }
    }
}