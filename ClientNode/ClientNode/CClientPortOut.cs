using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ClientNode
{
    // Klasa portu wyjściowego dziedzicząca po CClientPort

    class CClientPortOut : CClientPort
    {
        private static TcpClient client;
        private static String ip = "127.0.0.1";
        private NetworkStream stream;
        private StreamWriter clientStream;
        private int portNumber;
        //konstruktor
        public CClientPortOut(int id, bool busy): base(id,busy){}

        public void startPort(int portNumber)
        {
            this.portNumber = portNumber;
            Thread t = new Thread(new ThreadStart(init));
            t.Start();
        }
        
        public void init() // metoda łącząca socket z portNumber
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, portNumber);
                stream = client.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error.... " + e.StackTrace);
            }
        }

        private Data.CCharacteristicData prepareCharacteristicData(Data.CUserData data )
        {
            Data.CAdministrationData adData = new Data.CAdministrationData(Data.Contact.UNI);
            adData.setCLP(Data.CLP._1_);  //standardowy priorytet, moze w konsoli mozna podnosic priorytet wtedy byloby 0?
            adData.setHEC(calcHec());
            adData.setPayloadType(Data.PT._000_); // komorki uzytkownika zawsze maja 0 na poczatku, komorki zarzadzania ! maja 1 na poczatku- to raczej wazne
            adData.setVCI(this.VCI); //czy moze tez za pomoca PortInfo?
            adData.setVCI(this.VPI);
            Data.CCharacteristicData charData = new Data.CCharacteristicData();
            charData.setCAdministrationData(adData);
            charData.setCUserData(data);
            return charData;
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
    }
}
