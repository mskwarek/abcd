﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{

    public enum Contact { UNI, NNI };
    public enum PT { _000_=0, _001_=1, _010_=2, _011_=3, _100=4, _101_=5, _110_=6, _111_=7 };
    public enum CLP { _0_=0, _1_=1 };

    [Serializable]
    public class CAdministrationData
    {

        public const int VPI_UNI_MAX = 255;
        public const int VPI_NNI_MAX = 4095;
        public const int VCI_MAX = 65535;

        private Contact contact;
        private int GFC = 0;
        private int VPI;
        private int VCI;
        private PT payloadType;
        private CLP clp;
        private byte HEC = new Byte();


        //Constructor with contact type
        public CAdministrationData(Contact contact, PT payloadType, CLP clp )
        {
            this.contact = contact;
            this.payloadType = payloadType;
            this.clp = clp;
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
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            String GFCbinary = "";
            String VPIbinary = "";
            if (contact == Contact.UNI)
            {
                GFCbinary = Convert.ToString(GFC, 2).PadLeft(4, '0');
                VPIbinary = Convert.ToString(VPI, 2).PadLeft(8, '0');
            }
            else if(contact== Contact.NNI)
                VPIbinary = Convert.ToString(VPI, 2).PadLeft(12, '0');
           
            String VCIbinary = Convert.ToString(VCI, 2).PadLeft(16, '0');
            String payloadbinary = Convert.ToString((int)payloadType, 2).PadLeft(3,'0');
            String CLPbinary = Convert.ToString((int)clp, 2);
            
            String sHeader = GFCbinary + VPIbinary + VCIbinary+payloadbinary+CLPbinary;
            
            byte[] header=new byte[4];
            header = encoding.GetBytes(sHeader);

            CRC8 crc = new CRC8(CRC8_POLY.CRC8_CCITT);
            byte checksum = crc.Checksum(header);

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
    
    public enum CRC8_POLY
    {
        CRC8 = 0xd5,
        CRC8_CCITT = 0x07,
        CRC8_DALLAS_MAXIM = 0x31,
        CRC8_SAE_J1850 = 0x1D,
        CRC_8_WCDMA = 0x9b,
    };

    class CRC8
    {
        private byte[] table = new byte[256];
        public byte Checksum(params byte[] val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            byte c = 0;
            foreach (byte b in val)
            {
                c = table[c ^ b];
            }
            return c;
        }
        public byte[] Table
        {
            get
            {
                return this.table;
            }
            set
            {
                this.table = value;
            }
        }

        public byte[] GenerateTable(CRC8_POLY polynomial)
        {
            byte[] csTable = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                int curr = i;
                for (int j = 0; j < 8; ++j)
                {
                    if ((curr & 0x80) != 0)
                    {
                        curr = (curr << 1) ^ (int)polynomial;
                    }
                    else
                    {
                        curr <<= 1;
                    }
                }
                csTable[i] = (byte)curr;
            }
            return csTable;
        }
        public CRC8(CRC8_POLY polynomial)
        {
            this.table = this.GenerateTable(polynomial);
        }

    }
}

