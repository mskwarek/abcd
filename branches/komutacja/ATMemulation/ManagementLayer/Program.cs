using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ManagementLayer
{
    class Program
    {
        static void Main(string[] args)
        {

            //while (true)
            //{
            //    String input = Console.ReadLine();
            //    if (input.StartsWith("stop"))
            //    {
            //        Console.WriteLine("zawiera " + input);
            //    }
            //}
            Console.Title = "Management Layer";
            CMLConsole.Instance.consoleInit();
            
         }


    }
}
