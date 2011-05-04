using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                CConstrains.nodeNumber = Convert.ToInt32(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Argument musi być liczbą!!");
                Console.WriteLine(e.StackTrace);
            }
            System.Console.WriteLine("Network Node!");
           
            CPortManager.Instance.getNodePortConfiguration();
           
            CManagementAgent.Instance.resetCommutationTable();

            ////----------------test

            CCommutationTable.Instance.addEntry( new Data.PortInfo(1, 0, 0), new Data.PortInfo(2, 0, 4) );
            CCommutationTable.Instance.addEntry( new Data.PortInfo(2, 0, 1), new Data.PortInfo(2, 0, 1) );
            CCommutationTable.Instance.addEntry( new Data.PortInfo(1, 0, 3), new Data.PortInfo(1, 0, 3)) ;

            CCommutationTable.Instance.showAll();

            //\\----------------test
            System.Console.ReadKey();
            return 0;
        }
    }
}
