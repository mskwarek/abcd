using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    class CLink : IEquatable<CLink>
    {
        public CLinkInfo from;
        public CLinkInfo to;
        int _weight;
        private bool _state;


        public CLink(CLinkInfo f, CLinkInfo t, int weight)
        {
            from = f;
            to = t;
            _weight = weight;
        }
        
        private bool state
        {
            get { return _state; }
            set { _state = value; }
        }

        public CLinkInfo B
        {
            get { return to; }
            set { to = value; }
        }

        public CLinkInfo A
        {
            get { return from; }
            set { from = value; }
        }       

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
    }
}
