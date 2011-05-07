using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    public class CPort
    {
        protected int _id;
        protected Boolean _busy;
        protected int portNumber;
        protected String portClass;
        protected String portType;

        public CPort()
        {

        }

        public CPort(int id, Boolean x)
        {
            this._id = id;
            this._busy = x;
        }
        // metoda używana przy portach wyjściowych. Po otrzymaniu topologi sieci port dostaje informacje na jaki port systemowy ma nadawać. 
        public virtual void startPort(int systemPortNumber) { }

        public int ID { get { return _id; } }
        public int PORTNUMBER { get { return portNumber; } set { portNumber = value; } }
        public Boolean STATUS { get { return _busy; } set { _busy = value; } }
        public String PORTCLASS { get { return portClass; } set { portClass = value; } }
        public String PORTTYPE { get { return portType; } set { portType = value; } }


    }


}
