using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace  NetworkNode
{
    // Klasa portu wyjściowego dziedzicząca po CClientPort

    class CClientPortOut : CClientPort
    {



        //konstruktor
        public CClientPortOut(int i, bool p)
            : base(i, p)
        {
            base.PORTTYPE = CConstrains.PortType["PortTypeOUT"];
            base.PORTCLASS = CConstrains.PortType["ClientPortClass"];
        }
        // metoda używana przy portach wyjściowych. Po otrzymaniu topologi sieci port dostaje informacje na jaki port systemowy ma nadawać.
        public override void startPort(int systemPortNumber) 
        {

            base.PORTNUMBER = systemPortNumber;
            
        }




        public int send(Data.CCharacteristicData data)
        {
            TcpClient client = new TcpClient();
            client.Connect(CConstrains.ipAddress, base.PORTNUMBER);
            NetworkStream stream = client.GetStream();

            Data.CUserData cUserData = data.getCUserData();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, cUserData);
            Console.WriteLine("nadaje " + cUserData);
            List<byte> lista = new List<byte>();
            lista = cUserData.getInformation();

            

            stream.Flush();

            foreach (byte b in lista)
            {
                Console.Write(b + "  ");
            }
            return PORTNUMBER;
        }
    }
}
