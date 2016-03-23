using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MediaPresentationDescription
    {
        private readonly Stream stream;

        private readonly Lazy<XElement> mpdTag;

        public MediaPresentationDescription(Stream mpdStream)
        {
            stream = mpdStream;

            mpdTag = new Lazy<XElement>(ReadMpdTag);
            periods = new Lazy<IEnumerable<MpdPeriod>>(ParsePeriods);
        }

        public string Id
        {
            get { return mpdTag.Value.Attribute("id")?.Value; }
        }

        public string Type
        {
            get { return mpdTag.Value.Attribute("type")?.Value ?? "static"; }
        }

        public string Profiles
        {
            get { return mpdTag.Value.Attribute("profiles").Value; }
        }

        public DateTimeOffset? AvailabilityStartTime
        {
            get { return ParseDateTimeOffset("availabilityStartTime", Type == "dynamic"); }
        }

        public DateTimeOffset? PublishTime
        {
            get { return ParseOptionalDateTimeOffset("publishTime"); }
        }

        public DateTimeOffset? AvailabilityEndTime
        {
            get { return ParseOptionalDateTimeOffset("availabilityEndTime"); }
        }

        public TimeSpan? MediaPresentationDuration
        {
            get { return ParseOptionalTimeSpan("mediaPresentationDuration"); }
        }

        public TimeSpan? MinimumUpdatePeriod
        {
            get { return ParseOptionalTimeSpan("minimumUpdatePeriod"); }
        }

        public TimeSpan MinBufferTime
        {
            get { return XmlConvert.ToTimeSpan(mpdTag.Value.Attribute("minBufferTime").Value); }
        }

        public TimeSpan? TimeShiftBufferDepth
        {
            get { return ParseOptionalTimeSpan("timeShiftBufferDepth"); }
        }

        public TimeSpan? SuggestedPresentationDelay
        {
            get { return ParseOptionalTimeSpan("suggestedPresentationDelay"); }
        }

        public TimeSpan? MaxSegmentDuration
        {
            get { return ParseOptionalTimeSpan("maxSegmentDuration"); }
        }

        public TimeSpan? MaxSubsegmentDuration
        {
            get { return ParseOptionalTimeSpan("maxSubsegmentDuration"); }
        }

        public IEnumerable<MpdPeriod> Periods
        {
            get { return periods.Value; }
        }
        private readonly Lazy<IEnumerable<MpdPeriod>> periods;

        private XElement ReadMpdTag()
        {
            using (var reader = XmlReader.Create(stream))
            {
                reader.ReadToFollowing("MPD");
                return XNode.ReadFrom(reader) as XElement;
            }
        }

        private IEnumerable<MpdPeriod> ParsePeriods()
        {
            return mpdTag.Value.Elements()
                .Where(n => n.Name.LocalName == "Period")
                .Select(n => new MpdPeriod(n));
        }

        private DateTimeOffset? ParseDateTimeOffset(string attributeName, bool mandatoryCondition)
        {
            if (!mandatoryCondition && mpdTag.Value.Attribute(attributeName) == null)
                throw new Exception($"MPD attribute @{attributeName} should be present.");
            return ParseOptionalDateTimeOffset(attributeName);
        }

        private DateTimeOffset? ParseOptionalDateTimeOffset(string attributeName, DateTimeOffset? defaultValue = null)
        {
            var attr = mpdTag.Value.Attribute(attributeName);
            return attr == null 
                ? defaultValue
                : DateTimeOffset.Parse(attr.Value);
        }

        private TimeSpan? ParseOptionalTimeSpan(string attributeName, TimeSpan? defaultValue = null)
        {
            var attr = mpdTag.Value.Attribute(attributeName);
            return attr == null
                ? defaultValue
                : XmlConvert.ToTimeSpan(attr.Value);
        }
    }
}
