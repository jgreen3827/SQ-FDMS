/* 
* FILE : AsyncClient.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the for the AsyncClient class, this class holds all the main functionality for the
* Aircraft Transmission System client.
*/
using FDMS.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace FDMS_Aircraft_Transmission.ATS_Client_Socket
{
    /*
    * NAME : AsyncClient
    * PURPOSE : The AsyncClient class has been built to hold all of the functionality for the
    * Aircraft Transmission System client. This class will be able to open a socket connection 
    * to connect to the Ground Terminal Softqare. It will have the ability to read in telemetry
    * data and send the contents as a package object across the socket to GTC. It will then be 
    * able to receive a confirmation message coming back to ensure the content had been received.
    */
    public class AsyncClient
    {
        // The port number for the remote device.  
        private const int port = 11000;

        // ManualResetEvent instances signal completion.  
        private static readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent sendDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        /* 
        * FUNCTION : StartClient
        * DESCRIPTION :
        *   NOTE: This prototype is currently designed to connect to the ip address of a single
        *         Host. This means they are running on the same machine. Until a specific IP 
        *         address is specified for the remote connection, they cannot run on separate 
        *         machines. See NOTE on line 66 to do so.
        *   This function will open a socket and connect to the server. This will then call 
        *   functions that will read in a file, and will send the contents of the file to 
        *   the server. It will then close the socket once complete.
        * PARAMETERS :
        *   List<string> fileNames : telemetry file names to be read
        * RETURNS :
        *   void : none
        */
        public static void StartClient(List<string> fileNames)
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                // NOTE: Use a specific IP address and port number to connect to remote socket here.
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint. Will wait indefinitely until it connects
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // --> Connected to server here <---

                foreach (string fileName in fileNames)
                {
                    uint sequenceNumber = 0;

                    if (File.Exists(fileName))
                    {
                        // Read file using StreamReader. Reads file line by line
                        using (StreamReader file = new StreamReader(fileName))
                        {
                            string line = "";
                            while (!String.IsNullOrEmpty(line = file.ReadLine()))
                            {
                                // Remove line ending comma ',' if it's there
                                if (line.LastIndexOf(',') == line.Length)
                                {
                                    line.Remove(line.Length - 1, 1);
                                }

                                // Instantiate Packet
                                Packet packet = new Packet()
                                {
                                    Header = new PacketHeader()
                                    {
                                        TailNumber = Path.GetFileNameWithoutExtension(fileName),
                                        SequenceNumber = sequenceNumber
                                    },
                                    Body = line.Trim(),
                                    Checksum = CalculateChecksum(line)
                                };

                                Console.WriteLine($"Sending packet: {sequenceNumber}, Body: {packet.Body}");

                                // Send packet data to the remote device. Tries for 5 seconds
                                Send(client, packet);
                                sendDone.WaitOne(5000); 

                                // Receive the response from the remote device before continuing. Waits for 5 seconds
                                Receive(client);
                                receiveDone.WaitOne(5000);

                                // Write the response to the console.  
                                Console.WriteLine("Response received : {0}", response);

                                sequenceNumber++;

                                //wait for 1 second to read and send next line
                                Thread.Sleep(1000);
                            }

                            file.Close();
                            Console.WriteLine(fileName + "has completed...");
                        }
                    }
                }

                // Send test data to the remote device.  
                Send(client, null);
                sendDone.WaitOne();

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                sendDone.Close();
                connectDone.Close();
                receiveDone.Close();
                sendDone.Dispose();
                connectDone.Dispose();
                receiveDone.Dispose();

                Console.WriteLine("Transmission server shutting down.");
            }
        }

        /* 
        * FUNCTION : ConnectCallback
        * DESCRIPTION :
        *   This function will determine if the socket has been connected
        * PARAMETERS :
        *   IAsyncResult ar : status of async operation
        * RETURNS :
        *   void : none
        */
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /* 
        * FUNCTION : Receive
        * DESCRIPTION : 
        *   This function will be called when data is received from the remote device.
        * PARAMETERS :
        *   Socket client : The socket to be used to receive info.
        * RETURNS :
        *   void : none
        */
        private static void Receive(Socket client)
        {
            try
            {
                // Receive as a state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /* 
        * FUNCTION : ReceiveCallback
        * DESCRIPTION :
        *   This function will receive the state object from the client socket,
        *   read in the data, and store it for a response
        * PARAMETERS :
        *   IAsyncResult ar : status of async operation
        * RETURNS :
        *   void : none
        */
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /* 
        * FUNCTION : Send
        * DESCRIPTION :
        *   This function will convert the packet to bytes and send the info
        *   to the remote device.
        * PARAMETERS :
        *   Socket client : The client that will be used to send the info.
        *   Packet packet : This is the information which will be sent to the packet.
        * RETURNS :
        *   void : none
        */
        private static void Send(Socket client, Packet packet)
        {
            byte[] byteData = null;

            if (packet != null)
            {
                // Convert the string data to byte data using ASCII encoding.  
                byteData = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(packet));
            }
            else
            {
                byteData = new byte[0];
            }

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        /* 
        * FUNCTION : SendCallback
        * DESCRIPTION :
        *   This function will get the socket from the state object, send the data to
        *   the remote device, and signal all bytes have been sent.
        * PARAMETERS :
        *   IAsyncResult ar : status of async operation
        * RETURNS :
        *   void : none
        */
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /* 
        * FUNCTION : CalculateChecksum
        * DESCRIPTION :
        *   This function will Calulate the checksum for the packet
        * PARAMETERS :
        *   string data : This is the information the checksum is being created for
        * RETURNS :
        *   int : This is the checksum
        */
        private static int CalculateChecksum(string data)
        {
            string[] parts = data.Split(',');

            float altitude = float.Parse(parts[(int)Packet.Parameters.Altitude]);
            float pitch = float.Parse(parts[(int)(Packet.Parameters.Pitch)]);
            float bank = float.Parse(parts[(int)Packet.Parameters.Bank]);

            return Convert.ToInt32(Math.Ceiling((altitude + pitch + bank) / 3));
        }

    }
}
