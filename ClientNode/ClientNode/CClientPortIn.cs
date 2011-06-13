using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Data;
using System.Runtime.Serialization.Formatters.Binary;

// Klasa portu wyjściowego dziedzicząca po CClientPort

namespace ClientNode
{
    class CClientPortIn : CClientPort
    {
        private bool status;
        private IPAddress ip = IPAddress.Parse("127.0.0.1");     //adres serwera
        private TcpListener portListener;
        private TcpClient client;
        private NetworkStream clientStream;
        private int portNumber;
        private Logger.CLogger logger = Logger.CLogger.Instance;
        public CClientPortIn(int id, Boolean busy, int systemPortNumber): base(id, busy){
            this.portNumber = systemPortNumber;
        
            Thread portListen = new Thread(init);
            portListen.Start();
            portListen.Name = "init " + portNumber;

            
            
        }


        public void init() //metoda uruchamiająca nasłuchiwanie na porcie. 
        {
            String text = "Port : " + base.ID + " Listening on : " + portNumber;
            
            logger.print(null,text, (int)Logger.CLogger.Modes.background );
            status = true;
            portListener = new TcpListener(ip, portNumber);  //tworzymy obiekt  nasłuchujący na podanym porcie
            portListener.Start();   

            while (status) //uruchamiamy nasłuchiwanie
            {
                                   //uruchamiamy serwer
                
                client = portListener.AcceptTcpClient(); //akceptujemy żądanie połączenia
                clientStream = client.GetStream();  //pobieramy strumień do wymiany danych
                //Console.WriteLine("receiving");

                BinaryFormatter binaryFormater = new BinaryFormatter();
                CUserData dane = (CUserData)binaryFormater.Deserialize(clientStream);
                List<byte> lista = new List<byte>();
                lista = dane.getInformation();

                foreach (byte b in lista)
                {
                    Console.Write(b + "  ");
                }
                
                Thread.Sleep(5000);
                }

        }

        public void shutdown()
        {
            status = false;
            client.Close();
            portListener.Stop();
        }
        public int getPortNumber()
        {
            return portNumber;
        }

    
        

    
    }
}

