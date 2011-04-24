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
            System.Console.WriteLine("Hello, World!");
            System.Console.WriteLine("You entered the following {0} command line arguments: ", args.Length );
            for (int i = 0; i < args.Length; i++)
            {
                System.Console.WriteLine("{0}", args[i]);
            }
            System.Console.ReadKey();
            return 0;
        }
    }
}
