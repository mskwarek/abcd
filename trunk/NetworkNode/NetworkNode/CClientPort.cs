using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{ // klasa po której będą dziedziczyć porty wej i wyj
    class CClientPort : CPort
    {
 
        public CClientPort(int id, bool busy) :base(id,busy) {}

 

    }
}
