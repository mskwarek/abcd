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
       private Logger.CLogger logger = Logger.CLogger.Instance;

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
            //Console.WriteLine("*** Getting output port Info ***");
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
                }


                catch (Exception)
                {
                    logger.print(null,"Brak zadanego wpisu w tablicy komutacji !!!",(int)Logger.CLogger.Modes.error );
                }
            }
            return portOut;
        }

        public void setCommutationTable(Data.CCommutationTable commutationTable)
        {
            logger.print(null, "Setting connection", (int)Logger.CLogger.Modes.normal);
            this.commutationTable = commutationTable;
        }

        public void resetCommutationTable()
        {
            logger.print(null, "Reseting connection", (int)Logger.CLogger.Modes.normal);
            commutationTable.Clear();
        }

        public void addEntry(PortInfo portIn, PortInfo portOut)
        {
            logger.print(null, "Adding entry", (int)Logger.CLogger.Modes.normal);
            commutationTable.Add(portIn, portOut);
        }

        public void removeConnection(PortInfo portIn, PortInfo portOut)
        {
            logger.print(null, "Removing connection", (int)Logger.CLogger.Modes.normal);
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
                    String text = "Port In :" + key.getPortID() + " VPI :" + key.getVPI() + " VCI :" + key.getVCI() + " || Port Out :" + portOut.getPortID() + " VPI :" + portOut.getVPI() + " VCI :" + portOut.getVCI();
                    logger.print(null, text, (int)Logger.CLogger.Modes.background);
                }
                
            }
        }

    }
}
