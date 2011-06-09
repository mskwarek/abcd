using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{   [Serializable]
    public class CCharacteristicData
    {
        private CAdministrationData cAdministrationData;
        private CUserData cUserData;

        public CCharacteristicData(CAdministrationData cAdministrationData, CUserData cUserData)
        {
            this.cAdministrationData = cAdministrationData;
            this.cUserData = cUserData;
        }

        /******
         * Setters
         ******/

        public void setCAdministrationData(CAdministrationData cAdministrationData)
        {
            this.cAdministrationData = cAdministrationData;
        }

        public void setCUserData(CUserData cUserData)
        {
            this.cUserData = cUserData;
        }

        /******
         * Getters
         ******/

        public CAdministrationData getCAdministrationData()
        {
            return cAdministrationData;
        }

        public CUserData getCUserData()
        {
            return cUserData;
        }

    }
}
