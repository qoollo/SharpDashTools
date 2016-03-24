using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace Qoollo.MpegDash
{
    public class Track
    {
        private readonly MpdAdaptationSet adaptationSet;

        public Track(MpdAdaptationSet adaptationSet)
        {
            this.adaptationSet = adaptationSet;

            trackRepresentations = new Lazy<IEnumerable<TrackRepresentation>>(GetTrackRepresentations);
        }

        public ContentType ContentType
        {
            get { return new ContentType(adaptationSet.ContentType); }
        }

        public IEnumerable<TrackRepresentation> TrackRepresentations
        {
            get { return trackRepresentations.Value; }
        }
        private readonly Lazy<IEnumerable<TrackRepresentation>> trackRepresentations;

        private IEnumerable<TrackRepresentation> GetTrackRepresentations()
        {
            return adaptationSet.Representations.Select(r => new TrackRepresentation(adaptationSet, r));
        }
    }
}