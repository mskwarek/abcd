using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public enum Contact { UNI, NNI};
    public enum PT { _000_, _001_, _010_, _011_, _100_, _101_, _110_, _111_ };
    public enum CLP { _0_, _1_ };

    [Serializable]
    public class CAdministrationData
    {
        private static int VPI_UNI_MAX = 255;
        private static int VPI_NNI_MAX = 4095;
        private static int VCI_MAX = 65535;

        private Contact contact;
        private int GFC = 0;
        private int VPI;
        private int VCI;
        private PT payloadType;
        private CLP clp;
        private byte HEC;


        //Constructor with contact type
        CAdministrationData(Contact contact)
        {
            this.contact = contact;
        }


        /***************
         * Setters
         **************/

        public void setContact(Contact contact)
        {
            this.contact = contact;
        }

        public void setGFC(int GFC)
        {
            this.GFC = GFC;
        }

        public void setVPI(int VPI)
        {
            if (contact.Equals(Contact.UNI))
            {
                if (VPI <= VPI_UNI_MAX)
                    this.VPI = VPI;
                else
                    throw new Exception();
            }
            else if (contact.Equals(Contact.NNI))
            {
                if (VPI <= VPI_NNI_MAX)
                    this.VPI = VPI;
                else
                    throw new Exception();
            }
        }

        public void setVCI(int VCI)
        {
            if (VCI <= VCI_MAX)
                this.VCI = VCI;
            else
                throw new Exception();
        }

        public void setPayloadType(PT payloadType)
        {
            this.payloadType = payloadType;
        }

        public void setCLP(CLP clp)
        {
            this.clp = clp;
        }

        public void setHEC()
        {
            //liczenie HEC z pola naglowka
        }

        /***************
         * Getters
        **************/

        public CLP getCLP()
        {
            return clp;
        }

        public Contact getContact()
        {
            return contact;
        }

        public int getHEC()
        {
            return HEC;
        }

        public int getVCI()
        {
            return VCI;
        }

        public int getVPI()
        {
            return VPI;
        }

        public PT getPayloadType()
        {
            return payloadType;
        }

        public int getGFC()
        {
            return GFC;
        }

    }
}

