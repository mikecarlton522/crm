using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class WebStoreProductReviewDataProvider : EntityDataProvider<WebStoreProductReview>
    {
        private const string INSERT_COMMAND = "INSERT INTO WebStoreProductReview (WebStoreProductID, Address, Email, DisplayName, SkinType, SkinTone, Sex, AgeRange, Rating, Title, Summary, Review, CreateDT, Confirmed) VALUES (@WebStoreProductID, @Address, @Email, @DisplayName, @SkinType, @SkinTone, @Sex, @AgeRange, @Rating, @Title, @Summary, @Review, @CreateDT, @Confirmed); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE WebStoreProductReview SET WebStoreProductID=@WebStoreProductID, Address=@Address, Email=@Email, DisplayName=@DisplayName, SkinType=@SkinType, SkinTone=@SkinTone, Sex=@Sex, AgeRange=@AgeRange, Rating=@Rating, Title=@Title, Summary=@Summary, Review=@Review, CreateDT=@CreateDT, Confirmed=@Confirmed WHERE WebStoreProductReviewID=@WebStoreProductReviewID;";
        private const string SELECT_COMMAND = "SELECT * FROM WebStoreProductReview WHERE WebStoreProductReviewID=@WebStoreProductReviewID;";

        public override void Save(WebStoreProductReview entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.WebStoreProductReviewID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@WebStoreProductReviewID", MySqlDbType.Int32).Value = entity.WebStoreProductReviewID;
            }

            cmd.Parameters.Add("@WebStoreProductID", MySqlDbType.Int32).Value = entity.WebStoreProductID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.DateTime).Value = entity.CreateDT;
            cmd.Parameters.Add("@Address", MySqlDbType.VarChar).Value = entity.Address;
            cmd.Parameters.Add("@AgeRange", MySqlDbType.VarChar).Value = entity.AgeRange;
            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = entity.Email;
            cmd.Parameters.Add("@Rating", MySqlDbType.VarChar).Value = entity.Rating;
            cmd.Parameters.Add("@Review", MySqlDbType.VarChar).Value = entity.Review;
            cmd.Parameters.Add("@Sex", MySqlDbType.VarChar).Value = entity.Sex;
            cmd.Parameters.Add("@SkinTone", MySqlDbType.VarChar).Value = entity.SkinTone;
            cmd.Parameters.Add("@SkinType", MySqlDbType.VarChar).Value = entity.SkinType;
            cmd.Parameters.Add("@Summary", MySqlDbType.VarChar).Value = entity.Summary;
            cmd.Parameters.Add("@Title", MySqlDbType.VarChar).Value = entity.Title;
            cmd.Parameters.Add("@Confirmed", MySqlDbType.Bit).Value = entity.Confirmed;

            if (entity.WebStoreProductReviewID == null)
            {
                entity.WebStoreProductReviewID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("WebStoreProductReview({0}) was not found in database.", entity.WebStoreProductReviewID));
                }
            }
        }

        public override WebStoreProductReview Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@WebStoreProductReviewID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override WebStoreProductReview Load(System.Data.DataRow row)
        {
            WebStoreProductReview res = new WebStoreProductReview();

            if (!(row["WebStoreProductReviewID"] is DBNull))
                res.WebStoreProductReviewID = Convert.ToInt32(row["WebStoreProductReviewID"]);
            if (!(row["WebStoreProductID"] is DBNull))
                res.WebStoreProductID = Convert.ToInt32(row["WebStoreProductID"]);
            if (!(row["Address"] is DBNull))
                res.Address = Convert.ToString(row["Address"]);
            if (!(row["AgeRange"] is DBNull))
                res.AgeRange = Convert.ToString(row["AgeRange"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["Email"] is DBNull))
                res.Email = Convert.ToString(row["Email"]);
            if (!(row["Rating"] is DBNull))
                res.Rating = Convert.ToString(row["Rating"]);
            if (!(row["Review"] is DBNull))
                res.Review = Convert.ToString(row["Review"]);
            if (!(row["Sex"] is DBNull))
                res.Sex = Convert.ToString(row["Sex"]);
            if (!(row["SkinTone"] is DBNull))
                res.SkinTone = Convert.ToString(row["SkinTone"]);
            if (!(row["SkinType"] is DBNull))
                res.SkinType = Convert.ToString(row["SkinType"]);
            if (!(row["Summary"] is DBNull))
                res.Summary = Convert.ToString(row["Summary"]);
            if (!(row["Title"] is DBNull))
                res.Title = Convert.ToString(row["Title"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Confirmed"] is DBNull))
                res.Confirmed = Convert.ToBoolean(row["Confirmed"]);

            return res;
        }
    }
}
