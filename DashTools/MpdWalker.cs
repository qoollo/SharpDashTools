using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qoollo.MpegDash
{
    public class MpdWalker
    {
        private readonly MediaPresentationDescription mpd;

        public MpdWalker(MediaPresentationDescription mpd)
        {
            this.mpd = mpd;
        }

        public IEnumerable<Uri> GetAllFragmentsUrls()
        {
            var res = new List<Uri>();
            
            //mpd.Periods
            //    .SelectMany(p => p.AdaptationSets)
            //    .SelectMany(a => a.

            return res;
        }

        /// <summary>
        /// https://github.com/Dash-Industry-Forum/dash.js/blob/ea24350b2df53be20d566603a401fb59bdfae416/src/dash/models/DashManifestModel.js
        /// </summary>
        /// <returns></returns>
        //private IEnumerable<Period> GetRegularPeriods()
        //{
        //    var periods = mpd.Periods
        //        .Select(p => new Period { Id = p.Id, Start = p.Start.Value, Duration = p.Duration.Value })
        //        .ToList();
        //    Period vo1 = null, vo = null, p1 = null;

        //    for (int i = 0; i < periods.Count; i++)
        //    {
        //        var p = periods[i];

        //        // If the attribute @start is present in the Period, then the
        //        // Period is a regular Period and the PeriodStart is equal
        //        // to the value of this attribute.
        //        if (p.Start.HasValue)
        //        {
        //            vo = new Period { Start = p.Start.Value };
        //        }
        //        // If the @start attribute is absent, but the previous Period
        //        // element contains a @duration attribute then then this new
        //        // Period is also a regular Period. The start time of the new
        //        // Period PeriodStart is the sum of the start time of the previous
        //        // Period PeriodStart and the value of the attribute @duration
        //        // of the previous Period.
        //        else if (p1 != null && p.Duration.HasValue && vo1 != null)
        //        {
        //            vo = new Period();
        //            vo.Start = vo1.Start + vo1.Duration;
        //            vo.Duration = p.Duration.Value;
        //        }
        //        // If (i) @start attribute is absent, and (ii) the Period element
        //        // is the first in the MPD, and (iii) the MPD@type is 'static',
        //        // then the PeriodStart time shall be set to zero.
        //        else if (i == 0 && mpd.Type != "dynamic")
        //        {
        //            vo = new Period();
        //            vo.Start = TimeSpan.Zero;
        //        }

        //        // The Period extends until the PeriodStart of the next Period.
        //        // The difference between the PeriodStart time of a Period and
        //        // the PeriodStart time of the following Period.
        //        if (vo1 != null && vo1.Duration == default(TimeSpan))
        //        {
        //            vo1.Duration = vo.Start - vo1.Start;
        //        }

        //        if (vo != null)
        //        {
        //            vo.Id = GetPeriodId(p);
        //        }

        //        if (vo != null && p.Duration.HasValue)
        //        {
        //            vo.Duration = p.Duration.Value;
        //        }

        //        if (vo != null)
        //        {
        //            vo.index = i;
        //            vo.mpd = mpd;
        //            periods.Add(vo);
        //            p1 = p;
        //            vo1 = vo;
        //        }

        //        p = null;
        //        vo = null;
        //    }

        //    if (periods.Any() && vo1 != null && !vo1.Duration.HasValue)
        //        vo1.Duration = GetEndTimeForLastPeriod(vo1) - vo1.Start.Value;

        //    return periods;
        //}

        //private TimeSpan GetEndTimeForLastPeriod(Period period)
        //{
        //    var periodEnd;
        //    var checkTime = GetCheckTime(period);

        //    // if the MPD@mediaPresentationDuration attribute is present, then PeriodEndTime is defined as the end time of the Media Presentation.
        //    // if the MPD@mediaPresentationDuration attribute is not present, then PeriodEndTime is defined as FetchTime + MPD@minimumUpdatePeriod

        //    if (manifest.mediaPresentationDuration)
        //    {
        //        periodEnd = manifest.mediaPresentationDuration;
        //    }
        //    else if (!isNaN(checkTime))
        //    {
        //        // in this case the Period End Time should match CheckTime
        //        periodEnd = checkTime;
        //    }
        //    else {
        //        throw new Error('Must have @mediaPresentationDuration or @minimumUpdatePeriod on MPD or an explicit @duration on the last period.');
        //    }

        //    return periodEnd;
        //}

        //private object GetCheckTime(Period period)
        //{
        //    var checkTime = NaN;
        //    var fetchTime;

        //    // If the MPD@minimumUpdatePeriod attribute in the client is provided, then the check time is defined as the
        //    // sum of the fetch time of this operating MPD and the value of this attribute,
        //    // i.e. CheckTime = FetchTime + MPD@minimumUpdatePeriod.
        //    if (mpd.MinimumUpdatePeriod.HasValue)
        //    {
        //        fetchTime = GetFetchTime(period);
        //        checkTime = fetchTime + manifest.minimumUpdatePeriod;
        //    }
        //    // TODO If the MPD@minimumUpdatePeriod attribute in the client is not provided, external means are used to
        //    // determine CheckTime, such as a priori knowledge, or HTTP cache headers, etc.

        //    return checkTime;
        //}

        //private object GetFetchTime(Period period)
        //{
        //    // FetchTime is defined as the time at which the server processes the request for the MPD from the client.
        //    // TODO The client typically should not use the time at which it actually successfully received the MPD, but should
        //    // take into account delay due to MPD delivery and processing. The fetch is considered successful fetching
        //    // either if the client obtains an updated MPD or the client verifies that the MPD has not been updated since the previous fetching.

        //    return timelineConverter.calcPresentationTimeFromWallTime(mpd.loadedTime, period);
        //}

        private string GetPeriodId(Period period)
        {
            if (period == null)
                throw new ArgumentNullException(nameof(period));

            var res = Period.DEFAULT_ID;

            if (!string.IsNullOrWhiteSpace(period.Id))
            {
                res = period.Id;
            }

            return res;
        }

        class Period
        {
            public const string DEFAULT_ID = "defaultId";

            public string Id { get; set; }

            public TimeSpan? Start { get; set; }

            public TimeSpan? Duration { get; set; }
        }
    }
}
