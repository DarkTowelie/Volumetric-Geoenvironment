namespace VG_InputData.SEGY
{
    public class TraceHeaders
    {
        public int Inline { get; private set; }
        public int Crossline { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Relevanvce { get; set; }

        public TraceHeaders()
        {
            Inline = 0;
            Crossline = 0;
            X = 0;
            Y = 0;
            Relevanvce = false;
        }
        public TraceHeaders(Template template, byte[] TraceHeaderBuf, ref bool Error, ref string ErrorMessage)
        {
            Relevanvce = false;
            for (int j = 0; j < template.TraceHeader.GetLength(0); j++)
            {
                switch ((template.TraceHeader[j, 2]))
                {
                    case (1):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytes(TraceHeaderBuf, template.TraceHeader, j);
                            Inline = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка зписи заголовков трассы (TraceHeaders, case 1).";
                            return;
                        }
                    case (2):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytes(TraceHeaderBuf, template.TraceHeader, j);
                            Crossline = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка зписи заголовков трассы (TraceHeaders, case 2).";
                            return;
                        }
                    case (3):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytes(TraceHeaderBuf, template.TraceHeader, j);
                            X = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка зписи заголовков трассы (TraceHeaders, case 3).";
                            return;
                        }
                    case (4):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytes(TraceHeaderBuf, template.TraceHeader, j);
                            Y = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка зписи заголовков трассы (TraceHeaders, case 4).";
                            return;
                        }
                }
            }
        }
    }
}
