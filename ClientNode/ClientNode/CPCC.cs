using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    class CPCC
    {

        static CPCC instance = new CPCC();

        private CPCC()
        { }

        public static CPCC Instance
        {
            get
            {
                return instance;
            }
        }

        public void CallRequest(String CallSourceIdentifier, String CallDestinationIdentifier)
        {
            
        }
        public void CallTeardown()
        {}

    }
}
