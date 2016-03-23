using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MediaPresentationDescription
    {
        private readonly Stream stream;

        private readonly Lazy<XElement> mpdTag;

        public MediaPresentationDescription(Stream mpdStream)
        {
            stream = mpdStream;

            mpdTag = new Lazy<XElement>(ReadMpdTag);
        }

        public string MinBufferTime
        {
            get { return mpdTag.Value.Attribute("minBufferTime").Value; }
        }

        private XElement ReadMpdTag()
        {
            using (var reader = XmlReader.Create(stream))
            {
                reader.ReadToFollowing("MPD");
                return XNode.ReadFrom(reader) as XElement;
            }
        }
    }
}
