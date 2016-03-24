using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdInitialization : MpdElement
    {
        internal MpdInitialization(XElement node) 
            : base(node)
        {
        }

        public string SourceUrl
        {
            get { return node.Attribute("sourceURL")?.Value; }
        }
    }
}