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
           //portNum = 50000 + CConstrains.nodeNumber * 100;
           portNum = 161;

           Thread recieve = new Thread(new ThreadStart(SNMPMessagesListener));
           recieve.Start();
           Thread processMessage = new Thread(new ThreadStart(processReceivedData));
           processMessage.Start();
       }

       public static CManagementAgent Instance
       {
           get
           {
               return instance;
           }
       }      

        public void setCommutationTable(Dictionary<Data.PortInfo, Data.PortInfo> commutationTable)
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

        public void removeConnection(Data.PortInfo portIn) //metoda rozlaczajaca polaczenie w polu komutacyjnym danego wezla
        {
            CCommutationTable.Instance.removeConnection(portIn);
        }

        public void showConnections() //metoda wyswietlajaca zestawione polaczenia
        {
            
            CCommutationTable.Instance.showAll();   
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
                Data.CSNMPmessage dane = (Data.CSNMPmessage)binaryFormater.Deserialize(clientStream);
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
                        if (d.ContainsKey("add"))
                        {
                            //obsługa dodania połaczenia w polu kom.
                            addConnection(
                                (Data.PortInfo)d["from"],
                                (Data.PortInfo)d["to"]);
                        }
                        else if (d.ContainsKey("delete"))
                        {
                            //obsługa usuniecia połaczenia w polu kom.
                            removeConnection((Data.PortInfo)d["from"]);
                        }
                    }
                   Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
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
