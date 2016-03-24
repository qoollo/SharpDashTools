using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdSegmentList : MpdElement
    {
        internal MpdSegmentList(XElement node) 
            : base(node)
        {
            initialization = new Lazy<MpdInitialization>(ParseInitialization);
            segmentUrls = new Lazy<IEnumerable<MpdSegmentUrl>>(ParseSegmentUrls);
        }

        public uint? Timescale
        {
            get { return helper.ParseOptionalUint("timescale"); }
        }

        public uint? Duration
        {
            get { return helper.ParseOptionalUint("duration"); }
        }

        public MpdInitialization Initialization
        {
            get { return initialization.Value; }
        }
        private readonly Lazy<MpdInitialization> initialization;

        public IEnumerable<MpdSegmentUrl> SegmentUrls
        {
            get { return segmentUrls.Value; }
        }
        private readonly Lazy<IEnumerable<MpdSegmentUrl>> segmentUrls;

        private MpdInitialization ParseInitialization()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "Initialization")
                .Select(n => new MpdInitialization(n))
                .FirstOrDefault();
        }

        private IEnumerable<MpdSegmentUrl> ParseSegmentUrls()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentURL")
                .Select(n => new MpdSegmentUrl(n));
        }
    }
}