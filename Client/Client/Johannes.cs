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
        //private int _parallelThreads;
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

        #region Methoden
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
            column.DataType = System.Type.GetType("System.String");
            //column.DataType = Type.GetType("System.Data.DataTable");
            column.ColumnName = "Tree";
            table.Columns.Add(column);
            #endregion

            #region Threadhandling
            //Ermitteln der Anzahl an Ordnern im Root Verzeichnis (Aus management Gründen)
            int folderCount = 0;
            foreach (var subDir in _rootDir.GetDirectories())
            {
                folderCount++;
            }
            //
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            int toProcess = folderCount;
            //Auslesen der Root-Verzeichnisses und ausführen der entsprechenden Worker
            foreach (var subDir in _rootDir.GetDirectories())
            {
                new Thread(delegate ()
                {
                    //Spawnen des Workers
                    Worker(subDir, ref table);
                    // Wenn dies der letzte Thread ist, signal absetzen
                    if (Interlocked.Decrement(ref toProcess) == 0)
                    {
                        resetEvent.Set();
                    }
                }).Start();
                //Thread thread = new Thread(() => Worker(subDir, ref table));
                //thread.Start();
            }
            //Warten bis alle Threads fertig sind
            resetEvent.WaitOne();
            Console.WriteLine("Alle Threads fertig.");
            #endregion

            #region XML Zusammenbau

            //Anlegen von weitern nötigen XElements (Client-Name etc)
            string clientName = System.Environment.MachineName;
            //Erzeugen des "Root" XML-Elements
            var rootXML = new XElement("client", new XAttribute("name", clientName));
            //Zusammenfügen der XML-Teilbäume aus dem DataTable
            for(int i=0; i< table.Rows.Count; i++)
            {
                string temp = table.Rows[i][1].ToString();
                rootXML.Add(XElement.Parse(temp));
            }
            //Auslesen der Dateien des RootDir und einfügen ins XML
            #region Dateibehandlung
            //Schreiben der Dateien des Verzeichnisses ins XML
            foreach (var file in _rootDir.GetFiles())
            {
                //Konvertieren der Dateigröße von Byte in passende Einheit
                string filesize = "";
                long filelenght = file.Length;
                //kleiner Hack um Datentypbeschränkung in If-Bedingungen zu umgehen
                //Nachträglich konsequenterweise für alle Größen nachgetragen
                int B = 1024;
                int kB = 1024 * 1024;
                int MB = 1024 * 1024 * 1024;
                long GB = (long)MB * 1024;
                //Entscheidungsbaum uns Stringformatierung
                if (filelenght == 0)
                {
                    filesize = "0B";
                }
                else if (filelenght / B == 0)
                {
                    filesize = filelenght + "B";
                }
                else if (filelenght / kB == 0)
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

                rootXML.Add(new XElement("datei", new XAttribute("name", file.Name), new XAttribute("groesse", filesize)));
            }
            #endregion
            //Einbetten des Zusammengefügtrn XMLs in ein XDocument
            var xmlDoc = new XDocument(rootXML);
            //Zusammenbau des Speicherord-Pfades und abspeichern
            string saveDest = Path.Combine(_saveDir,_saveFile);
            if (!Directory.Exists(_saveDir))
            {
                Directory.CreateDirectory(_saveDir);
            }
            xmlDoc.Save(saveDest);
            Console.WriteLine("Dokument gespeichert");
            #endregion
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
                //row["Tree"] = tree;
                resultTable.Rows.Add(row);
            }

            finally
            {
                // Freigeben des Workslots
                S.Release();
                Console.WriteLine("Thread " + workDir + " ist fertig");
            }
        }

        //Rekursiver Ordner/Dateileser nach XML
        public XElement GetDirectoryXML(DirectoryInfo dir)
        {
            //Einfügen des aktuellen Verzeichnisses in die Struktur
            var DirXML = new XElement("verzeichnis", new XAttribute("name", dir.Name));
            try
            {
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
                    long GB = (long)MB * 1024;
                    //Entscheidungsbaum uns Stringformatierung
                    if (filelenght / B == 0)
                    {
                        filesize = filelenght + "B";
                    }
                    else if (filelenght / kB == 0)
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
            }
            catch (System.UnauthorizedAccessException)
            {
                DirXML.Add(new XElement("datei", new XAttribute("Fehler", "Zugriff verweigert")));
            }
            catch (Exception e)
            {
                DirXML.Add(new XElement("datei", new XAttribute("Exception", e)));
            }
            //Rekursiver Aufruf zur Behandlung der Ordner
            try
            {
                foreach (var subDir in dir.GetDirectories())
                {
                    DirXML.Add(GetDirectoryXML(subDir));
                }
            }
            catch (System.UnauthorizedAccessException)
            {
                DirXML.Add(new XElement("verzeichnis", new XAttribute("Fehler", "Zugriff verweigert")));
            }
            catch (Exception e)
            {
                DirXML.Add(new XElement("verzeichnis", new XAttribute("Exception", e)));
            }
            //Rückgabe des XML der Odernerstruktur als XElement
            return DirXML;
        }
        #endregion

    }
}
