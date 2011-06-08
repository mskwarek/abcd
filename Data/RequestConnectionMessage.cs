using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class RequestConnectionMessage
    {
        private int VCI;
        private int VPI;
        String message = "Request Connection";

        public RequestConnectionMessage(int VCI, int VPI)
        {
            this.VCI = VCI;
            this.VPI = VPI;
        }
    }
}
