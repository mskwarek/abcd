using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using RoutingController;
using LinkResourceManager;
using Data;

namespace ControlPlane
{
    class CConnectionController
    {
        private CLinkResourceManager cLinkResourceManager = CLinkResourceManager.Instance;
        private CRoutingController cRoutingController = CRoutingController.Instance;

        private Dictionary<int, RouteEngine.Route> establishedRoutes = new Dictionary<int, RouteEngine.Route>();
        
        private static CConnectionController connectionController= new CConnectionController();

        Queue<int> VCIPole;
        Queue<int> VPIPole;

        private CConnectionController()
        {
            for (int i = 0; i <= Data.CAdministrationData.VCI_MAX; i++)
            {
                VCIPole.Enqueue(i);
            }

            for (int i = 0; i <= Data.CAdministrationData.VPI_NNI_MAX; i++)
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
            if (route != null)
            {
                Console.WriteLine(" Route " + SNP_s + " to " + SNP_d + " set up ");
                List<CLink> links = route.Connections;
               
                
                // zamiana lacz na tablice komutacji
                //Dictionary<int, Dictionary<Data.PortInfo, Data.PortInfo>> listOfCommutationT = new Dictionary<int, Dictionary<PortInfo, PortInfo>>();

                

                for (int k = 0; k < links.Count; k++)
                {
                    CLink connection = links[k];
                    tablica[] listatablic= new tablica[k+1];
                    listatablic[connection.from.nodeNumber].port_d=connection.from.portNumber;
                    listatablic[connection.to.nodeNumber].port_s = connection.to.portNumber;
                    int VCItemp=VCIPole.Dequeue();
                    int VPItemp=VPIPole.Dequeue();

                    listatablic[connection.from.nodeNumber].vci_d = VCItemp;
                    listatablic[connection.to.nodeNumber].vci_s = VCItemp;
                    listatablic[connection.from.nodeNumber].vpi_d = VPItemp;
                    listatablic[connection.to.nodeNumber].vpi_s = VPItemp;
                }

                //


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

    }
    struct tablica
    {
        public int port_s;
        public int port_d;
        public int vpi_s;
        public int vpi_d;
        public int vci_s;
        public int vci_d;
    };
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
