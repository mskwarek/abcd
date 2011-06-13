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

        private int portNumber;
        //konstruktor
        public CClientPortOut(int id, bool busy): base(id,busy){}
        private Logger.CLogger logger = Logger.CLogger.Instance;
        public void startPort(int portNumber)
        {
            this.portNumber = portNumber;

        }
        
        // metoda nawiązująca połączenie z węzłem sieciowym i nadająca do niego TO DO
        public void send(CUserData data)
        {
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, portNumber);
            NetworkStream stream = client.GetStream();
            BinaryFormatter bf = new BinaryFormatter();
            logger.print(null,"--> SENDING : " + data,(int)Logger.CLogger.Modes.normal);
            bf.Serialize(stream, data);
            stream.Flush();

            List<byte> lista = new List<byte>();
            lista = data.getInformation();

            foreach (byte b in lista)
            {
                
                Console.Write(b + " ");
            }
            
        }

        public int getPortNumber()
        {
            return portNumber;
        }
    }


}
