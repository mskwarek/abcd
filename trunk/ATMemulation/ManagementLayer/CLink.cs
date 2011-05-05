using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    public class CLink : IEquatable<CLink>
    {
        public CLinkInfo from;
        public CLinkInfo to;
        int _weight;
        // do wytyczania ścieżek
        private bool _isBusy;


        public CLink(CLinkInfo f, CLinkInfo t, int weight)
        {
            from = f;
            to = t;
            _weight = weight;
        }

        public bool isBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; }
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

        public bool Equals(CLink other)
        {
            if (this.from.nodeNumber == other.from.nodeNumber && this.from.portNumber == other.from.portNumber && this.to.portNumber == other.to.portNumber && this.to.nodeNumber == other.to.nodeNumber)
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
