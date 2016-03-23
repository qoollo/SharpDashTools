using System;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdPeriod
    {
        private readonly XElement node;

        private readonly XmlAttributeParseHelper helper;

        internal MpdPeriod(XElement node)
        {
            this.node = node;
            this.helper = new XmlAttributeParseHelper(node);
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
    }
}