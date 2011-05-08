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

namespace ClientNode
{
    public sealed class CManagementAgent
    {

       static readonly CManagementAgent instance = new CManagementAgent();

       public Queue<CSNMPmessage> queue = new Queue<CSNMPmessage>();
       
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

       
        public void SNMPMessagesListener()
        {
            portListener = new TcpListener(ip, portNum);  //listener na porcie danego węzła
            portListener.Start();
            Console.WriteLine("SNMPMessageListener in ON");
            
            status = true;

            while (status) //uruchamiamy nasłuchiwanie
            {
                client = portListener.AcceptTcpClient();
                clientStream = client.GetStream();
                Console.WriteLine("connection with ML ");
                BinaryFormatter binaryFormater = new BinaryFormatter();
                CSNMPmessage dane = (CSNMPmessage)binaryFormater.Deserialize(clientStream);
                queue.Enqueue(dane);
                Thread processMessage = new Thread(new ThreadStart(processReceivedData));
                processMessage.Start();
                Thread.Sleep(1000);
            }
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
                        
                        if (d.ContainsKey("setTopologyConnection"))
                        {
                            //obsługa ustawienia portów wyjściowych.
                            startPort((Data.CLinkInfo)d["from"], (Data.CLinkInfo)d["to"]);
                            sendResponse(pdu.RequestIdentifier);
                        }
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }

       
        private void sendResponse(String responseId) 
        { 
            int portNumber = CConstrains.managementLayerPort;
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress,portNumber);
            NetworkStream stream = client.GetStream();

            CSNMPmessage msg = new CSNMPmessage(null, null, null);
            msg.pdu.RequestIdentifier = responseId;

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            stream.Flush();
            Console.WriteLine("sending respnse " + msg + " to ML");
            client.Close();
        
        }
        
        
        public void sendToML()      //wysyłanie do ML - co konkretnie to zaraz..
        {
            TcpClient agentClient = new TcpClient();
            agentClient.Connect(CConstrains.ipAddress, CConstrains.managementLayerPort);
            NetworkStream upStream = agentClient.GetStream();
            StreamWriter upStreamWriter = new StreamWriter(upStream);

            // T
            upStreamWriter.WriteLine();
            upStreamWriter.Flush();
        }


        // metoda która uruchamia port wyjściowy na podstawie informacji z ML
        public void startPort(CLinkInfo from, CLinkInfo to)
        {
            CClientPortOut outPort = (CClientPortOut)CPortManager.Instance.getOutputPort(from.portNumber);
            outPort.startPort(50000 + to.nodeNumber * 100 + to.portNumber);
        }
   }
}
