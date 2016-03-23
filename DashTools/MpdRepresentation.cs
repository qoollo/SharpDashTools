using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdRepresentation : MpdElement
    {
        internal MpdRepresentation(XElement node)
            : base(node)
        {
        }
    }
}