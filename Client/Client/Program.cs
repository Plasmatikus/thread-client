using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDaniel();
            //TestHergen();
            TestJohannes();

        }
        static void TestDaniel()
        {

        }
        static void TestHergen()
        {

        }
        //Testaufrufe von johannes
        static void TestJohannes()
        {
            //Variablen für Ordner, etc
            //Notiz: Ordnerpfad lautet C:\Users\[Nutzername]\AppData\Roaming\threadprogrammierung
            string saveFolder = "threadprogrammierung";
            string saveFilename = "Dateisystem.xml";
            int maxParallelThreads = 3;

            //Ermitteln des Appdata-Pfades um darin später den Ordner anzulegen.
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            saveFolder = Path.Combine(appDataFolder, saveFolder);

            //Abfrage für maximale Anzahl paralleler Threads
            bool inputErr = true;
            while (inputErr)
            {
                Console.WriteLine("Bitte die Anzahl paralleler Threads angeben(Ganzzahl):");
                string inputMaxThreads;
                inputMaxThreads = Console.ReadLine();
                bool result;
                result = int.TryParse(inputMaxThreads, out maxParallelThreads);
                if (result)
                {
                    inputErr = false;
                }
                else
                {
                    Console.WriteLine("Fehlerhafte Eingabe!");
                }
            }

            //Abfrage des Verzeichnispfades
            Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben.");
            string dirInput = Console.ReadLine();

            //Konvertieren des Inputs zu einem Verzeichnispfad
            var rootDir = new DirectoryInfo(dirInput);

            //Starten des Thread-Controllers
            Johannes johannes = new Johannes(rootDir, maxParallelThreads);
            johannes.Controller();


            //var xmlDoc = new XDocument(Johannes.GetDirectoryXML(rootDir));

            //Speichern des XMLs, Pfad: thread-client\Client\Client\bin\Debug\
            //xmlDoc.Save("test.xml");

            //Anzeigen des XMLs
            //Console.WriteLine(xmlDoc.ToString());

            //Warten auf Nutzeingabe zum beenden.
            Console.WriteLine("Fertig!");
            Console.ReadLine();
        }
    }
}
