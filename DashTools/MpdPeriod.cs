using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qoollo.MpegDash
{
    public class MpdPeriod : MpdElement
    {
        internal MpdPeriod(XElement node)
            : base(node)
        {
            this.adaptationSets = new Lazy<IEnumerable<MpdAdaptationSet>>(ParseAdaptationSets);
        }

        public string Id
        {
            get { return node.Attribute("id")?.Value; }
        }

        public TimeSpan? Start
        {
            get { return helper.ParseOptionalTimeSpan("start"); }
        }

        public TimeSpan? Duration
        {
            get { return helper.ParseOptionalTimeSpan("duration"); }
        }

        public bool BitstreamSwitching
        {
            get { return helper.ParseOptionalBool("bitstreamSwitching", false); }
        }

        public IEnumerable<MpdAdaptationSet> AdaptationSets
        {
            get { return adaptationSets.Value; }
        }
        private readonly Lazy<IEnumerable<MpdAdaptationSet>> adaptationSets;

        private IEnumerable<MpdAdaptationSet> ParseAdaptationSets()
        {
            return node.Elements()
                .Where(n => n.Name.LocalName == "AdaptationSet")
                .Select(n => new MpdAdaptationSet(n));
        }
    }
}