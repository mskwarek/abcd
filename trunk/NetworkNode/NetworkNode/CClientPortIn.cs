using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Data;
using System.Threading;

namespace NetworkNode
{
// Klasa portu wyjściowego dziedzicząca po CClientPort
    /// <summary>
    /// w ramach testów dodane zostały sleep
    /// </summary>

    class CClientPortIn : CClientPort
    {
        private bool status;
        private IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);     //adres serwera
        private TcpListener portListener;
        private TcpClient client;
        private NetworkStream clientStream;
       

        public CClientPortIn(int id, Boolean busy, int systemPortNumber) : base(id, busy)
        {
            base.PORTNUMBER = systemPortNumber;
            base.PORTTYPE = CConstrains.PortType["PortTypeIN"];
            base.PORTCLASS = CConstrains.PortType["ClientPortClass"];
            Console.WriteLine("Port kliencki o id = " + id + " będzie nasłuchiwał na porcie systemowym = " + base.PORTNUMBER);
            Thread t1 = new Thread(new ThreadStart(init));
            t1.Start();
            Thread t2 = new Thread(new ThreadStart(receiveData));
            t2.Start();
            
        }



        public void init() //metoda uruchamiająca nasłuchiwanie na porcie. 
        {
            status = true;
            portListener = new TcpListener(ip, base.PORTNUMBER);  //tworzymy obiekt  nasłuchujący na podanym porcie
            portListener.Start();                      //uruchamiamy serwer

            client = portListener.AcceptTcpClient(); //akceptujemy żądanie połączenia
            clientStream = client.GetStream();  //pobieramy strumień do wymiany danych
            Console.WriteLine("connection accepted ");
            while (status) //uruchamiamy nasłuchiwanie
            {
                //StreamReader sr = new StreamReader(clientStream);
                //Stream stream = new Stream(clientStream.); 
                BinaryFormatter binaryFormater = new BinaryFormatter();
                CUserData dane = (CUserData)binaryFormater.Deserialize(clientStream);
                queue.Enqueue(dane);
                Console.WriteLine(dane);
                Thread.Sleep(1000);
            }


        }

        private Data.CCharacteristicData prepareCharacteristicData(Data.CUserData data)
        {
            Data.CAdministrationData adData = new Data.CAdministrationData(Data.Contact.UNI, Data.PT._000_,Data.CLP._1_);
            //standardowy priorytet, moze w konsoli mozna podnosic priorytet wtedy byloby 0?
            adData.setHEC();
            //DO ROZWAZENIA
            //adData.setVCI(this.VCI); //czy moze tez za pomoca PortInfo?
            //adData.setVPI(this.VPI);
            Data.CCharacteristicData charData = new Data.CCharacteristicData(adData, data);
            
            return charData;
        }


        public void shutdown()
        {
            status = false;
            client.Close();
            portListener.Stop();
        }

        private void receiveData() //metoda odbierajace dane 
        {
            while (status)
            {
                if (queue.Count != 0)
                {
                    CCharacteristicData cCharacteristicData = prepareCharacteristicData(queue.Dequeue());
                    CCommutationField.Instance.passOnData(cCharacteristicData, this);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }
    
        

    
    }
}

