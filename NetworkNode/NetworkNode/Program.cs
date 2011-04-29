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
            System.Console.WriteLine("Hello, World!");
            System.Console.WriteLine("You entered the following {0} command line arguments: ", args.Length );
            for (int i = 0; i < args.Length; i++)
            {
                System.Console.WriteLine("{0}", args[i]);
                
                
            }
            CPortManager.Instance.getNodePortConfiguration();
            System.Console.ReadKey();
            return 0;
        }
    }
}
