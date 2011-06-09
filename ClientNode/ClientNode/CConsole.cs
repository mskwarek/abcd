using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace ClientNode
{ // klasa reprezentująca konsole 
    sealed class CConsole 
    {
        private String ConsoleInput;
        Thread sender;
        private String helloMessage = "****************************************************************************** \n*\n*" 
                                       + " Hello in ClientNode. \n* ID = "+CConstrains.nodeNumber+ " To exit type 'q' " 
                                       +"\n*\n****************************************************************************** ";
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
            CManagementAgent cma = CManagementAgent.Instance;

            foreach (Thread t in CConstrains.threadList)
            {
                t.Start(); 
            }

                Console.Title = "ClientNode ID = " + CConstrains.nodeNumber;
                Console.WriteLine("Wszystkie porty wstały");
                CManagementAgent.Instance.sendHelloMsgToML(CConstrains.nodeNumber);
                Console.WriteLine(helloMessage);
                while (true)
                {

                    Console.Write(commandPrompt + " ");
                    ConsoleInput = Console.ReadLine();
                    if (ConsoleInput.Equals("q"))
                    {
                        Console.WriteLine(exitMessage);

                        Environment.Exit(1);
                    }
                    else if (ConsoleInput.StartsWith("start"))
                    {
                        // wywołanie metod związanych z nadawaniem 
                        sender = new Thread(CSender.Instance.sendData);
                        sender.Name = " CSender";
                        sender.Start();
                        
                    }
                    else if (ConsoleInput.StartsWith("stop"))
                    {

                        String[] command = ConsoleInput.Split(' ');

                        if (sender != null)
                        {
                            sender.Abort();
                            Console.WriteLine("Wstrzymywanie nadawania ");
                        }
                        else
                        {
                            Console.WriteLine("Węzeł obecnie nie nadaje");
                        }
                            
                    }
                    else if (ConsoleInput.StartsWith("show"))
                    {

                        cpm.getNodePortConfiguration();
                    }
                    else if (ConsoleInput.StartsWith("rc"))
                    {
                        String[] command = ConsoleInput.Split(' ');
                        if (command.Count() != 2)
                        {
                            Console.WriteLine(" ERROR : Błędna liczba argumentów.");
                        }
                        
                        try
                        {
                            // wywołanie metod związanych z zestawieniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);

                            CPCC.Instance.CallRequest(CConstrains.nodeNumber, arg1);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" ERROR : argument nie jest liczbą");
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    else if (ConsoleInput.StartsWith("ec"))
                    {
                        String[] command = ConsoleInput.Split(' ');
                        if (command.Count() != 2)
                        {
                            Console.WriteLine(" ERROR : Błędna liczba argumentów.");
                        }

                        try
                        {
                            if (sender != null)
                            {
                                sender.Abort();
                                Console.WriteLine("Wstrzymywanie nadawania ");
                            }
                            else
                            {
                                Console.WriteLine("Węzeł obecnie nie nadaje");
                            }
                            // wywołanie metod związanych z rozłączaniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);

                            CPCC.Instance.CallTeardown(CConstrains.nodeNumber, arg1);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" ERROR : argument nie jest liczbą");
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    
                    else 
                        Console.WriteLine(ConsoleInput);

                }
            
        }


    }
}
