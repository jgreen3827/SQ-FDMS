/* 
* FILE : PacketHeader.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the model class for a packet header which will be sent with the packet from the server
*/

namespace FDMS.Server
{
    /*
    * NAME : PacketHeader
    * PURPOSE : The PacketHeader class is the model for a packet header which is sent with the packet.
    */
    public class PacketHeader
    {
        public string TailNumber { get; set; }

        public uint SequenceNumber { get; set; }
    }
}
