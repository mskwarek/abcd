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

       // IN/OUT
       private Dictionary<PortInfo, PortInfo> commutationTable;


       private CCommutationTable()
       {
           commutationTable = new Dictionary<PortInfo, PortInfo>(new PortInfo.EqualityComparer());
       }

       public static CCommutationTable Instance
       {
           get
           {
               return instance;
           }
       }      
        

        public PortInfo getOutputPortInfo(PortInfo portIn)
        {
            Console.WriteLine("Getting output port Info");
            PortInfo portOut;
            if (commutationTable.ContainsKey(portIn))
            {
                portOut = commutationTable[portIn];
            }
            else
            {
                Exception ex = new Exception();
                throw ex;
            }
            
            return portOut;
        }

        public void setCommutationTable(Dictionary<PortInfo, PortInfo> commutationTable)
        {
            Console.WriteLine("Setting connection");
            this.commutationTable = commutationTable;
        }

        public void resetCommutationTable()
        {
            Console.WriteLine("Reseting connection");
            commutationTable.Clear();
        }

        public void addEntry(PortInfo portIn, PortInfo portOut)
        {
            Console.WriteLine("Adding entry");
            commutationTable.Add(portIn, portOut);
        }

        public void removeConnection(PortInfo portIn)
        {
            Console.WriteLine("Removing connection");
            commutationTable.Remove(portIn);
        }



        public void showAll()
        {

            foreach (PortInfo key in commutationTable.Keys)
            {
                PortInfo portOut;
                if (commutationTable.ContainsKey(key))
                {
                    portOut = commutationTable[key];
                    Console.WriteLine("Port In :" + key.getPortID() + " VPI :" + key.getVPI() + " VCI :" + key.getVCI() + " || Port Out :" + portOut.getPortID() + " VPI :" + portOut.getVPI() + " VCI :" + portOut.getVCI());
                }
                
            }
        }

    }
}
