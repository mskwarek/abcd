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
using Data;

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
           recieve.Name = "SNMPMessagesListener thread";
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
                Console.WriteLine("*** ML CONNECTED ***");
                BinaryFormatter binaryFormater = new BinaryFormatter();
                Data.CSNMPmessage dane = (Data.CSNMPmessage)binaryFormater.Deserialize(clientStream);
                queue.Enqueue(dane);
                Thread process = new Thread(new ThreadStart(processReceivedData));
                process.Name = "Thread process data " + dane.pdu.RequestIdentifier;
                process.Start();
                
                Thread.Sleep(1000);
            }
        }
        // metoda wysyłająca response do ML
        private void sendResponse(String responseId, Data.CCommutationTable table) 
        {
            Data.CSNMPmessage msg;
            
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress,CConstrains.managementLayerPort);
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
            Console.WriteLine("--> Sending Response " + msg + " to ML");
        }

        // metoda kontaktująca sie z ML aby zestawić nowe fizyczne połączenie w sieci. 
        // nie używana 
        private void newLinkRequest(Data.CLink fromNode, Data.CLink toNode) 
        {
            Data.CSNMPmessage msg;

            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, CConstrains.managementLayerPort);
            NetworkStream stream = client.GetStream();

                Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
                {"FromLink", fromNode},
                {"ToLink", toNode},
                {"NodeNumber", CConstrains.nodeNumber},
                {"requestNewLink",null}
                };
                List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
                pduList.Add(pduDict);
                msg = new Data.CSNMPmessage(pduList, null, null);


                msg.pdu.RequestIdentifier = "newLinkRequest : " + CConstrains.nodeNumber.ToString() ;

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            stream.Flush();
            Console.WriteLine("--> Sending newLinkRequest " + msg + " to ML " + fromNode.A.nodeNumber + " ->" + toNode.B.nodeNumber + "<--");

            StreamReader sr = new StreamReader(stream);
            String dane = sr.ReadLine();
            Console.WriteLine("<-- " + dane);

        }


        public void sendHelloMsgToML(int nodeNumber)
        {
            Data.CSNMPmessage msg;

            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, CConstrains.managementLayerPort);
            NetworkStream stream = client.GetStream();

            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
                {"NodeNumber", nodeNumber},
                {"helloMsg",null}
                };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            msg = new Data.CSNMPmessage(pduList, null, null);


            msg.pdu.RequestIdentifier = "helloMsgtoML : " + CConstrains.nodeNumber.ToString();

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            stream.Flush();
            Console.WriteLine("--> Sending helloMsg  " + msg + " to ML " + nodeNumber );

            StreamReader sr = new StreamReader(stream);
            String dane = sr.ReadLine();

            String[] array = dane.Split(';');
            //CConstrains.domainName = array[1];
            Console.WriteLine("<-- " + array[0] + " domainName : " + array[1]);
        }

        public void sendNodeActivityToML(List<Data.CPNNITable> lista)
        {
            Data.CSNMPmessage msg;

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(CConstrains.ipAddress, CConstrains.managementLayerPort);
                NetworkStream stream = client.GetStream();
                
                
                msg = new Data.CSNMPmessage(null, null, null);
                msg.pdu.PNNIList = lista;


                msg.pdu.RequestIdentifier = "NodeActivity : " + CConstrains.nodeNumber.ToString();

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, msg);
                stream.Flush();
                Console.WriteLine("--> Sending NodeActivityMsg  " + msg + " to ML ");

                StreamReader sr = new StreamReader(stream);
                String dane = sr.ReadLine();
                Console.WriteLine("<-- " + dane);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : ML niesdostępny" );
            }
        }

        
        private void sendPCResponse(Data.CLinkInfo SNP, string requestIdentifier)
        {

            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, CConstrains.managementLayerPort);
            NetworkStream stream = client.GetStream();

            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
            {"CCI", null},
            {requestIdentifier, null},
            {"SNP", SNP}
            };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            Data.CSNMPmessage dataToSend = new Data.CSNMPmessage(pduList, null, null);
            dataToSend.pdu.RequestIdentifier = requestIdentifier + SNP.nodeNumber.ToString();
            
            Console.WriteLine("node : " + SNP.nodeNumber);
            
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, dataToSend);
            stream.Flush();
            Console.WriteLine("--> Sending Response " + dataToSend.pdu.RequestIdentifier);
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
                        if (d.ContainsKey("CCI"))
                        {
                            CLinkInfo response;
                            if ((response = PacketController.Instance.processReceivedData(d)) != null )
                                sendPCResponse(response, pdu.RequestIdentifier);
                        }
                        else if (d.ContainsKey("add"))
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
