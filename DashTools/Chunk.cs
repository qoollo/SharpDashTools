namespace Qoollo.MpegDash
{
    public class Chunk
    {
        public Chunk(string path)
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