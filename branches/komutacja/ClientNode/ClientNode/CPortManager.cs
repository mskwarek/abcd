using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace ClientNode
{
    // ta klasa ma za zadanie odczytać z pliku konfiguracyjnego i utworzyc odpowiednią liczbę portów wyjściowych klienckich
    class CPortManager
    {
        private List<CClientPortIn> InputClientPortList = new List<CClientPortIn>();
        private List<CClientPortOut> OutputClientPortList = new List<CClientPortOut>();
        private static  CPortManager instance = null;
        private Logger.CLogger logger = Logger.CLogger.Instance;
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
            XmlTextReader textReader;
            
            if (File.Exists(CConstrains.configFileURL))
            {
                textReader = new XmlTextReader(CConstrains.configFileURL);
            }
            else
            {
                logger.print(null,"Brak pliku konfiguracyjnego " + CConstrains.nodeNumber + "\n Wczytuje domysliny", (int)Logger.CLogger.Modes.error);
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
                        CConstrains.ControlPlanePortNumber = Convert.ToInt32(textReader.ReadElementContentAsInt());
                    }
                    
                    if (textReader.NodeType == XmlNodeType.Element && textReader.Name == ("node" + CConstrains.nodeNumber))
                    {
                        while (textReader.Read())
                        {
                            if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "InputClientPort")
                            { CConstrains.inputPortNumber = Convert.ToInt16(textReader.ReadString()); }
                            if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "OutputClientPort")
                            { CConstrains.outputPortNumber = Convert.ToInt16(textReader.ReadString()); }
                        }
                    }
                    //else if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "defaultNode")
                    //{
                    //    Console.WriteLine("zaczytałem default");
                    //    while (textReader.Read())
                    //    {
                    //        if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "InputClientPort")
                    //        { CConstrains.inputPortNumber = Convert.ToInt16(textReader.ReadString()); }
                    //        if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "OutputClientPort")
                    //        { CConstrains.outputPortNumber = Convert.ToInt16(textReader.ReadString()); }
                     
                    //    }
                    //}
                }
                textReader.Close();
                logger.print("readConfig",null,(int)Logger.CLogger.Modes.constructor);
            }
            catch(System.Exception e) {
                StreamWriter sw = new StreamWriter("error.txt", true);
                sw.Write(e.StackTrace);
                sw.Flush();
                sw.Close();

                logger.print(null, "Brak pliku konfiguracyjnego " + CConstrains.nodeNumber, (int)Logger.CLogger.Modes.error);
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
                logger.print(null, "in " + InputClientPortList[i].ID + "  " + InputClientPortList[i].STATUS,(int)Logger.CLogger.Modes.background);
            }
            for (int i = 0; i < OutputClientPortList.Count; i++) {
                logger.print(null, "out " + OutputClientPortList[i].ID + "  " + OutputClientPortList[i].STATUS, (int)Logger.CLogger.Modes.background);
            }
        }

        private CClientPortOut findFreePort() {   
            CClientPortOut t = OutputClientPortList.Find(delegate(CClientPortOut p) {  return p.STATUS == false; });
            return t;
        }
       
        
        // metoda odpowiedzialna za nadawanie wiadomości
        public void sendMsg(Data.CUserData data) {
            //Console.WriteLine("*** wyszukuje port... ***");
            CClientPortOut free = findFreePort();
            if (free == null) { logger.print("sendMsg","Wszystkie porty zajete",(int)Logger.CLogger.Modes.error); }
            else
            {
                //Console.WriteLine("*** port o id= " + free.ID + " jest wolny ***");
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
            logger.print(null,"Wstrzymywanie nadawnia na porcie : " + i,(int)Logger.CLogger.Modes.normal);
            if (OutputClientPortList[i].STATUS == true)
            {
                //OutputClientPortList[i].stop();
                OutputClientPortList[i].STATUS = false;
            }
            else { logger.print("stopSending","błędny numer portu",(int)Logger.CLogger.Modes.error ); }
        }

        public void shutdownAllPorts()
        {
            foreach(CClientPortIn p in InputClientPortList) {
                p.shutdown();
            }

        }

        public void getNodePortConfiguration()
        {
            logger.print(null,"\n\nNode ID = " + CConstrains.nodeNumber + " PORT CONFIGURATION\n\n",(int)Logger.CLogger.Modes.background);
            foreach (CClientPortIn p in InputClientPortList)
            {
                logger.print(null, "ID = " + p.ID + " LISTENING ON PORT = " + p.getPortNumber(), (int)Logger.CLogger.Modes.background);
            }
            foreach (CClientPortOut p in OutputClientPortList)
            {
                logger.print(null, "ID = " + p.ID + " SENDING TO PORT = " + p.getPortNumber(), (int)Logger.CLogger.Modes.background);
            }
        }



    }
}