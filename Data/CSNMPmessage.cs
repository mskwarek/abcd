using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Data
{
    public class CSNMPmessage
    {
        private string version;
        private string community;
        private SNMPpdu pdu;
    }

    class SNMPpdu
    {
        private string type;
        private List<Dictionary<String, String>> variablebinding; //lista zawierajaca jedna lub wiecej par nazwa obiektu - wartosci, dalsze tlumaczenie tego w ksiazce jest dla mnie lekko niezrozumiale o tej porze...
        private string requestIdentifier;
    }
    

    class SNMPpduResponse : SNMPpdu
    {
        private int errorStatus;           // w SNMPpdu dla request na tych miejscach sa 0 wiec nie definiowalem nowej klasy dla niego
        private int errorPosition; 
    
    }

   // class SNMPpduTrap :SNMPpdu  chyba nie bedziemy tego uzywac
   // {}



}
