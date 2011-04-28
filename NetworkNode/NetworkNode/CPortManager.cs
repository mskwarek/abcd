using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetworkNode
{
    // ta klasa ma za zadanie odczytać z pliku konfiguracyjnego i utworzyc odpowiednią liczbę portów wyjściowych klienckich
    public sealed class CPortManager
    {
        private List<CClientPortIn> InputClientPortList = new List<CClientPortIn>();
        private List<CClientPortOut> OutputClientPortList = new List<CClientPortOut>();
        private List<CNetworkPortIn> InputNetworkPortList = new List<CNetworkPortIn>();
        private List<CNetworkPortOut> OutputNetworkPortList = new List<CNetworkPortOut>();
        

        static readonly CPortManager instance = new CPortManager();

        static CPortManager()
        {
        }

        CPortManager()
        {
        }

        public static CPortManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void readConfig()
        {
            XmlTextReader textReader = new XmlTextReader("../../config" + CConstrains.nodeNumber + ".xml");
            try
            {
                while (textReader.Read())
                {
                    switch (textReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (textReader.Name)
                            {
                                case "InputClientPort":
                                    CConstrains.inputClientPortNumber = Convert.ToInt16(textReader.ReadString());
                                    continue;
                                case "OutputClientPort":
                                    CConstrains.outputClientPortNumber = Convert.ToInt16(textReader.ReadString());
                                    continue;
                                case "InputNetworkPort":
                                    CConstrains.inputNetworkPortNumber = Convert.ToInt16(textReader.ReadString());
                                    continue;
                                case "OutputNetworkPort":
                                    CConstrains.outputNetworkPortNumber = Convert.ToInt16(textReader.ReadString());
                                    continue;
                            }
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


        /* Mechanizm tworzenia portów:
         * porty klienckie mają numer od 50 x qq gdzie xx - numer węzła od 1 a qq to nr systemowego portu od 01 do 99
         * porty sieciowe mają numer od 55 x qq gdzie xx - numer węzła od 1 a qq to nr systemowego portu od 01 do 99
         */


        private void createPorts()
        {
            for (int i = 0; i < CConstrains.inputClientPortNumber; i++)
            {
                int systemPortNumber = 50000 + (CConstrains.nodeNumber * 100) + i;
                InputClientPortList.Add(new CClientPortIn(i, false, systemPortNumber));
            }

            for (int x = 0; x < CConstrains.outputClientPortNumber; x++)
            {
                OutputClientPortList.Add(new CClientPortOut(x, false));
            } 
            
            for (int y = 0; y < CConstrains.inputNetworkPortNumber; y++)
            {
                int systemPortNumber = 55000 + (CConstrains.nodeNumber * 100) + y;
                InputNetworkPortList.Add(new CNetworkPortIn(y, false, systemPortNumber));
            }

            for (int z = 0; z < CConstrains.outputNetworkPortNumber; z++)
            {
                OutputNetworkPortList.Add(new CNetworkPortOut(z, false));
            }
        }



    }
        //Dodatkowo musi zapewniać odwoływanie do portów po ich ID 
        //(Agent zarządzania wypełniając tablicę podaje ID, VPI, VCI, Pole komutacyjne odczytuje z tablicy komutacji ID portu na podstawie VPI, VCI
        // i pobiera odpowiedni port na podstawie jego ID odwołując się do CPortManager)
    
}