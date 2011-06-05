using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    class CNetworkCallController
    {

        public void CallIndication()
        { }

        public void ConnectionRequestOut()
        { }


        //uzywane przy wielu domenach
        public void NetworkCallCoordinationOut()
        { }

        // metoda zamieniajaca nazwe lokalna na identyfikator polaczenia 
        // kierowane do directory
        public void DirectoryRequest(string localName)
        { }

        public void CallTeardownOut()
        { }
    }
}
