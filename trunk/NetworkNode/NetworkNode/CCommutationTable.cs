using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class CCommutationTable
    {
        //<port_in, port_out>
        private Dictionary<int, int> commutationTable;
        //jak tu rozwiazac kwestie VPI/VCI?!?

        public void setCommutationTable(Dictionary<int, int> commutationTable)
        {
            this.commutationTable = commutationTable;
        }

        public int getOutputPortId(int VPI, int VCI)
        {
            int iD = 0;
            //metoda zwracac ma docelowo ID portu na ktory ma wyjsc dana komorka ATM
            return iD;
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
