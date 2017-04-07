using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Speech.Recognition;

namespace SpeechServer.XML
{
    /* +========================================+ */
    /*  XML Grammar Reader
     *  --------------
     *  Author: Johann Ostero
     *  -
     *  This class allows for specific xml file reading windows SpeechSDK, 
     *  It uses a custom built XML setup. A commented out example is at the bottom of the file.
     *  You provide the filename and the object will attempt to construct a Grammar from the XML file.
     *  
     *  When Loaded (Load()), you can access the Grammar through the GetGrammar() method*/

    class XMLGrammarReader
    {
        ConsoleWriter mCw = null;
        Grammar mGrammar = null;

        XmlDocument mDoc = null;
        float mGrammarValidity;


        Dictionary<string, GrammarBuilder> mGrammarBuilderCollection;
        Dictionary<string, Choices> mChoiceCollection;

        string[] mParserRefs = null;
        string mSeperator;
        Dictionary<string, List<string>> mRawChoiceCollection;



        public XMLGrammarReader()
        {
            mCw = new ConsoleWriter("XML Grammar Reader");
            mDoc = new XmlDocument();

            mGrammarBuilderCollection = new Dictionary<string, GrammarBuilder>();
            mChoiceCollection = new Dictionary<string, Choices>();
            mRawChoiceCollection = new Dictionary<string, List<string>>();
        }

        public bool Load(string filename)
        {
            // Checks to see if the xml file exists
            if (!File.Exists(filename))
            {
                mCw.Write("file: \"" + filename + "\" does was not found!", ConsoleWriter_MessageType.ERROR);
                return false;
            }

            // Try to load the file
            try
            { mDoc.Load(filename); }
            catch (Exception) 
            {
                mCw.Write("Could not load XML file: \"" + filename + "\"", ConsoleWriter_MessageType.ERROR);
                return false; 
            }

            // find the root node 
            XmlNode root = mDoc.SelectSingleNode("Grammar");
            if (root == null)
            {
                mCw.Write("Root node not found: 'Grammar'", ConsoleWriter_MessageType.ERROR);
                return false;
            }

            // get the validity attribute
            XmlAttribute validityAttrib = root.Attributes["validity"];
            if(validityAttrib == null)
            {
                mCw.Write("Failed to find validity attribute on Grammar tag");
                return false;
            }
            
            // Convert the validity value to float
            if(!float.TryParse(validityAttrib.Value, out mGrammarValidity))
            {
                mGrammarValidity = 0.0f;
                mCw.Write("Failed to convert validity into float");
            }


            // Collect the choices and grammar builders
            CollectAllChoices(root);
            CollectGrammarBuilder(root);
            CollectGrammarParser(root);

            // Generate the Grammar
            Choices commands = new Choices();

            bool commandFound = false;

            XmlNode GrammarChoiceNode = root.SelectSingleNode("GrammarChoices");
            foreach(XmlNode cmdNode in GrammarChoiceNode.SelectNodes("command"))
            {
                XmlAttribute cmdAttrib = cmdNode.Attributes["ref"];
                if (cmdAttrib != null)
                {
                    string refId = cmdAttrib.Value;
                    GrammarBuilder refgb = null;
                    if (mGrammarBuilderCollection.TryGetValue(refId, out refgb))
                    {
                        commands.Add(refgb);
                        commandFound = true;
                    }
                }
            }

            if (!commandFound)
            {
                mCw.Write("Failed to construct the Grammar", ConsoleWriter_MessageType.ERROR);
                return false;
            }

            mGrammar = new Grammar(new GrammarBuilder(commands));
            return true;
        }

        public Grammar GetGrammar()
        {
            return mGrammar;
        }

        public float Validity
        {
            get { return mGrammarValidity; }
        }

        private void CollectAllChoices(XmlNode root)
        {
            // loop over all the choices
            foreach(XmlNode choiceNode in root.SelectNodes("Choices"))
            {
                // Get the choice id name
                XmlAttribute choiceAttrib = choiceNode.Attributes["id"];
                if (choiceAttrib == null || choiceAttrib.Value == string.Empty)
                    continue;

                // collect all the items and store them in a Choices
                Choices newChoice = new Choices();
                List<string> rawChoiceValues = new List<string>();
                foreach(XmlNode item in choiceNode.SelectNodes("item"))
                {
                    newChoice.Add(item.InnerText);
                    rawChoiceValues.Add(item.InnerText);
                }

                mRawChoiceCollection.Add(choiceAttrib.Value, rawChoiceValues);
                mChoiceCollection.Add(choiceAttrib.Value, newChoice);
            }
        }

        private void CollectGrammarBuilder(XmlNode root)
        {
            // loop over all the grammar builder
            foreach(XmlNode gbNode in root.SelectNodes("GrammarBuilder"))
            {
                // get the grammar builder id name
                XmlAttribute gbAttrib = gbNode.Attributes["id"];
                if (gbAttrib == null || gbAttrib.Value == string.Empty)
                    continue;

                // collect all the items and store them in the GrammarBuilder
                GrammarBuilder newGB = new GrammarBuilder();
                foreach(XmlNode item in gbNode.SelectNodes("item"))
                {
                    // Checks to see if it has any attributes
                    //      if not, then just read the inner text
                    if (item.Attributes.Count == 0)
                    {
                        newGB.Append(item.InnerText);
                        continue;
                    }
                        
                    // Collects attributes
                    XmlAttribute refAttrib = item.Attributes["ref"]; 
                    XmlAttribute typeAttrib = item.Attributes["tagType"];

                    if(refAttrib == null || typeAttrib == null)
                        continue;

                    if (typeAttrib.Value == "Choices")
                    {
                        Choices refChoice = null;
                        if (mChoiceCollection.TryGetValue(refAttrib.Value, out refChoice))
                            newGB.Append(refChoice);
                    }
                    else if (typeAttrib.Value == "GrammarBuilder")
                    {
                        GrammarBuilder refBuilder = null;
                        if (mGrammarBuilderCollection.TryGetValue(refAttrib.Value, out refBuilder))
                            newGB.Append(refBuilder);
                    }
                }

                mGrammarBuilderCollection.Add(gbAttrib.Value, newGB);
            }
        }

        private void CollectGrammarParser(XmlNode root)
        {
            // Collect Grammar Parsing
            XmlNode GrammarParserNode = root.SelectSingleNode("GrammarParser");
            if (GrammarParserNode == null)
            {
                mCw.Write("No GrammarParserNode found!", ConsoleWriter_MessageType.WARNING);
                return;
            }

            // Get Seperator attribute
            XmlAttribute sepAttrib = GrammarParserNode.Attributes["seperator"];
            if (sepAttrib == null || sepAttrib.Value == string.Empty)
            {
                mCw.Write("No seperator attribute specified in GrammarParser", ConsoleWriter_MessageType.WARNING);
                return;
            }
            mSeperator = sepAttrib.Value;

            // Collect all order nodes
            XmlNodeList orderNodes = GrammarParserNode.SelectNodes("order");
            mParserRefs = new string[orderNodes.Count];

            // Collect the ref attributes and store them
            int count = 0;
            foreach(XmlNode orderNode in orderNodes)
            {
                XmlAttribute refAttrib = orderNode.Attributes["ref"];
                if (refAttrib == null || refAttrib.Value == string.Empty) continue;

                mParserRefs[count++] = refAttrib.Value;
            }
        }

        public string ParseString(string sentence)
        {
            // loops through all the collected GrammarParse References
            string temp = string.Empty;
            foreach(string parsRef in mParserRefs)
            {
                /* loops through the referenced values and checks if the sentence contains 
                 * any of the values in the references Choices:
                    If it does, then append the values to a temp with the provided seperator.
                    Then return the appended string */ 
                List<string> choiceList = null;
                if(mRawChoiceCollection.TryGetValue(parsRef, out choiceList))
                {
                    foreach(string choice in choiceList)
                    {
                        if (sentence.Contains(choice) && !temp.Contains(choice))
                        {
                            temp += choice + mSeperator;
                            break;
                        }
                    } // end of the (list<string>) choice loop
                }

            }// end of parseref loop
            return temp;
        }
    }
}


/* +=================================== EXAMPLE ===================================+ */
/*
<?xml version="1.0" encoding="utf-8" ?>
<Grammar>
  
  <!-- Choices -->
  <Choices id="action">
    <item>move</item>
    <item>attack</item>
  </Choices>

  <Choices id="unitType">
    <item>Alpha</item>
    <item>Bravo</item>
    <item>Charlie</item>
    <item>Delta</item>
    <item>Echo</item>
    <item>Foxtrot</item>
    <item>Golf</item>
    <item>Hotel</item>
    <item>India</item>
    <item>Juliet</item>
  </Choices>

  <Choices id="target">
    <item>nearest enemy</item>
  </Choices>
  
  <!-- Grammar Builder -->
  <GrammarBuilder id="unitCommand">
    <item>unit</item>
    <item ref="unitType" tagType="Choices" />
  </GrammarBuilder>

  <GrammarBuilder id="basicMoveCommand">
    <item ref="unitCommand" tagType="GrammarBuilder" />
    <item>move</item>
  </GrammarBuilder>

  <GrammarBuilder id="actionCommand">
    <item ref="unitCommand" tagType="GrammarBuilder" />
    <item ref="action" tagType="Choices" />
    <item ref="target" tagType="Choices" />
  </GrammarBuilder>

  <!-- Final Grammar -->
  <GrammarChoices>
    <command ref="basicMoveCommand" />
    <command ref="actionCommand" />
  </GrammarChoices>
  
</Grammar>
*/