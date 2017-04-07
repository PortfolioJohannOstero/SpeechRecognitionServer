using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using SpeechServer.XML;
using SpeechServer.XML.TagContainers;

namespace SpeechServer
{
    /* +========================================+ */
    /*  NetworkUDP
     *  --------------
     *  Author: Johann Ostero
     *  Credit: >PUT NAME HERE<
     *  -
     *  This is responsible for setting up a basic one way UDP server.
     *  When created, you can call the Send method to send an unmonitored string message 
     *  to the desired Socket.
     *  
     *  The NetworkUDP allows for direct hardcoded parameters (IP Address, Port, max string length),
     *  however, it also allows for reading from an XML file, which is recommended.
     *  This is done using UdpXMLTags and GenericXmlReader
     */

    class NetworkUDP
    {
        string mIP = string.Empty;
        byte[] mData = null;
        int mPort;

        Socket mSocket = null;
        IPEndPoint mIPE = null;

        public NetworkUDP(string ipAddress, int port, int packetSize)
        {
            Init(ipAddress, port, packetSize);
        }

        // collect the server information using xml
        public NetworkUDP(string serverDetailXmlFile, string rootNode, UdpXMLTags tagCollection)
        {
            GenericXMLReader serverXml = new GenericXMLReader();
            if (serverXml.Load(serverDetailXmlFile, rootNode, tagCollection))
            {
                Init(serverXml.GetValueAsString(0), // ip
                     serverXml.GetValueAsInt(1), // port
                     serverXml.GetValueAsInt(2)); // packet size
            }
        }

        void Init(string ipAddress, int port, int packetSize)
        {

            mIP = ipAddress;
            mPort = port;
            mData = new byte[packetSize];

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mIPE = new IPEndPoint(IPAddress.Parse(mIP), mPort);
        }

        public bool Send(string message)
        {
            if (mSocket == null)
                return false;

            mData = Encoding.ASCII.GetBytes(message);
            mSocket.SendTo(mData, mIPE);
            return true;
        }

        public void Close()
        {
            if (mSocket != null)
                mSocket.Close();
        }

        // +==== Getters ====+
        public string IProtocol
        { 
            get { return mIP; } 
        }
        public int PortNumber
        { 
            get { return mPort; } 
        }

        public int DataPacketSize
        { 
            get { return (mData != null) ? mData.Length : 0; }
        }
    }
}
