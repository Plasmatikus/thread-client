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
            //Abfrage des Verzeichnispfades
            Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben.");
            string dirInput = Console.ReadLine();

            //Konvertieren des Inputs zu einem Verzeichnispfad
            var rootDir = new DirectoryInfo(dirInput);

            //Erstellen des XMLs mit eigener Methode
            var xmlDoc = new XDocument(Johannes.GetDirectoryXML(rootDir));

            //Speichern des XMLs, Pfad: thread-client\Client\Client\bin\Debug\
            //xmlDoc.Save("test.xml");

            //Anzeigen des XMLs
            Console.WriteLine(xmlDoc.ToString());

            //Warten auf Nutzeingabe zum beenden.
            Console.ReadLine();
        }
    }
}
