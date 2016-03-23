﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.MpegDash
{
    public class MpdDownloader
    {
        private readonly MediaPresentationDescription mpd;

        private readonly Uri mpdUrl;

        private readonly string destinationDir;

        public MpdDownloader(MediaPresentationDescription mpd, Uri mpdUrl, string destinationDir)
        {
            this.mpd = mpd;
            this.mpdUrl = mpdUrl;
            this.destinationDir = destinationDir;
        }

        public Task Download()
        {
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            string mpdFileName = mpdUrl.AbsolutePath;
            if (mpdFileName.Contains("/"))
                mpdFileName = mpdFileName.Substring(mpdFileName.LastIndexOf("/") + 1);
            mpd.Save(Path.Combine(destinationDir, mpdFileName));

            var tasks = new List<Task>();

            foreach (var period in mpd.Periods)
            {
                foreach (var adaptationSet in period.AdaptationSets)
                {
                    foreach (var representation in adaptationSet.Representations)
                    {
                        tasks.Add(DownloadAllFragments(adaptationSet, representation));
                    }
                }
            }

            return Task.Factory.ContinueWhenAll(
                tasks.ToArray(),
                completed => { });
        }

        private Task DownloadAllFragments(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
        {
            return Task.Factory.StartNew(() => DownloadFragmentsUntilFirstFailure(adaptationSet, representation));
        }

        private void DownloadFragmentsUntilFirstFailure(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
        {
            var task = DownloadRepresentationInitFragment(adaptationSet, representation);

            if (TaskSucceded(task))
            {
                int i = 1;
                do
                {
                    task = DownloadRepresentationFragment(adaptationSet, representation, i);
                    i++;
                }
                while (TaskSucceded(task));
            }
        }

        private Task<bool> DownloadRepresentationInitFragment(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
        {
            string initUrl = adaptationSet.SegmentTemplate.Initialization
                .Replace("$RepresentationID$", representation.Id);
            var task = DownloadFragment(initUrl);
            task.Wait(TimeSpan.FromMinutes(5));

            return task;
        }

        private Task<bool> DownloadRepresentationFragment(MpdAdaptationSet adaptationSet, MpdRepresentation representation, int index)
        {
            string fragmentUrl = adaptationSet.SegmentTemplate.Media
                        .Replace("$RepresentationID$", representation.Id)
                        .Replace("$Number$", index.ToString());

            var task = DownloadFragment(fragmentUrl);
            task.Wait(TimeSpan.FromMinutes(5));

            return task;
        }

        private Task<bool> DownloadFragment(string fragmentUrl)
        {
            using (var client = new WebClient())
            {
                var url = new Uri(mpdUrl, fragmentUrl);
                return Task.Factory.StartNew(() =>
                {
                    try
                    {
                        client.DownloadFile(url, Path.Combine(destinationDir, fragmentUrl));
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
        }

        private bool TaskSucceded(Task<bool> task)
        {
            return task.IsCompleted && !task.IsFaulted && task.Result;
        }
    }
}