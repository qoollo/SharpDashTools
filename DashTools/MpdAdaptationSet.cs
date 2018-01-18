using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdAdaptationSet : MpdElement
    {
        internal MpdAdaptationSet(XElement node)
            : base(node)
        {
            this.segmentTemplate = new Lazy<MpdSegmentTemplate>(ParseSegmentTemplate);
            this.representations = new Lazy<IEnumerable<MpdRepresentation>>(ParseRepresentations);
        }
        
        public uint? Id
        {
            get { return helper.ParseOptionalUint("id"); }
        }

        public uint? Group
        {
            get { return helper.ParseOptionalUint("group"); }
        }

        public string Lang
        {
            get { return node.Attribute("lang")?.Value; }
        }

        public string ContentType
        {
            get
            {
                var attr = node.Attribute("contentType") ?? node.Attribute("mimeType");
                return attr?.Value; }
        }

        public AspectRatio Par
        {
            get { return helper.ParseOptionalAspectRatio("par"); }
        }

        public uint? MinBandwidth
        {
            get { return helper.ParseOptionalUint("minBandwidth"); }
        }

        public uint? MaxBandwidth
        {
            get { return helper.ParseOptionalUint("maxBandwidth"); }
        }

        public uint? MinWidth
        {
            get { return helper.ParseOptionalUint("minWidth"); }
        }

        public uint? MaxWidth
        {
            get { return helper.ParseOptionalUint("maxWidth"); }
        }

        public uint? MinHeight
        {
            get { return helper.ParseOptionalUint("minHeight"); }
        }

        public uint? MaxHeight
        {
            get { return helper.ParseOptionalUint("maxHeight"); }
        }

        public FrameRate MinFrameRate
        {
            get { return helper.ParseOptionalFrameRate("minFrameRate"); }
        }

        public FrameRate MaxFrameRate
        {
            get { return helper.ParseOptionalFrameRate("maxFrameRate"); }
        }

        public bool SegmentAlignment
        {
            get { return helper.ParseOptionalBool("segmentAlignment", false); }
        }

        public bool BitstreamSwitching
        {
            get { return helper.ParseOptionalBool("bitstreamSwitching", false); }
        }

        public bool SubsegmentAlignment
        {
            get { return helper.ParseOptionalBool("subsegmentAlignment", false); }
        }

        public uint SubsegmentStartsWithSAP
        {
            get
            {
                var value = helper.ParseOptionalUint("subsegmentStartsWithSAP", null) 
                    ?? helper.ParseOptionalUint("startWithSAP", null);
                return value.Value;
            }
        }

        /// <summary>
        /// Specifies default Segment Template information.
        /// 
        /// Information in this element is overridden by information in
        /// AdapationSet.SegmentTemplate and 
        /// Representation.SegmentTemplate, if present.
        /// </summary>
        public MpdSegmentTemplate SegmentTemplate
        {
            get { return segmentTemplate.Value; }
        }
        private readonly Lazy<MpdSegmentTemplate> segmentTemplate;

        public IEnumerable<MpdRepresentation> Representations
        {
            get { return representations.Value; }
        }
        private readonly Lazy<IEnumerable<MpdRepresentation>> representations;

        private MpdSegmentTemplate ParseSegmentTemplate()
        {            
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentTemplate")
                .Select(n => new MpdSegmentTemplate(n))
                .FirstOrDefault();
        }

        private IEnumerable<MpdRepresentation> ParseRepresentations()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "Representation")
                .Select(n => new MpdRepresentation(n));
        }
    }
}