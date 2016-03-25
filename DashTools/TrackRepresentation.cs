using System;
using System.Collections.Generic;
using System.Linq;

namespace Qoollo.MpegDash
{
    public class TrackRepresentation
    {
        private readonly MpdAdaptationSet adaptationSet;
        private readonly MpdRepresentation representation;

        public TrackRepresentation(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
        {
            this.adaptationSet = adaptationSet;
            this.representation = representation;

            initFragmentPath = new Lazy<string>(GetInitFragmentPath);
            fragmentsPaths = new Lazy<IEnumerable<string>>(GetFragmentsPaths);
        }

        public string InitFragmentPath
        {
            get { return initFragmentPath.Value; }
        }
        private readonly Lazy<string> initFragmentPath;

        public IEnumerable<string> FragmentsPaths
        {
            get { return fragmentsPaths.Value; }
        }
        private readonly Lazy<IEnumerable<string>> fragmentsPaths;

        public uint Bandwidth
        {
            get { return representation.Bandwidth; }
        }

        private string GetInitFragmentPath()
        {
            string res;

            var segmentTemplate = adaptationSet.SegmentTemplate ?? representation.SegmentTemplate;
            if (segmentTemplate != null)
                res = segmentTemplate.Initialization
                    .Replace("$RepresentationID$", representation.Id);
            else if (representation.SegmentList != null)
                res = representation.SegmentList.Initialization.SourceUrl;
            else
                throw new Exception("Failed to determine InitFragmentPath");

            return res;
        }

        private IEnumerable<string> GetFragmentsPaths()
        {
            var segmentTemplate = adaptationSet.SegmentTemplate ?? representation.SegmentTemplate;
            if (segmentTemplate != null)
            {
                int i = 1;
                while (true)
                {
                    yield return segmentTemplate.Media
                        .Replace("$RepresentationID$", representation.Id)
                        .Replace("$Number$", i.ToString());
                    i++;
                }
            }
            else if (representation.SegmentList != null)
            {
                foreach (var segmentUrl in representation.SegmentList.SegmentUrls.OrderBy(s => s.Index))
                {
                    yield return segmentUrl.Media;
                }
            }
            else
                throw new Exception("Failed to determine FragmentPath");
        }
    }
}