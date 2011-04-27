using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class CNetworkPortIn : CNetworkPort
    {
        public CNetworkPortIn(int id, Boolean status):base(id, status)
        {}

        public void receiveData(Data.CCharacteristicData data) //metoda odbierajace dane 
        {
            CCommutationTable.Instance.passOnData(data, this);
        }
    
    } 
}
