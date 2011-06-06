using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ControlPlane
{
    public sealed class CLinkResourceManager
    {

        private static CLinkResourceManager cLinkResourceManager = new CLinkResourceManager();

        private CLinkResourceManager()
        {

            Console.WriteLine("CLinkResourceManager");
            
        }

        public static  CLinkResourceManager Instance
        {
            get
            {
                return cLinkResourceManager;
            }
        }

        //
        public void LocalTopology()
        { 
        }

        public void SNPLinkConnectionRequest()
        {
        }

        public void SNPLinkConnectionDeallocation()
        {
        }

        public void Configuration()
        {
        }

        public void Translation()
        {
        }

    }
}
