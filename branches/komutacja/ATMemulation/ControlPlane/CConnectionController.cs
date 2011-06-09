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
        private static CConnectionController connectionController = new CConnectionController();

        private CLinkResourceManager cLinkResourceManager = CLinkResourceManager.Instance;
        private CRoutingController cRoutingController = CRoutingController.Instance;

        private Dictionary<int, RouteEngine.Route> establishedRoutes = new Dictionary<int, RouteEngine.Route>();

        // numer polaczenia // Dict numer wezla + tablica komutacji
        public struct commutationEntry
        {
            public int identifier;
            public int nodeNumber;
            public PortInfo portIn, portOut;

            public commutationEntry(int identifier, int nodeNumber, PortInfo portIn, PortInfo portOut)
            {
                this.identifier = identifier;
                this.portIn = portIn;
                this.portOut = portOut;
                this.nodeNumber = nodeNumber;
            }
        }

        private List<commutationEntry> commutationTables = new List<commutationEntry>();

        public List<commutationEntry> CommutationTables
        {
            get
            {
                return commutationTables;
            }
            set
            {
                commutationTables = value;
            }
        }

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
            if (ConnectionRequestOut(fromNode, toNode))
                return true;
            else
                return false;
        }

        //metoda zadajaca zestawienia polaczenia. uzywana w trybie hierarchicznym
        //parametry: para SNP
        //zwraca: polaczenie podsieciowe
        public bool ConnectionRequestOut(int SNP_s, int SNP_d)
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
                    return false;
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

                        addConnection(nodeNumber, portIn, VPIIn, VCIIn, portOut, VPIOut, VCIOut, identifier); 
                                                
                    }
                    Console.WriteLine(" Connection " + identifier + " established ");
                    return true;
                }
            }
            else
                return false;
            
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

        public void removeRouteByIdentifier(int identifier)
        {
            if (establishedRoutes.ContainsKey(identifier))
                establishedRoutes.Remove(identifier);
            
        }

        public void addConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B, int identifier)
        {
            Console.WriteLine("\n portIn " + portNumber_A + " VPI_A/VCI_A " + VPI_A + "/" + VCI_A);
            Console.WriteLine(" portOut " + portNumber_B + " VPI_B/VCI_B " + VPI_B + "/" + VCI_B);
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            commutationTables.Add(new commutationEntry(identifier, nodeNumber, portIn, portOut));

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

        public void removeConnection(commutationEntry commutationEntry)
        {
            Data.PortInfo portIn = commutationEntry.portIn;
            Data.PortInfo portOut = commutationEntry.portOut;
            int nodeNumber = commutationEntry.nodeNumber;
            int identifier = commutationEntry.identifier;

                Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
                {"from", portIn},
                {"to", portOut},
                {"remove", null}
                };
                List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
                pduList.Add(pduDict);
                Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
                dataToSend.pdu.RequestIdentifier = "REMOVE" + nodeNumber.ToString();

                send(nodeNumber, dataToSend);

                Console.WriteLine("node : " + nodeNumber + " from : " + portIn.getPortID() + " to : " + portOut.getPortID());
            
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
