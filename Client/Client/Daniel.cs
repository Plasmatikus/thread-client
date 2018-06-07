using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/* Author: Daniel Wolff
 * Matr.Nr.: 70472015
 * Fach: Threadprogrammierung
 * Aufgabe:
 * 
 * Anmerkung(en):
 */

namespace Client
{
    class Daniel
    {
          
        public class StateObject
        {
            // Client socket.  
            public Socket workSocket = null;
            // Größe des Buffers.  
            public const int BufferSize = 256;
            // Erhaltende Infos speichern -> wieder im Buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Erhaltende data string.  
            public StringBuilder sb = new StringBuilder();
        }

        public class AsynchronousClient
        {
            // Die port Nummer des Servers  
            private const int port = 11000;

            // ManualResetEvent instances signal completion.  
            private static ManualResetEvent connectDone =
                new ManualResetEvent(false);
            private static ManualResetEvent sendDone =
                new ManualResetEvent(false);
            private static ManualResetEvent receiveDone =
                new ManualResetEvent(false);

            // Die Antwort vom Server.  
            private static String response = String.Empty;

            public static void StartClient()
            {
                // Versuche zum Server zu verbinden  
                try
                {
                     
                    Console.WriteLine("Versuche auf den Remote zuzugreifen");
                    IPHostEntry ipHostInfo = Dns.GetHostEntry("192.168.2.1");
                    IPAddress ipAddress = ipHostInfo.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                    // Erstelle einen TCP/IP socket.  
                    Socket client = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                    // Verbinde zum Endpunkt des Servers 
                    client.BeginConnect(remoteEP,
                        new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();

                    // Das hier wäre eine Testnachricht  
                    Send(client, "This is a test<EOF>");
                    sendDone.WaitOne();

                    //Erhalte die Antwort vom Server  
                    Receive(client);
                    receiveDone.WaitOne();

                    // Schreibe die Antwort des Servers  
                    Console.WriteLine("Response received : {0}", response);

                    // Schließe den Socket wieder  
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void ConnectCallback(IAsyncResult ar)
            {
                try
                {
                    // Rufe das Socket vom objekt ab  
                    Socket client = (Socket)ar.AsyncState;

                    // Vervollständige die Verbindung  
                    client.EndConnect(ar);

                    Console.WriteLine("Socket connected to {0}",
                        client.RemoteEndPoint.ToString());

                    // Signal, dass die Verbindung steht  
                    connectDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void Receive(Socket client)
            {
                try
                {
                    // Erstelle ein state object.  
                    StateObject state = new StateObject();
                    state.workSocket = client;

                    // Beginne mit dem Erhalten der Daten vom Server 
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void ReceiveCallback(IAsyncResult ar)
            {
                try
                {
                     
                    StateObject state = (StateObject)ar.AsyncState;
                    Socket client = state.workSocket;

                    // Lese die Daten vom Server 
                    int bytesRead = client.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        // Mögliche zusätzliche Daten, die währenddessen erhalten wurden, somit sollen die gespeichert werden     
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                        // Hole des Rest der Daten  
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // Alle Daten, die angekommen sind und convertiere 
                        if (state.sb.Length > 1)
                        {
                            response = state.sb.ToString();
                        }
                        // Signal, dass alle Bytes angekommen sind 
                        receiveDone.Set();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void Send(Socket client, String data)
            {
                // Convertiere den String in ASCII  
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Sende die Daten zum Server 
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), client);
            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Erhalte den Socket vom State Objekt  
                    Socket client = (Socket)ar.AsyncState;

                    // Vervollstände das Verschicken der Daten zum Server  
                    int bytesSent = client.EndSend(ar);
                    Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                    // Signal, dass alles angekommen ist  
                    sendDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            
        }
    }


}
   