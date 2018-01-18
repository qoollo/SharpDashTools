using System;
using System.Linq;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdRepresentation : MpdElement
    {
        internal MpdRepresentation(XElement node)
            : base(node)
        {
            segmentList = new Lazy<MpdSegmentList>(ParseSegmentList);
            this.segmentTemplate = new Lazy<MpdSegmentTemplate>(ParseSegmentTemplate);
            this.baseURL = new Lazy<string>(ParseBaseURL);
        }

        public string Id
        {
            get { return node.Attribute("id").Value; }
        }
        
        public uint Bandwidth
        {
            get { return helper.ParseUint("bandwidth"); }
        }

        public uint? QualityRanking
        {
            get { return helper.ParseOptionalUint("qualityRanking"); }
        }
        
        public string DependencyId
        {
            get { return node.Attribute("dependencyId").Value; }
        }

        public string MediaStreamStructureId
        {
            get { return node.Attribute("mediaStreamStructureId").Value; }
        }

        public MpdSegmentList SegmentList
        {
            get { return segmentList.Value; }
        }
        private readonly Lazy<MpdSegmentList> segmentList;

        public MpdSegmentTemplate SegmentTemplate
        {
            get { return segmentTemplate.Value; }
        }
        private readonly Lazy<MpdSegmentTemplate> segmentTemplate;

        public string BaseURL
        {
            get { return baseURL.Value; }
        }
        private readonly Lazy<string> baseURL;

        private MpdSegmentList ParseSegmentList()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentList")
                .Select(n => new MpdSegmentList(n))
                .FirstOrDefault();
        }

        private MpdSegmentTemplate ParseSegmentTemplate()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "SegmentTemplate")
                .Select(n => new MpdSegmentTemplate(n))
                .FirstOrDefault();
        }
        private string ParseBaseURL()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "BaseURL")
                .Select(n => n.Value)
                .FirstOrDefault();
        }
    }
}