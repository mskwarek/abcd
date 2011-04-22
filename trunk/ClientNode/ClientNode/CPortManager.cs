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

        public CPortManager()
        {
        }

        public void readConfig()
        {
            XmlTextReader textReader = new XmlTextReader("../../config.xml");
            try
            {
                while (textReader.Read())
                {
                    switch (textReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (textReader.Name)
                            {
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
            catch(System.Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void showConfig()
        {
            Console.WriteLine("in : " + InputPortCount + " out : " + OutputPortCount); 

        }

        private void createPorts() {
        
            // TO DO: tworzenie listy portów
        
        }
    
    }
}
