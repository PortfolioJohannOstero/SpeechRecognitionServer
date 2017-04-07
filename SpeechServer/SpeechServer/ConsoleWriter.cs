using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* +========================================+ */
/*  Console Writer
 *  --------------
 *  Author: Johann Ostero
 *  -
 *  This class allows for consistent console writing, 
 *  by printing out the name of the class, and allowing for 3 message types:
 *      Regular, Error & Warning.
 *  A message would look like this:
 *      ClassName: Message
 *      ClassName - ERROR: Message
 *      ClassName - WARNING: Message
 *  
 * This Class is just a basic helper object, to put this consistency in one central location */

namespace SpeechServer
{
    public enum ConsoleWriter_MessageType
    {
        REGULAR, ERROR, WARNING
    };

    class ConsoleWriter
    {
        string mClassName = string.Empty;

        public ConsoleWriter(string className)
        {
            mClassName = className;
        }

        public void Write(string message, ConsoleWriter_MessageType msgType = ConsoleWriter_MessageType.REGULAR)
        {
            string startMsg = mClassName;
            switch (msgType)
            {
                case ConsoleWriter_MessageType.ERROR:
                    startMsg += " - ERROR: " + message;
                    break;
                case ConsoleWriter_MessageType.WARNING:
                    startMsg += " - WARNING: " + message;
                    break;
                default:
                    startMsg += ": " + message;
                    break;
            }

            Console.WriteLine(startMsg);
        }
    }
}
