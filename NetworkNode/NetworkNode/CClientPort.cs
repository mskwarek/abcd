using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{ // klasa po której będą dziedziczyć porty wej i wyj
    class CClientPort 
    {
        private int _id;
        private Boolean _busy;

        public CClientPort(int id, Boolean x)
        {
            this._id = id;
            this._busy = x;
        }


        public int ID { get { return _id; } }
        public Boolean STATUS { get { return _busy; } set { _busy = value; } }


    }
}
