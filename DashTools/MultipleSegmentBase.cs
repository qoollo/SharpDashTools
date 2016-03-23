using System;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    /// <summary>
    /// Specifies multiple Segment base information.
    /// </summary>
    public abstract class MultipleSegmentBase : SegmentBase
    {
        internal MultipleSegmentBase(XElement node) 
            : base(node)
        {
        }

        public uint? Duration
        {
            get { return helper.ParseOptionalUint("duration"); }
        }

        public uint? StartNumber
        {
            get { return helper.ParseOptionalUint("startNumber"); }
        }
    }
}