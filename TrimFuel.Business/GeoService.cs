using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class GeoService : BaseService
    {
        public Geo GetCountryByName(string countryName)
        {
            Geo res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from Geo g
                    where g.GeoTypeID = @geoType_Country and g.Name = @name
                ");
                q.Parameters.Add("@geoType_Country", MySqlDbType.Int32).Value = GeoTypeEnum.Country;
                q.Parameters.Add("@name", MySqlDbType.VarChar).Value = countryName;

                res = dao.Load<Geo>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public IList<Geo> GetCountryList()
        {
            IList<Geo> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from Geo g
                    where g.GeoTypeID = @geoType_Country
                ");
                q.Parameters.Add("@geoType_Country", MySqlDbType.Int32).Value = GeoTypeEnum.Country;

                res = dao.Load<Geo>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }
    }
}
