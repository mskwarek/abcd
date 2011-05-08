using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkNode
{
    public class CNetworkPortOut:CNetworkPort
    {
        private static TcpClient client;
        private NetworkStream stream;
        private StreamWriter clientStream;



        //konstruktor
        public CNetworkPortOut(int id, bool busy) : base(id, busy) {
            base.PORTTYPE = CConstrains.PortType["PortTypeOUT"];
            base.PORTCLASS = CConstrains.PortType["NetworkPortClass"];
        }
        // metoda używana przy portach wyjściowych. Po otrzymaniu topologi sieci port dostaje informacje na jaki port systemowy ma nadawać.
        public override void startPort(int systemPortNumber) 
        {
            base.PORTNUMBER = systemPortNumber;
            try
            {
                client = new TcpClient();
                client.Connect(CConstrains.ipAddress, base.PORTNUMBER);
                stream = client.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error.... " + e.StackTrace);
            }
        }

        public void shutdown()
        {
            client.Close();
        }

        // metoda nawiązująca połączenie z węzłem sieciowym i nadająca do niego TO DO
        public void send(String str)
        {
            Console.WriteLine("nadaje " + str);
            clientStream = new StreamWriter(stream);
            clientStream.WriteLine(str);
            clientStream.Flush();
        }
        private Data.CCharacteristicData prepareNewAdministrationData(Data.CCharacteristicData data, Data.PortInfo outputPortInfo)
        {
            //pobieram dawne dane administracyjne
            Data.CAdministrationData oldCAdministrationData = data.getCAdministrationData();
            Data.CAdministrationData newCAdministrationData = oldCAdministrationData;
            //zmieniam adresy VPI/VCI
            newCAdministrationData.setVCI(outputPortInfo.getVCI());
            newCAdministrationData.setVPI(outputPortInfo.getVPI());
            data.setCAdministrationData(newCAdministrationData);
            return data;
        }

        public int send(Data.CCharacteristicData data, Data.PortInfo outputPortInfo)
        {
            data = prepareNewAdministrationData(data, outputPortInfo);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, data);
            stream.Flush();
            Console.WriteLine("nadaje " + data);

            return PORTNUMBER;
        }

    }
}
