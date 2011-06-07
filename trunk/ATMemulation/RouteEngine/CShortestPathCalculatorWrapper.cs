using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;

namespace RouteEngine
{

    //liczy najkrotsza sciezke i ada
    public sealed class CShortestPathCalculatorWrapper
    {
        private List<Data.CLink> LinkList = new List<Data.CLink>();

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

        public List<Data.CLink> linkList
        {
            get
            {
                return LinkList;
            }
            set
            {
                linkList = value;
            }
        }

        // zmienia stan danego CLink na busy
        public void reserveCLink(CLink cLink)
        {
            int index = LinkList.IndexOf(cLink);
            cLink = LinkList[index];
            cLink.isBusy = true;
            LinkList.RemoveAt(index);
            LinkList.Add(cLink);
        }

        // zmienia stan danego CLink na ready
        public void releaseCLink(CLink cLink)
        {
            int index = LinkList.IndexOf(cLink);
            cLink = LinkList[index];
            cLink.isBusy = false;
            LinkList.RemoveAt(index);
            LinkList.Add(cLink);
        }

        public Route getShortestPath(int startNodeNumber, int endNodeNumber)
        {
            HashSet<int> locationIDs = new HashSet<int>();
            Route shortestPath;
            // dodaj obecnie niezajete CLink
            foreach (CLink cLink in LinkList)
            {  
                if (!cLink.isBusy)
                {
                    RouteEngine.Instance.Connections.Add(cLink);
                    locationIDs.Add(cLink.from.nodeNumber);
                    locationIDs.Add(cLink.to.nodeNumber);
                }
            }
            // dodaj lokalizacje na podstawie CLink
            foreach (int nodeID in locationIDs)
            {
                RouteEngine.Instance.Locations.Add(new Location(nodeID));
            }

            Dictionary<int, Route> shortestPaths = RouteEngine.Instance.CalculateMinCost(new Location(startNodeNumber));

            if( shortestPaths.ContainsKey(endNodeNumber ) )
            {
                if (shortestPaths[endNodeNumber].Cost != int.MaxValue)
                    shortestPath = shortestPaths[endNodeNumber];
                else
                {
                    RouteEngine.Instance.Connections.Clear();
                    RouteEngine.Instance.Locations.Clear();

                    return null;
                }

            }
            else
            {
                RouteEngine.Instance.Connections.Clear();
                RouteEngine.Instance.Locations.Clear();

                return null;
            }

            RouteEngine.Instance.Connections.Clear();
            RouteEngine.Instance.Locations.Clear();

            return shortestPath;
        }

    }
}
