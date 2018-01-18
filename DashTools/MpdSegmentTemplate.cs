using System;
using System.Linq;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    /// <summary>
    /// Specifies Segment Template information.
    /// </summary>
    public class MpdSegmentTemplate : MultipleSegmentBase
    {
        internal MpdSegmentTemplate(XElement node) 
            : base(node)
        {
            segmentTimeline = ParseSegmentTimeline();
        }

        /// <summary>
        /// Optional
        /// 
        /// Specifies the template to create the Media Segment List.
        /// </summary>
        public string Media
        {
            get { return node.Attribute("media")?.Value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// Specifies the template to create the Index Segment List. 
        /// If neither the $Number$ nor the $Time$ identifier is included, 
        /// this provides the URL to a Representation Index.
        /// </summary>
        public string Index
        {
            get { return node.Attribute("index")?.Value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// Specifies the template to create the Initialization Segment. 
        /// Neither $Number$ nor the $Time$ identifier shall be included.
        /// </summary>
        public string Initialization
        {
            get { return node.Attribute("initialization")?.Value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// Specifies the template to create the Bitstream Switching Segment. 
        /// Neither $Number$ nor the $Time$ identifier shall be included.
        /// </summary>
        public bool BitstreamSwitching
        {
            get { return helper.ParseOptionalBool("bitstreamSwitching", false); }
        }

        public SegmentTimeline SegmentTimeline
        {
            get { return segmentTimeline; }
        }
        private readonly SegmentTimeline segmentTimeline;

        private SegmentTimeline ParseSegmentTimeline()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentTimeline")
                .Select(n => new SegmentTimeline(n))
                .FirstOrDefault();
        }
    }
}