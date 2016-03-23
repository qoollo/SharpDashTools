using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdSegmentTemplate : MpdElement
    {
        internal MpdSegmentTemplate(XElement node) 
            : base(node)
        {
        }
    }
}