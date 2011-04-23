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
        private List<CClientPortIn> InputPortList = new List<CClientPortIn>();
        private List<CClientPortOut> OutputPortList = new List<CClientPortOut>();
        private int InputPortCount;
        private int OutputPortCount;
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
            showConfig();
            createPorts();
            showPorts();
        }

        public void readConfig() {
            XmlTextReader textReader = new XmlTextReader("../../config.xml");
            try {
                while (textReader.Read()) {
                    switch (textReader.NodeType) {
                        case XmlNodeType.Element:
                            switch (textReader.Name) {
                                case "InputPort":
                                    InputPortCount = Convert.ToInt16(textReader.ReadString());
                                    continue;
                                case "OutputPort":
                                    OutputPortCount = Convert.ToInt16(textReader.ReadString());
                                    continue;
                            }
                            break;
                    }
                }
            }
            catch(System.Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void showConfig() {
            Console.WriteLine("in : " + InputPortCount + " out : " + OutputPortCount); 
        }

        private void createPorts() {
            for (int i = 0; i < InputPortCount; i++) {
                InputPortList.Add(new CClientPortIn(i, false));
            }

            for (int x = 0; x < OutputPortCount; x++) {
                OutputPortList.Add(new CClientPortOut(x, false));
            }
         }

        public void showPorts() {
            for (int i = 0; i < InputPortList.Count; i++) {
                Console.WriteLine("in " + InputPortList[i].ID + "  " + InputPortList[i].STATUS);
            }
            for (int i = 0; i < OutputPortList.Count; i++) {
                Console.WriteLine("out " + OutputPortList[i].ID + "  " + OutputPortList[i].STATUS);
            }
        }

        private CClientPortOut findFreePort() {   
            CClientPortOut t = OutputPortList.Find(delegate(CClientPortOut p) {  return p.STATUS == false; });
            return t;
        }
        // metoda odpowiedzialna za nadawanie wiadomości
        public void sendMsg(String str) {
            Console.WriteLine("wyszukuje port...");
            CClientPortOut free = findFreePort();
            if (free == null) { Console.WriteLine("Wszystkie porty zajete"); }
            else
            {
                Console.WriteLine("port o id= " + free.ID + " jest wolny");
                int index = OutputPortList.IndexOf(free);
                OutputPortList[index].send(str);
                OutputPortList[index].STATUS = true;
            }
        }

        public void stopSending(int i)
        {
            Console.WriteLine("Wstrzymywanie nadawnia na porcie : " + i);
            if (OutputPortList[i].STATUS == true)
            {
                OutputPortList[i].stop();
                OutputPortList[i].STATUS = false;
            }
            else { Console.WriteLine("błędny numer portu" ); }
        }
    
    }
}