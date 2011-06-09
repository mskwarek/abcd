using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPlane
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CConstrains.domainName = args[0];
                CConstrains.configFileURL = CConstrains.defaultconfigFileURL + CConstrains.domainName + CConstrains.xmlEnding;
                
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : Brak argumentu");
                Console.WriteLine(e.StackTrace);
            }

            Console.Title = "Control Plane domainName = " + CConstrains.domainName;
            if (CConfigReader.Instance.readConfig())
            {
                CNetworkCallController.Instance.CNetworkCallControllerStart();
            }
            Console.ReadKey();            
       }
    }
}
