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
            Logger.CLogger logger = Logger.CLogger.Instance;
            foreach (Thread t in CConstrains.threadList)
            {
                t.Start(); 
            }

                Console.Title = "ClientNode ID = " + CConstrains.nodeNumber;
                CManagementAgent.Instance.sendHelloMsgToML(CConstrains.nodeNumber);
                logger.print(null,helloMessage,(int)Logger.CLogger.Modes.normal);
                while (true)
                {

                    logger.print(null, commandPrompt + " ", (int)Logger.CLogger.Modes.normal) ;
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
                            logger.print(null,"Wstrzymanie nadawania", (int)Logger.CLogger.Modes.normal);
                
                        }
                        else
                        {
                            logger.print(null,"Aktualnie nie nadajesz", (int)Logger.CLogger.Modes.normal);
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
                            logger.print(null,"Błędna liczba argumentów", (int)Logger.CLogger.Modes.error);
                
                            
                        }
                        
                        try
                        {
                            // wywołanie metod związanych z zestawieniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);

                            CPCC.Instance.CallRequest(CConstrains.nodeNumber, arg1);
                        }
                        catch (Exception e)
                        {
                            logger.print(null,"Agrument nie jest liczbą", (int)Logger.CLogger.Modes.error);
                        }
                    }
                    else if (ConsoleInput.StartsWith("ec"))
                    {
                        String[] command = ConsoleInput.Split(' ');
                        if (command.Count() != 2)
                        {
                            logger.print(null, "Błędna liczba argumentów", (int)Logger.CLogger.Modes.error);
                        }

                        try
                        {
                            if (sender != null)
                            {
                                sender.Abort();
                                logger.print(null, "Wstrzymanie nadawania", (int)Logger.CLogger.Modes.normal);
                            }
                            else
                            {
                                logger.print(null, "Aktualnie nie nadajesz", (int)Logger.CLogger.Modes.normal);
                            }
                            // wywołanie metod związanych z rozłączaniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);

                            CPCC.Instance.CallTeardown(CConstrains.nodeNumber, arg1);

                        }
                        catch (Exception e)
                        {
                            logger.print(null, "Agrument nie jest liczbą", (int)Logger.CLogger.Modes.error);
                        }
                    }
                    
                    else 
                        Console.WriteLine(ConsoleInput);

                }
            
        }


    }
}
