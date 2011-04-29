using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    public class CPort
    {
        private int _id;
        private Boolean _busy;
        private int portNumber;
        private String portClass;
        private String portType;

        public CPort(int id, Boolean x)
        {
            this._id = id;
            this._busy = x;
        }


        public int ID { get { return _id; } }
        public int PORTNUMBER { get { return portNumber; } set { portNumber = value; } }
        public Boolean STATUS { get { return _busy; } set { _busy = value; } }
        public String PORTCLASS { get { return portClass; } set { portClass = value; } }
        public String PORTTYPE { get { return portType; } set { portType = value; } }


    }


}
