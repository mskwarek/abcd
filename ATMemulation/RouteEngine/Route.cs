using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;

namespace RouteEngine
{
    public class Route
    {
        int _cost;
        List<CLink> _connections;
        string _identifier;

        public Route(int nodeNumber)
        {
            _identifier = nodeNumber.ToString();
            _cost = int.MaxValue;
            _connections = new List<CLink>();
            
        }


        public List<CLink> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }
        public int Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public override string ToString()
        {
            return "ID: " + _identifier + " Cost:" + Cost;
        }
    }
}
