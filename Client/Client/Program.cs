using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data;
using System.Net;
using System.Threading;

namespace Client
{
    class Program
    {
        #region fields
        IPAddress _ipaddress;
        int _port;
        int _readThreadCount;
        int _sendThreadCount;
        DirectoryInfo _rootDirectory;
        int _zykluszeit;
        List<byte[]> _list;
        DateTime _dateTimeTest = new DateTime();
        DateTime _dateTimeStart = new DateTime();
        int _timeToWait = 6000;
        #endregion

        #region main
        static void Main(string[] args)
        {
            Program Program = new Program();
            Program.Run();
        }
        #endregion

        #region methods
        void Run()
        {
            //Methode zum Abfragen aller benötigten Daten
            AbfrageGesamt();

            //Starten der Programmwiederholung
            Console.WriteLine("Warten auf ersten Timeslot...");
            _dateTimeTest = DateTime.Now;
            while (true)
            {
                //Warten bis gestartet werden soll
                while((_dateTimeTest.Minute % _zykluszeit) != 0)
                {
                    Thread.Sleep(_timeToWait);
                    _dateTimeTest = DateTime.Now;
                }
                _dateTimeStart = _dateTimeTest;


                //Erstellen des Auslesers
                Johannes johannes = new Johannes(_rootDirectory, _readThreadCount);


                //Starten des Auslesers
                Console.WriteLine("Auslesen des Dateisystems gestartet!");
                _list = johannes.Controller();
                Console.WriteLine("Auslesen des Dateisystems erfolgreich beendet!");


                //Erstellen des Senders
                Sender sender = new Sender(_list, _sendThreadCount, _ipaddress, _port);


                //Starten des Senders
                Console.WriteLine("Übertragen der Daten gestartet!");
                sender.Run();
                Console.WriteLine("Übertragen der Daten erfolgreich beendet!");

                //Verhindern des mehrfachen Startens in einer Minute
                while (_dateTimeStart.Minute == DateTime.Now.Minute)
                {
                    Thread.Sleep(_timeToWait);
                }
                _dateTimeTest = DateTime.Now;
            }
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
            _ipaddress = AbfrageServerAdresse();

            //Abfrage Portnummer vom Server
            _port = AbfrageServerPort();

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
            while (error)
            {
                try
                {
                    //Abfrage des Verzeichnispfades
                    Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben. (Nicht das Backslash am Ende vergessen!)");
                    dirInput = Console.ReadLine();
                    char c = dirInput[dirInput.Length - 1];
                    if(System.IO.Directory.Exists(dirInput) && c == '\\')
                    {
                        error = false;
                    }
                    else
                    {
                        Console.WriteLine("Fehlerhafte Eingabe! Bitte antworten Sie nochmal.");
                    }
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

        private IPAddress AbfrageServerAdresse()
        {

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            bool tryout = false;

            while (tryout == false)
            {
                Console.WriteLine("Bitte geben Sie die IP-Adresse des Zielservers an. (Format: 0.0.0.0)");
                if (IPAddress.TryParse(Console.ReadLine(), out ip))
                {
                    tryout = true;
                }
                else
                {
                    Console.WriteLine("Fehler aufgetreten: Nochmal versuchen");
                }
            }

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

        private int AbfrageZykluszeit()
        {

            int zyklus = 0;
            bool tryout = false;

            while (tryout == false)
            {
                Console.WriteLine("Bitte die Zeit für das Sendeintervall angeben. (In Minuten, zwischen 1 und 59)");
                if ((int.TryParse(Console.ReadLine(), out zyklus)) && zyklus > 0 && zyklus < 60)
                {
                    tryout = true;
                }
                else
                {
                    Console.WriteLine("Fehlerhafte Eingabe! Bitte antworten Sie nochmal.");
                }
            }

            return zyklus;
        }
        #endregion
    }
}
