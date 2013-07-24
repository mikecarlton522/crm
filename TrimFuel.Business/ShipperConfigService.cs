using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class ShipperConfigService : BaseService
    {
        public List<ShipperConfig> GetAllConfigs()
        {
            List<ShipperConfig> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM ShipperConfig");
                res = dao.Load<ShipperConfig>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IDictionary<ShipperConfig.ID, ShipperConfig> GetShipperConfig(int shipperID)
        {
            IDictionary<ShipperConfig.ID, ShipperConfig> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM ShipperConfig where ShipperID = @shipperID");
                q.Parameters.Add("@shipperID", MySqlDbType.Int16).Value = shipperID;
                res = dao.Load<ShipperConfig>(q).ToDictionary(i => i.ShipperConfigID.Value);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void CheckShipperConfig(Shipper shipper, IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            IList<string> invalidConfigValues = new List<string>();
            foreach (var item in ShipperConfigList.Values.Where(i => i.ShipperID == shipper.ShipperID))
            {
                if (!config.ContainsKey(item) ||
                    config[item] == null ||
                    string.IsNullOrEmpty(config[item].Value))
                {
                    invalidConfigValues.Add(item.Key);
                }
            }
            if (invalidConfigValues.Count > 0)
            {
                throw new Exception(string.Format("Shipper({0}) is not configured properly. Configuration settings({1}) are not valid.", 
                    shipper.Name, 
                    string.Join(",", invalidConfigValues.ToArray())));
            }
        }
    }
}
