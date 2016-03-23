using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdPeriod
    {
        private readonly XElement node;

        internal MpdPeriod(XElement node)
        {
            this.node = node;
        }
    }
}