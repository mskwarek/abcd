using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{ // klasa reprezentująca konsole 
    sealed class CMLConsole
    {
        private String ConsoleInput;
        private String helloMessage = "****************************************************************************** \n*\n* Hello in Network Manager. To exit type 'q' \n*\n*********************************************************************** ";
        private String exitMessage = "Exiting...";
        private String commandPrompt = "Network Manager$";
        private readonly static CMLConsole instance = new CMLConsole();
        private readonly Dictionary<string, string> cmdList = new Dictionary<string, string>() {
        
        {"show netCfg", "Wyświetla konfigurację połaczeń w sieci"},
        {"show nodes","Wyświetla węzły wraz z ich rolą w sieci"},
        {"getConnections [args]","Pobiera tablicę komutacji z węzła i ja wyświetla"},
        {"addConnections [args]","Dodaje połączenie w tablicy komutacji"},
        {"?","Wyswietla listę poleceń dostępnych w konsoli"}
        
        };

        private Logger.CLogger logger = Logger.CLogger.Instance;

        private CMLConsole()
        {
        }

        public static CMLConsole Instance
        {
            get
            {
                return instance;
            }
        }

        public void consoleInit()
        {
            //CPortManager cpm = CPortManager.Instance;
            logger.print(null,helloMessage,(int)Logger.CLogger.Modes.normal);
            CNetworkConfiguration cnc = CNetworkConfiguration.Instance;
            ConnectionsManager cm = ConnectionsManager.Instance;



            if (cnc.readConfig())
            {
                while (true)
                {

                    logger.print(null, commandPrompt + " ", (int)Logger.CLogger.Modes.normal);
                    ConsoleInput = Console.ReadLine();
                    if (ConsoleInput.Equals("q"))
                    {
                        logger.print(null, exitMessage, (int)Logger.CLogger.Modes.normal);
                        //cpm.shutdownAllPorts();
                        Environment.Exit(1);
                    }
                    else if (ConsoleInput.Equals("?"))
                    {
                        // pokazuje listę dostępnych poleceń
                        logger.print(null, "***\t Dostępne polecenia: \t***", (int)Logger.CLogger.Modes.normal);
                        foreach (KeyValuePair<String, String> pair in cmdList)
                        {

                            Console.WriteLine("{0} \t {1}",
                            pair.Key,
                            pair.Value );
                        }

                    }
                    else if (ConsoleInput.StartsWith("show"))
                    {
                        String[] alt = ConsoleInput.Split(new char[] { ' ' });
                        if (alt.Count() > 1)
                        {
                            if (alt[1].Equals("netCfg"))
                            {
                                // pokazuje konfigurację połączeń w sieci
                                cnc.showNetworkConfiguration();
                            }
                            else if (alt[1].Equals("nodes"))
                            {
                                //pokazuje listę nodów które zgłosiły sie do ML
                                cnc.showNodes();
                            }
                            else
                            {
                                logger.print(null, "Błędny argument",(int)Logger.CLogger.Modes.error);
                                continue;
                            }
                        }
                        else
                        {
                            logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                            continue;
                        }

                    }
                    else if (ConsoleInput.StartsWith("addconnection"))
                    {
                        String[] alt = ConsoleInput.Split(new char[] { ' ' });
                        List<int> args = new List<int>();
                        if (alt.Count() > 1)
                        {
                            if (alt.Count() == 8)
                            {
                                try
                                {
                                    for (int i = 1; i < alt.Count(); i++)
                                    {
                                        args.Add(Convert.ToInt32(alt[i]));
                                    }
                                    // node number, port in port out
                                    cm.addConnection(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                                    //CShortestPathCalculatorWrapper.Instance.getShortestPath(args[0], args[1]);

                                }
                                catch (Exception)
                                {
                                    logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                                }
                            }
                            else { logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error); }
                        }
                        else { logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error); }

                    }

                    else if (ConsoleInput.StartsWith("removeconnection"))
                    {
                        String[] alt = ConsoleInput.Split(new char[] { ' ' });
                        List<int> args = new List<int>();
                        if (alt.Count() > 1)
                        {
                            if (alt.Count() == 8)
                            {
                                try
                                {
                                    for (int i = 1; i < alt.Count(); i++)
                                    {
                                        args.Add(Convert.ToInt32(alt[i]));
                                    }
                                    // node number, port in port out
                                    cm.removeConnection(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                                }
                                catch (Exception)
                                {
                                    logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                                }
                            }
                            else { logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error); }
                        }
                        else { logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error); }

                    }
                    else if (ConsoleInput.StartsWith("getConnections"))
                    {
                        String[] alt = ConsoleInput.Split(new char[] { ' ' });
                        List<int> args = new List<int>();
                        if (alt.Count() > 1)
                        {
                            try
                            {
                                for (int i = 1; i < alt.Count(); i++)
                                {
                                    args.Add(Convert.ToInt16(alt[i]));
                                }
                            }
                            catch (Exception)
                            {
                                logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                            }
                            // usuwam powtarzające sie argumenty
                            int[] var = args.Distinct().ToArray();

                            // dla każdego argumentu sprawdza warunek i wysyła request do noda
                            foreach (int a in var)
                            {
                                if (cnc.checkFormula(a))
                                {
                                    logger.print(null,"<-- Pobierasz dane z noda: " + args[0],(int)Logger.CLogger.Modes.background);
                                    cm.getNodeCommutationTable(args[0]);
                                }
                            }

                        }
                        else {logger.print(null,"Nie podałeś numeru węzła",(int)Logger.CLogger.Modes.error); continue; }
                    }
                    else if (ConsoleInput.StartsWith("rc"))
                    {
                        String[] command = ConsoleInput.Split(' ');
                        if (command.Count() != 3)
                        {
                            logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                        }

                        try
                        {
                            // wywołanie metod związanych z zestawieniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);
                            int arg2 = Convert.ToInt32(command[2]);

                            ConnectionsManager.Instance.CallRequest(arg1, arg2);
                        }
                        catch (Exception)
                        {
                            logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                            //Console.WriteLine(e.StackTrace);
                        }
                    }
                    else if (ConsoleInput.StartsWith("ec"))
                    {
                        String[] command = ConsoleInput.Split(' ');
                        if (command.Count() != 3)
                        {
                            logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                        }

                        try
                        {
                            // wywołanie metod związanych z rozłączaniem połączenia 
                            int arg1 = Convert.ToInt32(command[1]);
                            int arg2 = Convert.ToInt32(command[2]);

                            ConnectionsManager.Instance.CallTeardown(arg1, arg2);

                        }
                        catch (Exception e)
                        {
                            logger.print(null, "Błędny argument", (int)Logger.CLogger.Modes.error);
                            //Console.WriteLine(e.StackTrace);
                        }
                    }
                    else Console.WriteLine();

                }
            }
            else
            {
                logger.print(null, "BRAK KONFIGURACJI", (int)Logger.CLogger.Modes.error);
            }
        }
  

        

    }
}
