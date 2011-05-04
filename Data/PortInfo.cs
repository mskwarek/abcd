using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class PortInfo
    {
        private int portID;
        private int VPI;
        private int VCI;

        public PortInfo(int portID, int VPI, int VCI)
        {
            this.portID = portID;
            this.VPI = VPI;
            this.VCI = VCI;
        }

        public int getPortID()
        {
            return portID;
        }

        public int getVCI()
        {
            return VCI;
        }

        public int getVPI()
        {
            return VPI;
        }
        // zmiana testowa do poprawienia
        public class EqualityComparer : IEqualityComparer<PortInfo>
        {

            public bool Equals(PortInfo x, PortInfo y)
            {
                return x.getPortID() == y.getPortID() && x.getVPI() == y.getVPI() && x.getVCI() == y.getVCI();
            }

            public int GetHashCode(PortInfo x)
            {
                return x.getPortID() ^ x.getVCI();
            }

        }
    }
}
