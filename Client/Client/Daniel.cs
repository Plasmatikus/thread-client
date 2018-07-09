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
 * 
 * Anmerkung(en): new Thread(delegate ()
                    {
                    server}).Start();

                   
 */
 
namespace Client
{
    class Daniel
    {/*

            public static void StartClient()
            {
            new Thread(delegate () { Servertest1.SynchronousSocketListener.StartListening(); }).Start();

            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {

                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.


                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                string HostName = Dns.GetHostName();
                Console.WriteLine("Host Name of machine =" + HostName);
                IPAddress[] ipaddress = Dns.GetHostAddresses(HostName);
                Console.WriteLine("IP Address of Machine is");
                foreach (IPAddress ip in ipaddress)
                {
                    Console.WriteLine(ip.ToString());
                }

                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11001);

                Socket sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                sock.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, 0);
                sock.Bind(new IPEndPoint(IPAddress.IPv6Any, 11001));
                sock.Listen(4);
                    
                Console.WriteLine("Client has connected successfully with the server");

                //Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                    
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device. 
                    Console.WriteLine("Wait on the answer from the server...");
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    Console.ReadKey();
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    Console.ReadKey();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

       */
    }
}
