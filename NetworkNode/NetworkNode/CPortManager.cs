﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace NetworkNode
{
    // ta klasa ma za zadanie odczytać z pliku konfiguracyjnego i utworzyc odpowiednią liczbę portów wyjściowych klienckich
    public sealed class CPortManager
    {

        private List<CPort> InputPortList = new List<CPort>();
        private List<CPort> OutputPortList = new List<CPort>();
        private Logger.CLogger logger = Logger.CLogger.Instance;
        private static readonly CPortManager instance = new CPortManager();

        private CPortManager()
        {
            if (readConfig())
            {
                createPorts();
            }
        }

        public static CPortManager Instance
        {
            get
            {       
                return instance;
            }
        }


        private bool readConfig()
        {
            XmlTextReader textReader;


            if (File.Exists(CConstrains.configFileURL))
            {
                textReader = new XmlTextReader(CConstrains.configFileURL);
            }
            else
            {
                logger.print(null, "Brak pliku konfiguracyjnego " + CConstrains.nodeNumber + "\n Wczytuje domysliny", (int)Logger.CLogger.Modes.error); 
                textReader = new XmlTextReader(CConstrains.configFileURL);
            }

            try
            {
                while (textReader.Read())
                {
                    if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "managementLayerPort")
                    {
                        CConstrains.managementLayerPort = Convert.ToInt32(textReader.ReadElementContentAsInt());
                    }
                    else if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "nccPort")
                    {
                        CConstrains.nccPort = Convert.ToInt32(textReader.ReadElementContentAsInt());
                    }
                    if (textReader.NodeType == XmlNodeType.Element && textReader.Name == ("node" + CConstrains.nodeNumber))
                    {
                        while (textReader.Read())
                        {
                            if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "InputClientPort")
                            { CConstrains.inputClientPortNumber = Convert.ToInt16(textReader.ReadString()); }
                            if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "OutputClientPort")
                            { CConstrains.outputClientPortNumber = Convert.ToInt16(textReader.ReadString()); }
                            if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "InputNetworkPort")
                            { CConstrains.inputNetworkPortNumber = Convert.ToInt16(textReader.ReadString()); }
                            if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "OutputNetworkPort")
                            { CConstrains.outputNetworkPortNumber = Convert.ToInt16(textReader.ReadString()); }
                        
                        }
                    }
                    
                }
                textReader.Close();
                logger.print("readConfig", null, (int)Logger.CLogger.Modes.constructor);
                return true;
            }
            catch (System.Exception e)
            {
                StreamWriter sw = new StreamWriter("error.txt", true);
                sw.Write(e.StackTrace);
                sw.Flush();
                sw.Close();

                logger.print(null, "Brak pliku konfiguracyjnego " + CConstrains.nodeNumber, (int)Logger.CLogger.Modes.error);
                return false;
            }
        }


        /* Mechanizm tworzenia portów:
         * numer port systemowy na którym będzie nasłuchiwał port sieciowy albo kliencki jest tworzony w następujący sposób:
         * wolne porty systemowe mają numery od 50 000 więc numer portu jest to kombinacja numeru węzła i id portu 
         * 50 x qq gdzie xx - numer węzła od 1 a qq to nr portu od 01 do 99
         */


        private void createPorts()
        {
                int z = 1;
                //tworzenie portów wejściowych
                for (int i = 0; i < CConstrains.inputClientPortNumber; i++)
                {
                    int systemPortNumber = 50000 + (CConstrains.nodeNumber * 100) + z;
                    InputPortList.Add(new CClientPortIn(z, false, systemPortNumber));
                    z++;
                }
                for (int x = 0; x < CConstrains.inputNetworkPortNumber; x++)
                {
                    int systemPortNumber = 50000 + (CConstrains.nodeNumber * 100) + z;
                    InputPortList.Add(new CNetworkPortIn(z, false, systemPortNumber));
                    z++;
                }


                int j = 1;
            //tworzenie portów wyjsciowych
                for (int y = 0; y < CConstrains.outputClientPortNumber; y++)
                {
                    OutputPortList.Add(new CClientPortOut(j, false));
                    j++;
                }
                for (int k = 0; k < CConstrains.outputNetworkPortNumber; k++)
                {
                    OutputPortList.Add(new CNetworkPortOut(j, false));
                    j++;
                } 
            
        }


        public CPort getOutputPort(int ID)
        {
            int id = ID - 1;
              
            if( OutputPortList.ElementAt(id).GetType() == typeof(CClientPortOut) )
            {
                
                CClientPortOut port = (CClientPortOut)OutputPortList.ElementAt(id);
                return port;
            }
            else
            {
                CNetworkPortOut port = (CNetworkPortOut)OutputPortList.ElementAt(id);
                return port;
            }
        }

        public CPort getInputPort(int ID)
        {
            int id = ID - 1;
            if (OutputPortList.ElementAt(id).GetType() == typeof(CClientPortOut))
            {
                CClientPortOut port = (CClientPortOut)InputPortList.ElementAt(id);
                return port;
            }
            else
            {
                CNetworkPortOut port = (CNetworkPortOut)InputPortList.ElementAt(id);
                return port;
            }
        }



        public void getNodePortConfiguration()
        {
            logger.print(null, "\n\nNode ID = " + CConstrains.nodeNumber + " PORT CONFIGURATION\n\n", (int)Logger.CLogger.Modes.background);
            foreach (CPort p in InputPortList)
            {
                logger.print(null, "ID = " + p.ID + " CLASS = " + p.PORTCLASS + " TYPE = " + p.PORTTYPE + " LISTENING ON PORT = " + p.PORTNUMBER, (int)Logger.CLogger.Modes.background);
            }
            foreach (CPort p in OutputPortList)
            {
                logger.print(null, "ID = " + p.ID + " CLASS = " + p.PORTCLASS + " TYPE = " + p.PORTTYPE + " SENDING TO PORT = " + p.PORTNUMBER, (int)Logger.CLogger.Modes.background);
            }
        }

    }
    
    
}