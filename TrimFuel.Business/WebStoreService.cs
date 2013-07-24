using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;

namespace TrimFuel.Business
{
    public class WebStoreService : BaseService
    {
        public List<WebStoreProduct> GetAllWebStoreProducts()
        {
            List<WebStoreProduct> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM WebStoreProduct");
                res = dao.Load<WebStoreProduct>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<WebStoreProduct>();
            }
            return res;
        }

        public List<WebStoreProduct> GetWebStoreProductsByWebStoreID(int? ID)
        {
            List<WebStoreProduct> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM WebStoreProduct Where CampaignID=@CampaignID");
                q.Parameters.AddWithValue("@CampaignID", ID);
                res = dao.Load<WebStoreProduct>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<WebStoreProduct>();
            }
            return res;
        }

        public WebStoreProduct GetWebStoreProductsByProductCode(string productCode)
        {
            WebStoreProduct res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM WebStoreProduct Where ProductCode=@ProductCode");
                q.Parameters.AddWithValue("@ProductCode", productCode);
                res = dao.Load<WebStoreProduct>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public List<WebStoreProduct> GetWebStoreProductsByCategoryID(int? categoryID, int? campaignID)
        {
            List<WebStoreProduct> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT wp.* FROM WebStoreProduct wp
                                                        inner join ProductCodeCategory pcc on pcc.ProductCode=wp.ProductCode
                                                        inner join ProductCategory pc on pcc.ProductCategoryID=pc.ProductCategoryID
                                                        inner join ProductCode p on pcc.ProductCode=p.ProductCode
                                                        inner join ProductCodeInfo pci on pci.ProductCodeID=p.ProductCodeID
                                                        where pc.ProductCategoryID=@ProductCategoryID
                                                        and wp.CampaignID=@CampaignID");
                q.Parameters.AddWithValue("@ProductCategoryID", categoryID);
                q.Parameters.AddWithValue("@CampaignID", campaignID);
                res = dao.Load<WebStoreProduct>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<WebStoreProduct>();
            }
            return res;
        }

        public int? GetProductCodeID(string productCode)
        {
            MySqlCommand q = new MySqlCommand("Select * from ProductCode Where ProductCode=@ProductCode");
            q.Parameters.AddWithValue("@ProductCode", productCode);
            var obj = dao.Load<ProductCode>(q).ToList().FirstOrDefault();
            if (obj == null)
                return null;
            return obj.ProductCodeID;
        }

        public List<ProductCodeInfo> GetAllProductCodeInfoByCampaignID(int? cid)
        {
            List<ProductCodeInfo> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreProduct w 
                                    inner join ProductCode p on w.ProductCode=p.ProductCode
                                    left join ProductCodeInfo pi on pi.ProductCodeID=p.ProductCodeID
                                    WHERE w.CampaignID=@CampaignID
                                    ORDER BY w.WebStoreProductID");
                    q.Parameters.AddWithValue("CampaignID", cid);
                    res = dao.Load<ProductCodeInfo>(q).ToList();
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public List<ProductCodeInfo> GetRandomProductCodeInfoByCampaignID(int? cid)
        {
            List<ProductCodeInfo> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreProduct w 
                                    left join ProductCode p on w.ProductCode=p.ProductCode
                                    left join ProductCodeInfo pi on pi.ProductCodeID=p.ProductCodeID
                                    Where w.CampaignID=@CampaignID
                                    Order by RAND()");
                    q.Parameters.AddWithValue("CampaignID", cid);
                    res = dao.Load<ProductCodeInfo>(q).ToList();
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public List<ProductCategory> GetAllWebStoreCategories(int? id)
        {
            List<ProductCategory> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT * FROM ProductCategory pc
                                                    where pc.ProductCategoryID in
                                                    (
                                                        Select pcc.ProductCategoryID from ProductCodeCategory pcc
                                                        inner join ProductCode p on pcc.ProductCode=p.ProductCode
                                                        inner join WebStoreProduct wp on wp.ProductCode=p.ProductCode
                                                        where wp.CampaignID=@CampaignID
                                                    )");
                q.Parameters.AddWithValue("@CampaignID", id);
                res = dao.Load<ProductCategory>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<ProductCategory>();
            }
            return res;
        }

        public List<ProductCategoryView> GetAllWebStoreCategoriesEx(int? id)
        {
            List<ProductCategoryView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreProduct wp
                                                   inner join ProductCodeCategory pcc on pcc.ProductCode=wp.ProductCode
                                                   inner join ProductCategory pc on pcc.ProductCategoryID=pc.ProductCategoryID
                                                   where wp.CampaignID=@CampaignID");
                q.Parameters.AddWithValue("@CampaignID", id);
                res = dao.Load<ProductCategoryView>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<ProductCategoryView>();
            }
            return res;
        }

        public List<ProductCodeInfo> GetAllProductCodeInfoByCategoryID(int? categoryID, int? campaignID)
        {
            List<ProductCodeInfo> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreProduct wp
                                                        inner join ProductCodeCategory pcc on pcc.ProductCode=wp.ProductCode
                                                        inner join ProductCategory pc on pcc.ProductCategoryID=pc.ProductCategoryID
                                                        inner join ProductCode p on pcc.ProductCode=p.ProductCode
                                                        inner join ProductCodeInfo pci on pci.ProductCodeID=p.ProductCodeID
                                                        where pc.ProductCategoryID=@ProductCategoryID
                                                        and wp.CampaignID=@CampaignID
                                                        ORDER BY wp.WebStoreProductID");
                    q.Parameters.AddWithValue("@ProductCategoryID", categoryID);
                    q.Parameters.AddWithValue("@CampaignID", campaignID);
                    res = dao.Load<ProductCodeInfo>(q).ToList();
                }
                catch (Exception ex)
                {
                    res = new List<ProductCodeInfo>();
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public List<ProductCodeInfo> GetRandomProductCodeInfoByCategoryID(int? categoryID, int? campaignID)
        {
            List<ProductCodeInfo> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreProduct wp
                                                        inner join ProductCodeCategory pcc on pcc.ProductCode=wp.ProductCode
                                                        inner join ProductCategory pc on pcc.ProductCategoryID=pc.ProductCategoryID
                                                        inner join ProductCode p on pcc.ProductCode=p.ProductCode
                                                        inner join ProductCodeInfo pci on pci.ProductCodeID=p.ProductCodeID
                                                        where pc.ProductCategoryID=@ProductCategoryID
                                                        and wp.CampaignID=@CampaignID
                                                        ORDER BY RAND()");
                    q.Parameters.AddWithValue("@ProductCategoryID", categoryID);
                    q.Parameters.AddWithValue("@CampaignID", campaignID);
                    res = dao.Load<ProductCodeInfo>(q).ToList();
                }
                catch (Exception ex)
                {
                    res = new List<ProductCodeInfo>();
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public string GetCategoryNameByID(int? id)
        {
            var obj = dao.Load<ProductCategory>(id);
            if (obj == null)
                return string.Empty;
            return obj.CategoryName;
        }

        public ProductCategory GetCategoryByProductCode(string productCode)
        {
            ProductCategory res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM ProductCategory pc
                                                        inner join ProductCodeCategory pcc on pcc.ProductCategoryID=pc.ProductCategoryID
                                                        and pcc.ProductCode=@ProductCode");
                    q.Parameters.AddWithValue("@ProductCode", productCode);
                    res = dao.Load<ProductCategory>(q).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public bool AddNewEmail(string email, int? webStoreID)
        {
            bool res = true;

            if (dao != null)
            {
                try
                {
                    WebStoreEmails existingEntity = null;

                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreEmails
                                                        where Email=@Email and WebStoreID=@WebStoreID");
                    q.Parameters.AddWithValue("@WebStoreID", webStoreID);
                    q.Parameters.AddWithValue("@Email", email);
                    existingEntity = dao.Load<WebStoreEmails>(q).FirstOrDefault();
                    if (existingEntity == null)
                    {
                        existingEntity = new WebStoreEmails()
                        {
                            WebStoreID = webStoreID,
                            Email = email
                        };
                        dao.Save<WebStoreEmails>(existingEntity);
                    }
                    else
                        res = false;
                }
                catch (Exception ex)
                {
                    res = false;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public List<WebStoreProductReview> GetWebStoreProductReviewsByWebStoreProductID(int? productID)
        {
            List<WebStoreProductReview> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreProductReview where WebStoreProductID=@WebStoreProductID");
                q.Parameters.AddWithValue("@WebStoreProductID", productID);
                res = dao.Load<WebStoreProductReview>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<WebStoreProductReview>();
            }
            return res;
        }

        public List<WebStoreProductReview> GetWebStoreProductReviews()
        {
            List<WebStoreProductReview> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT * FROM WebStoreProductReview");
                res = dao.Load<WebStoreProductReview>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<WebStoreProductReview>();
            }
            return res;
        }

        public void SaveWebStoreProductReview(WebStoreProductReview review)
        {
            try
            {
                dao.Save<WebStoreProductReview>(review);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void ConfirmWebStoreProductReview(int? reviewID)
        {
            try
            {
                var review = dao.Load<WebStoreProductReview>(reviewID);
                if (review != null)
                {
                    review.Confirmed = true;
                    dao.Save<WebStoreProductReview>(review);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void DeleteWebStoreProductReview(int? reviewID)
        {
            try
            {
                MySqlCommand q = new MySqlCommand("delete from WebStoreProductReview where WebStoreProductReviewID=@WebStoreProductReviewID");
                q.Parameters.AddWithValue("@WebStoreProductReviewID", reviewID);
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }
    }
}
