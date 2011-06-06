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
            String exitMessage = "Exiting...";
            
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
            
           
            CManagementAgent.Instance.resetCommutationTable();
            CManagementAgent.Instance.sendHelloMsgToML(CConstrains.nodeNumber);

            while (true)
            {
                String input = Console.ReadLine();
                if (input.StartsWith("startHelloMsg"))
                {
                    CNetManager.Instance.init();
                }
                else if (input.StartsWith("stopHelloMsg"))
                {
                    CNetManager.Instance.stopSending();
                }
                /*else if (input.StartsWith("turnOn"))
                {
                    CManagementAgent.Instance.sendHelloMsgToML(CConstrains.nodeNumber);
                }*/
                else if (input.Equals("q"))
                {
                    Console.WriteLine(exitMessage);
                    Environment.Exit(1);
                }
                else if (input.StartsWith("show portCfg"))
                {
                    CPortManager.Instance.getNodePortConfiguration();
                }
                
            }

            System.Console.ReadKey();
            return 0;
        }
    }
}
