using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Qoollo.MpegDash.Mpd
{
    public class BaseUrl : MpdElement
    {
        public BaseUrl(XElement node) 
            : base(node)
        {
        }

        public string Value
        {
            get { return node.Value; }
        }
    }
}
