using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private DataTable _table;
        private Daniel _daniel;
        private System.Threading.Semaphore S;
        #endregion

        #region ctor
        public Hergen(DataTable table, Daniel daniel, int maxParallelThreads)
        {
            _table = table;
            _daniel = daniel;
            S = new System.Threading.Semaphore(maxParallelThreads, maxParallelThreads);
        }
        #endregion

        #region methods

        #endregion
    }
}
