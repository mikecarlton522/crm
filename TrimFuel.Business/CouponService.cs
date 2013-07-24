using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TrimFuel.Model;

namespace TrimFuel.Business
{
    public class CouponService : BaseService
    {
        public IList<Coupon> GetCoupons()
        {
            IList<Coupon> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select c.* from Coupon c");

                res = dao.Load<Coupon>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Coupon> GetCampaignCoupons(int campaignID)
        {
            IList<Coupon> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select c.* from CampaignCoupon cc
                    left join Coupon c on c.ID = cc.CouponID
                    where cc.CampaignID = @CampaignID");
                q.Parameters.AddWithValue("@CampaignID", campaignID);

                res = dao.Load<Coupon>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BusinessError<Coupon> CreateCoupon(int? productID, string code, decimal? discount, decimal? newPrice)
        {
            BusinessError<Coupon> res = new BusinessError<Coupon>(null, BusinessErrorState.Error, null);

            try
            {
                dao.BeginTransaction();

                if (string.IsNullOrEmpty(code))
                {
                    res.ErrorMessage = "Code is not specified";
                }
                else if (GetByCode(code) != null)
                {
                    res.ErrorMessage = "Code is already occupied";
                }
                else
                {
                    Coupon coupon = new Coupon();

                    coupon.ProductID = productID;
                    coupon.Code = code;
                    coupon.Discount = discount;
                    coupon.NewPrice = newPrice;

                    dao.Save<Coupon>(coupon);

                    dao.CommitTransaction();

                    res.ReturnValue = coupon;
                    res.State = BusinessErrorState.Success;
                }                
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            
            return res;
        }

        public BusinessError<Coupon> UpdateCoupon(int couponID, int? productID, string code, decimal? discount, decimal? newPrice)
        {
            BusinessError<Coupon> res = new BusinessError<Coupon>(null, BusinessErrorState.Error, null);

            try
            {
                dao.BeginTransaction();

                Coupon coupon = EnsureLoad<Coupon>(couponID);

                coupon.ProductID = productID;
                coupon.Code = code;
                coupon.Discount = discount;
                coupon.NewPrice = newPrice;

                dao.Save<Coupon>(coupon);

                res.ReturnValue = coupon;
                res.State = BusinessErrorState.Success;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            
            return res;
        }

        public BusinessError<int> DeleteCoupon(int couponID)
        {
            BusinessError<int> res = new BusinessError<int>(0, BusinessErrorState.Error, null);
            
            try
            {
                //TODO: implement delete

                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"delete from Coupon where ID=@CouponID");
                q.Parameters.Add("@CouponID", MySqlDbType.Int32).Value = couponID;
                dao.ExecuteNonQuery(q);

                dao.CommitTransaction();

                res.ReturnValue = couponID;
                res.State = BusinessErrorState.Success;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);

                dao.RollbackTransaction();
                
                res.ReturnValue = 0;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }

            return res;
        }

        public void CreateCampaignCoupon(int campaignID, int couponID)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("insert into CampaignCoupon (CampaignID, CouponID) values (@CampaignID, @CouponID)");
                q.Parameters.AddWithValue("@CampaignID", campaignID);
                q.Parameters.AddWithValue("@CouponID", couponID);

                dao.ExecuteNonQuery(q);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void DeleteCampaignCoupons(int campaignID)
        {
            try
            {
                //TODO: implement delete
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"delete from CampaignCoupon where CampaignID=@CampaignID");
                q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = campaignID;
                dao.ExecuteNonQuery(q);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();

                logger.Error(GetType(), ex);
            }
        }

        public Coupon GetByCode(string code)
        {
            Coupon res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select * from Coupon where Code = @code");
                
                q.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;

                res = dao.Load<Coupon>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                
                res = null;
            }

            return res;
        }
    }
}
