using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        #region fields
        //Semaphor zur steuerung der Anzahl paralleler Threads
        public System.Threading.Semaphore S;
        private DirectoryInfo _rootDir;
        private int _parallelThreads;
        private string _saveDir;
        private string _saveFile;
        #endregion

        #region ctor
        public Johannes(DirectoryInfo rootDir, int parallelThreads, string saveDir, string saveFile)
        {
            _rootDir = rootDir;
            _saveDir = saveDir;
            _saveFile = saveFile;
            S = new System.Threading.Semaphore(parallelThreads, parallelThreads);
        }
        #endregion
        //Controller für das Multithreading
        public void Controller()
        {

            #region Vorbereiten des Table   
            //Erstellen einer Datatable um darin für jeden Ordner die Verzeichnisstruktur abzulegen
            DataTable table = new DataTable();

            // Deklarieren der Zeilen und Spalten Variablen
            DataColumn column;

            // Erstellen der Spalte für den Ordnernamen    
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Folder";
            table.Columns.Add(column);

            // Erstellen der Spalte für den zum String konvertierten XML-Baum. Idee: String durch XElement ersetzen
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Tree";
            table.Columns.Add(column);
            #endregion

            //Auslesen der Root-Verzeichnisses und ausführen der entsprechenden Worker
            foreach (var subDir in _rootDir.GetDirectories())
            {
                //ParameterizedThreadStart pts = new ParameterizedThreadStart(Worker);
                //Thread thread = new Thread(pts);
                //thread.Start(subDir, ref table);
            }

            //Temporär! Schreiben des Ergebnisses in eine Datei und Ende.
        }
        //Worker Thread
        public void Worker(DirectoryInfo workDir, ref DataTable resultTable)
        {
            try
            {
                //Warten bis ein Workslot freigegeben wird
                S.WaitOne();
                // Hier kommt der Payload hin

                //Variablen
                string xmlTreeString = "";
                DataRow row;
                XElement tree;
                //Ausführen der XML-GeneratorMethode
                tree = GetDirectoryXML(workDir);
                //Umwandeln des Ergebnisses zu einem String
                xmlTreeString = tree.ToString();
                //Speichern des Ergebnisses im referenzierten DataTable
                row = resultTable.NewRow();
                row["Folder"] = workDir;
                row["Tree"] = xmlTreeString;
                resultTable.Rows.Add(row);
            }

            finally
            {
                // Freigeben des Workslots
                S.Release();
            }
        }

        //Rekursiver Ordner/Dateileser nach XML
        public XElement GetDirectoryXML(DirectoryInfo dir)
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
