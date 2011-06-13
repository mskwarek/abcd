﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Net;
using System.IO;
using RoutingController;
using LinkResourceManager;
using RouteEngine;
using Data;

namespace ControlPlane
{
    public sealed class CNetworkCallController
    {
        CLinkResourceManager cLinkResourceManager = CLinkResourceManager.Instance;
        CConnectionController cCConectionController = CConnectionController.Instance;
        public List<Data.CPNNITable> PNNIList = new List<Data.CPNNITable>();
        private Logger.CLogger logger = Logger.CLogger.Instance;
        static readonly CNetworkCallController instance = new CNetworkCallController();

        public static CNetworkCallController Instance
        {
            get
            {
                return instance;
            }
        }

        private CNetworkCallController()
        {
            logger.print("NetworkCallController", null, (int)Logger.CLogger.Modes.constructor);
            
        }

        public void CNetworkCallControllerStart()
        {
           Thread t = new Thread(NCCListener);
           t.Name = " Network Call Controller listener";
           t.Start();
        
        }
        public void NCCListener() 
        {
            bool status = true;
            IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);     //adres serwera
            TcpListener portListener = new TcpListener(ip, CConstrains.NCCportNumber);
            portListener.Start();
            logger.print(null, "NCC listening on : " + CConstrains.NCCportNumber, (int)Logger.CLogger.Modes.background);
            while (status)
            {
                TcpClient client = portListener.AcceptTcpClient();
                NetworkStream clientStream = client.GetStream();
                StreamWriter downStream = new StreamWriter(clientStream);
                logger.print(null,"CONNECTION FROM CPCC ACCEPTED",(int)Logger.CLogger.Modes.normal);
                BinaryFormatter binaryFormater = new BinaryFormatter();
                Data.CSNMPmessage dane = (Data.CSNMPmessage)binaryFormater.Deserialize(clientStream);
                if (dane.pdu.RequestIdentifier.StartsWith("CallRequest"))
                {
                    foreach (Dictionary<String, Object> d in dane.pdu.variablebinding)
                    {
                      if (d.ContainsKey("CallRequest"))
　　　　　　　　　　　　{
　　　　　　　　　　　　　　int fromNode = Convert.ToInt16(d["FromNode"]);
　　　　　　　　　　　　　　int toNode = Convert.ToInt16(d["ToNode"]);
　　　　　　　　　　　　　　bool exists=false;
　　　　　　　　　　　　　　foreach ( Data.CPNNITable t in PNNIList)
　　　　　　　　　　　　　　{

　　　　　　　　　　　　　　　　if (t.NodeNumber == toNode || t.NeighbourNodeNumber == toNode)

　　　　　　　　　　　　　　　　　　exists = true;
　　　　　　　　　　　　　　}
　　　　　　　　　　　　　　if (exists)
　　　　　　　　　　　　　　{


　　　　　　　　　　　　　　　　if (ConnectionRequest(fromNode, toNode))
　　　　　　　　　　　　　　　　{
　　　　　　　　　　　　　　　　　　downStream.WriteLine("OK");
　　　　　　　　　　　　　　　　　　downStream.Flush();
　　　　　　　　　　　　　　　　}
　　　　　　　　　　　　　　　　else
　　　　　　　　　　　　　　　　{
　　　　　　　　　　　　　　　　　　downStream.WriteLine("ERROR");
　　　　　　　　　　　　　　　　　　downStream.Flush();
　　　　　　　　　　　　　　　　}
　　　　　　　　　　　　　　}
　　　　　　　　　　　　　　else
　　　　　　　　　　　　　　{

　　　　　　　　　　　　　　　　if (NetworkCallCoordinationOut(fromNode, toNode))
　　　　　　　　　　　　　　　　{
　　　　　　　　　　　　　　　　　　downStream.WriteLine("OK");
　　　　　　　　　　　　　　　　　　downStream.Flush();
　　　　　　　　　　　　　　　　}
　　　　　　　　　　　　　　　　else
　　　　　　　　　　　　　　　　{
　　　　　　　　　　　　　　　　　　downStream.WriteLine("ERROR");
　　　　　　　　　　　　　　　　　　downStream.Flush();
　　　　　　　　　　　　　　　　}
　　　　　　　　　　　　　　}

　　　　　　　　　　　　} 
                    }
                }
                else if (dane.pdu.RequestIdentifier.StartsWith("CallTeardown"))
                {
                    foreach (Dictionary<String, Object> d in dane.pdu.variablebinding) {
                        if (d.ContainsKey("CallTeardown")) {
                            int fromNode = Convert.ToInt16(d["FromNode"]);
                            int toNode = Convert.ToInt16(d["ToNode"]);
                            //Metoda zlecająca CC rozlaczenie połączenia.
                            if (CallTeardownOut(fromNode, toNode)) {
                                downStream.WriteLine("OK");
                                downStream.Flush();
                            }
                            else {
                                downStream.WriteLine("ERROR");
                                downStream.Flush();
                            }
                        }
                    }
                }
                else if (dane.pdu.RequestIdentifier.StartsWith("PNNIList ML")) {
                    foreach (Data.CPNNITable t in dane.pdu.PNNIList) {
                        if (PNNIList.Contains(t)) {
                            int index = PNNIList.IndexOf(t);
                            if (PNNIList.ElementAt(index).IsNeighbourActive != t.IsNeighbourActive) {
                                PNNIList.ElementAt(index).IsNeighbourActive = t.IsNeighbourActive;
                                CRoutingController.Instance.updateRCTable(t);
                                logger.print(null,"PNNIList Updated  " + PNNIList.ElementAt(index).NodeNumber + " " + PNNIList.ElementAt(index).NodeType + " ACTIVITY :  " + PNNIList.ElementAt(index).IsNeighbourActive,(int)Logger.CLogger.Modes.normal);
                            }
                        }
                        else {
                            PNNIList.Add(t);
                            CRoutingController.Instance.updateRCTable(t);
                            logger.print(null, "PNNIList  ADDED : " + t.NodeNumber + " " + t.NodeType + " " + t.NodePortNumberSender + " " + t.NeighbourNodeNumber + " " + t.NeighbourNodeType + " " + t.NeighbourPortNumberReciever + " " + t.IsNeighbourActive, (int)Logger.CLogger.Modes.normal);
                        }
                        downStream.WriteLine("--> SENDING RESPONSE : " + dane.pdu.RequestIdentifier + " -OK- ");
                        downStream.Flush();
                    }
                }
                else if (dane.pdu.RequestIdentifier.StartsWith("NodeActivity")) {
                    foreach (Data.CPNNITable t in dane.pdu.PNNIList) {
                        if (PNNIList.Contains(t)) {
                            int index = PNNIList.IndexOf(t);
                            if (PNNIList.ElementAt(index).IsNeighbourActive != t.IsNeighbourActive) {
                                PNNIList.ElementAt(index).IsNeighbourActive = t.IsNeighbourActive;
                                CRoutingController.Instance.updateRCTable(t);
                                logger.print(null, "PNNIList Updated  " + PNNIList.ElementAt(index).NodeNumber + " " + PNNIList.ElementAt(index).NodeType + " ACTIVITY :  " + PNNIList.ElementAt(index).IsNeighbourActive, (int)Logger.CLogger.Modes.normal);
                                sendPNNIListToML(PNNIList);
                            }
                        }
                        else {
                            PNNIList.Add(t);
                            CRoutingController.Instance.updateRCTable(t);
                            logger.print(null, "PNNIList  ADDED : " + t.NodeNumber + " " + t.NodeType + " " + t.NodePortNumberSender + " " + t.NeighbourNodeNumber + " " + t.NeighbourNodeType + " " + t.NeighbourPortNumberReciever + " " + t.IsNeighbourActive, (int)Logger.CLogger.Modes.normal);
                            sendPNNIListToML(PNNIList);
                        }
                        downStream.WriteLine("--> SENDING RESPONSE : " + dane.pdu.RequestIdentifier + " -OK- ");
                        downStream.Flush();
                    }
                }
                else if (dane.pdu.RequestIdentifier.StartsWith("NetworkCallCoordination")) {

                    bool exist = false;
                    int nodeNumber=0;
                    foreach (Dictionary<String, Object> d in dane.pdu.variablebinding) {
                        if (d.ContainsKey("CallRequest")) {
                        nodeNumber = Convert.ToInt32(d["ToNode"]);
                            foreach (Data.CPNNITable t in PNNIList) {
                                if (t.NodeNumber == nodeNumber || t.NeighbourNodeNumber== nodeNumber) {
                                    exist = true;
                                    downStream.WriteLine("Confirmation");
                                    downStream.Flush();
                                    break;
                                }
                            }
                        }
                    }
                    if (!exist) {
                        downStream.WriteLine("Rejected");
                        downStream.Flush();
                    }
                    else {
                        int borderNodeNumber = 0;
                        foreach (Data.CPNNITable t in PNNIList) {
                            if (t.NodeType.Equals("border")) {
                                borderNodeNumber = t.NodeNumber;
                                break; //bo i tak jeden border
                            }
                        }
                        ConnectionRequest(borderNodeNumber, nodeNumber);
                    }
                }
                logger.print(null,dane.pdu.RequestIdentifier,(int)Logger.CLogger.Modes.normal);
                Thread.Sleep(1000);
            }
        }

        public void sendPNNIListToML(List<Data.CPNNITable> lista)
        {
            Data.CSNMPmessage msg;

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(CConstrains.ipAddress, CConstrains.LMportNumber);
                NetworkStream stream = client.GetStream();
                msg = new Data.CSNMPmessage(null, null, null);
                msg.pdu.PNNIList = lista;
                msg.pdu.RequestIdentifier = "PNNIList ML";
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, msg);
                stream.Flush();
                logger.print(null, "--> Sending PNNIListMsg  " + msg + " to ML ", (int)Logger.CLogger.Modes.background);
                StreamReader sr = new StreamReader(stream);
                String dane = sr.ReadLine();
                logger.print(null,"<-- " + dane,(int)Logger.CLogger.Modes.background);
            }
            catch (Exception e)
            {
                logger.print("sendPNNIListToCP", "ML niesdostępny", (int)Logger.CLogger.Modes.error);
            }
        }

        public void CallIndication()
        { }
        

        // tu wywołujemy metody CC aby dalej zestawić połączenie - na końcu musi CC zwrócić true albo false
        public bool ConnectionRequest(int fromNode, int toNode)
        {
            if (cCConectionController.ConnectionRequestIn(fromNode, toNode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //uzywane przy wielu domenach
        public bool NetworkCallCoordinationOut(int fromNode, int toNode)
        {
            List<int> adjacentNCCs = CConstrains.NCCList;
            foreach (int NCC in adjacentNCCs)
            {
                TcpClient client = new TcpClient();
                client.Connect(CConstrains.ipAddress, NCC);
                NetworkStream stream = client.GetStream();
                Dictionary<String, Object> pduDict = new Dictionary<String, Object>(){
                    {"FromNode",fromNode},
                    {"ToNode", toNode},
                    {"CallRequest", null}
                };
                List<Dictionary<String, Object>> pduList = new List<Dictionary<String, Object>>();
                pduList.Add(pduDict);
                CSNMPmessage msg = new Data.CSNMPmessage(pduList, null, null);
                msg.pdu.RequestIdentifier = "NetworkCallCoordination:" + CConstrains.NCCportNumber;
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, msg);
                stream.Flush();
                logger.print(null," --> Sending NetworkCallCordination" + msg + " to other NCC [" + CConstrains.NCCportNumber + "->" + NCC + "]",(int)Logger.CLogger.Modes.normal);
                StreamReader sr = new StreamReader(stream);
                String responseFromNCC = sr.ReadLine();
                Console.WriteLine(responseFromNCC);
                //client.Close();
                if (responseFromNCC.Equals("Confirmation"))
                {
                    int borderNodeNumber = 0;
                    foreach (Data.CPNNITable t in PNNIList)
                    {
                        if (t.NodeType.Equals("border"))
                        {
                            borderNodeNumber = t.NodeNumber;

                            break ; //bo i tak jeden border
                        }
                    }
                    ConnectionRequest(fromNode, borderNodeNumber);
                    return true;
                }
                else if (responseFromNCC.Equals("Rejected"))
                    continue;
            }
            return false;
        }

        // metoda zamieniajaca nazwe lokalna na identyfikator polaczenia 
        // kierowane do directory
        public void DirectoryRequest(string localName)
        { }

        public bool CallTeardownOut(int source, int destination)
        {
            
            //potrzeba metody ktora zamienii source i destination na cala sciezke od source do destination
            //niby w route handlerze jest lista routow, ale duzo operacji chyba trzeba robic zeby wyluskac
            //to co trzeba, moze dodac do klasy route pole source i destination i napisac jakas metode?
            RouteEngine.Route route = CConnectionController.Instance.getRouteByIdentifier(CConnectionController.Instance.setIdentifier(source, destination));

            if (route != null && route.Connections != null)
            {
                List<CLink> links = route.Connections;
                int i = 0;
                CLink link;
                do
                {
                    link = links[i];
                    CLink temp;
                    temp = CConnectionController.Instance.LinkConnectionDeallocation(link);
                    i++;
                } while (i < links.Count);

            }
            else
                return false;
            List<CConnectionController.commutationEntry> commutationTables = CConnectionController.Instance.CommutationTables;
            if (commutationTables.Count > 0)
            {
                int i = commutationTables.Count - 1;
                CConnectionController.commutationEntry commutationEntry;
                do
                {
                    commutationEntry = commutationTables[i];
                    CConnectionController.Instance.removeConnection(commutationEntry);
                    commutationTables.Remove(commutationEntry);
                    i--;
                } while (i >= 0);

                CConnectionController.Instance.removeRouteByIdentifier(CConnectionController.Instance.setIdentifier(source, destination));
                return true;
            }
            else
                return false;
        }
    }
}