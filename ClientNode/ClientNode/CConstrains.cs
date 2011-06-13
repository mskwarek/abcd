using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace ClientNode
{
    public static class CConstrains
    {
        public static int inputPortNumber;
        public static int outputPortNumber;
        public static int nodeNumber;
        public  const String nodeType = "ClientNode";
        public static List<Thread> threadList = new List<Thread>();
        public static String defaultconfigFileURL = "../../../../ATMemulation/starter/networkConfig";
        public static String configFileURL; // = "../../defaultConfig.xml";
        public static String xmlEnding = ".xml";
        public static string ipAddress = "127.0.0.1";
        public static int managementLayerPort; //= 49999;
        public static int ControlPlanePortNumber;// = 50009;
        public static int CloudPortNumber;
        public static string domainName;

    }
}
