using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class SegmentBase : MpdElement
    {
        internal SegmentBase(XElement node) 
            : base(node)
        {
        }

        public uint? Timescale
        {
            get { return helper.ParseOptionalUint("timescale"); }
        }

        public ulong? PresentationTimeOffset
        {
            get { return helper.ParseOptionalUlong("presentationTimeOffset"); }
        }

        public string IndexRange
        {
            get { return node.Attribute("indexRange")?.Value; }
        }

        public bool IndexRangeExact
        {
            get { return helper.ParseOptionalBool("indexRangeExact", false); }
        }

        public double? AvailabilityTimeOffset
        {
            get { return helper.ParseOptionalDouble("availabilityTimeOffset"); }
        }

        public bool AvailabilityTimeComplete
        {
            get { return helper.ParseOptionalBool("availabilityTimeComplete", false); }
        }
    }
}