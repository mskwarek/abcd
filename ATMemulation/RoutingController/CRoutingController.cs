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
        private Logger.CLogger logger = Logger.CLogger.Instance;

        private CRoutingController()
        {

        }

        public static CRoutingController Instance
        {
            get
            {
                return cRoutingController;
            }
        }

        //inicjalizacja RC
        public void initRC( List<CPNNITable> PNNITable )
        {
            List<CLink> newLinkList = new List<CLink>();
            
            foreach (CPNNITable i in PNNITable)
            {
                if (i.IsNeighbourActive)
                {
                    CLinkInfo from = new CLinkInfo(i.NodeNumber, i.NodeType, i.NodePortNumberSender);
                    CLinkInfo to = new CLinkInfo(i.NeighbourNodeNumber, i.NeighbourNodeType, i.NeighbourPortNumberReciever);
                    newLinkList.Add(new CLink(from, to, 1));
                }
            }
            CShortestPathCalculatorWrapper.Instance.linkList = newLinkList;
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

        // aktualizacja listy polaczen
        public void updateRCTable(CPNNITable PNNITable)
        {
            CLinkInfo from = new CLinkInfo(PNNITable.NodeNumber, PNNITable.NodeType, PNNITable.NodePortNumberSender);
            CLinkInfo to = new CLinkInfo(PNNITable.NeighbourNodeNumber, PNNITable.NeighbourNodeType, PNNITable.NeighbourPortNumberReciever);

            CShortestPathCalculatorWrapper.Instance.updateLink(new CLink(from, to, 1), PNNITable.IsNeighbourActive);
            logger.print(null, "Routing Table UPDATED : " + PNNITable.NodeNumber + " " + PNNITable.NodeType + " " + PNNITable.NodePortNumberSender + " " + PNNITable.NeighbourNodeNumber + " " + PNNITable.NeighbourNodeType + " " + PNNITable.NeighbourPortNumberReciever + " " + PNNITable.IsNeighbourActive, (int)Logger.CLogger.Modes.normal);
            
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
