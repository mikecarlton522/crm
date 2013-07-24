using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Dao.EntityDataProviders;
using TrimFuel.Model;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Web.Admin.Magento;
using log4net;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class AdminNews : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            LabelNoItems.Text = "";
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            DashboardService service = new DashboardService();
            IList<TPClientNews> tpClientNews = service.GetNewsList(AdminMembership.CurrentAdmin.AdminID.Value);
            var query =
                from news in tpClientNews
                group news by news.CreateDT
                    into g1
                    select new
                    {
                        createDT = g1.Key,
                        news = from item in g1
                               select new
                               {
                                   content = item.Content,
                                   newsID = item.TPClientNewsID
                               }
                    };
            if (query.ToList().Count() > 0)
            {
                rNews.DataSource = query.ToList();
                rNews.DataBind();
            }
            else
            {
                LabelNoItems.Text = "No alerts right now";
            }

        }


        protected void rNews_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if(e.CommandName == "Read")
            {
                int newsID = Convert.ToInt32(e.CommandArgument);
                DashboardService service = new DashboardService();
                int adminID = Convert.ToInt32(AdminMembership.CurrentAdmin.AdminID.Value);
                service.MarkNewsAsRead(newsID, adminID);
                
                DataBind();
                
            }
        }
    }
}
