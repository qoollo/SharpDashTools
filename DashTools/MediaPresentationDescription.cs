using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MediaPresentationDescription : IDisposable
    {
        private readonly Stream stream;

        private readonly bool streamIsOwned;

        private readonly Lazy<XElement> mpdTag;

        private readonly Lazy<XmlAttributeParseHelper> helper;

        public MediaPresentationDescription(Stream mpdStream)
            : this(mpdStream, false)
        {
            this.baseURL = new Lazy<string>(ParseBaseURL);
        }

        public MediaPresentationDescription(string mpdFilePath)
            : this(File.OpenRead(mpdFilePath), true)
        {
            this.baseURL = new Lazy<string>(ParseBaseURL);
        }

        private MediaPresentationDescription(Stream mpdStream, bool streamIsOwned)
        {
            stream = mpdStream;
            this.streamIsOwned = streamIsOwned;

            mpdTag = new Lazy<XElement>(ReadMpdTag);
            fetchTime = new Lazy<DateTimeOffset>(() => { var v = mpdTag.Value; return DateTimeOffset.Now; });
            helper = new Lazy<XmlAttributeParseHelper>(() => new XmlAttributeParseHelper(mpdTag.Value));
            periods = new Lazy<IEnumerable<MpdPeriod>>(ParsePeriods);
        }

        public static MediaPresentationDescription FromUrl(Uri url, string saveFilePath = null)
        {
            if (saveFilePath == null)
                saveFilePath = Path.GetTempFileName();
            using (var client = new WebClient())
            {
                client.DownloadFile(url, saveFilePath);
                return new MediaPresentationDescription(saveFilePath);
            }
        }

        public DateTimeOffset FetchTime
        {
            get { return fetchTime.Value; }
        }
        private readonly Lazy<DateTimeOffset> fetchTime;

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
            get { return helper.Value.ParseDateTimeOffset("availabilityStartTime", Type == "dynamic"); }
        }

        public DateTimeOffset? PublishTime
        {
            get { return helper.Value.ParseOptionalDateTimeOffset("publishTime"); }
        }

        public DateTimeOffset? AvailabilityEndTime
        {
            get { return helper.Value.ParseOptionalDateTimeOffset("availabilityEndTime"); }
        }

        public TimeSpan? MediaPresentationDuration
        {
            get { return helper.Value.ParseOptionalTimeSpan("mediaPresentationDuration"); }
        }

        public TimeSpan? MinimumUpdatePeriod
        {
            get { return helper.Value.ParseOptionalTimeSpan("minimumUpdatePeriod"); }
        }

        public TimeSpan MinBufferTime
        {
            get { return XmlConvert.ToTimeSpan(mpdTag.Value.Attribute("minBufferTime").Value); }
        }

        public TimeSpan? TimeShiftBufferDepth
        {
            get { return helper.Value.ParseOptionalTimeSpan("timeShiftBufferDepth"); }
        }

        public TimeSpan? SuggestedPresentationDelay
        {
            get { return helper.Value.ParseOptionalTimeSpan("suggestedPresentationDelay"); }
        }

        public TimeSpan? MaxSegmentDuration
        {
            get { return helper.Value.ParseOptionalTimeSpan("maxSegmentDuration"); }
        }

        public TimeSpan? MaxSubsegmentDuration
        {
            get { return helper.Value.ParseOptionalTimeSpan("maxSubsegmentDuration"); }
        }

        public string BaseURL
        {
            get { return baseURL.Value; }
        }
        private readonly Lazy<string> baseURL;

        private string ParseBaseURL()
        {
            return mpdTag.Value.Elements()
                .Where(n => n.Name.LocalName == "BaseURL")
                .Select(n => n.Value)
                .FirstOrDefault();
        }

        public IEnumerable<MpdPeriod> Periods
        {
            get { return periods.Value; }
        }
        private readonly Lazy<IEnumerable<MpdPeriod>> periods;

        public void Save(string filename)
        {
            using (var fileStream = File.OpenWrite(filename))
            {
                stream.CopyTo(fileStream);
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

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

        #region IDisposable

        public void Dispose()
        {
            if (streamIsOwned)
                stream.Dispose();
        }

        #endregion
    }
}
