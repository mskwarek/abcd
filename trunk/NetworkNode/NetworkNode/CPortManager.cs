using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NetworkNode
{
    // ta klasa ma za zadanie odczytać z pliku konfiguracyjnego i utworzyc odpowiednią liczbę portów wyjściowych klienckich
    public sealed class CPortManager
    {
        static readonly CPortManager instance = new CPortManager();

        static CPortManager()
        {
        }

        CPortManager()
        {
        }

        public static CPortManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
        //Dodatkowo musi zapewniać odwoływanie do portów po ich ID 
        //(Agent zarządzania wypełniając tablicę podaje ID, VPI, VCI, Pole komutacyjne odczytuje z tablicy komutacji ID portu na podstawie VPI, VCI
        // i pobiera odpowiedni port na podstawie jego ID odwołując się do CPortManager)
    
}