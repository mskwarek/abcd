using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ManagementLayer
{
    class CHandleAgentConnection
    {
        TcpClient client;
        string clientNumber;

        private NetworkStream clientStream;

        public void startClient(TcpClient inClient, string clientNum)
        {
            this.client = inClient;
            this.clientNumber = clientNum;
            Thread clientThread = new Thread(handleClient);
            clientThread.Start();
        }

        private void handleClient()
        {
            clientStream = client.GetStream(); 
            StreamReader sr = new StreamReader(clientStream);
            String dane = sr.ReadLine();

            //obsługa tego co przyśle wynzeł
        }

    }
}
