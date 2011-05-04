using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Data;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClientNode
{
    // Klasa portu wyjściowego dziedzicząca po CClientPort

    class CClientPortOut : CClientPort
    {
        private static TcpClient client;
        private static String ip = "127.0.0.1";
        private NetworkStream stream;
        private StreamWriter clientStream;
        private int portNumber;
        //konstruktor
        public CClientPortOut(int id, bool busy): base(id,busy){}

        public void startPort(int portNumber)
        {
            this.portNumber = portNumber;
            init();
            //Thread t = new Thread(new ThreadStart(init));
            //t.Start();
        }
        
        private void init() // metoda łącząca socket z portNumber
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
        public void send(CUserData data)
        {
            Console.WriteLine("nadaje " + data);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, data);
            clientStream = new StreamWriter(stream);
            clientStream.WriteLine(data);
            clientStream.Flush();
        }
    }
}
