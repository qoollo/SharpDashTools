namespace Qoollo.MpegDash
{
    public class Mp4File
    {
        public Mp4File(string path)
        {
            this.path = path;
        }

        public string Path
        {
            get { return path; }
        }
        private readonly string path;
    }
}