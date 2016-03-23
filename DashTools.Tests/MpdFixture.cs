using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.MpegDash.Tests
{
    public class MpdFixture : IDisposable
    {
        public MpdFixture()
        {
            Stream = File.OpenRead("envivio.mpd");
            Mpd = new MediaPresentationDescription(Stream);
        }

        public void Dispose()
        {
            Stream.Dispose();
        }

        public Stream Stream { get; private set; }

        public MediaPresentationDescription Mpd { get; private set; }
    }
}
