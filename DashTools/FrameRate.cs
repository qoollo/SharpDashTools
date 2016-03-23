using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qoollo.MpegDash
{
    public class FrameRate
    {
        public FrameRate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            RawValue = value;
        }

        public string RawValue { get; }
    }
}
