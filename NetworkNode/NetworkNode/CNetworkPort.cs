using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class CNetworkPort
    {
        private int _id;
        private Boolean _busy;

        public CNetworkPort(int id, Boolean busy)
        {
            this._id = id;
            this._busy = busy;
        }

        public int ID
        {
            get
            {
                return _id;
            }
        }

        public Boolean STATUS
        {
            get
            {
                return _busy;
            }
            set
            {
                _busy = value;
            }
        }
    }
}
