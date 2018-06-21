using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
/* Author: Hergen Paradies
 * Matr.Nr.: 70452163
 * Fach: Threadprogrammierung(?)
 * Aufgabe:
 * 
 * Anmerkung(en):
 */
namespace Client
{
    
    class Hergen
    {
        #region fields
        private List<XDocument> _list;
        private Daniel _daniel;
        private System.Threading.Semaphore S;
        private Thread[] _threads;
        #endregion

        #region ctor
        public Hergen(List<XDocument> list, Daniel daniel, int maxParallelThreads)
        {
            _list = list;
            _daniel = daniel;
            _threads = new Thread[list.Count()-1];
            S = new System.Threading.Semaphore(maxParallelThreads, maxParallelThreads);
        }
        #endregion

        #region methods
        private void Run()
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            int toProcess = _list.Count() - 1;

            for (int i = 0; i < _list.Count() - 2; i++ )
            {
                new Thread(delegate ()
                {
                    //Spawnen des Workers
                    Worker(_list[i]);
                    // Wenn dies der letzte Thread ist, Signal absetzen
                    if (Interlocked.Decrement(ref toProcess) == 0)
                    {
                        resetEvent.Set();
                    }
                }).Start();
            }
            
            //Warten bis alle Threads fertig sind
            resetEvent.WaitOne();
            Console.WriteLine("Alle Threads fertig.");

            // RootDoc abarbeiten (Letzter Eintrag in _list)
            Worker(_list[_list.Count() - 1]);
        }
        private void Worker(XDocument Doc)
        {
            // _daniel.ConnectionOpen();
            // _daniel.Send(Doc);
            // _daniel.ConnectionClose();

            // Debugmessage
            Console.WriteLine("Datei übertragen.");
        }
        #endregion
    }
}
