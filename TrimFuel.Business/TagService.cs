using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business
{
    public class TagService : BaseService
    {
        public IList<Tag> GetTagList()
        {
            IList<Tag> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select t.* from Tag t " +
                    "order by t.TagValue");

                res = dao.Load<Tag>(q);
            }
            catch (Exception ex)
            {
                res = null;
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public IList<TagGroup> GetTagGroupList()
        {
            IList<TagGroup> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select tg.* from TagGroup tg " +
                    "order by tg.TagGroupValue");

                res = dao.Load<TagGroup>(q);
            }
            catch (Exception ex)
            {
                res = null;
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public IList<Tag> GetTagListByBilling(long billingID)
        {
            IList<Tag> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select t.* from Tag t " +
                    "inner join TagBillingLink tb on tb.TagID = t.TagID " +
                    "where tb.BillingID = @billingID " +
                    "order by t.TagValue");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<Tag>(q);
            }
            catch (Exception ex)
            {
                res = null;
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public IList<Tag> GetTagListByTagGroup(int tagGroupID)
        {
            IList<Tag> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select t.* from Tag t " +
                    "inner join TagGroupTagLink tgt on tgt.TagID = t.TagID " +
                    "where tgt.TagGroupID = @tagGroupID");
                q.Parameters.Add("@tagGroupID", MySqlDbType.Int32).Value = tagGroupID;

                res = dao.Load<Tag>(q);
            }
            catch (Exception ex)
            {
                res = null;
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public IList<TagGroup> GetTagGroupListByTag(int tagID)
        {
            IList<TagGroup> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select tg.* from TagGroup tg " +
                    "inner join TagGroupTagLink tgt on tgt.TagGroupID = tg.TagGroupID " +
                    "where tgt.TagID = @tagID");
                q.Parameters.Add("@tagID", MySqlDbType.Int32).Value = tagID;

                res = dao.Load<TagGroup>(q);
            }
            catch (Exception ex)
            {
                res = null;
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public void SetTagListToTagGroup(int tagGroupID, IList<int> tagIDList)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("delete from TagGroupTagLink " +
                    "where TagGroupID = @tagGroupID");
                q.Parameters.Add("@tagGroupID", MySqlDbType.Int32).Value = tagGroupID;
                dao.ExecuteNonQuery(q);

                foreach (int tagID in tagIDList)
                {
                    TagGroupTagLink link = new TagGroupTagLink()
                    {
                        TagGroupID = tagGroupID,
                        TagID = tagID
                    };
                    
                    dao.Save<TagGroupTagLink>(link);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public void SetTagGroupListToTag(int tagID, IList<int> tagGroupIDList)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("delete from TagGroupTagLink " +
                    "where TagID = @tagID");
                q.Parameters.Add("@tagID", MySqlDbType.Int32).Value = tagID;
                dao.ExecuteNonQuery(q);

                foreach (int tagGroupID in tagGroupIDList)
                {
                    TagGroupTagLink link = new TagGroupTagLink()
                    {
                        TagGroupID = tagGroupID,
                        TagID = tagID
                    };

                    dao.Save<TagGroupTagLink>(link);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public void SetTagListToBilling(long billingID, IList<int> tagIDList)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("delete from TagBillingLink " +
                    "where BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int32).Value = billingID;
                dao.ExecuteNonQuery(q);

                foreach (int tagID in tagIDList)
                {
                    TagBillingLink link = new TagBillingLink()
                    {
                        TagID = tagID,
                        BillingID = billingID
                    };

                    dao.Save<TagBillingLink>(link);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public Tag AddTag(string tagValue)
        {
            Tag res = null;

            try
            {
                dao.BeginTransaction();

                res = new Tag()
                {
                    TagValue = tagValue
                };

                dao.Save<Tag>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public Tag UpdateTag(int tagID, string tagValue)
        {
            Tag res = null;

            try
            {
                dao.BeginTransaction();

                res = EnsureLoad<Tag>(tagID);
                res.TagValue = tagValue;

                dao.Save<Tag>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public bool DeleteTag(int tagID)
        {
            bool res = false;

            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("delete from TagGroupTagLink " +
                    "where TagID = @tagID; " +
                    "delete from TagBillingLink " +
                    "where TagID = @tagID; " +
                    "delete from Tag " +
                    "where TagID = @tagID;");
                q.Parameters.Add("@tagID", MySqlDbType.Int32).Value = tagID;

                dao.ExecuteNonQuery(q);

                res = true;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }

            return res;
        }
    }
}
