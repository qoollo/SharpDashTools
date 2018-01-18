using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public abstract class MpdElement
    {
        protected readonly XElement node;

        protected readonly XmlAttributeParseHelper helper;

        internal MpdElement(XElement node)
        {
            this.node = node;
            this.helper = new XmlAttributeParseHelper(node);
        }
    }
}