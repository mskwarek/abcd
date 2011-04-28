using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    sealed class CConsole
    {
        private String ConsoleInput;
        private String helloMessage = "****************************************************************************** \n*\n* Hello in ClientNode. To exit type 'q' \n*\n****************************************************************************** ";
        private String exitMessage = "Exiting...";
        private String commandPrompt = "ClientNode$";
        private readonly static CConsole instance = new CConsole();

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
                if (ConsoleInput.Equals("q"))
                {
                    Console.WriteLine(exitMessage);
                    //cpm.shutdownAllPorts();
                    Environment.Exit(1);
                }
                else if (ConsoleInput.StartsWith("add")) //metoda dodajaca polaczenie w wybranym wezle, miedzy wybranymi portami
                {
                    
                }
                else if (ConsoleInput.StartsWith("remove"))  //metoda usuwajaca polaczenie
                {

                }
                else if (ConsoleInput.StartsWith("show"))  //metoda pokazujaca wszystkie polaczenia we wszystkich wezlach
                {

                }
                else Console.WriteLine(ConsoleInput);

            }
        }

    }
}
