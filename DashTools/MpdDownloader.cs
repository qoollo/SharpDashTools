using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.MpegDash
{
    public class MpdDownloader : IDisposable
    {
        private readonly Uri mpdUrl;

        private readonly string destinationDir;

        private readonly Lazy<string> mpdFileName;

        private readonly Lazy<MediaPresentationDescription> mpd;

        private readonly Lazy<MpdWalker> walker;

        private readonly int downloadConcurrency;

        public MpdDownloader(Uri mpdUrl, string destinationDir, int downloadConcurrency = 2)
        {
            if (downloadConcurrency < 1)
                throw new ArgumentException("downloadConcurrency cannot be less than 1.", "downloadConcurrency");

            this.mpdUrl = mpdUrl;
            this.destinationDir = destinationDir;
            this.downloadConcurrency = downloadConcurrency;

            mpdFileName = new Lazy<string>(GetMpdFileName);
            mpd = new Lazy<MediaPresentationDescription>(DownloadMpd);
            walker = new Lazy<MpdWalker>(CreateMpdWalker);
        }

        public IEnumerable<Track> GetTracksFor(TrackContentType type)
        {
            return walker.Value.GetTracksFor(type);
        }

        public Task<IEnumerable<Mp4File>> Download(TrackRepresentation trackRepresentation)
        {
            return Task.Factory.StartNew(() => DownloadTrackRepresentation(trackRepresentation, TimeSpan.Zero, TimeSpan.MaxValue));
        }

        public Task<IEnumerable<Mp4File>> Download(TrackRepresentation trackRepresentation, TimeSpan from, TimeSpan to)
        {
            return Task.Factory.StartNew(() => DownloadTrackRepresentation(trackRepresentation, from, to));
        }

        public FileInfo CombineChunksFast(IEnumerable<Mp4File> chunks, Action<string> ffmpegRunner)
        {
            string concatFile = Path.Combine(Path.GetDirectoryName(chunks.First().Path), string.Format("{0:yyyyMMddHHmmssfffffff}_concat.mp4", DateTime.Now));
            string outFile = concatFile.Replace("_concat.mp4", "_video.mp4");

            if (File.Exists(concatFile))
                File.Delete(concatFile);
            if (File.Exists(outFile))
                File.Delete(outFile);

            var initFile = chunks.OfType<Mp4InitFile>().First();
            var files = chunks.ToList();
            files.Remove(initFile);
            files.Insert(0, initFile);

            ConcatFiles(files.Select(f => f.Path), concatFile);

            ffmpegRunner(string.Format("-i \"concat:{0}\" -c copy {1}", ConvertPathForFfmpeg(concatFile), ConvertPathForFfmpeg(outFile)));

            return new FileInfo(outFile);
        }

        private void ConcatFiles(IEnumerable<string> files, string outFile)
        {
            using (var stream = File.OpenWrite(outFile))
            {
                foreach (var f in files)
                {
                    var bytes = File.ReadAllBytes(f);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public FileInfo CombineChunksFastOld(IEnumerable<Mp4File> chunks, Action<string> ffmpegRunner, int maxCmdLength = 32672)
        {
            var cmdBuilder = new StringBuilder(maxCmdLength);
            var initFile = chunks.OfType<Mp4InitFile>().First();
            var files = chunks.Except(new[] { initFile }).ToList();

            cmdBuilder.AppendFormat("-i \"concat:{0}", ConvertPathForFfmpeg(initFile.Path));
            string outputFile = Path.Combine(Path.GetDirectoryName(chunks.First().Path), string.Format("{0:yyyyMMddHHmmssfffffff}_combined.mp4", DateTime.Now));
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            string cmdEnd = "\" -c copy " + ConvertPathForFfmpeg(outputFile);

            bool overflow = false;
            while (!overflow && files.Any())
            {
                string toAppend = "|" + ConvertPathForFfmpeg(files[0].Path);
                if (cmdBuilder.Length + toAppend.Length + cmdEnd.Length > maxCmdLength)
                    overflow = true;
                else
                {
                    cmdBuilder.Append(toAppend);
                    files.RemoveAt(0);
                }
            }
            cmdBuilder.Append(cmdEnd);

            ffmpegRunner(cmdBuilder.ToString());
            cmdBuilder.Clear();

            FileInfo res;
            if (files.Any())
            {
                files.Insert(0, new Mp4InitFile(outputFile));
                res = CombineChunksFastOld(files, ffmpegRunner, maxCmdLength);
            }
            else
                res = new FileInfo(outputFile);

            return res;
        }

        public FileInfo CombineChunks(IEnumerable<Mp4File> chunks, Action<string> ffmpegRunner)
        {
            chunks = ProcessChunks(chunks);

            string tempFile = Path.Combine(Path.GetDirectoryName(chunks.First().Path), string.Format("{0:yyyyMMddHHmmss}_temp.mp4", DateTime.Now));
            foreach (var c in chunks)
            {
                ffmpegRunner(string.Format(
                    @"-i ""{0}"" -filter:v ""setpts=PTS-STARTPTS"" -f mp4 ""{1}""",
                    ConvertPathForFfmpeg(c.Path),
                    ConvertPathForFfmpeg(tempFile)));
                File.Delete(c.Path);
                File.Move(tempFile, c.Path);
            }
            File.Delete(tempFile);

            string filesListFile = Path.Combine(Path.GetDirectoryName(chunks.First().Path), string.Format("{0:yyyyMMddHHmmss}_list.txt", DateTime.Now));
            File.WriteAllText(filesListFile, string.Join("", chunks.Select(c => string.Format("file '{0}'\r\n", Path.GetFileName(c.Path)))));
            string outFile = Path.Combine(Path.GetDirectoryName(chunks.First().Path), string.Format("{0:yyyyMMddHHmmss}_combined.mp4", DateTime.Now));
            if (File.Exists(outFile))
                File.Delete(outFile);
            ffmpegRunner(string.Format(
                @"-f concat -i ""{0}"" -c copy ""{1}""",
                ConvertPathForFfmpeg(filesListFile),
                ConvertPathForFfmpeg(outFile)));
            File.Delete(filesListFile);

            return new FileInfo(outFile);
        }

        private string ConvertPathForFfmpeg(string path)
        {
            return path.Replace("\\", "/");
        }

        private IEnumerable<Mp4File> DownloadTrackRepresentation(TrackRepresentation trackRepresentation, TimeSpan from, TimeSpan to, int concurrency = 2)
        {
            string initFile;
            var files = new List<string>();

            var task = DownloadFragment(trackRepresentation.InitFragmentPath);
            task.Wait(TimeSpan.FromMinutes(5));
            if (DownloadTaskSucceded(task))
            {
                initFile = task.Result;

                bool complete = false;
                int downloadedCount = 0;
                while (!complete)
                {
                    var tasks = trackRepresentation.GetFragmentsPaths(from, to)
                        .Skip(downloadedCount)
                        .Take(concurrency)
                        .Select(p => DownloadFragment(p))
                        .ToList();
                    tasks.ForEach(t => t.Wait(TimeSpan.FromMinutes(5)));
                    var succeeded = tasks.Where(t => DownloadTaskSucceded(t)).ToList();
                    succeeded.ForEach(t => files.Add(t.Result));

                    downloadedCount += succeeded.Count;
                    complete = !tasks.Any() || succeeded.Count != tasks.Count;
                }

                //var chunks = ProcessChunks(initFile, files);
                var chunks = files.Select(f => new Mp4File(f)).ToList();
                chunks.Insert(0, new Mp4InitFile(initFile));

                //DeleteAllFilesExcept(outputFile, destinationDir);

                return chunks;
            }
            else
                throw new Exception("Failed to download init file");
        }

        private IEnumerable<Mp4File> ProcessChunks(IEnumerable<Mp4File> files)
        {
            List<Mp4File> res;

            var initFile = files.OfType<Mp4InitFile>().FirstOrDefault();
            if (initFile != null)
            {
                res = new List<Mp4File>();

                var initFileBytes = File.ReadAllBytes(initFile.Path);
                foreach (var f in files)
                {
                    var bytes = File.ReadAllBytes(f.Path);
                    if (!bytes.StartsWith(initFileBytes))
                    {
                        bytes = initFileBytes.Concat(bytes).ToArray();
                        File.WriteAllBytes(f.Path, bytes);
                        res.Add(f);
                    }
                }
            }
            else
                res = new List<Mp4File>(files);

            return res;

            //string outputFile = Path.Combine(destinationDir, DateTime.Now.ToString("yyyyMMddHHmmss") + "_video.mp4");
            //using (var stream = File.OpenWrite(outputFile))
            //using (var writer = new BinaryWriter(stream))
            //{                
            //foreach (var f in files.Skip(1))
            //{
            //    var bytes = File.ReadAllBytes(f);
            //    var mdatBytes = Encoding.ASCII.GetBytes("mdat");
            //    int offset = FindAtomOffset(bytes, mdatBytes);
            //    //if (offset >= 0)
            //    //    writer.Write(bytes, offset - mdatBytes.Length, bytes.Length - offset + mdatBytes.Length);
            //    writer.Write(bytes);
            //}
            //}
        }

        private int FindAtomOffset(byte[] chunkBytes, byte[] atomBytes)
        {
            int mdatOffset = -1;
            for (int i = 0; i < chunkBytes.Length - atomBytes.Length && mdatOffset < 0; i++)
            {
                int matchCount = 0;
                for (int j = 0; j < atomBytes.Length; j++)
                {
                    if (chunkBytes[i + j] == atomBytes[j])
                        matchCount++;
                }
                if (matchCount == atomBytes.Length)
                    mdatOffset = i;
            }
            return mdatOffset;
        }

        private void DeleteAllFilesExcept(string outputFile, string destinationDir)
        {
            outputFile = Path.GetFullPath(outputFile);
            var files = Directory.GetFiles(destinationDir);
            mpd.Value.Dispose();
            foreach (var f in files)
            {
                string file = Path.GetFullPath(f);
                if (outputFile != file)
                    File.Delete(file);
            }
        }

        public Task<string> Download()
        {
            var tasks = new List<Task>();

            foreach (var period in mpd.Value.Periods)
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
                completed => CombineFragments(mpd.Value, mpdFileName.Value, Path.Combine(Path.GetDirectoryName(mpdFileName.Value), "video.mp4")));
        }

        private Task DownloadAllFragments(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
        {
            return Task.Factory.StartNew(() => DownloadFragmentsUntilFirstFailure(adaptationSet, representation));
        }

        private string CombineFragments(MediaPresentationDescription mpd, string mpdFilePath, string outputFilePath)
        {
            var walker = new MpdWalker(mpd);
            var track = walker.GetTracksFor(TrackContentType.Video).First();
            var trackRepresentation = track.TrackRepresentations.OrderByDescending(r => r.Bandwidth).First();

            using (var stream = File.OpenWrite(outputFilePath))
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

            return outputFilePath;
        }

        private void DownloadFragmentsUntilFirstFailure(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
        {
            var task = DownloadRepresentationInitFragment(adaptationSet, representation);

            if (DownloadTaskSucceded(task))
            {
                int i = 1;
                do
                {
                    task = DownloadRepresentationFragment(adaptationSet, representation, i);
                    i++;
                }
                while (DownloadTaskSucceded(task));
            }
        }

        private Task<string> DownloadRepresentationInitFragment(MpdAdaptationSet adaptationSet, MpdRepresentation representation)
        {
            string initUrl = adaptationSet.SegmentTemplate.Initialization
                .Replace("$RepresentationID$", representation.Id);
            var task = DownloadFragment(initUrl);
            task.Wait(TimeSpan.FromMinutes(5));

            return task;
        }

        private Task<string> DownloadRepresentationFragment(MpdAdaptationSet adaptationSet, MpdRepresentation representation, int index)
        {
            string fragmentUrl = adaptationSet.SegmentTemplate.Media
                        .Replace("$RepresentationID$", representation.Id)
                        .Replace("$Number$", index.ToString());

            var task = DownloadFragment(fragmentUrl);
            task.Wait(TimeSpan.FromMinutes(5));

            return task;
        }

        private Task<string> DownloadFragment(string fragmentUrl)
        {
            using (var client = new WebClient())
            {
                var url = IsAbsoluteUrl(fragmentUrl)
                    ? new Uri(fragmentUrl)
                    : mpd.Value.BaseURL != null
                    ? new Uri(mpd.Value.BaseURL + fragmentUrl)
                    : new Uri(mpdUrl, fragmentUrl);

                string destPath = Path.Combine(destinationDir, GetFileNameForFragmentUrl(fragmentUrl));

                int i = 0;
                while (File.Exists(destPath))
                {
                    i++;
                    destPath = Path.Combine(Path.GetDirectoryName(destPath), Path.ChangeExtension((Path.GetFileNameWithoutExtension(destPath) + "_" + i), Path.GetExtension(destPath)));
                }

                // create directory recursive
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                return Task.Factory.StartNew(() =>
                {
                    client.DownloadFile(url, destPath);
                    return destPath;
                });
            }
        }

        private bool DownloadTaskSucceded(Task<string> task)
        {
            return task.IsCompleted && !task.IsFaulted && !string.IsNullOrWhiteSpace(task.Result);
        }

        private string GetMpdFileName()
        {
            string mpdFileName = mpdUrl.AbsolutePath;
            if (mpdFileName.Contains("/"))
                mpdFileName = mpdFileName.Substring(mpdFileName.LastIndexOf("/") + 1);
            string mpdPath = Path.Combine(destinationDir, mpdFileName);

            return mpdPath;
        }

        private MediaPresentationDescription DownloadMpd()
        {
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);
            else
                Directory.GetFiles(destinationDir).ToList().ForEach(f => File.Delete(f));

            return MediaPresentationDescription.FromUrl(mpdUrl, mpdFileName.Value);
        }

        private MpdWalker CreateMpdWalker()
        {
            return new MpdWalker(mpd.Value);
        }

        private string GetFileNameForFragmentUrl(string url)
        {
            string fileName = url;
            if (IsAbsoluteUrl(url))
            {
                fileName = new Uri(url).AbsolutePath;
                if (fileName.Contains("/"))
                    fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
            }

            int queryStartIndex = fileName.IndexOf("?");
            if (queryStartIndex >= 0)
                fileName = fileName.Substring(0, queryStartIndex);

            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
                fileName = Path.ChangeExtension(fileName, "mp4");

            fileName = ReplaceIllegalCharsInFileName(fileName);

            return fileName;
        }

        private string ReplaceIllegalCharsInFileName(string fileName)
        {
            var illegalChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
            foreach (var ch in illegalChars)
            {
                fileName = fileName.Replace(ch, '_');
            }
            return fileName;
        }

        private bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        public void Dispose()
        {
            if (mpd.IsValueCreated)
                mpd.Value.Dispose();
        }
    }
}
