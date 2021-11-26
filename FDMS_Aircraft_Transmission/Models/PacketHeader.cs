/* 
* FILE : PacketHeader.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file hold the model for a Packet Header.
*/

namespace FDMS.Server
{
    /*
    * NAME : PacketHeader
    * PURPOSE : The PacketHeader class is the model for all the things needed for the packet header
    * to allow a packet to be sent.
    */
    public class PacketHeader
    {
        public string TailNumber { get; set; }

        public uint SequenceNumber { get; set; }
    }
}
