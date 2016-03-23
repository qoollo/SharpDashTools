using System;
using System.Linq;
using Xunit;

namespace Qoollo.MpegDash.Tests
{
    public class MpdDownloaderTests : IClassFixture<MpdFixture>
    {
        private MediaPresentationDescription mpd;

        public MpdDownloaderTests(MpdFixture mpdFixture)
        {
            this.mpd = mpdFixture.Mpd;
        }

        [Fact]
        public void Download()
        {
            //  arrange
            var downloader = new MpdDownloader(mpd, new Uri("http://dash.edgesuite.net/envivio/EnvivioDash3/manifest.mpd"), "envivio");

            //  act
            var task = downloader.Download();
            task.Wait();

            //  assert
        }
    }
}
