using System;
using System.Collections.Generic;

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

        public uint Badwidth
        {
            get { return representation.Bandwidth; }
        }

        private readonly Lazy<IEnumerable<string>> fragmentsPaths;

        private string GetInitFragmentPath()
        {
            return adaptationSet.SegmentTemplate.Initialization
               .Replace("$RepresentationID$", representation.Id);
        }

        private IEnumerable<string> GetFragmentsPaths()
        {
            int i = 1;
            while (true)
            {
                yield return adaptationSet.SegmentTemplate.Media
                            .Replace("$RepresentationID$", representation.Id)
                            .Replace("$Number$", i.ToString());
                i++;
            }
        }
    }
}