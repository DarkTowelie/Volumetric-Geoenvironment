using System.IO;

namespace VG_InputData.SEGY
{
    public class FileHeaders
    {
        public int InlineCount { get; }
        public int CrosslineCount { get; }
        public int SampleSize { get; }
        public int SamplesCount { get; }
        public int TimeStep { get; }
        public int FormatCode { get; }
        public int MeasurementSystem { get; }

        public bool Error;
        public string ErrorMessage;

        public FileHeaders(FileStream SEGY, Template template)
        {
            InlineCount = 0;
            CrosslineCount = 0;
            SampleSize = 0;
            SamplesCount = 0;
            TimeStep = 0;
            FormatCode = 0;
            MeasurementSystem = 0;

            byte[] Buf;
            byte[] DigidalHeaderBuf = new byte[400];

            Error = false;
            ErrorMessage = null;
            try
            {
                SEGY.Seek(3200, SeekOrigin.Begin);
                SEGY.Read(DigidalHeaderBuf, 0, DigidalHeaderBuf.Length);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка считывания цифрового заголовка SEGY в буфер.";
                return;
            }

            for (int i = 0; i < template.FileHeader.GetLength(0); i++)
            {
                switch (template.FileHeader[i, 2])
                {
                    case (1):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(DigidalHeaderBuf, template.FileHeader, i);
                            InlineCount = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи данных из буфера в структуру (FileHeader, case 1).";
                            return;
                        }

                    case (2):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(DigidalHeaderBuf, template.FileHeader, i);
                            CrosslineCount = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи данных из буфера в структуру (FileHeader, case 2).";
                            return;
                        }

                    case (3):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(DigidalHeaderBuf, template.FileHeader, i);
                            TimeStep = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи данных из буфера в структуру (FileHeader, case 3).";
                            return;
                        }

                    case (4):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(DigidalHeaderBuf, template.FileHeader, i);
                            SamplesCount = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи данных из буфера в структуру (FileHeader, case 4).";
                            return;
                        }

                    case (5):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(DigidalHeaderBuf, template.FileHeader, i);
                            FormatCode = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи данных из буфера в структуру (FileHeader, case 5).";
                            return;
                        }

                    case (6):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(DigidalHeaderBuf, template.FileHeader, i);
                            MeasurementSystem = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи данных из буфера в структуру (FileHeader, case 6).";
                            return;
                        }
                }
            }

            SampleSize = GetSampleSize(FormatCode);
            if (SampleSize == 0)
            {
                Error = true;
                ErrorMessage = "Неизвестный формат данных (FileHeader)";
                return;
            }
        }
        int GetSampleSize(int Format_code)
        {
            switch (Format_code)
            {
                case (1):
                    return 4;
                case (5):
                    return 4;
                default:
                    return 0;
            }
        }
    }
}
