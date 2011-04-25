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

        public void receiveData() //metoda odbierajace dane 
        { }
    
    } 
}
