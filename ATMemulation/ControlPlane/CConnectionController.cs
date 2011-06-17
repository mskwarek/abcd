using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using RoutingController;
using LinkResourceManager;
using Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ControlPlane
{
    class CConnectionController
    {
        private static CConnectionController connectionController = new CConnectionController();
        private Logger.CLogger logger = Logger.CLogger.Instance;
        private CRoutingController cRoutingController = CRoutingController.Instance;

        private Dictionary<int, RouteEngine.Route> establishedRoutes = new Dictionary<int, RouteEngine.Route>();

        static readonly object _locker = new object();

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
                if ( i != 5 && i!=22)
                    VCIPole.Enqueue(i);
            }

            for (int i = 1; i <= Data.CAdministrationData.VPI_NNI_MAX; i++)
            {
                if (i != 66)
                    VPIPole.Enqueue(i);
            }

            logger.print("ConnectionController",null,(int)Logger.CLogger.Modes.constructor);
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

            lock (_locker)
            {
                RouteEngine.Route route = RouteTableQuery(SNP_s, SNP_d);
                logger.print("RC", "Routing", (int)Logger.CLogger.Modes.background); 
                if (route != null && route.Connections.Count != 0)
                {
                    logger.print("RC", " Route " + SNP_s + " to " + SNP_d + " set up ", (int)Logger.CLogger.Modes.normal);
                    List<CLink> links = route.Connections;

                    int i = 0;
                    CLink link;
                    Boolean failed = false;
                    int identifier = setIdentifier(SNP_s, SNP_d);
                    logger.print("ConnectionRequestOut", "Identifier is " + identifier, (int)Logger.CLogger.Modes.normal);
                    do
                    {
                        link = links[i];
                        //temp
                        if (LinkConnectionRequest(link.A) == null || LinkConnectionRequest(link.B) == null)
                            failed = true;
                        i++;
                    } while (failed != true && i < links.Count);
                    if (failed)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            link = links[i];
                            LinkConnectionDeallocation(link.A);
                            LinkConnectionDeallocation(link.B);
                        }
                        return false;
                    }
                    else
                    {
                        establishedRoutes.Add(identifier, route);

                        //tworzenie tablic komutacji

                        int[] VPIs = new int[links.Count];
                        int[] VCIs = new int[links.Count];

                        for (int a = 0; a < links.Count; a++)
                        {
                            VPIs[a] = VPIPole.Dequeue();
                            VCIs[a] = VCIPole.Dequeue();
                        }


                        int nodeNumber = 0;
                        int portIn = 0;
                        int portOut = 0;
                        int VPIIn = 0;
                        int VCIIn = 0;
                        int VPIOut = 0;
                        int VCIOut = 0;
                        bool intra = false;
                        bool intraDomain = false;
                        for (int a = 0; a < links.Count - 1; a++)
                        {
                            RouteEngine.CShortestPathCalculatorWrapper.Instance.reserveCLink(links[a]);
                            //logger.print(null, "liczba laczy: " + links.Count, (int)Logger.CLogger.Modes.background); 

                            nodeNumber = links[a].B.nodeNumber;
                            portIn = links[a].B.portNumber;
                            portOut = links[a + 1].A.portNumber;
                            int borderNode = 555;
                            int borderNodeInPort = 777;
                            int borderNodeOutPort = 999;
                           

                            if (a == 0 && links[a].B.portType == "network" && !intra)
                            {
                                borderNode = links[a].A.nodeNumber;
                                foreach (CPNNITable t in CNetworkCallController.Instance.PNNIList)
                                {
                                    if (t.NodeNumber == borderNode && t.NeighbourNodeType == "border")
                                    {
                                        borderNodeInPort = t.NeighbourPortNumberReciever;
                                        break;
                                    }
                                }
                                portOut = links[a].A.portNumber;
                                VPIIn = 66;
                                VCIIn = 22;
                                VPIOut = VPIs[a];
                                VCIOut = VCIs[a];
                                logger.print(null, " domena 2: nodeNumber: " + borderNode + " port in " + portIn + "vpi in" + VPIIn + "vci in " + VCIIn + " port out " + portIn + "vpi out" + VPIIn + "vci out " + VCIIn, (int)Logger.CLogger.Modes.background); 
                                addConnection(borderNode, portIn, VPIIn, VCIIn, portOut, VPIOut, VCIOut, identifier);
                                intra = true;
                                a--;
                                continue;
                            }
                            if (intra)
                            {
                                VPIIn = VPIOut;
                                VCIIn = VCIOut;
                                VPIOut = VPIs[a+1];
                                VCIOut = VCIs[a + 1];
                                addConnection(nodeNumber, portIn, VPIIn, VCIIn, portOut, VPIOut, VCIOut, identifier);
                                logger.print(null, "nodeNumber: " + nodeNumber + " port in " + portIn + " vpi in " + VPIIn + " vci in " + VCIIn + " port out " + portIn + " vpi out " + VPIIn + " vci out " + VCIIn, (int)Logger.CLogger.Modes.background); 

                            }
                            else{
                         


                                if (!intraDomain)
                                {
                                    if (a == 0 )
                                    {

                                        VCIIn = 0;
                                        VPIIn = 0;
                                        VPIOut = VPIs[a];
                                        VCIOut = VCIs[a];
                                        
                                    }

                                    else
                                    {
                                        VPIIn = VPIs[a - 1];
                                        VCIIn = VCIs[a - 1];
                                        VPIOut = VPIs[a];
                                        VCIOut = VCIs[a];
                                        
                                    }
                                    addConnection(nodeNumber, portIn, VPIIn, VCIIn, portOut, VPIOut, VCIOut, identifier);
                                    logger.print(null, "nodeNumber: " + nodeNumber + " port in " + portIn + "vpi in" + VPIIn + "vci in " + VCIIn + " port out " + portIn + "vpi out" + VPIIn + "vci out " + VCIIn, (int)Logger.CLogger.Modes.background); 

                                    if (links[a + 1].A.portType != "client" && a == (links.Count - 2))
                                    {

                                       
                                        intraDomain = true;
                                        borderNode = links[a + 1].B.nodeNumber;
                                        borderNodeInPort = links[a + 1].B.portNumber;
                                       


                                        VPIIn = VPIOut;
                                        VCIIn = VCIOut;

                                        VPIOut = 66;
                                        VCIOut = 22;
                                        foreach (CPNNITable t in CNetworkCallController.Instance.PNNIList)
                                        {
                                            if (t.NodeNumber == borderNode && t.NeighbourNodeType == "border")
                                            {

                                                borderNodeOutPort = t.NodePortNumberSender;
                                                //logger.print(null, "node number :" + t.NodeNumber + " border port out :" + borderNodeOutPort, (int)Logger.CLogger.Modes.background); 
                                                break;
                                            }
                                        }


                                    }


                                }

                                if (intraDomain)
                                {
                                    
                                    addConnection(borderNode, borderNodeInPort, VPIIn, VCIIn, borderNodeOutPort, VPIOut, VCIOut, identifier);


                                }
                            }

                        }
                        logger.print("ConnectionRequestOut", " Connection " + identifier + " established ", (int)Logger.CLogger.Modes.normal);
                        return true;

                    }
                }
                else
                {
                    logger.print(null, "No such route!!", (int)Logger.CLogger.Modes.error); 
                    return false;

                }
            }
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
        public CLinkInfo LinkConnectionRequest(CLinkInfo SNP)
        {
            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"CCI", null},
            {"SNPLinkConnectionRequest", null},
            {"SNP", SNP}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = "SNPLinkConnectionRequest" + SNP.nodeNumber.ToString();

            logger.print("SNPLinkConnectionRequest", "request " + SNP.nodeNumber.ToString(), (int)Logger.CLogger.Modes.normal);

            send(SNP.nodeNumber, dataToSend);

            //Console.WriteLine("node : " + SNP.nodeNumber );
            return SNP;
        }

        public CLinkInfo LinkConnectionDeallocation( CLinkInfo SNP )
        {
            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"CCI", null},
            {"SNPLinkConnectionDeallocation", null},
            {"SNP", SNP}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = "SNPLinkConnectionDeallocation " + SNP.nodeNumber.ToString();

            logger.print("SNPLinkConnectionDeallocation", "SNPLinkConnectionDeallocation" + SNP.nodeNumber.ToString(), (int)Logger.CLogger.Modes.normal);
            send(SNP.nodeNumber, dataToSend);

            //Console.WriteLine("node : " + SNP.nodeNumber );

            return SNP;
            
            //TODO: Komunikacja
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

        public bool findRouteForNode(int nodeNumber)
        {

            foreach (RouteEngine.Route r in establishedRoutes.Values)
            {
                foreach (CLink l in r.Connections)
                {
                    if (l.A.nodeNumber == nodeNumber || l.B.nodeNumber == nodeNumber)
                    {
                        CNetworkCallController.Instance.CallTeardownOut(r.Connections[0].A.nodeNumber, r.Connections[r.Connections.Count - 1].B.nodeNumber);
                        CNetworkCallController.Instance.ConnectionRequest(r.Connections[0].A.nodeNumber, r.Connections[r.Connections.Count - 1].B.nodeNumber);
                    }
                }
            }
            return true;
        }


        public void addConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B, int identifier)
        {
            logger.print(null, "\n portIn " + portNumber_A + " VPI_A/VCI_A " + VPI_A + "/" + VCI_A, (int)Logger.CLogger.Modes.background);
            logger.print(null, " portOut " + portNumber_B + " VPI_B/VCI_B " + VPI_B + "/" + VCI_B, (int)Logger.CLogger.Modes.background);
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

            logger.print(null, "node : " + nodeNumber + " from : " + portNumber_A + " to : " + portNumber_B, (int)Logger.CLogger.Modes.background);
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

                logger.print(null, "node : " + nodeNumber + " from : " + portIn.getPortID() + " to : " + portOut.getPortID(), (int)Logger.CLogger.Modes.background);
            
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
            //logger.print(null,"--> SENDING " + msg + " TO NODE : " + nodeNumber, (int)Logger.CLogger.Modes.normal);

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
