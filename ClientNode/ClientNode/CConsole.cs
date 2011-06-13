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
        private Logger.CLogger logger = Logger.CLogger.Instance;
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
                //Console.WriteLine("Wszystkie porty wstały");
                CManagementAgent.Instance.sendHelloMsgToML(CConstrains.nodeNumber);
                //Console.WriteLine(helloMessage);
                while (true)
                {

                    logger.print(null,commandPrompt + " ",(int)Logger.CLogger.Modes.normal);
                    ConsoleInput = Console.ReadLine();
                    if (ConsoleInput.Equals("q"))
                    {
                        logger.print(null, exitMessage, (int)Logger.CLogger.Modes.normal);

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
                            logger.print(null, "Wstrzymywanie nadawania ", (int)Logger.CLogger.Modes.normal);
                        }
                        else
                        {
                            logger.print(null, "Węzeł obecnie nie nadaje", (int)Logger.CLogger.Modes.normal);
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
                            logger.print(null, "Błędna liczba argumentów.", (int)Logger.CLogger.Modes.error);
                        }
                        
                        try
                        {
                            // wywołanie metod związanych z zestawieniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);

                            CPCC.Instance.CallRequest(CConstrains.nodeNumber, arg1);
                        }
                        catch (Exception e)
                        {
                            logger.print(null, "Argument nie jest liczbą", (int)Logger.CLogger.Modes.error);
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    else if (ConsoleInput.StartsWith("ec"))
                    {
                        String[] command = ConsoleInput.Split(' ');
                        if (command.Count() != 2)
                        {
                            logger.print(null, "Błędna liczba argumentów.", (int)Logger.CLogger.Modes.error);
                        }

                        try
                        {
                            if (sender != null)
                            {
                                sender.Abort();
                                logger.print(null,"Wstrzymywanie nadawania ",(int)Logger.CLogger.Modes.normal);
                            }
                            else
                            {
                                logger.print(null, "Węzeł obecnie nie nadaje", (int)Logger.CLogger.Modes.normal);
                            }
                            // wywołanie metod związanych z rozłączaniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);

                            CPCC.Instance.CallTeardown(CConstrains.nodeNumber, arg1);

                        }
                        catch (Exception e)
                        {
                            logger.print(null, "Argument nie jest liczbą", (int)Logger.CLogger.Modes.error);
                            //Console.WriteLine(e.StackTrace);
                        }
                    }
                    
                    else
                        logger.print(null, ConsoleInput, (int)Logger.CLogger.Modes.normal);

                }
            
        }


    }
}
