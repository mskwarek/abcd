using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;

namespace RouteEngine
{
    public sealed class RouteEngine
    {
        static readonly RouteEngine instance = new RouteEngine();

        public static RouteEngine Instance
        {
            get
            {
                return instance;
            }
        }

        List<CLink> _connections;
        List<Location> _locations;

        public List<Location> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }
        public List<CLink> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        RouteEngine()
        {
            _connections = new List<CLink>();
            _locations = new List<Location>();
        }



        /// <summary>
        /// Calculates the shortest route to all the other locations
        /// </summary>
        /// <param name="_startLocation"></param>
        /// <returns>List of all locations and their shortest route</returns>
        public Dictionary<int, Route> CalculateMinCost(Location _startLocation)
        {
            //Initialise a new empty route list
            Dictionary<int, Route> _shortestPaths = new Dictionary<int, Route>();            
            //Initialise a new empty handled locations list
            List<int> _handledLocations = new List<int>();

            //Initialise the new routes. the constructor will set the route weight to in.max
            foreach (Location location in _locations)
            {
                _shortestPaths.Add(location.Identifier, new Route(location.Identifier));
            }

            
            if( !_shortestPaths.ContainsKey(_startLocation.Identifier ) )
                _shortestPaths.Add(_startLocation.Identifier, new Route(_startLocation.Identifier));
            //The startPosition has a weight 0. 
            _shortestPaths[_startLocation.Identifier].Cost = 0;
            

            //If all locations are handled, stop the engine and return the result
            while (_handledLocations.Count != _locations.Count)
            {
                //Order the locations
                List<int> _shortestLocations = (List<int>)(from s in _shortestPaths
                                                        orderby s.Value.Cost                                       
                                                        select s.Key).ToList();


                int _locationToProcess = 0;

                //Search for the nearest location that isn't handled
                foreach (int _location in _shortestLocations)
                {
                    if (!_handledLocations.Contains(_location))
                    {
                        //If the cost equals int.max, there are no more possible connections to the remaining locations
                        if (_shortestPaths[_location].Cost == int.MaxValue)
                            return _shortestPaths;
                        _locationToProcess = _location;
                        break;
                    }
                }

                //Select all connections where the startposition is the location to Process
                var _selectedConnections = from c in _connections
                                           where c.A.nodeNumber == _locationToProcess
                                           select c;

                //Iterate through all connections and search for a connection which is shorter
                foreach (CLink conn in _selectedConnections)
                {
                    if (_shortestPaths[conn.B.nodeNumber].Cost > conn.Weight + _shortestPaths[conn.A.nodeNumber].Cost)
                    {
                        _shortestPaths[conn.B.nodeNumber].Connections = _shortestPaths[conn.A.nodeNumber].Connections.ToList();
                        _shortestPaths[conn.B.nodeNumber].Connections.Add(conn);
                        _shortestPaths[conn.B.nodeNumber].Cost = conn.Weight + _shortestPaths[conn.A.nodeNumber].Cost;
                    }
                }
                //Add the location to the list of processed locations
                _handledLocations.Add(_locationToProcess);
            }


            return _shortestPaths;
        }
    }
}
