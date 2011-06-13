using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetworkNode
{
    class CNetManager
    {

        private static readonly CNetManager instance = new CNetManager();
        private bool state = true;
        private List<Data.CPNNITable> PNNIList = new List<Data.CPNNITable>();
        private CNetManager()
        {

        }

        public static CNetManager Instance
        {
            get
            {       
                return instance;
            }
        }

        public void init()
        {

            Thread t1 = new Thread(new ThreadStart(startSending));
            t1.Name = "CNetManager";
            t1.Start();
        }

        private Data.CCharacteristicData prepareHelloMsg()
        {

            Data.CUserData uData = new Data.CUserData();
            Data.CAdministrationData adData = new Data.CAdministrationData(Data.Contact.UNI, Data.PT._000_, Data.CLP._1_);
            adData.setHEC();
            adData.setVPI(0);
            adData.setVCI(5);
            Data.CCharacteristicData charData = new Data.CCharacteristicData(adData, uData);

            return charData;
        }


        public void fillTable(int portNumber, String nodeResponse, bool isActive)
        {
            String[] array = nodeResponse.Split(';');
            Data.CPNNITable table = new Data.CPNNITable(CConstrains.nodeNumber, CConstrains.nodeType, portNumber, Convert.ToInt16(array[0]), array[1], Convert.ToInt16(array[2]),array[3], isActive);
            Console.WriteLine("<-- RESPONSE FOR HELLO MSG RECIEVED : " + table.NodeNumber + " " + table.NodeType + " " + table.NodePortNumberSender + " " + table.NeighbourNodeNumber + " " + table.NeighbourNodeType + " "  + table.NeighbourPortNumberReciever + " " + table.IsNeighbourActive + " " + table.DomainName );

            if (PNNIList.Contains(table))
            {
                int index = PNNIList.IndexOf(table);
                
                if (PNNIList.ElementAt(index).IsNeighbourActive != isActive)
                {
                    PNNIList.ElementAt(index).IsNeighbourActive = isActive;
                    Console.WriteLine("*** Zmieniłem flagę na " + isActive);
                    CManagementAgent.Instance.sendNodeActivityToML(PNNIList);
                }
            }
            else
            {
                PNNIList.Add(table);
                Console.WriteLine("PNNIList ADDED : " + table.NodeNumber + " " + table.NodeType + " " + table.NodePortNumberSender + " " + table.NeighbourNodeNumber + " " + table.NeighbourNodeType + " " + table.NeighbourPortNumberReciever + " " + table.IsNeighbourActive + " " + table.DomainName);
                CManagementAgent.Instance.sendNodeActivityToML(PNNIList);
                
            }
        }



        public void startSending()
        {
            Data.CCharacteristicData helloMsg = prepareHelloMsg();
            while (state)
            {
                for (int i = 1; i <= CConstrains.outputPortNumber; i++)
                {
                    if (CPortManager.Instance.getOutputPort(i).GetType() == typeof(CNetworkPortOut))
                    {
                        CNetworkPortOut outputPort;
                        outputPort = (CNetworkPortOut)CPortManager.Instance.getOutputPort(i);
                        if (outputPort.STATUS == true)
                        {
                            outputPort.send(helloMsg, null, true);
                        }
                    }
                }
                Thread.Sleep(10000);
            }

        }

        public void stopSending()
        {
            this.state = false;
                    
        }
    }
}
