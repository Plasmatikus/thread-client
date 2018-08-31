using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    class Sender
    {
        #region fields
        private List<byte[]> _list;
        private System.Threading.Semaphore S;
        private IPAddress _ipaddress;
        private int _port;
        private ManualResetEvent waitForLoop = new ManualResetEvent(false);
        #endregion

        #region ctor
        public Sender(List<byte[]> list, int parallelThreads, IPAddress ipaddress, int port)
        {
            _list = list;
            S = new System.Threading.Semaphore(parallelThreads, parallelThreads);
            _ipaddress = ipaddress;
            _port = port;
        }
        #endregion

        #region methods
        public void Run()
        {
            // Erstelle den Lichtschalter
            ManualResetEvent waitAllWorkers = new ManualResetEvent(false);
            int toProcess = _list.Count() - 1;

            //Debug Ausgabe
            Console.WriteLine("Spawne " + (_list.Count()-1) + " Threads mit Listen ID 0 bis " + (_list.Count() - 2) + ".");

            for (int i = 0; i <= _list.Count() - 2; i++)
            {
                waitForLoop.Reset();

                Console.WriteLine("Spawne Thread für Listeneintrag "+ i);
                new Thread(delegate ()
                {
                    //Spawnen des Workers
                    Worker(_list[i], (i));
                    // Wenn dies der letzte Thread ist, Signal absetzen (Der Letzte macht das Licht aus!)
                    if (Interlocked.Decrement(ref toProcess) == 0)
                    {
                        waitAllWorkers.Set();
                    }
                }).Start();
                waitForLoop.WaitOne();
            }

            // Warten bis alle Threads fertig sind (Wenn das Licht ausgeht!)
            waitAllWorkers.WaitOne();

            //Debug Ausgaben
            Console.WriteLine("Sende das RootDoc, ListenID " + (_list.Count() - 1));
            
            // RootDoc abarbeiten (Letzter Eintrag in _list) -- Übergabe des ResetEvents ist zwar sinnlos, aber nötig 
            Worker(_list[_list.Count() - 1], (_list.Count()-1));

            //Debug Ausgabe
            Console.WriteLine("Senden des RootDoc abgeschlossen.");

        }

        private void Worker(byte[] bear, int listID)
        {
            waitForLoop.Set();
            bool sent = false;
            while (!sent)
            {
                try
                {
                    //Warten bis ein Workslot freigegeben wird
                    S.WaitOne();

                    //Debug-Output
                    Console.WriteLine("Sendethread gestartet! ID:"+listID);

                    try
                    {
                        // Erstellen des Sockets
                        Socket mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        // Verbindungsversuch, und neuversuch nach einer Sekunde bei misserfolg
                        bool connect = false;
                        while (!connect)
                        {
                            try
                            {
                                mysocket.Connect(_ipaddress, _port);
                                connect = true;
                            }
                            catch
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        //Senden!
                        mysocket.Send(bear);
                        // Debug-Warten, um dem Server etwas Zeit zu geben.
                        Thread.Sleep(100);
                        // Schließen der Verbindung
                        mysocket.Close();

                        sent = true;
                    }
                    catch
                    {
                        Thread.Sleep(30);
                    }


                }
                finally
                {
                    // Freigeben des Workslots
                    S.Release();
                    Console.WriteLine("Sendethread fertig.ID:" + listID);

                }
            }
        }
        #endregion
    }
}
