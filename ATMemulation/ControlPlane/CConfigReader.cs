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
            if(File.Exists(CConstrains.configFileURL) )
            {

            XmlTextReader textReader = new XmlTextReader(CConstrains.configFileURL);
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
            }
            else
            {
                Logger.CLogger.Instance.print(null, "Nie znalazłem pliku konfiguracyjnego!!!", (int)Logger.CLogger.Modes.error);
              
          return false;
          }
        }


    }
}
