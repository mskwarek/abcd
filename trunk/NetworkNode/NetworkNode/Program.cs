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
            CConstrains.nodeNumber = 1;
            System.Console.WriteLine("Network Node!");
           
            CPortManager.Instance.getNodePortConfiguration();
           
            CManagementAgent.Instance.resetCommutationTable();
            
            System.Console.ReadKey();
            return 0;
        }
    }
}
