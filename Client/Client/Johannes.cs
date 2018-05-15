using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
/* Author: Johannes Schuhn
 * Matr.Nr.: 70445161
 * Fach: Threadprogrammierung(?)
 * Aufgabe:
 * 
 * Anmerkung(en):
 */
namespace Client
{
    class Johannes
    {
        public Johannes()
        {

        }
        public static void Worker()
        {

        }

        //Rekursiver Ordner/Dateileser nach XML
        public static XElement GetDirectoryXML(DirectoryInfo dir)
        {
            //Einfügen des aktuellen Verzeichnisses in die Struktur
            var DirXML = new XElement("dir", new XAttribute("name", dir.Name));
            
            //Schreiben der Dateien des Verzeichnisses ins XML
            foreach (var file in dir.GetFiles())
            {
                DirXML.Add(new XElement("file", new XAttribute("name", file.Name)));
            }

            //Rekursiver Aufruf zur Behandlung der Ordner
            foreach (var subDir in dir.GetDirectories())
            {
                DirXML.Add(GetDirectoryXML(subDir));
            }

            //Rückgabe des XML der Odernerstruktur als XElement
            return DirXML;
        } 


    }
}
