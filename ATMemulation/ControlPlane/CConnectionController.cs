using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using RoutingController;
using LinkResourceManager;
using Data;
using System.Runtime.Serialization.Formatters.Binary;

namespace ControlPlane
{
    class CConnectionController
    {
        private CLinkResourceManager cLinkResourceManager = CLinkResourceManager.Instance;
        private CRoutingController cRoutingController = CRoutingController.Instance;

        private Dictionary<int, RouteEngine.Route> establishedRoutes = new Dictionary<int, RouteEngine.Route>();
        
        private static CConnectionController connectionController= new CConnectionController();
    
        Queue<int> VCIPole = new Queue<int>();
        Queue<int> VPIPole = new Queue<int>();

        private CConnectionController()
        {
            for (int i = 1; i <= Data.CAdministrationData.VCI_MAX; i++)
            {
                if ( i != 5)
                    VCIPole.Enqueue(i);
            }

            for (int i = 1; i <= Data.CAdministrationData.VPI_NNI_MAX; i++)
            {
                VPIPole.Enqueue(i);
            }

            Console.WriteLine("ConnectionController");
        }

        public static  CConnectionController Instance
        {
            get
            {
                return connectionController;
            }
        }

        //metoda do wymiany pomiedzy CC
        // parametry: para SNP, SNP i SNPP, para SNPP
        // zwraca: potwierdzenie
        public bool PeerCoordinationOut(int SNP_s, int SNP_d)
        {
            bool confirmation=false;
            return confirmation;
        }

        public bool ConnectionRequestIn(int fromNode, int toNode)
        {
            if (ConnectionRequestOut(fromNode, toNode)!=null)
                return true;
            else
                return false;
        }

        //metoda zadajaca zestawienia polaczenia. uzywana w trybie hierarchicznym
        //parametry: para SNP
        //zwraca: polaczenie podsieciowe
        public Object ConnectionRequestOut(int SNP_s, int SNP_d)
        {
            RouteEngine.Route route = RouteTableQuery(SNP_s, SNP_d);
            if (route != null && route.Connections.Count != 0)
            {
                Console.WriteLine(" Route " + SNP_s + " to " + SNP_d + " set up ");
                List<CLink> links = route.Connections;

                int i = 0;
                CLink link;
                Boolean failed = false;
                int identifier = setIdentifier(SNP_s, SNP_d);
                System.Console.WriteLine("Identifier is " + identifier);
                do
                {
                    link = links[i];
                    CLink temp;
                    if ((temp = LinkConnectionRequest(link)) == null)
                        failed = true;
                    i++;
                } while (failed != true && i < links.Count);
                if (failed)
                {
                    for (int j = 0; j < i; j++)
                    {
                        link = links[i];
                        LinkConnectionDeallocation(link);
                    }
                    return null;
                }
                else
                {
                    establishedRoutes.Add(identifier, route);

                    //tworzenie tablic komutacji

                    int[] VPIs =new int[links.Count];
                    int[] VCIs =new int[links.Count];

                    for( int a = 0 ; a < links.Count ; a++)
                    {
                        VPIs[a] = VPIPole.Dequeue();
                        VCIs[a] = VCIPole.Dequeue();
                    }
                    
                    for ( int a = 0; a < links.Count - 1; a++)
                    {
                        int nodeNumber = links[a].B.nodeNumber;
                        int portIn = links[a].B.portNumber;
                        int portOut = links[a+1].A.portNumber;
                        int VPIIn = VPIs[a];
                        int VCIIn = VCIs[a];
                        int VPIOut = VPIs[a+1];
                        int VCIOut = VCIs[a+1];
                        if (a == 0 )
                        {
                            VCIIn = 0;
                            VPIIn = 0;
                        }
                        if ( a == links.Count -2 )
                        {
                            VCIOut = 0;
                            VPIOut = 0;
                        }
                        addConnection(nodeNumber, portIn, VPIIn, VCIIn, portOut, VPIOut, VCIOut); 
                    }
                    Console.WriteLine(" Connection " + identifier + " established ");
                    return true;
                }
            }
            else
                return null;
            
        }

        //metoda zwraca identyfikator połączenia
        public int setIdentifier(int SNP_s, int SNP_d)
        {
            return SNP_s * SNP_d;
        }

        //metoda kierowana do RC by uzyskac sciezke pomiedzy dwoma punktami 
        //parametry: 'unresolved route fragment'
        //zwraca: zbior SNPP
        public RouteEngine.Route RouteTableQuery(int source, int destination)
        {
            return cRoutingController.RouteTableQuery(source, destination);
        }

        //metoda do zestawienia polaczenia? kierowana do LRM
        //parametry:brak
        //zwraca: link connection ( pare SNP)
        public CLink LinkConnectionRequest( CLink SNPtoSNP )
        {
            return cLinkResourceManager.SNPLinkConnectionRequest(SNPtoSNP);
        }

        public CLink LinkConnectionDeallocation(CLink SNPtoSNP)
        {
            return cLinkResourceManager.SNPLinkConnectionDeallocation(SNPtoSNP);
        }

        // listener żądań od NCC jedno, czy wielowatkowy?
        private void nccListener()
        {
            bool status = true;
            IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);
            TcpListener portListener = new TcpListener(ip, CConstrains.NCCportNumber);
            portListener.Start();
            Console.WriteLine(" Control Plane nasluchuje na porcie : " + CConstrains.NCCportNumber);

            while (status)
            {
                TcpClient client = portListener.AcceptTcpClient();
                new ClientHandler(client);
            }
        }

        public RouteEngine.Route getRouteByIdentifier(int identifier)
        {
            if (establishedRoutes.ContainsKey(identifier))
                return establishedRoutes[identifier];
            else
                return null;
        }

        public void addConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B)
        {
            Console.WriteLine("\n portIn " + portNumber_A + " VPI_A/VCI_A " + VPI_A + "/" + VCI_A);
            Console.WriteLine(" portOut " + portNumber_B + " VPI_B/VCI_B " + VPI_B + "/" + VCI_B);
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"from", portIn},
            {"to", portOut},
            {"add", null}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = "ADD" + nodeNumber.ToString();

            send(nodeNumber, dataToSend);

            Console.WriteLine("node : " + nodeNumber + " from : " + portNumber_A + " to : " + portNumber_B);
        }
        private void send(int nodeNumber, Data.CSNMPmessage msg)
        {
            int portNumber = 50000 + 100 * nodeNumber;
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, portNumber);
            NetworkStream stream = client.GetStream();

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            //stream.Flush();
            Console.WriteLine("--> SENDING " + msg + " TO NODE : " + nodeNumber);

        }

    }


    class ClientHandler
    {
        public ClientHandler(TcpClient client)
        {
            handling(client);
        }
        
        private void handling(TcpClient client)
        {
            // nasluch
        }
    }
}
