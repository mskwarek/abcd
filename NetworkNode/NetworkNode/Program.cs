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
            Console.Title = "NetworkNode ID = " + CConstrains.nodeNumber;
            CPortManager.Instance.getNodePortConfiguration();
           
            CManagementAgent.Instance.resetCommutationTable();

            System.Console.ReadKey();
            return 0;
        }
    }
}
