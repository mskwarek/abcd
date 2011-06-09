using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
namespace ControlPlane
{
    public sealed class CConfigReader
    {
        static readonly CConfigReader instance = new CConfigReader();

        CConfigReader() { }

        public static CConfigReader Instance
        {
            get
            {
                return instance;
            }
        }

        public bool readConfig()
        {
            if(File.Exists("../../../starter/networkConfig.xml") )
            {

            XmlTextReader textReader = new XmlTextReader("../../../starter/networkConfig.xml");
            try
            {
                while (textReader.Read())
                {
                    if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "managementLayerPort")
                    {
                        CConstrains.LMportNumber = Convert.ToInt32(textReader.ReadElementContentAsInt());
                    }
                    else if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "nccPort")
                    {
                        CConstrains.NCCportNumber = Convert.ToInt32(textReader.ReadElementContentAsInt());
                    }
                    else if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "neighbourNccPort")
                    {
                        CConstrains.NCCList.Add(Convert.ToInt32(textReader.ReadElementContentAsInt()));
                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
          }else {Console.WriteLine("Nie znalazłem pliku konfiguracyjnego!!!");
          return false;
          }
        }


    }
}
