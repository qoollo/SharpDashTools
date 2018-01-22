using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class XmlAttributeParseHelper
    {
        private readonly XElement node;

        public XmlAttributeParseHelper(XElement node)
        {
            this.node = node;
        }

        public string ParseMandatoryString(string attributeName)
        {
            var attr = node.Attribute(attributeName);
            if (attr == null)
                throw new Exception("Attribute \"" + attributeName + "\" not found on element " + node.ToString());
            return attr.Value;
        }

        public string ParseOptionalString(string attributeName)
        {
            var attr = node.Attribute(attributeName);
            return attr == null ? null : attr.Value;
        }

        public DateTimeOffset? ParseDateTimeOffset(string attributeName, bool mandatoryCondition)
        {
            if (!mandatoryCondition && node.Attribute(attributeName) == null)
                throw new Exception($"MPD attribute @{attributeName} should be present.");
            return ParseOptionalDateTimeOffset(attributeName);
        }

        public uint ParseUint(string attributeName)
        {
            return uint.Parse(node.Attribute(attributeName).Value);
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

        public bool ParseOptionalBool(string attributeName, bool defaultValue)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : bool.Parse(attr.Value);
        }

        public uint? ParseOptionalUint(string attributeName, uint? defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : uint.Parse(attr.Value);
        }

        public int? ParseOptionalInt(string attributeName, int? defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : int.Parse(attr.Value);
        }

        public ulong ParseMandatoryUlong(string attributeName)
        {
            var attr = node.Attribute(attributeName);
            if (attr == null)
                throw new Exception("Attribute \"" + attributeName + "\" not found on element " + node.ToString());
            return ulong.Parse(attr.Value);
        }

        public ulong? ParseOptionalUlong(string attributeName, ulong? defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : ulong.Parse(attr.Value);
        }

        public double? ParseOptionalDouble(string attributeName, double? defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : double.Parse(attr.Value);
        }

        public AspectRatio ParseOptionalAspectRatio(string attributeName, AspectRatio defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : new AspectRatio(attr.Value);
        }

        public FrameRate ParseOptionalFrameRate(string attributeName, FrameRate defaultValue = null)
        {
            var attr = node.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : new FrameRate(attr.Value);
        }
    }
}
