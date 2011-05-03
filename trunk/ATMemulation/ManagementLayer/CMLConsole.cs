﻿using System;
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
        {"?","Wyswietla listę poleceń dostępnych w konsoli"}
        
        };

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
            Console.WriteLine(helloMessage);
            CNetworkConfiguration cnc = new CNetworkConfiguration();
            cnc.readConfig();
            while (true)
            {

                Console.Write(commandPrompt + " ");
                ConsoleInput = Console.ReadLine();
                if (ConsoleInput.Equals("q"))
                {
                    Console.WriteLine(exitMessage);
                    //cpm.shutdownAllPorts();
                    Environment.Exit(1);
                }
                else if(ConsoleInput.Equals("?")) 
                {
                    // pokazuje listę dostępnych poleceń
                    Console.WriteLine("***\t Dostępne polecenia: \t***");
                    foreach (KeyValuePair<String, String> pair in cmdList)
                    {
                        
                        Console.WriteLine("{0} \t {1}",
                        pair.Key,
                        pair.Value);
                    }

                }
                else if(ConsoleInput.StartsWith("show")) 
                {
                    String[] alt = ConsoleInput.Split(new char[] {' '});
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
                            Console.WriteLine("Brak podanego argumentu...");
                            continue; 
                        }
                    }                  

                }
                else if (ConsoleInput.Equals("addconnection")) 
                {
                    String[] alt = ConsoleInput.Split(new char[] { ' ' });
                    List<int> args = new List<int>();
                    if (alt.Count() > 1)
                    {
                        if (alt.Count() != 5)
                        {
                            try
                            {
                                for (int i = 1; i < alt.Count(); i++)
                                {
                                    args.Add(Convert.ToInt16(alt[i]));
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Błędny argument");
                            }

                            // sprawdza poprawność argumentów
                            if(cnc.checkFormula(args))
                            {
                                // zestaw połączenie.
                            }

                        }
                        else { Console.WriteLine("Błęda liczba argumentów"); }
                    }

                }
                else Console.WriteLine(ConsoleInput);

            }
        }



    }
}