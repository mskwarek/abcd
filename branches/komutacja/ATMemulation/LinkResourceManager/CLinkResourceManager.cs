using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Data;
using RouteEngine;

namespace LinkResourceManager
{
    public sealed class CLinkResourceManager
    {

        private static CLinkResourceManager cLinkResourceManager = new CLinkResourceManager();

        private List<Data.CLinkInfo> allocatedSNPs = new List<Data.CLinkInfo>();


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
        public List<CLinkInfo> LocalTopology()
        {
            return allocatedSNPs;
        }

        public CLinkInfo SNPLinkConnectionRequest(CLinkInfo SNP)
        {
            if ( !allocatedSNPs.Contains(SNP) )
            {
                Console.WriteLine("RLM: Reserving connection ");
                reserveSNP( SNP );
                //CShortestPathCalculatorWrapper.Instance.reserveCLink(SNP);

                return SNP;
            }

            return null;
        }

        public CLinkInfo SNPLinkConnectionDeallocation(CLinkInfo SNP)
        {
            if (allocatedSNPs.Contains(SNP))
            {
                Console.WriteLine("RLM: Deallocating connection ");
                releaseSNP(SNP);
                //TODO: Powiadom RC o alokacji
                //CShortestPathCalculatorWrapper.Instance.releaseCLink(SNP);

                return SNP;
            }
            
            return null;
        }


        // dodaje SNP do puli wykorzystywanych
        private void reserveSNP(CLinkInfo cLinkInfo)
        {
            allocatedSNPs.Add(cLinkInfo);
        }

        // usuwa SNP z puli wykorzystywanych
        private void releaseSNP(CLinkInfo cLinkInfo)
        {
            allocatedSNPs.Remove(cLinkInfo);
        }




        public List<CLinkInfo> Configuration()
        {
            return allocatedSNPs;
        }

        public void Translation()
        {
        }

    }
}
