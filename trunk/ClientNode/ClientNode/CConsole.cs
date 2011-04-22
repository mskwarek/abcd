using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{ // klasa reprezentująca konsole 
    sealed class CConsole 
    {
        private String ConsoleInput;
        private String helloMessage = "Hello in ClientNode. To exit type 'q'";
        private String exitMessage = "Exiting...";
        private String commandPrompt = "ClientNode$";
        private readonly static  CConsole instance = new CConsole();

        private CConsole()
        {
        }

        public static CConsole Instance
        {
            get
            {
                return instance;
            }
        }

        public void consoleInit()
        {
            Console.WriteLine(helloMessage);
            while (true)
            {
                Console.Write(commandPrompt + " ");
                ConsoleInput = Console.ReadLine();
                switch (ConsoleInput)
                {
                    case "q": 
                        Console.WriteLine(exitMessage);
                        Environment.Exit(0);
                        break;
                    case "start":
                        Console.WriteLine("wywołanie start");
                        // wywołanie metod związanych z nadawaniem 

                        continue;
                    case "stop":
                        Console.WriteLine("wywołanie stop");
                        // wywoałenie metod związanych z wstrzymaniem nadawania
                        continue;
                    default:
                        Console.WriteLine(ConsoleInput);
                        break;
                }
            }
        }

        

    }
}
