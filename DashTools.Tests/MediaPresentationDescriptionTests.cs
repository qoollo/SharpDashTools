using System;
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
        public void MinBufferTime()
        {
            Assert.Equal(TimeSpan.FromSeconds(5), mpd.MinBufferTime);
        }

        [Fact]
        public void MaxSegmentDuration()
        {
            Assert.Equal(TimeSpan.FromMilliseconds(2005), mpd.MaxSegmentDuration);
        }

        [Fact]
        public void AvailabilityStartTime()
        {
            Assert.Equal(new DateTimeOffset(2016, 1, 20, 21, 10, 2, TimeSpan.Zero), mpd.AvailabilityStartTime);
        }

        [Fact]
        public void MediaPresentationDuration()
        {
            Assert.Equal(TimeSpan.FromSeconds(193.680), mpd.MediaPresentationDuration);
        }
    }
}
