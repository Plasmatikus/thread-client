using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data;

namespace Client
{
    class Program
    {
        string _serverAddress;
        int _serverPort;
        int _readThreadCount;
        int _sendThreadCount;
        DirectoryInfo _rootDirectory;
        DateTime _zykluszeit;
        List<byte[]> _list;
        static void Main(string[] args)
        {
            Program Test = new Program();
            Test.Run();
        }
        void Run()
        {
            AbfrageGesamt();

            //Erstellen des Auslesers
            Johannes johannes = new Johannes(_rootDirectory, _readThreadCount);

            
            //Starten des Auslesers
            //_list = johannes.Controller();

            //Statusausgabe
            Console.WriteLine("Auslesen des Dateisystem erfolgreich beendet!");

            //Erstellen des Senders
            Sender sender = new Sender(_list, _sendThreadCount, _ipaddress,_port);
            

        }
    

        private void AbfrageGesamt()
        {
            //Abfrage für maximale Anzahl paralleler Threads beim Auslesen
            _readThreadCount = AbfrageAnzahlThreads("Mengenangabe für Threads beim Auslesen der Dateien (Default: 3)", 3);

            //Abfrage für maximale Anzahl paralleler Threads beim Übertragen
            _sendThreadCount = AbfrageAnzahlThreads("Mengenangabe für Threads beim Übertragen der Daten (Default: 2)", 2);

            //Abfrage für Rootdirectory in dem Dateien ausgelesen werden
            _rootDirectory = AbfrageRootDirectory();

            //Abfrage IP-Adresse vom Server als Sendeziel der Daten
            _serverAddress = AbfrageServerAdresse();

            //Abfrage Portnummer vom Server
            _serverPort = AbfrageServerPort();

            //Abfrage Zykluszeit
            _zykluszeit = AbfrageZykluszeit();
        }

        private int AbfrageAnzahlThreads(string message, int maxParallelThreads)
        {
            //Abfrage für maximale Anzahl paralleler Threads
            bool inputErr = true;
            bool inputTest = false;
            while (inputTest == false)
            {
                Console.WriteLine(message);
                Console.WriteLine("Wollen Sie die  Anzahl der parallel benutzten Threads angeben? (Y/n)");
                string inputYesNo;
                inputYesNo = Console.ReadLine();
                if (inputYesNo == "Y" || inputYesNo == "y" || inputYesNo == "Yes" || inputYesNo == "YES" || inputYesNo == "yes" ||
                    inputYesNo == "N" || inputYesNo == "n" || inputYesNo == "No" || inputYesNo == "NO" || inputYesNo == "no" ||
                    inputYesNo == "")
                {
                    inputTest = true;
                    if (inputYesNo == "Y" || inputYesNo == "y" || inputYesNo == "Yes" || inputYesNo == "YES" || inputYesNo == "yes" || inputYesNo == "")
                    {
                        while (inputErr)
                        {
                            Console.WriteLine("Bitte die Anzahl paralleler Threads angeben(Ganzzahl):");
                            string inputMaxThreads;
                            inputMaxThreads = Console.ReadLine();
                            bool result;
                            result = int.TryParse(inputMaxThreads, out maxParallelThreads);
                            if (result && (maxParallelThreads > 0))
                            {
                                inputErr = false;
                            }
                            else
                            {
                                Console.WriteLine("Fehlerhafte Eingabe! Bitte antworten Sie nochmal.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Fehlerhafte Eingabe! Bitte antworten Sie nochmal.");
                }
            }
            return maxParallelThreads;
        }

        private DirectoryInfo AbfrageRootDirectory()
        {
            //Variablen für Ordner
            string dirInput = "C:\\";
            bool error = true;
            while(error)
            {
                try
                {
                    //Abfrage des Verzeichnispfades
                    Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben.");
                    dirInput = Console.ReadLine();
                    error = false;
                }
                catch
                {
                    Console.WriteLine("Fehlerhafte Eingabe! Bitte antworten Sie nochmal.");
                }
            }

            //Konvertieren des Inputs zu einem Verzeichnispfad
            var rootDir = new DirectoryInfo(dirInput);
            return rootDir;
        }

        private string AbfrageServerAdresse()
        {
            string ip;
            Console.WriteLine("Bitte geben Sie die IP-Adresse oder den Hostnamen des Zielservers an.");
            ip = Console.ReadLine();
            return ip;
        }

        private int AbfrageServerPort()
        {
            int port = 11001;
            bool tryout = false;
            while (tryout == false)
            {
                Console.WriteLine("Bitte geben Sie die Portnummer des Zielservers an.");
                if (int.TryParse(Console.ReadLine(), out port))
                {
                    tryout = true;
                }
                else
                {
                    Console.WriteLine("Fehler aufgetreten: Nochmal versuchen");
                }
            }
            return port;
        }

        private DateTime AbfrageZykluszeit()
        {
            DateTime zyklus;
            int min = 5;
            bool tryout = false;
            bool inputTest = false;
            while (inputTest == false)
            {
                Console.WriteLine("Wollen Sie die  Anzahl der parallel benutzten Threads angeben? (Y/n)");
                string inputYesNo;
                inputYesNo = Console.ReadLine();
                if (inputYesNo == "Y" || inputYesNo == "y" || inputYesNo == "Yes" || inputYesNo == "YES" || inputYesNo == "yes" ||
                    inputYesNo == "N" || inputYesNo == "n" || inputYesNo == "No" || inputYesNo == "NO" || inputYesNo == "no" ||
                    inputYesNo == "")
                {
                    inputTest = true;
                    if (inputYesNo == "Y" || inputYesNo == "y" || inputYesNo == "Yes" || inputYesNo == "YES" || inputYesNo == "yes" || inputYesNo == "")
                    {
                        while (tryout == false)
                        {
                            Console.WriteLine("Bitte die Zeit für den Sendeintervall angeben. (In Minuten)");
                            if (int.TryParse(Console.ReadLine(), out min))
                            {
                                tryout = true;
                            }
                            else
                            {
                                Console.WriteLine("Fehlerhafte Eingabe! Bitte antworten Sie nochmal.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Fehlerhafte Eingabe! Bitte antworten Sie nochmal.");
                }
            }
            
            zyklus = new DateTime(0, 0, 0, 0, min, 0);
            return zyklus;
        }
    }
}
