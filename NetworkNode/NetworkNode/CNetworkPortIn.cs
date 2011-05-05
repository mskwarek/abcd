using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace NetworkNode
{
    public class CNetworkPortIn : CNetworkPort
    {

        private bool status;
        private IPAddress ip = IPAddress.Parse(CConstrains.ipAddress);     //adres serwera
        private TcpListener portListener;
        private TcpClient client;
        private NetworkStream clientStream;
    


        public CNetworkPortIn(int id, Boolean busy, int systemPortNumber)
            : base(id, busy)
        {
            base.PORTTYPE = CConstrains.PortType["PortTypeIN"];
            base.PORTCLASS = CConstrains.PortType["NetworkPortClass"];
            base.PORTNUMBER = systemPortNumber;
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
            Console.WriteLine(base.PORTCLASS + " o id = " + base.ID + " będzie nasłuchiwał na porcie systemowym = " + base.PORTNUMBER);
            client = portListener.AcceptTcpClient(); //akceptujemy żądanie połączenia
            clientStream = client.GetStream();  //pobieramy strumień do wymiany danych
            Console.WriteLine("connection accepted");
            while (status) //uruchamiamy nasłuchiwanie
            {
                //StreamReader sr = new StreamReader(clientStream);
                //Stream stream = new Stream(clientStream.); 
                BinaryFormatter binaryFormater =new BinaryFormatter();
                CCharacteristicData dane = (CCharacteristicData) binaryFormater.Deserialize(clientStream);
                queue.Enqueue(dane);
                Console.WriteLine(dane);
                Thread.Sleep(1000);
            }

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
                    CCommutationField.Instance.passOnData(queue.Dequeue(), this);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }


        
    } 
}
