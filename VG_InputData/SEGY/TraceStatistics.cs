using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VG_InputData.SEGY
{
    public class TraceStatistics
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

        public TraceStatistics()
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

        public void UpdateTraceStat(Template template, TraceHeaders traceHeader, ref bool Error, ref string ErrorMessage)
        {
            for (int j = 0; j < template.TraceHeader.GetLength(0); j++)
            {
                switch ((template.TraceHeader[j, 2]))
                {
                    case (1):
                        try
                        {
                            if (MaxInline < traceHeader.Inline)
                                MaxInline = traceHeader.Inline;

                            if (MinInline > traceHeader.Inline)
                                MinInline = traceHeader.Inline;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка расчета статистики трасс (CalcTraceStat, case 1).";
                            return;
                        }
                    case (2):
                        try
                        {
                            if (MaxCrossline < traceHeader.Crossline)
                                MaxCrossline = traceHeader.Crossline;

                            if (MinCrossline > traceHeader.Crossline)
                                MinCrossline = traceHeader.Crossline;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка расчета статистики трасс (CalcTraceStat, case 2).";
                            return;
                        }
                    case (3):
                        try
                        {
                            if (MaxX < traceHeader.X)
                                MaxX = traceHeader.X;

                            if (MinX > traceHeader.X)
                                MinX = traceHeader.X;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка расчета статистики трасс (CalcTraceStat, case 3)."; ;
                            return;
                        }
                    case (4):
                        try
                        {
                            if (MaxY < traceHeader.Y)
                                MaxY = traceHeader.Y;

                            if (MinY > traceHeader.Y)
                                MinY = traceHeader.Y;

                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка расчета статистики трасс (CalcTraceStat, case 4)."; ;
                            return;
                        }
                }
            }
        }
        public void FastCalsAmplStats(long[] sampleForRelCheck, FileHeaders fileHeaders, FileStream stream, ref TraceHeaders traceHeader, TraceInfo traceInfo, double absenceCode, ref bool Error, ref string ErrorMessage)
        {
            long traceShift = 0;
            byte[] sampleBuf = new byte[4];
            double readedSample;
            for (int j = 0; j < sampleForRelCheck.GetLength(0); j++)
            {
                try
                {
                    stream.Seek(sampleForRelCheck[j] - fileHeaders.SampleSize, SeekOrigin.Current);
                    stream.Read(sampleBuf, 0, sampleBuf.Length);
                }
                catch
                {
                    Error = true;
                    ErrorMessage = "Ошибка чтения трасс при проверке на актуальность (FastCalsAmplStats)";
                    return;
                }

                try
                {
                    readedSample = DataTransform.ByteToDouble(fileHeaders.FormatCode, sampleBuf);
                    traceShift += sampleForRelCheck[j];
                    if (readedSample != absenceCode)
                    {
                        traceHeader.Relevanvce = true;
                        break;
                    }
                }
                catch
                {
                    Error = true;
                    ErrorMessage = "Ошибка конвертации отсчета при проверке на актуальность (FastCalsAmplStats)";
                    return;
                }
            }

            if (traceHeader.Relevanvce == false)
                IrrelevantCount++;
            try
            {
                stream.Seek(traceInfo.TraceByteLength - traceShift, SeekOrigin.Current);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка считывания трассы (class SEG_Y, digital_trace_header)";
                return;
            }
        }
        public void CalcAmplStats(FileHeaders fileHeaders, FileStream stream, ref TraceHeaders traceHeader, TraceInfo GenInf, double absenceCode, ref bool Error, ref string ErrorMessage)
        {
            byte[] sampleBuf = new byte[4];
            byte[] traceBuf = new byte[GenInf.TraceByteLength];
            double readedSample;
            try
            {
                stream.Read(traceBuf, 0, traceBuf.Length);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка чтения данных трасс из файла (CalcAmplStats)";
                return;
            }

            try
            {
                for (int j = 0; j < GenInf.TraceByteLength;)
                {
                    Array.Copy(traceBuf, j, sampleBuf, 0, fileHeaders.SampleSize);
                    readedSample = DataTransform.ByteToDouble(fileHeaders.FormatCode, sampleBuf);

                    if (!traceHeader.Relevanvce)
                        if (readedSample != absenceCode)
                        {
                            traceHeader.Relevanvce = true;
                        }

                    if (readedSample > MaxAmpl)
                        MaxAmpl = readedSample;

                    if (readedSample < MinAmpl)
                        MinAmpl = readedSample;

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
