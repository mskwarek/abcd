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
        private Logger.CLogger logger = Logger.CLogger.Instance;
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
            
            logger.print("CallRequest", "--> Sending CallRequest " + msg + " to NCC [" + fromNode + "->" + toNode + "]", (int)Logger.CLogger.Modes.normal);
           
            StreamReader sr = new StreamReader(stream);
            String responseFromCP = sr.ReadLine();
            client.Close();


            if (responseFromCP.Equals("OK"))
            {
                //Console.WriteLine("<-- " + responseFromCP + " <-- RESPONSE FROM NCC");

                logger.print(null, "<-- " + responseFromCP + " <-- RESPONSE FROM NCC", (int)Logger.CLogger.Modes.normal);
                return true;
            }
            else
            {
                logger.print(null, "<-- " + responseFromCP + " <-- RESPONSE FROM NCC", (int)Logger.CLogger.Modes.error);
                return false;
            }


        }
        public bool CallTeardown(int fromNode, int toNode)
        {

            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, CConstrains.ControlPlanePortNumber);
            NetworkStream stream = client.GetStream();

            Dictionary<String, Object> pduDict = new Dictionary<String, Object>() {
                {"FromNode", fromNode},
                {"ToNode",toNode},
                {"CallTeardown",null}
                };
            List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
            pduList.Add(pduDict);
            CSNMPmessage msg = new Data.CSNMPmessage(pduList, null, null);
            msg.pdu.RequestIdentifier = "CallTeardown:" + fromNode;

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, msg);
            stream.Flush();
            logger.print("CallTeardown", "--> Sending CallTeardown " + msg + " to NCC [" + fromNode + "->" + toNode + "]", (int)Logger.CLogger.Modes.normal);
            StreamReader sr = new StreamReader(stream);
            String responseFromCP = sr.ReadLine();
            client.Close();


            if (responseFromCP.Equals("OK"))
            {
                logger.print(null, "<-- " + responseFromCP + " <-- RESPONSE FROM NCC", (int)Logger.CLogger.Modes.normal);
                return true;
            }
            else
            {
                logger.print(null, "<-- " + responseFromCP + " <-- RESPONSE FROM NCC", (int)Logger.CLogger.Modes.error);
                return false;
            }


        }

    }
}
