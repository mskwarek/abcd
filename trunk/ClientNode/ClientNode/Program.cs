using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    class Program
    {
        

        static void Main(string[] args)
        {
            try
            {
                CConstrains.nodeNumber = Convert.ToInt32("1");
            }
            catch (Exception e)
            {
                Console.WriteLine("Argument musi być liczbą!!");
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
            CConsole.Instance.consoleInit();


        }
    }
}
