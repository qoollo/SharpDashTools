using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.MpegDash.Samples
{
    class Program
    {
        private static readonly string[] mpdFiles = new string[]
        {
            "http://ncplusgo.s43-po.live.e56-po.insyscd.net/out/u/eskatvsd.mpd",
            "http://dash.edgesuite.net/envivio/EnvivioDash3/manifest.mpd",
            "http://dash.edgesuite.net/dash264/TestCases/1a/netflix/exMPD_BIP_TC1.mpd",
            "http://10.5.5.7/q/p/userapi/streams/32/mpd",
            "http://10.5.7.207/userapi/streams/30/mpd",
            "http://10.5.7.207/userapi/streams/11/mpd?start_time=1458816642&stop_time=1458819642",
        };

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await MainAsync(args);
            }).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            string dir = "envivio";
            string mpdUrl = mpdFiles[0];
            var from = TimeSpan.Zero;
            var to = TimeSpan.MaxValue;
            var stopwatch = Stopwatch.StartNew();
            
            var downloader = new MpdDownloader(new Uri(mpdUrl), dir);
            var trackRepresentation = downloader.GetTracksFor(TrackContentType.Video).First().TrackRepresentations.OrderByDescending(r => r.Bandwidth).First();
            var prepareTime = stopwatch.Elapsed;

            var chunks =
                //Directory.GetFiles(@"C:\Users\Alexander\Work\Github\qoollo\SharpDashTools\DashTools.Samples\bin\Debug\envivio", "*.mp4")
                //.Select(f => Path.GetFileName(f) == "make_chunk_path_longer_32.mp4"
                //    ? new Mp4InitFile(Path.Combine("envivio", Path.GetFileName(f)))
                //    : new Mp4File(Path.Combine("envivio", Path.GetFileName(f))))
                //    .ToArray();
                await downloader.Download(trackRepresentation, from, to);
            var downloadTime = stopwatch.Elapsed - prepareTime;

            var ffmpeg = new FFMpegConverter();
            ffmpeg.LogReceived += (s, e) => Console.WriteLine(e.Data);
            var combined = downloader.CombineChunksFast(chunks, s => ffmpeg.Invoke(s));
            //downloader.CombineChunks(chunks, s => ffmpeg.Invoke(s));
            var combineTime = stopwatch.Elapsed - prepareTime - downloadTime;

            if (!ffmpeg.Stop())
                ffmpeg.Abort();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("================================================================");
            Console.WriteLine("Prepared in {0} s", prepareTime.TotalSeconds);
            Console.WriteLine("Downloaded {0} chunks in {1} s", chunks.Count(), downloadTime.TotalSeconds);
            Console.WriteLine("Combined in {0} s", combineTime.TotalSeconds);
            Console.WriteLine("Total: {0} s", (prepareTime + downloadTime + combineTime).TotalSeconds);
            Console.ReadLine();

            return;

            string mpdFilePath = //@"C:\Users\Alexander\AppData\Local\Temp\note_5_video_5_source_2-index_00000\manifest.mpd";
                await DownloadMpdStreams(mpdUrl, dir);
            await ConcatStreams(mpdFilePath, Path.Combine(Path.GetDirectoryName(mpdFilePath), "output.mp4"));
        }

        private static async Task<string> DownloadMpdStreams(string url, string destinationDir)
        {
            var dir = new DirectoryInfo(destinationDir);
            if (dir.Exists)
                dir.Delete(recursive: true);
            var downloader = new MpdDownloader(new Uri(url), dir.FullName);

            return await downloader.Download();
        }

        private static async Task ConcatStreams(string mpdFilePath, string outputFile)
        {
            var mpd = new MediaPresentationDescription(mpdFilePath);
            var walker = new MpdWalker(mpd);
            var track = walker.GetTracksFor(TrackContentType.Video).First();
            var trackRepresentation = track.TrackRepresentations.OrderByDescending(r => r.Bandwidth).First();

            using (var stream = File.OpenWrite(outputFile))
            using (var writer = new BinaryWriter(stream))
            {
                string fragmentPath = Path.Combine(Path.GetDirectoryName(mpdFilePath), trackRepresentation.InitFragmentPath);
                writer.Write(File.ReadAllBytes(fragmentPath));

                foreach (var path in trackRepresentation.FragmentsPaths)
                {
                    fragmentPath = Path.Combine(Path.GetDirectoryName(mpdFilePath), path);
                    if (!File.Exists(fragmentPath))
                        break;
                    writer.Write(File.ReadAllBytes(fragmentPath));
                }
            }

            string[] files = new[]
            {
                "v1_257-Header.m4s",
                "v1_257-270146-i-1.m4s",
                "v1_257-270146-i-2.m4s",
                "v1_257-270146-i-3.m4s",
                "v1_257-270146-i-4.m4s",
                "v1_257-270146-i-5.m4s",
                "v1_257-270146-i-6.m4s",
                "v1_257-270146-i-7.m4s",
                "v1_257-270146-i-8.m4s",
                "v1_257-270146-i-9.m4s",
                "v1_257-270146-i-10.m4s",
                "v1_257-270146-i-11.m4s",
                "v1_257-270146-i-12.m4s",
                "v1_257-270146-i-13.m4s",
                "v1_257-270146-i-14.m4s",
                "v1_257-270146-i-15.m4s",
                "v1_257-270146-i-16.m4s",
                "v1_257-270146-i-17.m4s",
                "v1_257-270146-i-18.m4s",
                "v1_257-270146-i-19.m4s",
                "v1_257-270146-i-20.m4s",
                "v1_257-270146-i-21.m4s",
                "v1_257-270146-i-22.m4s",
                "v1_257-270146-i-23.m4s",
                "v1_257-270146-i-24.m4s",
                "v1_257-270146-i-25.m4s",
                "v1_257-270146-i-26.m4s",
                "v1_257-270146-i-27.m4s",
                "v1_257-270146-i-28.m4s",
                "v1_257-270146-i-29.m4s",
                "v1_257-270146-i-30.m4s",
                "v1_257-270146-i-31.m4s",
                "v1_257-270146-i-32.m4s",
                "v1_257-270146-i-33.m4s",
                "v1_257-270146-i-34.m4s",
                "v1_257-270146-i-35.m4s",
                "v1_257-270146-i-36.m4s",
                "v1_257-270146-i-37.m4s",
                "v1_257-270146-i-38.m4s",
                "v1_257-270146-i-39.m4s",
                "v1_257-270146-i-40.m4s",
                "v1_257-270146-i-41.m4s",
                "v1_257-270146-i-42.m4s",
                "v1_257-270146-i-43.m4s",
                "v1_257-270146-i-44.m4s",
                "v1_257-270146-i-45.m4s",
                "v1_257-270146-i-46.m4s",
                "v1_257-270146-i-47.m4s",
                "v1_257-270146-i-48.m4s",
                "v1_257-270146-i-49.m4s",
                "v1_257-270146-i-50.m4s",
                "v1_257-270146-i-51.m4s",
                "v1_257-270146-i-52.m4s",
                "v1_257-270146-i-53.m4s",
                "v1_257-270146-i-54.m4s",
                "v1_257-270146-i-55.m4s",
                "v1_257-270146-i-56.m4s",
                "v1_257-270146-i-57.m4s",
                "v1_257-270146-i-58.m4s",
                "v1_257-270146-i-59.m4s",
                "v1_257-270146-i-60.m4s",
                "v1_257-270146-i-61.m4s",
                "v1_257-270146-i-62.m4s",
                "v1_257-270146-i-63.m4s",
                "v1_257-270146-i-64.m4s",
                "v1_257-270146-i-65.m4s",
                "v1_257-270146-i-66.m4s",
                "v1_257-270146-i-67.m4s",
                "v1_257-270146-i-68.m4s",
                "v1_257-270146-i-69.m4s",
                "v1_257-270146-i-70.m4s",
                "v1_257-270146-i-71.m4s",
                "v1_257-270146-i-72.m4s",
                "v1_257-270146-i-73.m4s",
                "v1_257-270146-i-74.m4s",
                "v1_257-270146-i-75.m4s",
                "v1_257-270146-i-76.m4s",
                "v1_257-270146-i-77.m4s",
                "v1_257-270146-i-78.m4s",
                "v1_257-270146-i-79.m4s",
                "v1_257-270146-i-80.m4s",
                "v1_257-270146-i-81.m4s",
                "v1_257-270146-i-82.m4s",
                "v1_257-270146-i-83.m4s",
                "v1_257-270146-i-84.m4s",
                "v1_257-270146-i-85.m4s",
                "v1_257-270146-i-86.m4s",
                "v1_257-270146-i-87.m4s",
                "v1_257-270146-i-88.m4s",
                "v1_257-270146-i-89.m4s",
                "v1_257-270146-i-90.m4s",
                "v1_257-270146-i-91.m4s",
                "v1_257-270146-i-92.m4s",
                "v1_257-270146-i-93.m4s",
                "v1_257-270146-i-94.m4s",
                "v1_257-270146-i-95.m4s",
                "v1_257-270146-i-96.m4s",
                "v1_257-270146-i-97.m4s",
            };
            //string outputFile = Path.Combine(Path.GetDirectoryName(mpdFilePath), "output.mp4");
            //using (var stream = File.OpenWrite(outputFile))
            //using (var writer = new BinaryWriter(stream))
            //{
            //    files.Select(f => Path.Combine(Path.GetDirectoryName(mpdFilePath), f))
            //        .ToList()
            //        .ForEach(f => writer.Write(File.ReadAllBytes(f)));
            //}
        }
    }
}
