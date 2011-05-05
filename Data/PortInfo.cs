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
        public PortInfo(string portInfoStr)
        {
            //portInfoStr == "portID;VCI;VPI"
            this.portID = Convert.ToInt32(portInfoStr.Split(';')[0]);
            this.VCI = Convert.ToInt32(portInfoStr.Split(';')[1]);
            this.VPI = Convert.ToInt32(portInfoStr.Split(';')[2]);
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

        public override string ToString()
        {
            // "portID;VCI;VPI"
            return Convert.ToString(this.portID) + ";" + Convert.ToString(this.VCI) + ";" + Convert.ToString(this.VPI);
        } 
        // zmiana testowa do poprawienia
// rozwiązanie maka
        public bool Equals(PortInfo other) {

            if (this.getPortID() == other.getPortID() && this.getVPI() == other.getVPI() && this.getVCI() == other.getVCI())
            {
                return true;
            }
            else
            {
                return false;
            }
        
        }

        
        public override int GetHashCode()
        {
            return this.getVCI().GetHashCode() + this.getVPI().GetHashCode() + this.getPortID().GetHashCode();
        }
            
// rozwiązanie festera            
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
