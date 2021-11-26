/* 
* FILE : AsyncSocketListener.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the async socket listener being used for the server. It will take in information parse it
* out and pass it to the data access layer for it to be added to the db.
*/

using BlazorServerSignalRApp.Server.Hubs;
using FDMS.Server;
using FDMS.Server.Controllers;
using FDMS.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace FDMS_Aircraft_Transmission
{
    /*
    * NAME : AsyncSocketListener : IHostedService, IDisposable
    * PURPOSE : This class is that will hold the fucntionality for the Async Socket Listener for the server
    */
    public class AsyncSocketListener : IHostedService, IDisposable
    {
        public static IHubContext<ChatHub> HubContext;
        private static readonly ManualResetEvent allDone = new(false);
        private static readonly ManualResetEvent sendDone = new(false);
        Socket listener;

        /* 
        * FUNCTION : AsyncSocketListener - constructor
        * DESCRIPTION :
        *   This is the constructor for the AsyncSocketListener
        * PARAMETERS :
        *   IHubContext<ChatHub> hubContext : hub context to for the server hub to send messages to the client
        * RETURNS :
        *   void : none
        */
        public AsyncSocketListener(IHubContext<ChatHub> hubContext)
        {
            HubContext = hubContext;
        }

        /* 
        * FUNCTION : StartAsync
        * DESCRIPTION :
        *   This will start running the async function to allow the program to send messages through the hub.
        * PARAMETERS :
        *   CancellationToken cancellationToken : cancellation token which will be sent when the async needs to stop
        * RETURNS :
        *   Task : The task that the hub connection will be using
        */
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //TODO: your start logic, some timers, singletons, etc
            var t = Task.Run(() => StartListening(), cancellationToken);
            return Task.CompletedTask;
        }

        /* 
        * FUNCTION : StopAsync
        * DESCRIPTION :
        *   This will stop running the async function to allow the program to send messages through the hub and close the socket.
        * PARAMETERS :
        *   CancellationToken cancellationToken : cancellation token which will be sent when the async needs to stop
        * RETURNS :
        *   Task : The task that the hub connection will be using
        */
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //TODO: your stop logic
            listener.Shutdown(SocketShutdown.Both);
            listener.Close();
            listener.Dispose();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        /* 
        * FUNCTION : StartListening
        * DESCRIPTION :
        *   This function begins listening for the socket coming from the Aircraft Transmission system and then will connect
        *   to it.
        * PARAMETERS :
        *   none
        * RETURNS :
        *   void : none
        */
        public void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                // Set up listener
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
        }

        /* 
        * FUNCTION : AcceptCallback
        * DESCRIPTION :
        *   This function will accept the callback that is connecting with the client request.
        * PARAMETERS :
        *   IAsyncResult ar : status of async operation
        * RETURNS :
        *   void : none
        */
        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        /* 
        * FUNCTION : ReadCallback
        * DESCRIPTION :
        *   This function will read the information from the Aircraft Transmission System. Parse all of the information into a telemetry
        *   object. Add the new telemetry object to the database, and then send the information to the front end.
        * PARAMETERS :
        *   IAsyncResult ar : status of async operation
        * RETURNS :
        *   void : none
        */
        public static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket socket = state.workSocket;

            try
            {
                // Read data from the client socket.
                int bytesLength = socket.EndReceive(ar);

                if (bytesLength > 0)
                {
                    string content = Encoding.ASCII.GetString(state.buffer, 0, bytesLength);
                    if (content != null)
                    {
                        Packet packet = JsonSerializer.Deserialize<Packet>(content);
                        if (packet != null)
                        {
                            if (ValidateChecksum(packet))
                            {
                                string[] parameters = packet.Body.Split(',');

                                Telemetry telemetry = new()
                                {
                                    AircraftTailNumber = packet.Header.TailNumber,
                                    GForceData = new GForce()
                                    {
                                        AccelX = Convert.ToSingle(parameters[(int)Packet.Parameters.AccelX]),
                                        AccelY = Convert.ToSingle(parameters[(int) Packet.Parameters.AccelY]),
                                        AccelZ = Convert.ToSingle(parameters[(int) Packet.Parameters.AccelZ]),
                                        Weight = Convert.ToSingle(parameters[(int) Packet.Parameters.Weight]),
                                    },
                                    AttitudeData = new Attitude()
                                    {
                                        Altitude = Convert.ToSingle(parameters[(int)Packet.Parameters.Altitude]),
                                        Pitch = Convert.ToSingle(parameters[(int)Packet.Parameters.Pitch]),
                                        Bank = Convert.ToSingle(parameters[(int)Packet.Parameters.Bank]),
                                    },
                                    TimeStamp = DateTime.ParseExact(parameters[(int)Packet.Parameters.TimeStamp], "M_d_yyyy H:m:s", CultureInfo.InvariantCulture),
                                };

                                // SignalR
                                DatabaseController controller = new DatabaseController();
                                controller.CreateTelemetry(telemetry);
                                try
                                {
                                    HubContext.Clients.All.SendAsync("ReceiveTelemetry", telemetry);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                Send(socket, "success");
                                sendDone.WaitOne(5000);
                            }
                        }

                        // Start receiving next data packet 
                        socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
                else
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        /* 
        * FUNCTION : Send
        * DESCRIPTION :
        *   This function will convert the incoming data to ASCII and then send that inforamtion to the front end client.
        * PARAMETERS :
        *   Socket handler : This is the socket that will be used to send info.
        *   String data : This is the info that will need to be sent.
        * RETURNS :
        *   void : none
        */
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        /* 
        * FUNCTION : SendCallback
        * DESCRIPTION :
        *   This function will send a callback to the Aircraft Transmission System to confirm the what was sent originally.
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
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /* 
        * FUNCTION : ValidateChecksum
        * DESCRIPTION :
        *   This function will validate the checksum coming from the information from the ATS.
        * PARAMETERS :
        *   Packet packet : This is the packet information sent from the ATS.
        * RETURNS :
        *   bool : If the checksum passed or not
        */
        private static bool ValidateChecksum(Packet packet)
        {
            string[] parts = packet.Body.Split(',');

            float altitude = float.Parse(parts[(int)Packet.Parameters.Altitude]);
            float pitch = float.Parse(parts[(int)(Packet.Parameters.Pitch)]);
            float bank = float.Parse(parts[(int)Packet.Parameters.Bank]);

            return packet.Checksum == Convert.ToInt32(Math.Ceiling((altitude + pitch + bank) / 3));
        }
    }
}
