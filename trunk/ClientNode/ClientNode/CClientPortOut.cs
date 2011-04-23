using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    // Klasa portu wyjściowego dziedzicząca po CClientPort

    class CClientPortOut : CClientPort
    {
        private int i;
        private bool p;

        public CClientPortOut(int i, bool p): base(i,p){}

        // metoda nawiązująca połączenie z węzłem sieciowym i nadająca do niego TO DO
        public void send(String str)
        {
            Console.WriteLine("nadaje " + str);
        }

        public void stop()
        {
            Console.WriteLine("wstrzymałem nadawnie");
        }
    }
}
