using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ManagementLayer
{
    public sealed class CNetworkConfiguration
    {
        static readonly CNetworkConfiguration instance = new CNetworkConfiguration();

        CNetworkConfiguration()
        {
        }

        public static CNetworkConfiguration Instance
        {
            get
            {
                return instance;
            }
        }

        Dictionary<int, String> nodesType = new Dictionary<int, string>() { {1,"client"}};
        private List<CLink> LinkList = new List<CLink>();

        public List<CLink> linkList
        {
            get { return LinkList; }
            set { LinkList = value; }
        }

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
                        LinkList.Add(new CLink(from, to, 1));
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void showNetworkConfiguration()
        {
            Console.WriteLine("Konfiguracja sieci : ");
            foreach (CLink cl in LinkList)
                Console.WriteLine("node " + cl.from.nodeNumber + " port " + cl.from.portNumber + " type  " + cl.from.portType + " --> node " + cl.to.nodeNumber + " port " + cl.to.portNumber + " type " + cl.to.portType + " ");

        }

        public void addNodeToDictionary(int i, String type)
        {
            try
            {
                nodesType.Add(i, type);
                Console.WriteLine("dodałem " + i + " " + type);
            }
            catch (Exception e)
            {
                Console.WriteLine("Błąd przy dodawaniu węzła do słownika, węzeł o id = " + i + " juz isnieje");
            }
        }

        public void showNodes()
        {
            foreach(KeyValuePair<int, String> pair in nodesType)
            {
                Console.WriteLine("\t Wezly w sieci");
                Console.WriteLine("{0}, {1}",
                pair.Key,
                pair.Value);
            }
        }

        public bool checkFormula(int args)
        {
            // sprawdzanie czy takie połączenie już istnieje oraz czy istnieją nody
            bool warunek1 = false;
            //bool warunek2 = false;


            if (nodesType.ContainsKey(args)) 
            {
                warunek1 = true;
            }

            //CLink cl = new CLink(new CLinkInfo(args[0], null, args[1]), new CLinkInfo(args[2], null, args[3]));

            //if (!LinkList.Contains(cl)) 
            //{
            //    warunek2 = true;
            //}

            if(warunek1)
            { return true; }

            return false;
        }


    }
}
