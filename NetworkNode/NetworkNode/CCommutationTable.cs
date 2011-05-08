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
       private Data.CCommutationTable commutationTable;


       private CCommutationTable()
       {
           commutationTable = new Data.CCommutationTable(new PortInfo.EqualityComparer());
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
                portOut = new PortInfo(0, 0, 0);
                portOut = null;
                try
                {
                    Exception ex = new Exception();
                    throw ex;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Brak zadanego wpisu w tablicy komutacji " + e.StackTrace);
                }
            }
            return portOut;
        }

        public void setCommutationTable(Data.CCommutationTable commutationTable)
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

        public void removeConnection(PortInfo portIn, PortInfo portOut)
        {
            Console.WriteLine("Removing connection");
            if (commutationTable[portIn].Equals(portOut))
            {
                commutationTable.Remove(portIn);
            }
        }

        public Data.CCommutationTable getCommutationTable()
        {
            return commutationTable;
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
