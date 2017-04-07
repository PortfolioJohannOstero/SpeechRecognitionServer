using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using System.Diagnostics;

using SpeechServer.XML.TagContainers;

namespace SpeechServer.XML
{
    /* +========================================+ */
    /*  Generic XML Reader
     *  --------------
     *  Author: Johann Ostero
     *  -
     *  This class allows for generic xml file reading, 
     *  You provide the filename, the root node and what child root node values you want.
     *  And for an extra level of security, default values are required ("fallbackValues")
     *  
     *  When Loaded (Load()), you can access the values through, getValue(index) methods*/

    class GenericXMLReader
    {
        string[] mTagValues = null;
        ConsoleWriter cw = null;

        public GenericXMLReader()
        {
            cw = new ConsoleWriter("GenericXmlReader");
        }

        public bool Load(string filename, string rootNodeTag, BaseXMLTagCollection tagCol)
        {
            Trace.Assert(tagCol != null);

            mTagValues = new string[tagCol.Size];

            XmlDocument doc = new XmlDocument();

            // Does the file exist?
            if (!File.Exists(filename))
            {
                cw.Write("\"" + filename + "\" does not exist", ConsoleWriter_MessageType.ERROR);
                return false;
            }

            doc.Load(filename); // load xml file

            // Does the root node exist?
            XmlNode rootNode = doc.SelectSingleNode(rootNodeTag);
            if(rootNode == null)
            {
                cw.Write("\"" + rootNodeTag + "\" could not be found!", ConsoleWriter_MessageType.ERROR);
                return false;
            }


            for (int i = 0; i < tagCol.Size; i++)
            {
                XmlNode node = rootNode.SelectSingleNode(tagCol.Tag(i));
                string value = string.Empty;
                if(node == null)
                {
                    cw.Write("\"" + tagCol.Tag(i) + "\" was not found! Giving it default value: " + tagCol.FallBack(i), ConsoleWriter_MessageType.WARNING);
                    value = tagCol.FallBack(i);
                }
                else
                    value = node.InnerText;

                mTagValues[i] = value;
            }

            return true;
        }

        public int GetValueAsInt(int index)
        {
            Trace.Assert(index > -1 && index < mTagValues.Length);
            
            // Tries to conver the string into an integer
            int i = 0;
            if (!Int32.TryParse(mTagValues[index], out i))
                cw.Write("Could not convert " + mTagValues[index] + " to int");

            return i;
        }
        public string GetValueAsString(int index)
        {
            Trace.Assert(index > -1 && index < mTagValues.Length);
            return mTagValues[index];
        }
    }
}
