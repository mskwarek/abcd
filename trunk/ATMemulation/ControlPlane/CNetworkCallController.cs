using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Net;
using System.IO;

namespace ControlPlane
{
    class CNetworkCallController
    {

        CConnectionController cCConectionController = CConnectionController.Instance;
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
            Thread t = new Thread(NCCListener);
            t.Name = " Network Call Controller listener";
            t.Start();
        }

        public void NCCListener() 
        {
            bool status = true;
            IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);     //adres serwera
            TcpListener portListener = new TcpListener(ip, CConstrains.CCportNumber);
            portListener.Start();
            Console.WriteLine("*** NCC nasłuchuje na porcie : " + CConstrains.CCportNumber + " *** ");
            while (status)
            {
                TcpClient client = portListener.AcceptTcpClient();
                NetworkStream clientStream = client.GetStream();
                StreamWriter downStream = new StreamWriter(clientStream);
                Console.WriteLine("*** CONNECTION FROM CPCC ACCEPTED ***");
                BinaryFormatter binaryFormater = new BinaryFormatter();
                Data.CSNMPmessage dane = (Data.CSNMPmessage)binaryFormater.Deserialize(clientStream);
                if (dane.pdu.RequestIdentifier.StartsWith("CallRequest"))
                {
                    foreach (Dictionary<String, Object> d in dane.pdu.variablebinding)
                    {
                        if (d.ContainsKey("requestNewLink"))
                        {
                            int fromNode = Convert.ToInt16(d["FromNode"]);
                            int toNode = Convert.ToInt16(d["ToNode"]);
                            //Metoda zlecająca CC ustanowienie połączenia.
                            if (ConnectionRequest(fromNode, toNode))
                            {
                                downStream.WriteLine("OK");
                                downStream.Flush();
                            }
                            else
                            {
                                downStream.WriteLine("ERROR");
                                downStream.Flush();
                            }
                        }
                    }
                }
                Console.WriteLine(dane.pdu.RequestIdentifier);
                Thread.Sleep(1000);
            }
        }



        public void CallIndication()
        { }
        

        // tu wywołujemy metody CC aby dalej zestawić połączenie - na końcu musi CC zwrócić true albo false
        public bool ConnectionRequest(int fromNode, int toNode)
        {

            if (cCConectionController.ConnectionRequestIn(fromNode, toNode))
                return true;
            else
                return false;

        }


        //uzywane przy wielu domenach
        public void NetworkCallCoordinationOut()
        { 
            
            
        
        }

        // metoda zamieniajaca nazwe lokalna na identyfikator polaczenia 
        // kierowane do directory
        public void DirectoryRequest(string localName)
        { }

        public void CallTeardownOut()
        { }




    }
}
