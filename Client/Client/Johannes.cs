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
 * Aufgabe: Dateisystem auslesen und XML generieren
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
        List<XDocument> _subxmls = new List<XDocument>();
        #endregion

        #region ctor
        public Johannes(DirectoryInfo rootDir, int parallelThreads)
        {
            _rootDir = rootDir;
            S = new System.Threading.Semaphore(parallelThreads, parallelThreads);
        }
        #endregion

        #region Methoden
        //Controller für die Operationen
        public List<XDocument> Controller()
        {
            //Säubern der Liste, just to be sure
            _subxmls.Clear();

            //Anlegen des XML Verzechnisses, gegebenenfalls vorweg löschen von alten Dateien
            System.IO.Directory.CreateDirectory("C:\\XML\\");

            System.IO.DirectoryInfo di = new DirectoryInfo("C:\\XML\\");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            

            //Auslesen des Client Namens
            string clientName = System.Environment.MachineName;
            
            //Ermitteln der Anzahl an Ordnern im Root Verzeichnis (Aus management Gründen)
            //User/Home Verzeichnisse werden wegen Privatsphäre, und das XML Verzeichnis Programmstabilität, ausgeschlossen
            int folderCount = 0;
            foreach (var subDir in _rootDir.GetDirectories())
            {
                if (!(subDir.ToString() == "home") || !(subDir.ToString() == "Users") || !(subDir.ToString() == "XML"))
                {
                    folderCount++;
                }
            }
            //Anlegen des Reset-Events für das warten auf die Threads
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            int toProcess = folderCount;

            //Auslesen der Root-Verzeichnisses und ausführen der entsprechenden Worker
            //User /Home Verzeichnisse und das XML Verzeichnis werden wegen Privatsphäre und Programmstabilität ausgeschlossen
            foreach (var subDir in _rootDir.GetDirectories())
            {
                if (!(subDir.ToString() == "home") || !(subDir.ToString() == "Users") || !(subDir.ToString() == "XML"))
                {
                    new Thread(delegate ()
                    {
                    //Spawnen des Workers
                    Worker(subDir, clientName, ref _subxmls);
                    // Wenn dies der letzte Thread ist, signal absetzen
                    if (Interlocked.Decrement(ref toProcess) == 0)
                        {
                            resetEvent.Set();
                        }
                    }).Start();
                }
            }

            //Warten bis alle Threads fertig sind
            resetEvent.WaitOne();
            Console.WriteLine("Alle Threads fertig.");


            //Auslesen der Dateien des RootDir und einfügen ins "Root" XML

            //Erzeugen des "Root" XML-Elements
            var rootXML = new XElement("client", new XAttribute("name", clientName));

            //Schreiben der Dateien des Verzeichnisses ins XML
            foreach (var file in _rootDir.GetFiles())
            {
                //Output
                string filesize="";

                //Konvertieren der Dateigröße von Byte in passende Einheit
                long filelenght = file.Length;
                filesize = CalcFileSize(filelenght);

                //Eintragen in's XML
                rootXML.Add(new XElement("datei", new XAttribute("name", file.Name), new XAttribute("groesse", filesize)));
            }

            //Erstellen eines XDocuments aus dem XElement und einer XDeclaration
            XDocument rootXMLdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                rootXML);
            _subxmls.Add(rootXMLdoc);

            //Schreiben des XML in Datei
            //string savename = "\\XML\\" + "RootDir" + ".xml";
            //rootXML.Save(savename);

            return _subxmls;

        }
        //Worker Methode
        private void Worker(DirectoryInfo workDir, string client, ref List<XDocument> subxmls)
        {
            try
            {
                //Warten bis ein Workslot freigegeben wird
                S.WaitOne();

                //Let's do this ... LEEEEROY JENKINS !!!
                Console.WriteLine("Thread " + workDir + " hat gestartet");

                //Anlegen des XML Baums mit dem client Root-Tag
                XElement tree = new XElement("client", new XAttribute("name", client));

                //Zusammenbau eines XDocuments aus XElement und XDeclaration
                XDocument rootXMLdoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    tree);
                subxmls.Add(rootXMLdoc);

                //Ausführen der XML-GeneratorMethode
                tree.Add(GetDirectoryXML(workDir));


                //Debug: Speichern des XDocuments vor dem Umwandlen zum string.
                //Test warum gewisse XDocuments leer bleiben. Ergebnis: Alle XDocuments sind befüllt ...
                //string savename = "\\XML\\" + workDir + ".xml";
                //tree.Save(savename);
                
            }

            finally
            {
                // Freigeben des Workslots
                S.Release();
                Console.WriteLine("Thread " + workDir + " ist fertig");
            }
        }
        //DataTable Formatierer -- Inzwischen überflüssig
        private DataTable FormatDataTable(DataTable table)
        {
            // Deklarieren der Spalten Variable
            DataColumn column;

            // Erstellen der Spalte für den Ordnernamen    
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Folder";
            table.Columns.Add(column);

            // Erstellen der Spalte für den zum String konvertierten XML-Baum.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Tree";
            table.Columns.Add(column);

            //Rückgabe der formatierten Tabelle
            return table;
        }
        //Dateigrößen Berechner
        private string CalcFileSize(long filelenght)
        {
            //Variable
            string filesize = "";
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

            return filesize;
        }
        //Konverter für XDocuments zu String, der die Deklaration am Anfang erhält -- Inzwischen überflüssig
        private string XDocumentToString(XDocument doc)
        {
            //Abfangen des Falls, dass ein leeres XDocument übergeben wird
            //Lieber meine Exception als ein Programmabsturz ;)
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            //Erstellen eines neuen Stringbuilders
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                doc.Save(writer);
            }
            return builder.ToString();
        }
        //Rekursiver Ordner/Dateileser nach XML
        private XElement GetDirectoryXML(DirectoryInfo dir)
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
                    filesize = CalcFileSize(filelenght);

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
