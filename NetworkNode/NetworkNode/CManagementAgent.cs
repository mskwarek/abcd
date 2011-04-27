using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetworkNode
{
    static class CManagementAgent
    {

        private List<CNetworkPortIn> InputPortList = new List<CNetworkPortIn>();
        private List<CNetworkPortOut> OutputPortList = new List<CNetworkPortOut>();
        private int InputPortCount;
        private int OutputPortCount;
        CCommutationTable switchingField = new CCommutationTable();

        private  CManagementAgent()
        {
            readConfig();
            showConfig();
            createPorts();
            showPorts();
        }

        public void addConnection(CNetworkPortIn portIn,CNetworkPortOut portOut) //metoda zestawiajaca polaczenia w polu komutacyjnym danego wezla
        {
            portIn.STATUS = true;
            portOut.STATUS = true;
            switchingField.addConnection(portIn, portOut);
        }

        public void removeConnection(CNetworkPortIn portIn, CNetworkPortOut portOut) //metoda rozlaczajaca polaczenie w polu komutacyjnym danego wezla
        {
            portIn.STATUS = false;
            portOut.STATUS = false;
            switchingField.removeConnection(portIn);
        }

        public void showConnections() //metoda wyswietlajaca zestawione polaczenia
        {
            
            switchingField.showAll();   
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
                InputPortList.Add(new CNetworkPortIn(i, false));
            }

            for (int x = 0; x < OutputPortCount; x++) {
                OutputPortList.Add(new CNetworkPortOut(x, false));
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

        private CNetworkPortOut findFreePort()
        {
            CNetworkPortOut t = OutputPortList.Find(delegate(CNetworkPortOut p) { return p.STATUS == false; });
            return t;
        }
    }
}
