using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{ // klasa po której będą dziedziczyć porty wej i wyj
    public class CClientPort : CPort
    {

        public Queue<Data.CUserData> queue = new Queue<Data.CUserData>();

        public CClientPort(int id, bool busy) :base(id,busy) {}

    }
}
