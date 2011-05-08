using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

namespace ManagementLayer
{
    public sealed class ConnectionsManager
    {
        static readonly ConnectionsManager instance = new ConnectionsManager();

        public static ConnectionsManager Instance
        {
            get
            {
                return instance;
            }
        }


        private ConnectionsManager()
        {

            Thread t = new Thread(responseListener);
            t.Name = "responseListener thread";
            t.Start();
        }


        // nasłuchiwanie na response od noda. 
        private void responseListener()
        {
           bool status = true;
           IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);     //adres serwera
           TcpListener portListener = new TcpListener(ip, CConstrains.LMportNumber);
           portListener.Start();
           Console.WriteLine("ML nasłuchuje na porcie : " +CConstrains.LMportNumber);
           
           while (status)
           {
               TcpClient client = portListener.AcceptTcpClient();
               NetworkStream clientStream = client.GetStream();
               Console.WriteLine("connection from node accepted");
               BinaryFormatter binaryFormater = new BinaryFormatter();
               Data.CSNMPmessage dane = (Data.CSNMPmessage)binaryFormater.Deserialize(clientStream);

               // potencjalny fuck up
               if (dane.pdu.variablebinding != null && dane.pdu.RequestIdentifier.StartsWith("getTable")) 
               {

                   printCommutationTable(dane.pdu.variablebinding);  
                
               }
               
               Console.WriteLine(dane.pdu.RequestIdentifier);
               Thread.Sleep(1000);
           }
        }
        private void send(int nodeNumber, Data.CSNMPmessage msg)
        {
            int portNumber = 50000 + 100 * nodeNumber;
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress,portNumber);
            NetworkStream stream = client.GetStream();
            
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            //stream.Flush();
            Console.WriteLine("sending " + msg + " to node : " +nodeNumber);
            
        }



        public void addConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B)
        {
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"from", portIn},
            {"to", portOut},
            {"add", null}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = "ADD" + nodeNumber.ToString();

            send(nodeNumber, dataToSend);

            Console.WriteLine("node : " + nodeNumber + " from : " + portNumber_A + " to : " + portNumber_B);
        }

        public void removeConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B)
        {
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"from", portIn},
            {"to", portOut},
            {"remove", null}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = "REMOVE" + nodeNumber.ToString();

            send(nodeNumber, dataToSend);

            Console.WriteLine("node : " + nodeNumber + " from : " + portNumber_A + " to : " + portNumber_B);
        }

        public void setNetworkConnections(int nodeNumber, Data.CLink link)
        {
            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"from", link.from},
            {"to", link.to},
            {"setTopologyConnection", null}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = "STC" + nodeNumber.ToString();

            send(nodeNumber, dataToSend);

            Console.WriteLine("node : " + nodeNumber + " Setting link on port : " + link.from.portNumber + " to port : " + link.to.portNumber + " on node : " + link.to.nodeNumber);
        }
        
        public void getNodeCommutationTable(int nodeNumber)
        {
            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"getTable", null}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);


            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = "getTable" + nodeNumber.ToString();

            send(nodeNumber, dataToSend);

            Console.WriteLine("Request for CommutationTable sent to node : " +nodeNumber);
        }

        // metoda wypisująca tablićę komutacji.
        public void printCommutationTable(List<Dictionary<String, Object>> lista)
        {
            foreach (Dictionary<String, Object> l in lista)
            {

                if (l.ContainsKey("CommutationTable"))
                {
                    Data.CCommutationTable ct = (Data.CCommutationTable)l["CommutationTable"];

                    foreach (Data.PortInfo key in ct.Keys)
                    {
                        Data.PortInfo portOut;
                        if (ct.ContainsKey(key))
                        {
                            portOut = ct[key];
                            Console.WriteLine("Port In :" + key.getPortID() + " VPI :" + key.getVPI() + " VCI :" + key.getVCI() + " || Port Out :" + portOut.getPortID() + " VPI :" + portOut.getVPI() + " VCI :" + portOut.getVCI());
                        }

                    }
                }

            }
        }

    
    }
}
