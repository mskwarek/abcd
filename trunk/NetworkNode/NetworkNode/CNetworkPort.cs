using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
namespace NetworkNode
{
    public class CNetworkPort : CPort
    {
        public Queue<CCharacteristicData> queue = new Queue<CCharacteristicData>();

        public CNetworkPort(int id, bool busy) :base(id,busy) {}

        
    }
}
