using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logger
{
    public sealed class CLogger
    {
        private static CLogger cLogger = new CLogger();

        public static CLogger Instance
        {
            get { return cLogger; }
        }

        private CLogger() { }


        public enum Modes
        { 
            normal = 1,
            error = 2,
            background =3,
            constructor = 4
        }

        //mode = 1-error , 2-normal, 3- background
        public void print(String methodName, String text, int modes)
        {
            if (modes == (int)Modes.normal) 
            {
                Console.ResetColor();
                if (methodName == null) { Console.WriteLine(text); } 
                else 
                Console.WriteLine("[" + methodName + "] " + text);
                
            }
            else if(modes == (int)Modes.background)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[" + methodName + "] " + text);
                
            }
            else if (modes == (int)Modes.error)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("ERROR : [" + methodName + "] " + text);
                
            }
            else if (modes == (int)Modes.constructor)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("INIT : [" + methodName + "]");
            }
        }

    }
}
