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
        private static IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);     //adres serwera
        private static int portNum =  CConstrains.LMportNumber;
        private static TcpListener portListener;
        private static TcpClient client;

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
            CMLConsole.Instance.consoleInit();

            portListener = new TcpListener(ip, portNum);
            portListener.Start();

            client = default(TcpClient);
            int counter = 0;

            Console.WriteLine("czeka na połączenia od agentów..");

            counter = 0;
            while (true)
            {
                client = portListener.AcceptTcpClient();
                counter += 1;
                Console.WriteLine("połączono z agentwem węzła nr: " + Convert.ToString(counter));
                CHandleAgentConnection handleConn = new CHandleAgentConnection();
                handleConn.startClient(client, Convert.ToString(counter));
            }
         }
    }
}
