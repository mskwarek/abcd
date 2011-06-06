using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace ControlPlane
{
    class CRoutingController
    {
        private static CRoutingController cRoutingController = new CRoutingController();

        private CRoutingController()
        {

        }

        public static CRoutingController Instance
        {
            get
            {
                return cRoutingController;
            }
        }

        private ArrayList RouteTableQuery()
        {
            return new ArrayList();
        }

        private void LocalTopologyIn()
        {

        }

        private void LocalTopologyOut()
        {

        }

        
        private void NetworkTopologyIn()
        {

        }

        private void NetworkTopologyOut()
        {

        }

        


    }
}
