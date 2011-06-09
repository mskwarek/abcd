using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{   [Serializable]
    public class CUserData
    {
        private static int MAX_CAPACITY = 48;
        private List<byte> data;

        public CUserData()
        {
            data = new List<byte>();
        }

        public void setInformation(List<byte> data)
        {
            if (data.Count <= MAX_CAPACITY)
                this.data = data;
        }
        public List<byte> getInformation()
        {
            return data;
        }
    }
}
