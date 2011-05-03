using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    class CLink : IEquatable<CLinkInfo>
    {
        public CLinkInfo from;
        public CLinkInfo to;


        public CLink(CLinkInfo f, CLinkInfo t)
        {
            from = f;
            to = t;
        }

        public bool Equals(CLink other)
        {
            if (this.from.nodeNumber == other.from.nodeNumber && this.from.portNumber == other.from.portNumber && this.to.portNumber == other.to.portNumber && this.to.nodeNumber == this.to.nodeNumber)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
