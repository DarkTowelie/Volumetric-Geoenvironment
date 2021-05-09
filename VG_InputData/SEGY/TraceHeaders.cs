namespace VG_InputData.SEGY
{
    public class TraceHeaders
    {
        public int Inline { get; set; }
        public int Crossline { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Relevanvce { get; set; }

        public TraceHeaders()
        {
            Inline = 0;
            Crossline = 0;
            X = 0;
            Y = 0;
            Relevanvce = false;
        }
    }
}
