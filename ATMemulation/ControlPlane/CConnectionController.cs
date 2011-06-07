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
        CLinkResourceManager cLinkResourceManager = CLinkResourceManager.Instance;
        CRoutingController cRoutingController = CRoutingController.Instance;
        
        private static CConnectionController connectionController= new CConnectionController();

        private CConnectionController()
        {
            Thread t = new Thread(nccListener);
            t.Name = "NCC Listener";
            t.Start();

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

        //metoda zadajaca zestawienia polaczenia. uzywana w trybie hierarchicznym
        //parametry: para SNP
        //zwraca: polaczenie podsieciowe
        public void ConnectionRequestOut(int SNP_s, int SNP_d)
        {
            
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
        public void LinkConnectionRequest( CLink SNPtoSNP )
        {
            cLinkResourceManager.SNPLinkConnectionRequest(SNPtoSNP);
        }

        public void LinkConnectionDeallocation(CLink SNPtoSNP)
        {
            cLinkResourceManager.SNPLinkConnectionDeallocation(SNPtoSNP);
        }

        // listener żądań od NCC jedno, czy wielowatkowy?
        private void nccListener()
        {
            bool status = true;
            IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);
            TcpListener portListener = new TcpListener(ip, CConstrains.CCportNumber);
            portListener.Start();
            Console.WriteLine(" Control Plane nasluchuje na porcie : " + CConstrains.CCportNumber);

            while (status)
            {
                TcpClient client = portListener.AcceptTcpClient();
                new ClientHandler(client);
            }
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
