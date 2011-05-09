using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ClientNode
{
    // ta klasa ma za zadanie odczytać z pliku konfiguracyjnego i utworzyc odpowiednią liczbę portów wyjściowych klienckich
    class CPortManager
    {
        private List<CClientPortIn> InputClientPortList = new List<CClientPortIn>();
        private List<CClientPortOut> OutputClientPortList = new List<CClientPortOut>();
        private static  CPortManager instance = null;

        public static CPortManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CPortManager();
                }
                return instance; 
            }
        }


        private  CPortManager()
        {
            readConfig();
            createPorts();
        }

        public void readConfig() {
            XmlTextReader textReader = new XmlTextReader("../../config" + CConstrains.nodeNumber +".xml");
            try {
                while (textReader.Read()) {
                    switch (textReader.NodeType) {
                        case XmlNodeType.Element:
                            switch (textReader.Name) {
                                case "InputClientPort":
                                    CConstrains.inputPortNumber = Convert.ToInt16(textReader.ReadString());
                                    continue;
                                case "OutputClientPort":
                                    CConstrains.outputPortNumber = Convert.ToInt16(textReader.ReadString());
                                    continue;
                            }
                            break;
                    }
                }
                textReader.Close();
                Console.WriteLine("Config loaded. " + showConfig());
            }
            catch(System.Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        public String showConfig() {
            return "in : " + CConstrains.inputPortNumber.ToString() + " out : " + CConstrains.outputPortNumber.ToString(); 
        }

        public CClientPort getOutputPort(int ID)
        {
            int id = ID - 1;
                CClientPortOut port = (CClientPortOut)OutputClientPortList.ElementAt(id);
                return port;
            
        }


        private void createPorts() {
            
            for (int i = 1; i <= CConstrains.inputPortNumber; i++) {
                int systemPortNumber = 50000 + (CConstrains.nodeNumber * 100) +i;
                InputClientPortList.Add(new CClientPortIn(i, false, systemPortNumber));
                
            }

            for (int x = 1; x <= CConstrains.outputPortNumber; x++) {
                OutputClientPortList.Add(new CClientPortOut(x, false));
            }
         }

        public void showPorts() {
            for (int i = 0; i < InputClientPortList.Count; i++) {
                Console.WriteLine("in " + InputClientPortList[i].ID + "  " + InputClientPortList[i].STATUS);
            }
            for (int i = 0; i < OutputClientPortList.Count; i++) {
                Console.WriteLine("out " + OutputClientPortList[i].ID + "  " + OutputClientPortList[i].STATUS);
            }
        }

        private CClientPortOut findFreePort() {   
            CClientPortOut t = OutputClientPortList.Find(delegate(CClientPortOut p) {  return p.STATUS == false; });
            return t;
        }
       
        
        // metoda odpowiedzialna za nadawanie wiadomości
        public void sendMsg(Data.CUserData data) {
            Console.WriteLine("wyszukuje port...");
            CClientPortOut free = findFreePort();
            if (free == null) { Console.WriteLine("Wszystkie porty zajete"); }
            else
            {
                Console.WriteLine("port o id= " + free.ID + " jest wolny");
                int index = OutputClientPortList.IndexOf(free);
                //testowa zmiana------------
               // OutputClientPortList[index].startPort(50201);
                //------------------
                OutputClientPortList[index].send(data);
                //OutputClientPortList[index].STATUS = true;
            }
        }

        public void stopSending(int i)
        {
            Console.WriteLine("Wstrzymywanie nadawnia na porcie : " + i);
            if (OutputClientPortList[i].STATUS == true)
            {
                //OutputClientPortList[i].stop();
                OutputClientPortList[i].STATUS = false;
            }
            else { Console.WriteLine("błędny numer portu" ); }
        }

        public void shutdownAllPorts()
        {
            foreach(CClientPortIn p in InputClientPortList) {
                p.shutdown();
            }

        }

        public void getNodePortConfiguration()
        {
            Console.WriteLine("\n\nNode ID = " + CConstrains.nodeNumber + " PORT CONFIGURATION\n\n");
            foreach (CClientPortIn p in InputClientPortList)
            {
                Console.WriteLine("ID = " + p.ID + " LISTENING ON PORT = " + p.getPortNumber());
            }
            foreach (CClientPortOut p in OutputClientPortList)
            {
                Console.WriteLine("ID = " + p.ID +  " SENDING TO PORT = " + p.getPortNumber());
            }
        }



    }
}