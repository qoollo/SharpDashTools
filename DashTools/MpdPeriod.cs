using Qoollo.MpegDash.Mpd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdPeriod : MpdElement
    {
        internal MpdPeriod(XElement node)
            : base(node)
        {
            baseUrls = new Lazy<IEnumerable<BaseUrl>>(ParseBaseUrls);
            segmentBase = new Lazy<SegmentBase>(ParseSegmentBase);
            segmentList = new Lazy<MpdSegmentList>(ParseSegmentList);
            segmentTemplate = new Lazy<MpdSegmentTemplate>(ParseSegmentTemplate);
            assetIdentifier = new Lazy<AssetIdentifier>(ParseAssetIdentifier);
            adaptationSets = new Lazy<IEnumerable<MpdAdaptationSet>>(ParseAdaptationSets);

        }

        public string Id
        {
            get { return node.Attribute("id")?.Value; }
        }

        public TimeSpan? Start
        {
            get { return helper.ParseOptionalTimeSpan("start"); }
        }

        public TimeSpan? Duration
        {
            get { return helper.ParseOptionalTimeSpan("duration"); }
        }

        public bool BitstreamSwitching
        {
            get { return helper.ParseOptionalBool("bitstreamSwitching", false); }
        }

        /// <summary>
        /// 0...N
        /// 
        /// Specifies a base URL that can be used for reference resolution 
        /// and alternative URL selection
        /// </summary>
        public IEnumerable<BaseUrl> BaseUrls
        {
            get { return baseUrls.Value; }
        }
        private readonly Lazy<IEnumerable<BaseUrl>> baseUrls;
        
        private IEnumerable<BaseUrl> ParseBaseUrls()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "BaseUrl")
                .Select(n => new BaseUrl(n));
        }

        /// <summary>
        /// 0...1
        /// 
        /// Specifies default Segment Base information. 
        /// 
        /// Information in this element is overridden by information in 
        /// AdapationSet.SegmentBase and Representation.SegmentBase, if present.
        /// </summary>
        public SegmentBase SegmentBase
        {
            get { return segmentBase.Value; }
        }
        private readonly Lazy<SegmentBase> segmentBase;

        private SegmentBase ParseSegmentBase()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentBase")
                .Select(n => new SegmentBase(n))
                .FirstOrDefault();
        }

        /// <summary>
        /// 0...1
        /// 
        /// Specifies default Segment List information.
        /// 
        /// Information in this element is overridden by information in 
        /// AdapationSet.SegmentList and Representation.SegmentList, if present.
        /// </summary>
        public MpdSegmentList SegmentList
        {
            get { return segmentList.Value; }
        }
        private readonly Lazy<MpdSegmentList> segmentList;

        private MpdSegmentList ParseSegmentList()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentList")
                .Select(n => new MpdSegmentList(n))
                .FirstOrDefault();
        }

        /// <summary>
        /// 0...1
        /// 
        /// Specifies default Segment Template information.
        ///  
        /// Information in this element is overridden by information in 
        /// AdapationSet.SegmentTemplate and Representation.SegmentTemplate, if present.
        /// </summary>
        public MpdSegmentTemplate SegmentTemplate
        {
            get { return segmentTemplate.Value; }
        }
        private readonly Lazy<MpdSegmentTemplate> segmentTemplate;

        private MpdSegmentTemplate ParseSegmentTemplate()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentTemplate")
                .Select(n => new MpdSegmentTemplate(n))
                .FirstOrDefault();
        }

        /// <summary>
        /// 0...1
        /// 
        /// Specifies that this Period belongs to a certain asset. 
        /// </summary>
        public AssetIdentifier AssetIdentifier
        {
            get { return assetIdentifier.Value; }
        }
        private readonly Lazy<AssetIdentifier> assetIdentifier;

        private AssetIdentifier ParseAssetIdentifier()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "AssetIdentifier")
                .Select(n => new AssetIdentifier(n))
                .FirstOrDefault();
        }

        public IEnumerable<MpdAdaptationSet> AdaptationSets
        {
            get { return adaptationSets.Value; }
        }
        private readonly Lazy<IEnumerable<MpdAdaptationSet>> adaptationSets;

        private IEnumerable<MpdAdaptationSet> ParseAdaptationSets()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "AdaptationSet")
                .Select(n => new MpdAdaptationSet(n));
        }
    }
}