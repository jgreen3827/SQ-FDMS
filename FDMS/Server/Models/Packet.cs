/* 
* FILE : Packet.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the model class for a packet which will be sent from the server
*/


namespace FDMS.Server
{
    /*
    * NAME : Packet
    * PURPOSE : The Packet class is the model for the packets that will be sent.
    */
    public class Packet
    {
        public enum Parameters
        {
            TimeStamp,
            AccelX,
            AccelY,
            AccelZ,
            Weight,
            Altitude,
            Pitch,
            Bank
        }

        public PacketHeader Header { get; set; }

        public string Body { get; set; }

        public int Checksum { get; set; }
    }
}
