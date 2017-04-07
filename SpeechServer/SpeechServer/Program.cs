using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Speech.Recognition;
using SpeechToCommand;

using SpeechServer.XML.TagContainers;
using SpeechServer.XML;

namespace SpeechServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the XML grammar file
            XMLGrammarReader grammarReader = new XMLGrammarReader();
            if (!grammarReader.Load(@"InfiniGrammar.xml"))
            {
                Console.ReadKey(true);
                return;
            }
            
            // Init the speech recogniser
            SpeechCommandRecogniser listener = new SpeechCommandRecogniser(grammarReader.GetGrammar());
            listener.SetValidity(grammarReader.Validity);

            // Init the server
            UdpXMLTags xmlServerTags = new UdpXMLTags("IP", "127.0.0.1",      "Port", "4001",     "PacketSize", "256");
            NetworkUDP server = new NetworkUDP(@"ServerDetails.xml", "ServerDetails", xmlServerTags);

            Console.WriteLine("UDP Socket Established");
            Console.WriteLine("IP: " + server.IProtocol);
            Console.WriteLine("Port: " + server.PortNumber);
            Console.WriteLine("Packet Size: " + server.DataPacketSize);
            Console.WriteLine("-------------------");
            while(true)
            {
                string cmd = listener.WaitFor();
                if(cmd != string.Empty)
                {
                    string formattedCmd = grammarReader.ParseString(cmd);

                    if (server.Send(formattedCmd))
                        Console.WriteLine("Sending: " + formattedCmd);
                }
                listener.ClearUtterance();
            }
        }
    }
}
