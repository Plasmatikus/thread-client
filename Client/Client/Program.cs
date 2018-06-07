﻿using System;
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
        // Datentyp für Zyklenabfrage
        static void Main(string[] args)
        {
            Program Test = new Program();
            Test.Run();
        }
        void Run()
        {
            AbfrageGesamt();
            TestJohannes();
            TestDaniel();
            TestHergen();
        }
        void TestDaniel()
        {

            Daniel.AsynchronousClient.StartClient();
            
        }
        void TestHergen()
        {

            //Abfrage für maximale Anzahl paralleler Threads
            _sendThreadCount = AbfrageAnzahlThreads("Mengenangabe für Threads beim Übertragen der Daten (Default: 2)", 2);

            // Hergen hergen = new Hergen(_list, _daniel, maxParallelThreads)
        }
        //Testaufrufe von johannes
        void TestJohannes()
        {
            //Variablen für Ordner, etc
            string dirInput;

            //Abfrage für maximale Anzahl paralleler Threads
            _readThreadCount = AbfrageAnzahlThreads("Mengenangabe für Threads beim Auslesen der Dateien (Default: 3)", 3);
            
            //Abfrage des Verzeichnispfades
            Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben.");
            dirInput = Console.ReadLine();

            //Konvertieren des Inputs zu einem Verzeichnispfad
            var rootDir = new DirectoryInfo(dirInput);

            //Starten des Thread-Controllers
            Johannes johannes = new Johannes(rootDir, _readThreadCount);

            //Ausgabe der XML-Dateien ist in C:\XML\
            johannes.Controller();
            

            //Warten auf Nutzeingabe zum beenden.
            Console.WriteLine("Auslesen Fertig! - Warte auf Internet");
            Console.ReadLine();
            
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
            string dirInput;

            //Abfrage des Verzeichnispfades
            Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben.");
            dirInput = Console.ReadLine();

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
    }
}
