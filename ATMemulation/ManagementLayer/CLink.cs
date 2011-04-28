using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    class CLink
    {
        public CLinkInfo from;
        public CLinkInfo to;


        public CLink(CLinkInfo f, CLinkInfo t)
        {
            from = f;
            to = t;
        }
    }
}
