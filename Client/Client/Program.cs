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
        static void Main(string[] args)
        {
            //Variablen
            DataTable _datatable;
            //Aufrufe
            _datatable = TestJohannes();
            //TestDaniel();
            //TestHergen();
        }
        static void TestDaniel()
        {

        }
        static void TestHergen()
        {
            int maxParallelThreads = 2;

            //Abfrage für maximale Anzahl paralleler Threads
            maxParallelThreads = AbfrageAnzahlThreads("Mengenangabe für Threads beim Übertragen der Daten (Default: 2)", 2);

            // Hergen hergen = new Hergen(_table, _daniel, maxParallelThreads)
        }
        //Testaufrufe von johannes
        static DataTable TestJohannes()
        {
            //Variablen für Ordner, etc
            DataTable table;
            int maxParallelThreads = 3;

            //Abfrage für maximale Anzahl paralleler Threads
            maxParallelThreads = AbfrageAnzahlThreads("Mengenangabe für Threads beim Auslesen der Dateien (Default: 3)", 3);
            
            //Abfrage des Verzeichnispfades
            Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben.");
            string dirInput = Console.ReadLine();

            //Konvertieren des Inputs zu einem Verzeichnispfad
            var rootDir = new DirectoryInfo(dirInput);

            //Starten des Thread-Controllers
            Johannes johannes = new Johannes(rootDir, maxParallelThreads);
            table = johannes.Controller();


            //var xmlDoc = new XDocument(Johannes.GetDirectoryXML(rootDir));

            //Speichern des XMLs, Pfad: thread-client\Client\Client\bin\Debug\
            //xmlDoc.Save("test.xml");

            //Anzeigen des XMLs
            //Console.WriteLine(xmlDoc.ToString());

            //Warten auf Nutzeingabe zum beenden.
            Console.WriteLine("Auslesen Fertig! - Warte auf Internet");
            Console.ReadLine();

            return table;
        }

        private static int AbfrageAnzahlThreads(string message, int maxParallelThreads)
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
    }
}
