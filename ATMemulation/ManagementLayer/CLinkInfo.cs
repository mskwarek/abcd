using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    class CLinkInfo
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
   
        
    }

}
