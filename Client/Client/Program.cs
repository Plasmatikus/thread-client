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

            Console.WriteLine("Bitte Pfad des zu durchsuchenden Verzeichnisses angeben.");

            string dirInput = Console.ReadLine();

            var rootDir = new DirectoryInfo(dirInput);

            var xmlDoc = new XDocument(Johannes.GetDirectoryXML(rootDir));
            
            Console.WriteLine(xmlDoc.ToString());

            Console.ReadLine();
        }
    }
}
