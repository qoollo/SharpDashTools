using System;
using System.Linq;
using Xunit;

namespace Qoollo.MpegDash.Tests
{
    public class MediaPresentationDescriptionTests : IClassFixture<MpdFixture>
    {
        private MediaPresentationDescription mpd;

        public MediaPresentationDescriptionTests(MpdFixture mpdFixture)
        {
            this.mpd = mpdFixture.Mpd;
        }

        [Fact]
        public void Id()
        {
            Assert.Null(mpd.Id);
        }

        [Fact]
        public void Profiles()
        {
            Assert.Equal("urn:mpeg:dash:profile:isoff-live:2011", mpd.Profiles);
        }

        [Fact]
        public void Type()
        {
            Assert.Equal("static", mpd.Type);
        }

        [Fact]
        public void AvailabilityStartTime()
        {
            Assert.Equal(new DateTimeOffset(2016, 1, 20, 21, 10, 2, TimeSpan.Zero), mpd.AvailabilityStartTime);
        }

        [Fact]
        public void PublishTime()
        {
            Assert.Null(mpd.PublishTime);
        }

        [Fact]
        public void AvailabilityEndTime()
        {
            Assert.Null(mpd.AvailabilityEndTime);
        }

        [Fact]
        public void MediaPresentationDuration()
        {
            Assert.Equal(TimeSpan.FromSeconds(193.680), mpd.MediaPresentationDuration);
        }

        [Fact]
        public void MinimumUpdatePeriod()
        {
            Assert.Null(mpd.MinimumUpdatePeriod);
        }

        [Fact]
        public void MinBufferTime()
        {
            Assert.Equal(TimeSpan.FromSeconds(5), mpd.MinBufferTime);
        }

        [Fact]
        public void TimeShiftBufferDepth()
        {
            Assert.Null(mpd.TimeShiftBufferDepth);
        }

        [Fact]
        public void SuggestedPresentationDelay()
        {
            Assert.Null(mpd.SuggestedPresentationDelay);
        }

        [Fact]
        public void MaxSegmentDuration()
        {
            Assert.Equal(TimeSpan.FromMilliseconds(2005), mpd.MaxSegmentDuration);
        }

        [Fact]
        public void MaxSubsegmentDuration()
        {
            Assert.Null(mpd.MaxSubsegmentDuration);
        }

        [Fact]
        public void Periods_Count()
        {
            Assert.Equal(1, mpd.Periods.Count());
        }

        [Fact]
        public void Period_Id()
        {
            Assert.Equal("period0", mpd.Periods.First().Id);
        }

        [Fact]
        public void Period_Start()
        {
            Assert.Null(mpd.Periods.First().Start);
        }

        [Fact]
        public void Period_Duration()
        {
            Assert.Null(mpd.Periods.First().Duration);
        }

        [Fact]
        public void Period_BitstreamSwitching()
        {
            Assert.False(mpd.Periods.First().BitstreamSwitching);
        }
    }
}
