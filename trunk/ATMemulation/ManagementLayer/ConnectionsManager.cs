using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    public sealed class ConnectionsManager
    {
        static readonly ConnectionsManager instance = new ConnectionsManager();

        public static ConnectionsManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void addConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B)
        {
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            //TODO send to node nodeNumber as add request

            Console.WriteLine("node : " + nodeNumber + "from : " + portNumber_A + " to : " + portNumber_B);
        }

        public void removeConnection(int nodeNumber, int portNumber_A, int VPI_A, int VCI_A, int portNumber_B, int VPI_B, int VCI_B)
        {
            Data.PortInfo portIn = new Data.PortInfo(portNumber_A, VPI_A, VCI_A);
            Data.PortInfo portOut = new Data.PortInfo(portNumber_B, VPI_B, VCI_B);

            //TODO send to node nodeNumber as remove request

            Console.WriteLine("node : " + nodeNumber + "from : " + portNumber_A + " to : " + portNumber_B);
        }
    }
}
