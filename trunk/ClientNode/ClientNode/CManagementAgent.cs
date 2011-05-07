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
           //portNum = 50000 + CConstrains.nodeNumber * 100;
           portNum = 161;
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

            client = portListener.AcceptTcpClient(); 
            clientStream = client.GetStream();  
            Console.WriteLine("connection with ML ");
            status = true;

            while (status) //uruchamiamy nasłuchiwanie
            {
                BinaryFormatter binaryFormater = new BinaryFormatter();
                CSNMPmessage dane = (CSNMPmessage)binaryFormater.Deserialize(clientStream);
                queue.Enqueue(dane);
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

                    foreach (Dictionary<Object, Object> d in queue.Dequeue().pdu.variablebinding)
                    {
                        
                        if (d.ContainsKey("setTopologyConnection"))
                        {
                            //obsługa ustawienia portów wyjściowych.
                            startPort((Data.CLinkInfo)d["from"], (Data.CLinkInfo)d["to"]);
                        }
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
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
