﻿using System;
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
        public List<Data.CPNNITable> PNNIList = new List<Data.CPNNITable>();


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
           Console.WriteLine("*** ML nasłuchuje na porcie : " +CConstrains.LMportNumber + " *** ");
           
           while (status)
           {
               TcpClient client = portListener.AcceptTcpClient();
               NetworkStream clientStream = client.GetStream();
               StreamWriter downStream = new StreamWriter(clientStream);
               
               Console.WriteLine("*** CONNECTION FROM NODE ACCEPTED ***");
               BinaryFormatter binaryFormater = new BinaryFormatter();
               Data.CSNMPmessage dane = (Data.CSNMPmessage)binaryFormater.Deserialize(clientStream);

               // potencjalny fuck up
               if (dane.pdu.variablebinding != null && dane.pdu.RequestIdentifier.StartsWith("getTable")) 
               {

                   printCommutationTable(dane.pdu.variablebinding);  
                
               }
               else if(dane.pdu.RequestIdentifier.StartsWith("newLinkRequest")) 
               {
                   String nodeType = "network";
                   foreach (Dictionary<String, Object> d in dane.pdu.variablebinding)
                   { 
                    if(d.ContainsKey("requestNewLink")) 
                    {
                        if (CNetworkConfiguration.Instance.addNewLink((Data.CLink)d["FromLink"], (Data.CLink)d["ToLink"], Convert.ToInt16(d["NodeNumber"]) ,nodeType))
                        {
                            downStream.WriteLine(dane.pdu.RequestIdentifier + " -OK- ");
                            downStream.Flush();
                        }
                        else
                        {
                            downStream.WriteLine(dane.pdu.RequestIdentifier + " -ERROR- ");
                            downStream.Flush();
                        }
                    }
                  }
               }
               else if (dane.pdu.RequestIdentifier.StartsWith("helloMsgtoML"))
               {
                   foreach (Dictionary<String, Object> d in dane.pdu.variablebinding)
                   {
                       if (d.ContainsKey("helloMsg"))
                       {
                           if (CNetworkConfiguration.Instance.getNodeNumberFromDict(Convert.ToInt16(d["NodeNumber"])))
                           {
                               int index = -1;
                               for (int i = 0; i < CNetworkConfiguration.Instance.linkList.Count; i++)
                               {
                                   if (CNetworkConfiguration.Instance.linkList.ElementAt(i).from.nodeNumber == (Convert.ToInt16(d["NodeNumber"])))
                                   {
                                       index = i;
                                       setNetworkConnections((Convert.ToInt16(d["NodeNumber"])),CNetworkConfiguration.Instance.linkList.ElementAt(i) );
                                   }
                               }
                               if (index != -1)
                               {
                                   Data.CLink link = CNetworkConfiguration.Instance.linkList.ElementAt(index);
                                   //Wysylanie informacji o wezle klienckim
                                   PNNIList.Add(new Data.CPNNITable(link.A.nodeNumber, link.A.portType, link.A.portNumber, link.B.nodeNumber, link.B.portType, link.B.portNumber, true));
                                   Console.WriteLine("PNNIList  ADDED Client NODE");
                                   sendPNNIListToCP(PNNIList);
                               }
                               downStream.WriteLine(dane.pdu.RequestIdentifier + " -OK- ");
                               downStream.Flush();
                           }
                           else
                           {
                               downStream.WriteLine(dane.pdu.RequestIdentifier + " -ERROR- ");
                               downStream.Flush();
                           }
                       }
                   }
               }
               else if (dane.pdu.RequestIdentifier.StartsWith("NodeActivity"))
               {


                   foreach(Data.CPNNITable t in dane.pdu.PNNIList)
                   {
                       if (PNNIList.Contains(t))
                       {
                           int index = PNNIList.IndexOf(t);

                           if (PNNIList.ElementAt(index).IsNeighbourActive != t.IsNeighbourActive)
                           {
                               PNNIList.ElementAt(index).IsNeighbourActive = t.IsNeighbourActive;
                               Console.WriteLine("*** PNNIList Updated  " + PNNIList.ElementAt(index).NodeNumber + " " + PNNIList.ElementAt(index).NodeType + " ACTIVITY :  " + PNNIList.ElementAt(index).IsNeighbourActive);
                               sendPNNIListToCP(PNNIList); 
                           }
                       }
                       else
                       {
                           PNNIList.Add(t);
                           Console.WriteLine("PNNIList  ADDED : " + t.NodeNumber + " " + t.NodeType + " " + t.NodePortNumberSender + " " + t.NeighbourNodeNumber + " " + t.NeighbourNodeType + " " + t.NeighbourPortNumberReciever + " " + t.IsNeighbourActive);
                           sendPNNIListToCP(PNNIList); 
                       }
                       downStream.WriteLine("--> SENDING RESPONSE : " + dane.pdu.RequestIdentifier + " -OK- ");
                       downStream.Flush();
                   }
               }


               
               Console.WriteLine(dane.pdu.RequestIdentifier);
               Thread.Sleep(1000);
           }
        }


        public void sendPNNIListToCP(List<Data.CPNNITable> lista)
        {
            Data.CSNMPmessage msg;

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(CConstrains.ipAddress, CConstrains.NCCportNumber);
                NetworkStream stream = client.GetStream();


                msg = new Data.CSNMPmessage(null, null, null);
                msg.pdu.PNNIList = lista;


                msg.pdu.RequestIdentifier = "PNNIList ML";

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, msg);
                stream.Flush();
                Console.WriteLine("--> Sending PNNIListMsg  " + msg + " to NCC ");

                StreamReader sr = new StreamReader(stream);
                String dane = sr.ReadLine();
                Console.WriteLine("<-- " + dane);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : NCC niesdostępny");
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
            Console.WriteLine("--> SENDING " + msg + " TO NODE : " +nodeNumber);
            
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
