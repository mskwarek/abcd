using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetworkNode
{
    public sealed class CManagementAgent
    {

       static readonly CManagementAgent instance = new CManagementAgent();


       private CManagementAgent()
       {
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
