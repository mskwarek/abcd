using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{
    class Program
    {
        public static String consoleInput;

        static void Main(string[] args)
        {
            CPortManager cpm = new CPortManager();

            cpm.readConfig();
            cpm.showConfig();

            CConsole.Instance.consoleInit();


        }
    }
}
