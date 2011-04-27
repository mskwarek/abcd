using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    sealed class CCommutationTable
    {
        private Dictionary<CNetworkPortIn, CNetworkPortOut> commutationTable;
        private static CCommutationTable instance = new CCommutationTable();

        private CCommutationTable()
        { }

        public static CCommutationTable Instance
        {
            get
            {
                return Instance;
            }
        }

        public void setCommutationTable(Dictionary<CNetworkPortIn, CNetworkPortOut> commutationTable)
        {
            this.commutationTable = commutationTable;
        }

        public int getOutputPortId(int VPI, int VCI)
        {
            int iD = 0;
            //metoda zwracac ma docelowo ID portu na ktory ma wyjsc dana komorka ATM
            return iD;
        }
        public void addConnection(CNetworkPortIn portIn, CNetworkPortOut portOut)
        {
            commutationTable.Add(portIn, portOut);
        }
        public void removeConnection(CNetworkPortIn portIn)
        {
            commutationTable.Remove(portIn);
        }

        public void passOnData(Data.CCharacteristicData data,CNetworkPortIn port )
        {
            commutationTable[port].send(data);
        }

        public void showAll()
        {
            
            foreach(int key in commutationTable.Keys)
            {
                Console.WriteLine("Port In :" + key + " Port Out :" + commutationTable.ElementAt(key));
            }
        }

    }
}
