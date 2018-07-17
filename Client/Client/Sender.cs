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
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            int toProcess = _list.Count() - 1;

            for (int i = 0; i <= _list.Count() - 2; i++)
            {
                new Thread(delegate ()
                {
                    //Spawnen des Workers
                    Worker(_list[i - 1]);
                    // Wenn dies der letzte Thread ist, Signal absetzen (Der Letzte macht das Licht aus!)
                    if (Interlocked.Decrement(ref toProcess) == 0)
                    {
                        resetEvent.Set();
                    }
                }).Start();
            }

            // Warten bis alle Threads fertig sind (Wenn das Licht ausgeht!)
            resetEvent.WaitOne();

            // RootDoc abarbeiten (Letzter Eintrag in _list)
            Worker(_list[_list.Count() - 1]);
        }

        private void Worker(byte[] bear)
        {
            bool sent = false;
            while (!sent)
            {
                try
                {
                    //Warten bis ein Workslot freigegeben wird
                    S.WaitOne();

                    //Debug-Output
                    Console.WriteLine("Bär senden gestartet!");

                    try
                    {
                        // Erstellen des Sockets
                        Socket mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        mysocket.Connect(_ipaddress, _port);
                        mysocket.Send(bear);
                        //Debug-Warten, um dem Server etwas Zeit zu geben.
                        Thread.Sleep(100);
                        mysocket.Close();

                        sent = true;
                        //Thread.Sleep(100);
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
                    Console.WriteLine("Ein Bär wurde gesendet.");

                }
            }
        }
        #endregion
    }
}
