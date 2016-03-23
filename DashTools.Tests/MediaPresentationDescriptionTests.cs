using System;
using Xunit;

namespace Qoollo.MpegDash.Tests
{
    public class MediaPresentationDescriptionTests : IClassFixture<MpdFileFixture>
    {
        private MpdFileFixture mpdFile;

        public MediaPresentationDescriptionTests(MpdFileFixture mpdFile)
        {
            this.mpdFile = mpdFile;
        }

        [Fact]
        public void MinBufferTime()
        {
            //  arrange
            var mpd = new MediaPresentationDescription(mpdFile.Stream);

            //  act
            string actual = mpd.MinBufferTime;

            //  assert
            Assert.Equal("PT5.000S", actual);
        }
    }
}
