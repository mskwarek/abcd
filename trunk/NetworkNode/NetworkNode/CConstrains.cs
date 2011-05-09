using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    public static class CConstrains
    {
        public static int inputClientPortNumber;
        public static int outputClientPortNumber;
        public static int inputNetworkPortNumber;
        public static int outputNetworkPortNumber;
        public static int nodeNumber;
        public const String nodeType = "NetworkNode";
        public static string ipAddress = "127.0.0.1";
        public static int managementLayerPort = 49999;
        public static String configFileURL = "../../config" + nodeNumber + ".xml";
        public static String defaultConfigFileURL = "../../defaultConfig.xml";

        public static Dictionary<string, string> PortType = new Dictionary<string, string>()
        {
                {"PortTypeIN" , "IN" },
                {"PortTypeOUT" ,"OUT"},
                {"ClientPortClass", "CLIENTPORT"},
                {"NetworkPortClass" , "NETWORKPORT"}
        };        
        
        


        public static int inputPortNumber { get { return inputClientPortNumber + inputNetworkPortNumber; } }
        public static int outputPortNumber { get { return outputClientPortNumber + outputNetworkPortNumber; } }


    }
}
