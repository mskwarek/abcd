using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ManagementLayer
{
    public sealed class ConnectionsManager
    {
        static readonly ConnectionsManager instance = new ConnectionsManager();

        private NetworkStream stream;
        private StreamWriter clientStream;

        public static ConnectionsManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void addConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B)
        {
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            Dictionary<Object, Object> pduDict = new Dictionary<Object, Object>() {
            {"from", portIn},
            {"to", portOut},
            {"add", null}
            };
            List<Dictionary<Object, Object>> pduList = new List<Dictionary<Object, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, dataToSend);
            clientStream = new StreamWriter(stream);
            clientStream.WriteLine(dataToSend);
            clientStream.Flush();

            Console.WriteLine("node : " + nodeNumber + "from : " + portNumber_A + " to : " + portNumber_B);
        }

        public void removeConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B)
        {
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            Dictionary<Object, Object> pduDict = new Dictionary<Object, Object>() {
            {"from", portIn},
            {"to", portOut},
            {"remove", null}
            };
            List<Dictionary<Object, Object>> pduList = new List<Dictionary<Object, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, dataToSend);
            clientStream = new StreamWriter(stream);
            clientStream.WriteLine(dataToSend);
            clientStream.Flush();

            Console.WriteLine("node : " + nodeNumber + " from : " + portNumber_A + " to : " + portNumber_B);
        }

        public void setNetworkConnections(int nodeNumber, Data.CLink link)
        {
            //TODO send to node nodeNumber to establish connection with another node

            Console.WriteLine("node : " + nodeNumber + " Setting link on port : " + link.from.portNumber + " to port : " + link.to.portNumber + " on node : " + link.to.nodeNumber);
        }
    }
}
