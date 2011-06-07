using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using Data;
using RouteEngine;

namespace RoutingController
{
    public sealed class CRoutingController
    {
        private static CRoutingController cRoutingController = new CRoutingController();

        private CRoutingController()
        {
            Thread t = new Thread(routingInfoListener);
            t.Name = "routingInfoListener";
            t.Start();
        }

        public static CRoutingController Instance
        {
            get
            {
                return cRoutingController;
            }
        }

        //inicjalizacja RC
        public void initRC( List<Data.CLink> linkList )
        {
            CShortestPathCalculatorWrapper.Instance.linkList = linkList;
        }

        // listener nasluchuje komunikatow o routingu
        private void routingInfoListener()
        {
            bool status = true;
            IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);
            TcpListener portListener = new TcpListener(ip, CConstrains.CPportNumber);
            portListener.Start();
            Console.WriteLine(" Control Plane nasluchuje na porcie : " + CConstrains.CPportNumber);

            while (status)
            {
                TcpClient client = portListener.AcceptTcpClient();
                new ClientHandler(client);
            }

        }
       // na zapytanie zrodlo-cel odpowiada zbiorem polaczen niezbednych do osiagniecia celu ze zrodla
        public RouteEngine.Route RouteTableQuery(int source, int destination)
        {
            return CShortestPathCalculatorWrapper.Instance.getShortestPath(source, destination);
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



















        public void LocalTopologyIn()
        {

        }

        public void LocalTopologyOut()
        {

        }


        public void NetworkTopologyIn()
        {

        }

        public void NetworkTopologyOut()
        {

        }

        


    }
}
