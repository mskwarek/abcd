using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class CNetworkPortOut:CNetworkPort
    {
        private int _id;
        private Boolean _busy;

        public CNetworkPortOut(int id, Boolean busy):base(id, busy)    
        {
        }

        private void setCurrentVPI_VCI(Data.CCharacteristicData data)
        {

        }

        public void send( Data.CCharacteristicData data )
        {
            setCurrentVPI_VCI(data);

            //send
        }
    }
}
