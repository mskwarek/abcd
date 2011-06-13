using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{   [Serializable]
    public class CLinkInfo
    {
        public  int nodeNumber;
        public  int portNumber;
        public  String portType;


        public CLinkInfo(int nn, String pt, int pn)
        {
            nodeNumber = nn;
            portNumber = pn;
            portType = pt;

        }

        public override bool Equals(object obj)
        {
            CLinkInfo comparable = (CLinkInfo)obj;
            if (this.nodeNumber == comparable.nodeNumber && this.portNumber == comparable.portNumber && this.portType.Equals(comparable.portType))
                return true;
            else
                return false; 
        }
        
    }

}
