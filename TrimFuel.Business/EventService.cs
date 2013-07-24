using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Enums;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways;
using System.Net;

namespace TrimFuel.Business
{
    public class EventService : BaseService
    {
        public void RegistrationAndConfirmation(int? campaignID, int? productID, string email, string zip, string phone, string firstName, string lastName, long? registrationID, int? eventTypeID)
        {
            try
            {
                int? curProdID = productID;
                if (productID == null)
                {
                    if (campaignID == null)
                        return;
                    var campaign = Load<Campaign>(campaignID);
                    if (campaign == null)
                        return;
                    var subscription = Load<Subscription>(campaign.SubscriptionID);
                    if (subscription == null)
                        return;

                    curProdID = subscription.ProductID;
                }
                else
                {
                    curProdID = productID;
                }

                MySqlCommand q = new MySqlCommand("SELECT * FROM ProductEvents WHERE ProductID=@ProductID AND EventTypeID=@EventTypeID LIMIT 1");
                q.Parameters.AddWithValue("@ProductID", curProdID);
                q.Parameters.AddWithValue("@EventTypeID", eventTypeID);
                var productEvent = dao.Load<ProductEvent>(q).SingleOrDefault();
                if (productEvent == null)
                    return;
                else
                {
                    var request = new StringBuilder(productEvent.URl);
                    request = request.Replace("#ZIP#", zip);
                    request = request.Replace("#TELEPHONE#", phone);
                    request = request.Replace("#EMAIL#", email);
                    request = request.Replace("#LASTNAME#", lastName);
                    request = request.Replace("#FIRSTNAME#", firstName);

                    string url = request.ToString();
                    if (!url.Contains("http:"))
                        url = "http://" + url;

                    string responseString = string.Empty;
                    using (WebClient wc = new WebClient())
                    {
                        responseString = wc.DownloadString(url);
                    }
                    //var response = new POSTGETRequest.GetRequest().Send(url);

                    ////test
                    //int count = 0;
                    //int length = (int)response.ContentLength;
                    //var bytes = new byte[length];
                    //while (count != length)
                    //{
                    //    count += response.GetResponseStream().Read(bytes, count, length - count);
                    //}
                    //string responseString = ASCIIEncoding.Default.GetString(bytes);                    
                    q = new MySqlCommand("Insert into EventPost (Request, Response, CreateDT, RegistrationID) Values (@Request, @Response, @CreateDT, @RegistrationID);");
                    q.Parameters.Add("@Request", MySqlDbType.VarChar).Value = url;
                    q.Parameters.Add("@Response", MySqlDbType.VarChar).Value = responseString;
                    q.Parameters.Add("@CreateDT", MySqlDbType.DateTime).Value = DateTime.Now;
                    q.Parameters.Add("@RegistrationID", MySqlDbType.Int32).Value = registrationID;
                    dao.ExecuteNonQuery(q); 
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }
    }
}
