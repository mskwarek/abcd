using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Net;
using System.IO;
using RoutingController;
using LinkResourceManager;

namespace ControlPlane
{
    class CNetworkCallController
    {
        CLinkResourceManager cLinkResourceManager = CLinkResourceManager.Instance;
        RouteHandler routeHandler = RouteHandler.Instance;
        CConnectionController cCConectionController = CConnectionController.Instance;
        static CNetworkCallController instance = new CNetworkCallController();
        public List<Data.CPNNITable> PNNIList = new List<Data.CPNNITable>();
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
            TcpListener portListener = new TcpListener(ip, CConstrains.NCCportNumber);
            portListener.Start();
            Console.WriteLine("*** NCC nasłuchuje na porcie : " + CConstrains.NCCportNumber + " *** ");
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
                else if (dane.pdu.RequestIdentifier.StartsWith("PNNIList ML"))
                {


                    foreach (Data.CPNNITable t in dane.pdu.PNNIList)
                    {
                        if (PNNIList.Contains(t))
                        {
                            int index = PNNIList.IndexOf(t);

                            if (PNNIList.ElementAt(index).IsNeighbourActive != t.IsNeighbourActive)
                            {
                                PNNIList.ElementAt(index).IsNeighbourActive = t.IsNeighbourActive;
                                Console.WriteLine("*** PNNIList Updated  " + PNNIList.ElementAt(index).NodeNumber + " " + PNNIList.ElementAt(index).NodeType + " ACTIVITY :  " + PNNIList.ElementAt(index).IsNeighbourActive);
                            }
                        }
                        else
                        {
                            PNNIList.Add(t);
                            Console.WriteLine("PNNIList  ADDED : " + t.NodeNumber + " " + t.NodeType + " " + t.NodePortNumberSender + " " + t.NeighbourNodeNumber + " " + t.NeighbourNodeType + " " + t.NeighbourPortNumberReciever + " " + t.IsNeighbourActive);

                        }
                        downStream.WriteLine("--> SENDING RESPONSE : " + dane.pdu.RequestIdentifier + " -OK- ");
                        downStream.Flush();
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

        public void CallTeardownOut(int source, int destination)
        {
            
            //potrzeba metody ktora zamienii source i destination na cala sciezke od source do destination
            //niby w route handlerze jest lista routow, ale duzo operacji chyba trzeba robic zeby wyluskac
            //to co trzeba, moze dodac do klasy route pole source i destination i napisac jakas metode?

        
        }




    }
}
