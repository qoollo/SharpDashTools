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
            else if (representation.BaseURL != null)
                res = representation.BaseURL;
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

        private IEnumerable<TrackRepresentationSegment> GetSegments()
        {
            var segmentTemplate = adaptationSet.SegmentTemplate ?? representation.SegmentTemplate;
            if (segmentTemplate != null)
            {
                var segments = GetSegmentsFromTimeline(segmentTemplate);

                bool hasTimelineItems = false;
                foreach (var segment in segments)
                {
                    hasTimelineItems = true;

                    yield return segment;
                }

                if (!hasTimelineItems)
                {
                    segments = GetSegmentsFromRepresentation(representation);
                    foreach (var segment in segments)
                    {
                        yield return segment;
                    }
                }

            }
            else if (representation.SegmentList != null)
            {
                foreach (var segmentUrl in representation.SegmentList.SegmentUrls.OrderBy(s => s.Index))
                {
                    yield return new TrackRepresentationSegment
                    {
                        Path = segmentUrl.Media,
                        Duration = TimeSpan.FromMilliseconds(representation.SegmentList.Duration.Value)
                    };
                }
            }
            else
                // no segments (it's possible)
                yield break;
        }

        private IEnumerable<TrackRepresentationSegment> GetSegmentsFromRepresentation(MpdRepresentation representation)
        {
            int i = 1;
            while (true)
            {
                yield return new TrackRepresentationSegment
                {
                    Path = representation.SegmentTemplate.Media
                        .Replace("$RepresentationID$", representation.Id)
                        .Replace("$Number$", i.ToString()),
                    Duration = TimeSpan.FromMilliseconds(representation.SegmentTemplate.Duration.Value)
                };
                i++;
            }
        }

        private IEnumerable<TrackRepresentationSegment> GetSegmentsFromTimeline(MpdSegmentTemplate segmentTemplate)
        {
            int i = 1;
            foreach (var item in segmentTemplate.SegmentTimeline)
            {
                int count = Math.Max(1, item.RepeatCount);
                for (int j = 0; j < count; j++)
                {
                    yield return new TrackRepresentationSegment
                    {
                        Path = segmentTemplate.Media
                            .Replace("$RepresentationID$", representation.Id)
                            .Replace("$Number$", i.ToString()),
                        Duration = TimeSpan.FromMilliseconds(item.Duration)
                    };
                    i++;
                }
            }
        }
        
        internal IEnumerable<string> GetFragmentsPaths(TimeSpan from, TimeSpan to)
        {
            var span = TimeSpan.Zero;
            foreach (var segment in GetSegments())
            {
                if (span >= from && span + segment.Duration <= to)
                    yield return segment.Path;

                span += segment.Duration;

                if (span > to)
                    break;
            }
        }
    }
}