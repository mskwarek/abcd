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
        private Logger.CLogger logger = Logger.CLogger.Instance;



        //konstruktor
        public CNetworkPortOut(int id, bool busy) : base(id, busy) {
            base.PORTTYPE = CConstrains.PortType["PortTypeOUT"];
            base.PORTCLASS = CConstrains.PortType["NetworkPortClass"];
        }
        // metoda używana przy portach wyjściowych. Po otrzymaniu topologi sieci port dostaje informacje na jaki port systemowy ma nadawać.
        public override void startPort(int systemPortNumber) 
        {
            base.PORTNUMBER = systemPortNumber;
            base._busy = true;
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine("Error.... " + e.StackTrace);
            }
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

        public int send(Data.CCharacteristicData data, Data.PortInfo outputPortInfo, bool helloMsg)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(CConstrains.ipAddress, base.PORTNUMBER);
                NetworkStream stream = client.GetStream();
                // jeżeli helloMsg = true wysyła hello msg 
                if (helloMsg == false)
                {
                    data = prepareNewAdministrationData(data, outputPortInfo);
                }


                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, data);
                stream.Flush();
                if (helloMsg == true)
                {
                    logger.print(null,"-->SENDING HELLO TO : " + base.PORTNUMBER,(int)Logger.CLogger.Modes.background);
                    StreamReader sr = new StreamReader(stream);
                    String dane = sr.ReadLine();
                    CNetManager.Instance.fillTable(base.ID, dane, true);

                }
                else
                    logger.print(null,"--> SENDING " + data + " TO : " + base.PORTNUMBER,(int)Logger.CLogger.Modes.normal);


                List<byte> lista = new List<byte>();
                lista = data.getCUserData().getInformation();


                foreach (byte b in lista)
                {
                    Console.Write("["+b+"]");
                }
                }
            catch (Exception e)
            {
                logger.print(null,base.PORTNUMBER + " is unreachable ",(int)Logger.CLogger.Modes.normal);
                CNetManager.Instance.fillTable(base.ID, "0;x;0;0", false);
                //return PORTNUMBER;
                
            }

            return PORTNUMBER;
            
            
        }

    }
}
