using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{

    //liczy najkrotsza sciezke i ada
    public sealed class CShortestPathCalculatorWrapper
    {
        static readonly CShortestPathCalculatorWrapper instance = new CShortestPathCalculatorWrapper();

        CShortestPathCalculatorWrapper()
        {
        }

        public static CShortestPathCalculatorWrapper Instance
        {
            get
            {
                return instance;
            }
        }


        // zmienia stan danego CLink na busy
        public void reserveCLink(CLink cLink)
        {
            int index = CNetworkConfiguration.Instance.linkList.IndexOf(cLink);
            cLink = CNetworkConfiguration.Instance.linkList[index];
            cLink.isBusy = true;
            CNetworkConfiguration.Instance.linkList.RemoveAt(index);
            CNetworkConfiguration.Instance.linkList.Add(cLink);
        }

        // zmienia stan danego CLink na ready
        public void releaseCLink(CLink cLink)
        {
            int index = CNetworkConfiguration.Instance.linkList.IndexOf(cLink);
            cLink = CNetworkConfiguration.Instance.linkList[index];
            cLink.isBusy = false;
            CNetworkConfiguration.Instance.linkList.RemoveAt(index);
            CNetworkConfiguration.Instance.linkList.Add(cLink);
        }

        public RouteEngine.Route getShortestPath(int startNodeNumber, int endNodeNumber)
        {
            HashSet<int> locationIDs = new HashSet<int>();
            RouteEngine.Route shortestPath;
            // dodaj obecnie niezajete CLink
            foreach (CLink cLink in CNetworkConfiguration.Instance.linkList)
            {  
                if (!cLink.isBusy)
                {
                    RouteEngine.RouteEngine.Instance.Connections.Add(cLink);
                    locationIDs.Add(cLink.from.nodeNumber);
                    locationIDs.Add(cLink.to.nodeNumber);
                }
            }
            // dodaj lokalizacje na podstawie CLink
            foreach (int nodeID in locationIDs)
            {
                RouteEngine.RouteEngine.Instance.Locations.Add(new RouteEngine.Location(nodeID));
            }

            Dictionary<int, RouteEngine.Route> shortestPaths = RouteEngine.RouteEngine.Instance.CalculateMinCost(new RouteEngine.Location(startNodeNumber));

            if( shortestPaths.ContainsKey(endNodeNumber ) )
            {
                if (shortestPaths[endNodeNumber].Cost != int.MaxValue)
                    shortestPath = shortestPaths[endNodeNumber];
                else
                {
                    RouteEngine.RouteEngine.Instance.Connections.Clear();
                    RouteEngine.RouteEngine.Instance.Locations.Clear();

                    return null;
                }

            }
            else
            {
                RouteEngine.RouteEngine.Instance.Connections.Clear();
                RouteEngine.RouteEngine.Instance.Locations.Clear();

                return null;
            }

            RouteEngine.RouteEngine.Instance.Connections.Clear();
            RouteEngine.RouteEngine.Instance.Locations.Clear();

            return shortestPath;
        }

    }
}
