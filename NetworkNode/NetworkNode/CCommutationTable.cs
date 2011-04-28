using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;

namespace NetworkNode
{
   public sealed class CCommutationTable
    {
       static readonly CCommutationTable instance = new CCommutationTable();

       static CCommutationTable()
       {
       }

       CCommutationTable()
       {
       }

       public static CCommutationTable Instance
       {
           get
           {
               return instance;
           }
       }

        // IN/OUT
        private Dictionary<PortInfo, PortInfo> commutationTable;
        

        public PortInfo getOutputPort(PortInfo portIn)
        {
            PortInfo portOut;
            if (commutationTable.ContainsKey(portIn))
            {
                portOut = commutationTable[portIn];
            }
            else
            {
                // Risky i nieladnie :P
                portOut = null; 
            }
            
            return portOut;
        }
        
        public void addEntry(PortInfo portIn, PortInfo portOut)
        {
            commutationTable.Add(portIn, portOut);
        }

        public void removeConnection(PortInfo portIn)
        {
            commutationTable.Remove(portIn);
        }

        //public void passOnData(Data.CCharacteristicData data, PortInfo port )
        //{
        //    commutationTable[port].send(data);
        //}

        public void showAll()
        {

            foreach (PortInfo key in commutationTable.Keys)
            {
                PortInfo portOut;
                if (commutationTable.ContainsKey(key))
                {
                    portOut = commutationTable[key];
                }
                Console.WriteLine("Port In :" + key.getPortID() + " Port Out :" + portOut.getPortID());
            }
        }

    }
}
