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
            try
            {
                CConstrains.domainName = args[0];
                CConstrains.configFileURL = CConstrains.defaultconfigFileURL + CConstrains.domainName + CConstrains.xmlEnding;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : Brak argumentu!");
                Console.WriteLine(e.StackTrace);
            }
            //while (true)
            //{
            //    String input = Console.ReadLine();
            //    if (input.StartsWith("stop"))
            //    {
            //        Console.WriteLine("zawiera " + input);
            //    }
            //}
            Console.Title = "Management Layer domainName = " + CConstrains.domainName ;
            CMLConsole.Instance.consoleInit();
            
         }


    }
}
