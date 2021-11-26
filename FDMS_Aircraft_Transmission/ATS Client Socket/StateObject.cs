/* 
* FILE : StateObject.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the StateObject class which holds the socket to be able to send information.
*/
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace FDMS_Aircraft_Transmission.ATS_Client_Socket
{
    /*
    * NAME : StateObject
    * PURPOSE : The StateObject class holds all the inforamtion for the socket to allow information
    * to be sent and received.
    */
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 200;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
}
