using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

/* +========================================+ */
/*  BaseXMLTagCollection
 *  --------------
 *  Author: Johann Ostero
 *  -
 *  This is a file with multiple "tag containers".
 *  These containers are used to group up tags and fallback values that the GenericXMLReader requires.
 *  
 *  @ BaseXMLTagCollection
 *  - This is the base version, that will store a variable amount of tags and fallbacks, however, they will always be equal.
 */

namespace SpeechServer.XML.TagContainers
{
    class BaseXMLTagCollection
    {
        string[] mTags = null;
        string[] mFallBackValues = null;

        // Both of them have to be the same size
        public BaseXMLTagCollection(int numOfTags)
        {
            mTags = new string[numOfTags];
            mFallBackValues = new string[numOfTags];
        }

        public void SetTag(int index, string tagname, string fallbackValue)
        {
            Trace.Assert(index > -1 && index < mTags.Length);
            mTags[index] = tagname;
            mFallBackValues[index] = fallbackValue;
        }

        public string Tag(int index)
        {
            Trace.Assert(index > -1 && index < mTags.Length);
            return mTags[index];
        }

        public string FallBack(int index)
        {
            Trace.Assert(index > -1 && index < mTags.Length);
            return mFallBackValues[index];
        }

        public int Size
        {
            get { return mTags.Length; }
        }
    };
}
