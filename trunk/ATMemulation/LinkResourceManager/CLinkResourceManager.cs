using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Data;
using RouteEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace LinkResourceManager
{
    public sealed class CLinkResourceManager
    {

        private static CLinkResourceManager cLinkResourceManager = new CLinkResourceManager();


        
        private List<Data.CLink> establishedLinksList = new List<Data.CLink>();


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
        public List<CLink> LocalTopology()
        {
            return establishedLinksList;
        }

        public CLink SNPLinkConnectionRequest(CLink SNPtoSNP)
        {
            if (!establishedLinksList.Contains(SNPtoSNP))
            {
                Console.WriteLine("RLM: Reserving connection ");
                reserveCLink(SNPtoSNP);
                CShortestPathCalculatorWrapper.Instance.reserveCLink(SNPtoSNP);
                int sourceNode = SNPtoSNP.from.nodeNumber;
                int sourcePort = SNPtoSNP.from.portNumber;
                int destinationNode = SNPtoSNP.to.nodeNumber;
                int destinationPort = SNPtoSNP.to.portNumber;
                if (sendLinkConnectionRequest(sourceNode, sourcePort, VPIPole.Dequeue(), VCIPole.Dequeue()) &&
                    sendLinkConnectionRequest(destinationNode, destinationPort, VPIPole.Dequeue(), VCIPole.Dequeue()))
                    return SNPtoSNP;
                else
                    return null;
            }

            return null;
        }

        public bool sendLinkConnectionRequest(int nodeNumber, int portNumber,int VPI, int VCI)
        {
            Data.RequestConnectionMessage msg;

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(CConstrains.ipAddress, CConstrains.calculateSocketPort(nodeNumber,portNumber));
                NetworkStream stream= client.GetStream();
                msg = new Data.RequestConnectionMessage(VPI, VCI);
                BinaryFormatter binaryFormater = new BinaryFormatter();
                binaryFormater.Serialize(stream, msg);
                stream.Flush();

                StreamReader sr = new StreamReader(stream);
                String dane = sr.ReadLine();
                if (dane.Equals("Done"))
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : Node"+ nodeNumber +" Port "+ portNumber+" niesdostępny");
                return false;
            }

        }

        public CLink SNPLinkConnectionDeallocation(CLink SNPtoSNP)
        {
            if (!establishedLinksList.Contains(SNPtoSNP))
            {
                Console.WriteLine("RLM: Deallocating connection ");
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
