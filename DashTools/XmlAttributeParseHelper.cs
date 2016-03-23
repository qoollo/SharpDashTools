using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    class XmlAttributeParseHelper
    {
        private readonly XElement node;

        public XmlAttributeParseHelper(XElement node)
        {
            this.node = node;
        }

        public DateTimeOffset? ParseDateTimeOffset(string attributeName, bool mandatoryCondition)
        {
            if (!mandatoryCondition && node.Attribute(attributeName) == null)
                throw new Exception($"MPD attribute @{attributeName} should be present.");
            return ParseOptionalDateTimeOffset(attributeName);
        }

        public DateTimeOffset? ParseOptionalDateTimeOffset(string attributeName, DateTimeOffset? defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : DateTimeOffset.Parse(attr.Value);
        }

        public TimeSpan? ParseOptionalTimeSpan(string attributeName, TimeSpan? defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : XmlConvert.ToTimeSpan(attr.Value);
        }

        internal bool ParseOptionalBool(string attributeName, bool defaultValue)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : bool.Parse(attr.Value);
        }
    }
}
