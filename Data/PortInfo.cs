using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class PortInfo : IEquatable<PortInfo>
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
        public bool Equals( PortInfo other)
        {
            if (this.getPortID() == other.getPortID() && this.getVPI() == other.getVPI() && this.getVCI() == other.getVCI())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public int GetHashCode(Data.PortInfo portInfo)
        {
            return 1;
        }
    }
}
