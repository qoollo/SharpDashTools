using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdRepresentation : MpdElement
    {
        internal MpdRepresentation(XElement node)
            : base(node)
        {
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
    }
}