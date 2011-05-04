using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace NetworkNode
{
    public sealed class CManagementAgent
    {

       static readonly CManagementAgent instance = new CManagementAgent();

       
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

        public void mlConnectionListener()
        {
            portListener = new TcpListener(ip, portNum);  //listener na porcie danego węzła
            portListener.Start();

            client = portListener.AcceptTcpClient(); 
            clientStream = client.GetStream();  
            Console.WriteLine("connection with ML ");

            status = true;

            while (status) //uruchamiamy nasłuchiwanie
            {
                StreamReader sr = new StreamReader(clientStream);
                String dane = sr.ReadLine();
                
                //obsługa tego co przyśle ML
            }
        }

        public void sendToML()      //wysyłanie do ML - co konkretnie to zaraz..
        {
            TcpClient agentClient = new TcpClient();
            agentClient.Connect(CConstrains.ipAddress, CConstrains.managementLayerPort);
            NetworkStream upStream = agentClient.GetStream();
            StreamWriter upStreamWriter = new StreamWriter(upStream);

            //wysyłanie:
            upStreamWriter.WriteLine();
            upStreamWriter.Flush();
        }
   }
}
