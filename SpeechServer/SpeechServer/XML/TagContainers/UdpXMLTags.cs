using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* +========================================+ */
/*  UdpXmlTags
 *  --------------
 *  Author: Johann Ostero
 *  -
 *  This is a file with multiple "tag containers".
 *  These containers are used to group up tags and fallback values that the GenericXMLReader requires.
 *  
 * @ UdpXMLTags
 *  - This is just a dumbed down child of the BaseXmlTagCollection, taking direct amounts of variables, with a fixed size of 3,
 *    since they are the values required for the NetworkUDP xml-server file
 */

namespace SpeechServer.XML.TagContainers
{
    class UdpXMLTags : BaseXMLTagCollection
    {
        public UdpXMLTags(string ipTag, string ipfallback, string portTag, string portfallback, string packetSizeTag, string packetSizeFallback)
            : base(3)
        {
            SetTag(0, ipTag, ipfallback);
            SetTag(1, portTag, portfallback);
            SetTag(2, packetSizeTag, packetSizeFallback);
        }
    }
}
