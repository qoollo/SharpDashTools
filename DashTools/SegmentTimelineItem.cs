using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class SegmentTimelineItem : MpdElement
    {
        public SegmentTimelineItem(XElement node) 
            : base(node)
        {
        }

        /// <summary>
        /// Optional 
        /// 
        /// Specifies the MPD start time, in @timescale units, the
        /// first Segment in the series starts relative to the beginning
        /// of the Period.
        ///  
        /// The value of this attribute must be equal to or greater 
        /// than the sum of the previous S element earliest 
        /// presentation time and the sum of the contiguous 
        /// Segment durations.
        ///  
        /// If the value of the attribute is greater than what is 
        /// expressed by the previous S element, it expresses 
        /// discontinuities in the timeline.
        ///  
        /// If not present then the value shall be assumed to 
        /// be zero for the first S element and for the subsequent S 
        /// elements, the value shall be assumed to be the sum of 
        /// the previous S element's earliest presentation time and 
        /// contiguous duration (i.e.previous S@t + @d* (@r + 1)). 
        /// </summary>
        public ulong? Time
        {
            get { return helper.ParseOptionalUlong("t"); }
        }

        /// <summary>
        /// Mandatory
        /// 
        /// Specifies the Segment duration, in units of the value of 
        /// the @timescale.
        /// </summary>
        public ulong Duration
        {
            get { return helper.ParseMandatoryUlong("d"); }
        }

        /// <summary>
        /// Optional. Default: 0
        /// 
        /// Specifies the repeat count of the number of following 
        /// contiguous Segments with the same duration expressed 
        /// by the value of @d.This value is zero-based (e.g.a value 
        /// of three means four Segments in the contiguous series). 
        /// A negative value of the @r attribute of the S 
        /// element indicates that the duration indicated in @d
        /// attribute repeats until the start of the next S 
        /// element, the end of the Period or until the next 
        /// MPD update.
        /// </summary>
        public int RepeatCount
        {
            get { return helper.ParseOptionalInt("r", 0).Value; }
        }
    }
}