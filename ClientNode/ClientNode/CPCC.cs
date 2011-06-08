using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ClientNode
{
    class CPCC
    {

        static CPCC instance = new CPCC();

        private CPCC()
        { }

        public static CPCC Instance
        {
            get
            {
                return instance;
            }
        }

        // metoda zgłaszająca call request do NCC
        public bool CallRequest(int fromNode, int toNode)
        {

            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, CConstrains.ControlPlanePortNumber);
            NetworkStream stream = client.GetStream();

            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
                {"FromNode", fromNode},
                {"ToNode",toNode},
                {"CallRequest",null}
                };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            CSNMPmessage msg = new Data.CSNMPmessage(pduList, null, null);
            msg.pdu.RequestIdentifier = "CallRequest:" + fromNode;

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            stream.Flush();
            Console.WriteLine("--> Sending CallRequest " + msg + " to NCC [" + fromNode + "->" + toNode + "]");
            client.Close();


            StreamReader sr = new StreamReader(stream);
            String responseFromCP = sr.ReadLine();

            if (responseFromCP.Equals("OK"))
            {
                Console.WriteLine("<-- " + responseFromCP + " <-- RESPONSE FROM NCC");
                return true;
            }
            else
            {
                Console.WriteLine("<-- " + responseFromCP + " <-- RESPONSE FROM NCC");
                return false;
            }


        }
        public void CallTeardown()
        {}

    }
}
