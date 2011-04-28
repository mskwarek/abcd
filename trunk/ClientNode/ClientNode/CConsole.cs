using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientNode
{ // klasa reprezentująca konsole 
    sealed class CConsole 
    {
        private String ConsoleInput;
        private String helloMessage = "****************************************************************************** \n*\n* Hello in ClientNode. To exit type 'q' \n*\n****************************************************************************** ";
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
            CPortManager cpm = CPortManager.Instance;
            Console.WriteLine(helloMessage);
            while (true)
            {
                
                Console.Write(commandPrompt + " ");
                ConsoleInput = Console.ReadLine();
                if (ConsoleInput.Equals("q")) {
                    Console.WriteLine(exitMessage);
                    Environment.Exit(0);
                }
                else if(ConsoleInput.StartsWith("start")) {
                    // wywołanie metod związanych z nadawaniem 
                    cpm.sendMsg("dupa");
                    cpm.showPorts();
                }
                else if(ConsoleInput.StartsWith("stop")) {

                    String[] command = ConsoleInput.Split(' ');
                    try
                    {
                        // wywołanie metod związanych z zaprzestaniem nadawnia
                        int arg = Convert.ToInt32(command[1]);
                        cpm.stopSending(arg);
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("argument nie jest liczbą");
                        Console.WriteLine(e.StackTrace);
                    }
                        
                    
                    
                    

                }
                else Console.WriteLine(ConsoleInput);
                
                //switch (ConsoleInput)
                //{
                //    case "q": 
                //        Console.WriteLine(exitMessage);
                //        Environment.Exit(0);
                //        break;
                //    case "start":
                //        Console.WriteLine("wywołanie start");
                //        // wywołanie metod związanych z nadawaniem 
                //        cpm.sendMsg("dupa");
                //        cpm.showPorts();
                //        continue;
                //    case "stop":
                //        Console.WriteLine("wywołanie stop");
                //        // wywoałenie metod związanych z wstrzymaniem nadawania
                //        continue;
                //    default:
                //        Console.WriteLine(ConsoleInput);
                //        break;
                //}
            }
        }

        

    }
}
