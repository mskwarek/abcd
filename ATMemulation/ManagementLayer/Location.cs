using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteEngine
{
    public class Location
    {

        int _identifier;
        public Location(int id)
        {
            this._identifier = id;
        }
        public int Identifier
        {
            get { return this._identifier; }
            set { this._identifier = value; }
        }
        public override string ToString()
        {
            return _identifier.ToString();
        }
    }
}
