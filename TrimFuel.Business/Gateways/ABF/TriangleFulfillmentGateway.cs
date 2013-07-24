using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.ABF
{
    public class TriangleFulfillmentGateway : ABFGateway
    {
        protected override string GetConfig_ThreePLKey(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.TF_ThreePLKey].Value;
        }
        protected override string GetConfig_Login(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.TF_Login].Value;
        }
        protected override string GetConfig_Password(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.TF_Password].Value;
        }
        protected override int GetConfig_FacilityID(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return Convert.ToInt32(config[ShipperConfigEnum.TF_FacilityID].Value);
        }
        protected override int GetConfig_ThreePLID(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return Convert.ToInt32(config[ShipperConfigEnum.TF_ThreePLID].Value);
        }

        public override bool IsCheckShippedImplemented
        {
            get
            {
                return false;
            }
        }
    }
}
