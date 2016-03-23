using System;

namespace Qoollo.MpegDash
{
    public class AspectRatio
    {
        public AspectRatio(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            RawValue = value;
            X = double.Parse(value.Split(':')[0]);
            Y = double.Parse(value.Split(':')[1]);
        }

        public double X { get; }

        public double Y { get; }

        public string RawValue { get; }
    }
}