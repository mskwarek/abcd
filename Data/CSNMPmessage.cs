using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Data
{
    [Serializable]
    public class CSNMPmessage
    {
        private string version;
        private string community;
        public SNMPpdu pdu = new SNMPpdu();

        public CSNMPmessage(List<Dictionary<String, Object>> list, string ver, string com)
        {
            this.version = ver;
            this.community = com;
            this.pdu.variablebinding = list;
        }

        string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        string Community
        {
            get
            {
                return community;
            }
            set
            {
                community = value;
            }
        }

        SNMPpdu PDU
        {
            get
            {
                return pdu;
            }
            set
            {
                pdu = value;
            }
        }
    }
    [Serializable]
    public class SNMPpdu
    {
        private string type;
        public List<Dictionary<String, Object>> variablebinding; //lista zawierajaca jedna lub wiecej par nazwa obiektu - wartosci, dalsze tlumaczenie tego w ksiazce jest dla mnie lekko niezrozumiale o tej porze...
        private string requestIdentifier;

        public SNMPpdu() { }

        
        string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public string RequestIdentifier
        {
            get
            {
                return requestIdentifier;
            }
            set
            {
                requestIdentifier = value;
            }
        }


    }

    [Serializable]
    class SNMPpduResponse : SNMPpdu
    {
        private int errorStatus;           // w SNMPpdu dla request na tych miejscach sa 0 wiec nie definiowalem nowej klasy dla niego
        private int errorPosition;

        int ErrorStatus
        {
            get
            {
                return errorStatus;
            }
            set
            {
                errorStatus = value;
            }
        }

        int ErrorPosition
        {
            get
            {
                return errorPosition;
            }
            set
            {
                errorPosition = value;
            }
        }

    }

   // class SNMPpduTrap :SNMPpdu  chyba nie bedziemy tego uzywac
   // {}



}
