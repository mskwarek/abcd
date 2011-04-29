using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
namespace NetworkNode
{ // klasa po której będą dziedziczyć porty wej i wyj
    class CClientPort : CPort
    {

        private Queue<CUserData> queue = new Queue<CUserData>();

        public CClientPort(int id, bool busy) :base(id,busy) {}

 

    }
}
