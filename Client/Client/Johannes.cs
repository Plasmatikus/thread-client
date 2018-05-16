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
            var DirXML = new XElement("verzeichnis", new XAttribute("name", dir.Name));
            
            //Schreiben der Dateien des Verzeichnisses ins XML
            foreach (var file in dir.GetFiles())
            {
                //Konvertieren der Dateigröße von Byte in passende Einheit
                string filesize = "";
                long filelenght = file.Length;
                //kleiner Hack um Datentypbeschränkung in If-Bedingungen zu umgehen
                //Nachträglich konsequenterweise für alle Größen nachgetragen
                int B = 1024;
                int kB = 1024 * 1024;
                int MB = 1024 * 1024 * 1024;
                long GB = MB * 1024;
                //Entscheidungsbaum uns Stringformatierung
                if (filelenght / B == 0)
                {
                    filesize = filelenght + "B";
                }
                else if(filelenght / kB == 0)
                {
                    filelenght = filelenght / B;
                    filesize = filelenght + "kB";
                }
                else if (filelenght / MB == 0)
                {
                    filelenght = filelenght / kB;
                    filesize = filelenght + "MB";
                }
                else if (filelenght / GB == 0)
                {
                    filelenght = filelenght / MB;
                    filesize = filelenght + "GB";
                }
                else
                {
                    filelenght = filelenght / GB;
                    filesize = filelenght + "TB";
                }

                DirXML.Add(new XElement("datei", new XAttribute("name", file.Name), new XAttribute("groesse", filesize)));
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
