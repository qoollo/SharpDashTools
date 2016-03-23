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

        [Fact]
        public void Period_AdaptationSets_Count()
        {
            Assert.Equal(2, mpd.Periods.First().AdaptationSets.Count());
        }

        [Fact]
        public void Period_AdaptationSets_0_Id()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().Id);
        }

        [Fact]
        public void Period_AdaptationSets_0_Group()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().Group);
        }

        [Fact]
        public void Period_AdaptationSets_0_Lang()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().Lang);
        }

        [Fact]
        public void Period_AdaptationSets_0_ContentType()
        {
            Assert.Equal("video/mp4", mpd.Periods.First().AdaptationSets.First().ContentType);
        }

        [Fact]
        public void Period_AdaptationSets_0_Par()
        {
            Assert.Equal("1:1", mpd.Periods.First().AdaptationSets.First().Par.RawValue);
        }

        [Fact]
        public void Period_AdaptationSets_0_MinBandwidth()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().MinBandwidth);
        }

        [Fact]
        public void Period_AdaptationSets_0_MaxBandwidth()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().MaxBandwidth);
        }

        [Fact]
        public void Period_AdaptationSets_0_MinWidth()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().MinWidth);
        }

        [Fact]
        public void Period_AdaptationSets_0_MaxWidth()
        {
            Assert.Equal(1920u, mpd.Periods.First().AdaptationSets.First().MaxWidth);
        }

        [Fact]
        public void Period_AdaptationSets_0_MinHeight()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().MinHeight);
        }

        [Fact]
        public void Period_AdaptationSets_0_MaxHeight()
        {
            Assert.Equal(1080u, mpd.Periods.First().AdaptationSets.First().MaxHeight);
        }

        [Fact]
        public void Period_AdaptationSets_0_MinFrameRate()
        {
            Assert.Null(mpd.Periods.First().AdaptationSets.First().MinFrameRate);
        }

        [Fact]
        public void Period_AdaptationSets_0_MaxFrameRate()
        {
            Assert.Equal("30000/1001", mpd.Periods.First().AdaptationSets.First().MaxFrameRate.RawValue);
        }

        [Fact]
        public void Period_AdaptationSets_0_SegmentAlignment()
        {
            Assert.True(mpd.Periods.First().AdaptationSets.First().SegmentAlignment);
        }

        [Fact]
        public void Period_AdaptationSets_0_BitstreamSwitching()
        {
            Assert.False(mpd.Periods.First().AdaptationSets.First().BitstreamSwitching);
        }

        [Fact]
        public void Period_AdaptationSets_0_SubsegmentAlignment()
        {
            Assert.False(mpd.Periods.First().AdaptationSets.First().SubsegmentAlignment);
        }

        [Fact]
        public void Period_AdaptationSets_0_SubsegmentStartsWithSAP()
        {
            Assert.Equal(1u, mpd.Periods.First().AdaptationSets.First().SubsegmentStartsWithSAP);
        }
    }
}
