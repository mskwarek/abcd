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
        private Logger.CLogger logger = Logger.CLogger.Instance;
        private List<Data.CLink> establishedLinksList = new List<Data.CLink>();


        private CLinkResourceManager()
        {
            logger.print("LinkResourceManager", null, (int)Logger.CLogger.Modes.constructor);
            
            
        }

        public static  CLinkResourceManager Instance
        {
            get
            {
                return cLinkResourceManager;
            }
        }

        //
        public List<CLink> LocalTopology()
        {
            return establishedLinksList;
        }

        public CLink SNPLinkConnectionRequest(CLink SNPtoSNP)
        {
            if (!establishedLinksList.Contains(SNPtoSNP))
            {
                
                logger.print("SNPLinkConnectionRequest", "RLM: Reserving connection ", (int)Logger.CLogger.Modes.normal);
            
                reserveCLink(SNPtoSNP);
                CShortestPathCalculatorWrapper.Instance.reserveCLink(SNPtoSNP);

                return SNPtoSNP;
            }

            return null;
        }

        public CLink SNPLinkConnectionDeallocation(CLink SNPtoSNP)
        {
            if (establishedLinksList.Contains(SNPtoSNP))
            {
                logger.print("SNPLinkConnectionDeallocation", "RLM: Deallocating connection ", (int)Logger.CLogger.Modes.normal);
            
                releaseCLink(SNPtoSNP);
                CShortestPathCalculatorWrapper.Instance.releaseCLink(SNPtoSNP);

                return SNPtoSNP;
            }
            
            return null;
        }


        // zmienia stan danego CLink na busy
        private void reserveCLink(CLink cLink)
        {
            establishedLinksList.Add(cLink);
        }

        // zmienia stan danego CLink na ready
        private void releaseCLink(CLink cLink)
        {
            establishedLinksList.Remove(cLink);
        }









        public List<CLink> Configuration()
        {
            return establishedLinksList;
        }

        public void Translation()
        {
        }

    }
}
