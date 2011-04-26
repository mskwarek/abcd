using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class CCommutationTable
    {
        private Dictionary<int, int> commutationTable;


        public void setCommutationTable(Dictionary<int,int> commutationTable)
        {
            this.commutationTable = commutationTable;
        }

        public int getOutputPortId(int VPI, int VCI)
        {
            int iD = 0;
            //metoda zwracac ma docelowo ID portu na ktory ma wyjsc dana komorka ATM
            return iD;
        }
        public void addConnection(int portIn, int portOut)
        {
            commutationTable.Add(portIn, portOut);
        }
        public void removeConnection(int portIn)
        {
            commutationTable.Remove(portIn);
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
