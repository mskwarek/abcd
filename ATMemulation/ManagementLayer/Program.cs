using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            CNetworkConfiguration cnc = new CNetworkConfiguration();
            cnc.readConfig();
            cnc.showList();

            cnc.addNodeToDictionary(1, "net");
            cnc.addNodeToDictionary(2, "cli");
            cnc.addNodeToDictionary(1, "cli");
            cnc.addNodeToDictionary(3, "cli");



        }
    }
}
