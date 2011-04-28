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
       //CCommutationTable switchingField = new CCommutationTable();

        private  CManagementAgent()
        {

        }

        public void addConnection(CNetworkPortIn portIn,CNetworkPortOut portOut) //metoda zestawiajaca polaczenia w polu komutacyjnym danego wezla
        {
            portIn.STATUS = true;
            portOut.STATUS = true;
            //Wydaje mi się, że to nie tak!
                //CCommutationTable.Instance.addEntry(
        }

        public void removeConnection(CNetworkPortIn portIn, CNetworkPortOut portOut) //metoda rozlaczajaca polaczenie w polu komutacyjnym danego wezla
        {
            portIn.STATUS = false;
            portOut.STATUS = false;
            //CCommutationTable.Instance.removeConnection(portIn);
        }

        public void showConnections() //metoda wyswietlajaca zestawione polaczenia
        {
            
            CCommutationTable.Instance.showAll();   
        }

        
   }
}
