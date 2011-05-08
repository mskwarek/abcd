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
        
        public CClientPortIn(int id, Boolean busy, int systemPortNumber): base(id, busy){
            this.portNumber = systemPortNumber;
        
            Thread portListen = new Thread(init);
            CConstrains.threadList.Add(portListen);
            
        }


        public void init() //metoda uruchamiająca nasłuchiwanie na porcie. 
        {
            Console.WriteLine("Port o id = " + base.ID + " będzie nasłuchiwał na porcie systemowym = " + portNumber);
            status = true;
            
            while (status) //uruchamiamy nasłuchiwanie
            {
                portListener = new TcpListener(ip, portNumber);  //tworzymy obiekt  nasłuchujący na podanym porcie
                portListener.Start();                      //uruchamiamy serwer
                Thread.Sleep(10000);
                client = portListener.AcceptTcpClient(); //akceptujemy żądanie połączenia
                clientStream = client.GetStream();  //pobieramy strumień do wymiany danych
                Console.WriteLine("connection accepted ");

                BinaryFormatter binaryFormater = new BinaryFormatter();
                CUserData dane = (CUserData)binaryFormater.Deserialize(clientStream);
                Console.WriteLine(dane);
            }

        }

        public void shutdown()
        {
            status = false;
            client.Close();
            portListener.Stop();
        }


    
        

    
    }
}

