using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace ClientNode
{
    public static class CConstrains
    {
        public static int inputPortNumber;
        public static int outputPortNumber;
        public static int nodeNumber;
        public  const String nodeType = "ClientNode";
        public static List<Thread> threadList = new List<Thread>();
    }
}
