using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace ControlPlane
{
    class CNetworkCallController
    {

        static CNetworkCallController instance = new CNetworkCallController();

        public static CNetworkCallController Instance
        {
            get
            {
                return Instance;
            }
        }

        private CNetworkCallController()
        {
            Thread t = new Thread(ccListener);
            t.Name = "Call Controller listener";
            t.Start();
        }


        public void CallIndication()
        { }

        public void ConnectionRequestOut()
        {
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, CConstrains.CCportNumber);
            NetworkStream stream = client.GetStream();
            //wyslanie wiadomosc - pytanie jest jaki format ma byc tej wiadomosci?
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, new String[] { "connection request", "source identifier", "dest identifier" });
            stream.Flush();
            Console.WriteLine(" Sending Connection Request Out");
            stream.Close();
            client.Close();

        }


        //uzywane przy wielu domenach
        public void NetworkCallCoordinationOut()
        { }

        // metoda zamieniajaca nazwe lokalna na identyfikator polaczenia 
        // kierowane do directory
        public void DirectoryRequest(string localName)
        { }

        public void CallTeardownOut()
        { }

        public void ccListener()
        {
            //nasluch CC
        }


    }
}
