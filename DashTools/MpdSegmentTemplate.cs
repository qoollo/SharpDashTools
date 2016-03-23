using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    /// <summary>
    /// Specifies Segment template information.
    /// </summary>
    public class MpdSegmentTemplate : MultipleSegmentBase
    {
        internal MpdSegmentTemplate(XElement node) 
            : base(node)
        {
        }

        /// <summary>
        /// Specifies the template to create the Media Segment List.
        /// </summary>
        public string Media
        {
            get { return node.Attribute("media")?.Value; }
        }

        /// <summary>
        /// Specifies the template to create the Index Segment List. 
        /// If neither the $Number$ nor the $Time$ identifier is included, 
        /// this provides the URL to a Representation Index.
        /// </summary>
        public string Index
        {
            get { return node.Attribute("index")?.Value; }
        }

        /// <summary>
        /// Specifies the template to create the Initialization Segment. 
        /// Neither $Number$ nor the $Time$ identifier shall be included.
        /// </summary>
        public string Initialization
        {
            get { return node.Attribute("initialization")?.Value; }
        }

        /// <summary>
        /// Specifies the template to create the Bitstream Switching Segment. 
        /// Neither $Number$ nor the $Time$ identifier shall be included.
        /// </summary>
        public bool BitstreamSwitching
        {
            get { return helper.ParseOptionalBool("bitstreamSwitching", false); }
        }
    }
}