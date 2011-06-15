using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPlane
{
    public static class CConstrains
    {
        public static List<int> NCCList = new List<int>();
        public static string ipAddress = "127.0.0.1";
        public static int NCCportNumber;// = 50009;
        public static int LMportNumber;
        public static string domainName;
        public static String defaultconfigFileURL = "../../../starter/networkConfig";
        public static String configFileURL; // = "../../defaultConfig.xml";
        public static String xmlEnding = ".xml";
        public static Dictionary<string, string> PortType = new Dictionary<string, string>()
        {
                {"PortTypeIN" , "IN" },
                {"PortTypeOUT" ,"OUT"},
                {"ClientPortClass", "CLIENTPORT"},
                {"NetworkPortClass" , "NETWORKPORT"}
        };       
    }
}
