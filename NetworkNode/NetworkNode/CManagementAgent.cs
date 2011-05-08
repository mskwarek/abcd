using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace NetworkNode
{
    public sealed class CManagementAgent
    {

       static readonly CManagementAgent instance = new CManagementAgent();

       public Queue<Data.CSNMPmessage> queue = new Queue<Data.CSNMPmessage>();
       
       private bool status;
       private IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);     //adres serwera
       private int portNum;
       private TcpListener portListener;
       private TcpClient client;
       private NetworkStream clientStream;

       private CManagementAgent()
       {
           portNum = 50000 + CConstrains.nodeNumber * 100;
           //portNum = 161;

           Thread recieve = new Thread(new ThreadStart(SNMPMessagesListener));
           
           recieve.Start();

       }

       public static CManagementAgent Instance
       {
           get
           {
               return instance;
           }
       }      

        public void setCommutationTable(Data.CCommutationTable commutationTable)
        {
            CCommutationTable.Instance.setCommutationTable(commutationTable);
        }

        public void resetCommutationTable()
        {
            CCommutationTable.Instance.resetCommutationTable();
        }

        public void addConnection(Data.PortInfo portIn, Data.PortInfo portOut)
        {
            CCommutationTable.Instance.addEntry(portIn, portOut);
        }

        public void removeConnection(Data.PortInfo portIn, Data.PortInfo portOut) //metoda rozlaczajaca polaczenie w polu komutacyjnym danego wezla
        {
            CCommutationTable.Instance.removeConnection(portIn, portOut);
        }

        public void showConnections() //metoda wyswietlajaca zestawione polaczenia
        {
            
            CCommutationTable.Instance.showAll();   
        }

        public Data.CCommutationTable getCommutationTable()
        {
            return CCommutationTable.Instance.getCommutationTable();
        }

        
        public void SNMPMessagesListener()
        {
            portListener = new TcpListener(ip, portNum);  //listener na porcie danego węzła
            portListener.Start();

            
            status = true;

            while (status) //uruchamiamy nasłuchiwanie
            {
                client = portListener.AcceptTcpClient();
                clientStream = client.GetStream();
                Console.WriteLine("connection with ML ");
                BinaryFormatter binaryFormater = new BinaryFormatter();
                Data.CSNMPmessage dane = (Data.CSNMPmessage)binaryFormater.Deserialize(clientStream);
                queue.Enqueue(dane);
                Thread process = new Thread(new ThreadStart(processReceivedData));
                process.Start();
                
                Thread.Sleep(1000);
            }
        }
        // metoda wysyłająca response do ML
        private void sendResponse(String responseId, Data.CCommutationTable table) 
        {
            Data.CSNMPmessage msg;
            int portNumber = CConstrains.managementLayerPort;
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress,portNumber);
            NetworkStream stream = client.GetStream();

            // przypadek wysyłania tablicy komutacji do ML
            if (table != null)
            {
                Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
                {"CommutationTable", table}
                };
                List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
                pduList.Add(pduDict);
                msg = new Data.CSNMPmessage(pduList, null, null);
            }
            else
            {
                msg = new Data.CSNMPmessage(null, null, null);
            }

            msg.pdu.RequestIdentifier = responseId;

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            stream.Flush();
            Console.WriteLine("sending respnse " + msg + " to ML");
            
        
        }


        public void processReceivedData()
        {
            while (status)
            {
                if (queue.Count != 0)
                {
                    //obiekty w słowniku: [from = CPortInfo1][to = CPortInfo2][add = null]

                    Data.SNMPpdu pdu = queue.Dequeue().pdu;

                    foreach (Dictionary<String, Object> d in pdu.variablebinding)
                    {
                        if (d.ContainsKey("add"))
                        {
                            //obsługa dodania połaczenia w polu kom.
                            addConnection(
                                (Data.PortInfo)d["from"],
                                (Data.PortInfo)d["to"]);
                            sendResponse(pdu.RequestIdentifier, null);
        
                        }
                        else if (d.ContainsKey("remove"))
                        {
                            //obsługa usuniecia połaczenia w polu kom.
                            removeConnection((Data.PortInfo)d["from"], (Data.PortInfo)d["to"]);
                            sendResponse(pdu.RequestIdentifier, null);
        
                        }
                        else if (d.ContainsKey("setTopologyConnection"))
                        {
                            //obsługa ustawienia portów wyjściowych.
                            startPort((Data.CLinkInfo)d["from"], (Data.CLinkInfo)d["to"]);
                            sendResponse(pdu.RequestIdentifier, null);
        
                        }
                        else if (d.ContainsKey("getTable"))
                        {
                            //pobranie i wysłanie tablicy komutacji dla wybranego noda.
                            sendResponse(pdu.RequestIdentifier, getCommutationTable());
                        }

                    }
                   Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }

        public void startPort(Data.CLinkInfo from, Data.CLinkInfo to)
        {
            CPort outPort = (CPort)CPortManager.Instance.getOutputPort(from.portNumber);
            outPort.startPort(50000 + to.nodeNumber * 100 + to.portNumber);
        }

        //public void sendToML()      //wysyłanie do ML - co konkretnie to zaraz..
        //{
        //    TcpClient agentClient = new TcpClient();
        //    agentClient.Connect(CConstrains.ipAddress, CConstrains.managementLayerPort);
        //    NetworkStream upStream = agentClient.GetStream();
        //    StreamWriter upStreamWriter = new StreamWriter(upStream);

        //    //wysyłanie:
        //    upStreamWriter.WriteLine();
        //    upStreamWriter.Flush();
        //}
   }
}
