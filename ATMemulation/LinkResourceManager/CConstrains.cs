using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkResourceManager
{
    public static class CConstrains
    {
        public static string ipAddress = "127.0.0.1";


        public static int calculateSocketPort(int node, int port)
        {

            return 50000 + (node * 100) + port;
        }
    }
}
