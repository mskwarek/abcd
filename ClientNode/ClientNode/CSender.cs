using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClientNode
{
    sealed class CSender
    {
        CPortManager cpm = CPortManager.Instance;

        private readonly static CSender instance = new CSender();

        private CSender()
        {
        }

        public static CSender Instance
        {
            get
            {
                return instance;
            }
        }

        public void sendData()
        {
            while (true)
            {
                Data.CUserData data = new Data.CUserData();
                List<byte> temp = new List<byte>();
                System.Random x = new Random(System.DateTime.Now.Millisecond);
                for (int i = 0; i < 48; i++)
                {
                    temp.Add((byte)x.Next(0, 127));        // dodawanie kolejnych bajtow do danych do wyslania
                }

                data.setInformation(temp);
                cpm.sendMsg(data);
                Thread.Sleep(5000);
            }
        }

    }
}
