using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VG_InputData.SEGY
{
    public class SegyStatistics
    {
        public int MaxInline { get; private set; }
        public int MinInline { get; private set; }
        public int MaxCrossline { get; private set; }
        public int MinCrossline { get; private set; }

        public int MaxX { get; private set; }
        public int MinX { get; private set; }
        public int MaxY { get; private set; }
        public int MinY { get; private set; }

        public int StepX { get; private set; }
        public int StepY { get; private set; }

        public double MaxAmpl { get; private set; }
        public double MinAmpl { get; private set; }

        public int IrrelevantCount { get; private set; }

        public SegyStatistics()
        {
            MaxInline = -2147483648;
            MinInline = 2147483647;
            MaxCrossline = -2147483648;
            MinCrossline = 2147483647;

            MaxX = -2147483648;
            MinX = 2147483647;
            MaxY = -2147483648;
            MinY = 2147483647;

            StepX = 0;
            StepY = 0;

            MaxAmpl = -2147483648;
            MinAmpl = 214483647;

            IrrelevantCount = 0;
        }

        public void CalcCoordStats(Template template, byte[] TraceHeaderBuf, TraceHeaders traceHeader, ref bool Error, ref string ErrorMessage)
        {
            for (int j = 0; j < template.TraceHeader.GetLength(0); j++)
            {
                switch ((template.TraceHeader[j, 2]))
                {
                    case (1):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytesFromHeaderBuffer(TraceHeaderBuf, template.TraceHeader, j);
                            traceHeader.Inline = DataTransform.ByteToInt(Buf);
                            if (MaxInline < traceHeader.Inline)
                                MaxInline = traceHeader.Inline;

                            if (MinInline > traceHeader.Inline)
                                MinInline = traceHeader.Inline;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи Inline трасс (class SEG_Y, digital_trace_header, case 1).";
                            return;
                        }
                    case (2):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytesFromHeaderBuffer(TraceHeaderBuf, template.TraceHeader, j);
                            traceHeader.Crossline = DataTransform.ByteToInt(Buf);
                            if (MaxCrossline < traceHeader.Crossline)
                                MaxCrossline = traceHeader.Crossline;

                            if (MinCrossline > traceHeader.Crossline)
                                MinCrossline = traceHeader.Crossline;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи Crossline трасс (class SEG_Y, digital_trace_header, case 2)";
                            return;
                        }
                    case (3):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytesFromHeaderBuffer(TraceHeaderBuf, template.TraceHeader, j);
                            traceHeader.X = DataTransform.ByteToInt(Buf);
                            if (MaxX < traceHeader.X)
                                MaxX = traceHeader.X;

                            if (MinX > traceHeader.X)
                                MinX = traceHeader.X;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи координат X трасс (class SEG_Y, digital_trace_header, case 3)";
                            return;
                        }
                    case (4):
                        try
                        {
                            byte[] Buf = DataTransform.GetRevBytesFromHeaderBuffer(TraceHeaderBuf, template.TraceHeader, j);
                            traceHeader.Y = DataTransform.ByteToInt(Buf);
                            if (MaxY < traceHeader.Y)
                                MaxY = traceHeader.Y;

                            if (MinY > traceHeader.Y)
                                MinY = traceHeader.Y;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи координат Y трасс (class SEG_Y, digital_trace_header, case 4)";
                            return;
                        }
                }
            }
        }
        public void FastCalsAmplStats(long[] SampleForRelCheck, FileHeaders fileHeaders, FileStream stream, TraceHeaders traceHeader, TraceGeneralInfo GenInf, double absenceCode, ref bool Error, ref string ErrorMessage)
        {
            long TraceShift = 0;
            byte[] Buf = new byte[4];
            double readed_sample;
            for (int j = 0; j < SampleForRelCheck.GetLength(0); j++)
            {
                try
                {
                    stream.Seek(SampleForRelCheck[j] - fileHeaders.SampleSize, SeekOrigin.Current);
                    stream.Read(Buf, 0, Buf.Length);
                }
                catch
                {
                    Error = true;
                    ErrorMessage = "Ошибка чтения трасс при проверке на актуальность (SegyStatistics, быстрая проверка на актуалность)";
                    return;
                }

                try
                {
                    readed_sample = DataTransform.ByteToDouble(fileHeaders.FormatCode, Buf);
                    TraceShift += SampleForRelCheck[j];
                    if (readed_sample != absenceCode)
                    {
                        traceHeader.Relevanvce = true;
                        break;
                    }
                }
                catch
                {
                    Error = true;
                    ErrorMessage = "Ошибка конвертации отсчета при проверке на актуальность (class SEG_Y, digital_trace_header, быстрая проверка на актуалность)";
                    return;
                }
            }

            if (traceHeader.Relevanvce == false)
                IrrelevantCount++;
            try
            {
                stream.Seek(GenInf.TrLength - TraceShift, SeekOrigin.Current);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка считывания трассы (class SEG_Y, digital_trace_header)";
                return;
            }
        }
        public void CalcAmplStats(FileHeaders fileHeaders, FileStream stream, TraceHeaders traceHeader, TraceGeneralInfo GenInf, double absenceCode, ref bool Error, ref string ErrorMessage)
        {
            byte[] Buf = new byte[4];
            byte[] trace_buffer = new byte[GenInf.TrLength];
            double readed_sample;
            try
            {
                stream.Read(trace_buffer, 0, trace_buffer.Length);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка чтения данных трасс из файла (class SEG_Y, digital_trace_header, полная проверка на актуальность)";
                return;
            }

            try
            {
                for (int j = 0; j < GenInf.TrLength;)
                {
                    for (int k = 0; k < fileHeaders.SampleSize; k++)
                    {
                        Buf[k] = trace_buffer[j + k];
                    }

                    readed_sample = DataTransform.ByteToDouble(fileHeaders.FormatCode, Buf);

                    if (traceHeader.Relevanvce == false)
                        if (readed_sample != absenceCode)
                        {
                            traceHeader.Relevanvce = true;
                        }

                    if (readed_sample > MaxAmpl)
                        MaxAmpl = readed_sample;

                    if (readed_sample < MinAmpl)
                        MinAmpl = readed_sample;

                    j += fileHeaders.SampleSize;
                }
                if (traceHeader.Relevanvce == false)
                    IrrelevantCount++;
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка чтения данных трасс из файла (class SEG_Y, digital_trace_header, полная проверка на актуальность)";
                return;
            }
        }
    }
}
