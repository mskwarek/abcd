using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Klasa portu wyjściowego dziedzicząca po CClientPort

namespace ClientNode
{
    class CClientPortIn : CClientPort
    {

        public CClientPortIn(int i, Boolean p):       
           base(i, p){}

        private bool setHeader(Data.CUserData, int destination)
        {
            return true;
        }

        public void prepareData(Data.CUserData userData, int destination)
        {
            if(setHeader(userData, destination))
            {
                //wrzuc na pole komutacyjne
                
            }

        }

    }
}

