using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace NetworkNode
{
    class CNetworkPortOut:CNetworkPort
    {
        private static TcpClient client;
        private static String ip = "127.0.0.1";
        private NetworkStream stream;
        private StreamWriter clientStream;

        //konstruktor
        public CNetworkPortOut(int id, bool busy) : base(id, busy) { }

        public void init(int portNumber) // metoda łącząca socket z portNumber
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, portNumber);
                stream = client.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error.... " + e.StackTrace);
            }
        }

        public void shutdown()
        {
            client.Close();
        }

        // metoda nawiązująca połączenie z węzłem sieciowym i nadająca do niego TO DO
        public void send(String str)
        {
            Console.WriteLine("nadaje " + str);
            clientStream = new StreamWriter(stream);
            clientStream.WriteLine(str);
            clientStream.Flush();
        }
        private void setCurrentVPI_VCI(Data.CCharacteristicData data)
        {

        }

        public void send( Data.CCharacteristicData data )
        {
            setCurrentVPI_VCI(data);

            //send
        }
    }
}
