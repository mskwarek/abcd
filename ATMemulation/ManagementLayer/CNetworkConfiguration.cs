using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ManagementLayer
{
    class CNetworkConfiguration
    {
        List<CLink> LinkList = new List<CLink>();

        public void readConfig()
        {
            XmlTextReader textReader = new XmlTextReader("../../networkConfig.xml");
            try
            {
                while (textReader.Read())
                {
                    if (textReader.NodeType == XmlNodeType.Element && textReader.Name =="link")
                    {
                        //from
                        String[] fromArray = textReader.GetAttribute(0).Split(';');
                        String[] toArray = textReader.GetAttribute(1).Split(';');

                        if (fromArray[1] != toArray[1])
                        {
                            Console.WriteLine("błędne połączenie, łączysz port " + fromArray[1] + " z portem " + toArray[1]);
                            break;
                        }

                        CLinkInfo from = new CLinkInfo(Convert.ToInt32(fromArray[0]), fromArray[1], Convert.ToInt32(fromArray[2]));
                        CLinkInfo to = new CLinkInfo(Convert.ToInt32(toArray[0]), toArray[1], Convert.ToInt32(toArray[2]));
                        //add
                        LinkList.Add(new CLink(from, to));
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void showList()
        {
            Console.WriteLine("Konfiguracja sieci : ");
            foreach (CLink cl in LinkList)
                Console.WriteLine("node " + cl.from.nodeNumber + " port " + cl.from.portNumber + " type  " + cl.from.portType + " --> node " + cl.to.nodeNumber + " port " + cl.to.portNumber + " type " + cl.to.portType + " ");

        }

    }
}
