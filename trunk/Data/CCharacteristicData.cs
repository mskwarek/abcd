using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class CCharacteristicData
    {
        private CAdministrationData cAdministrationData;
        private CUserData cUserData;
        
        /******
         * Setters
         ******/

        void setCAdministrationData(CAdministrationData cAdministrationData)
        {
            this.cAdministrationData = cAdministrationData;
        }

        void setCUserData(CUserData cUserData)
        {
            this.cUserData = cUserData;
        }

        /******
         * Getters
         ******/

        CAdministrationData getCAdministrationData()
        {
            return cAdministrationData;
        }

        CUserData getCUserData()
        {
            return cUserData;
        }

    }
}
