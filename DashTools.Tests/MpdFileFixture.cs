using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.MpegDash.Tests
{
    public class MpdFileFixture : IDisposable
    {
        public MpdFileFixture()
        {
            Stream = File.OpenRead("envivio.mpd");
        }

        public void Dispose()
        {
            Stream.Dispose();
        }

        public Stream Stream { get; private set; }
    }
}
