using System;
using System.Collections.Generic;
using System.Text;

namespace VG_InputData.SEGY
{
    public class TraceGeneralInfo
    {
        public int TimeStep { get; }
        public int StartTime { get; }
        public int SamplesCount { get; }
        public long TrLength { get; }
        public double[] SamplesTime { get; }

        public TraceGeneralInfo()
        {
            TimeStep = 0;
            StartTime = 0;
            SamplesCount = 0;
            TrLength = 0;
            SamplesTime = null;
        }
        public TraceGeneralInfo(FileHeaders fileHeader, byte[] TraceHeaderBuf, Template template, ref bool Error, ref string ErrorMessage)
        {
            byte[] Buf;
            TimeStep = 0;
            StartTime = 0;
            SamplesCount = fileHeader.SamplesCount;
            TrLength = fileHeader.SamplesCount * fileHeader.SampleSize;
            SamplesTime = null;

            for (int j = 0; j < template.TraceHeader.GetLength(0); j++)
            {
                switch (template.TraceHeader[j, 2])
                {
                    case (5):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(TraceHeaderBuf, template.TraceHeader, j);
                            StartTime = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи начального времени (TraceGeneralInfo, case 5).";
                            return;
                        }
                    case (6):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(TraceHeaderBuf, template.TraceHeader, j);
                            SamplesCount = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи количества отсчетов трассы (TraceGeneralInfo, case 6).";
                            return;
                        }
                    case (7):
                        try
                        {
                            Buf = DataTransform.GetRevBytesFromHeaderBuffer(TraceHeaderBuf, template.TraceHeader, j);
                            TimeStep = DataTransform.ByteToInt(Buf);
                            break;
                        }
                        catch
                        {
                            Error = true;
                            ErrorMessage = "Ошибка записи шага дискретизации (TraceGeneralInfo, case 7).";
                            return;
                        }
                }
            }

            try
            {
                SamplesTime = new double[SamplesCount];
                SamplesTime[0] = (double)StartTime / template.StartTimeDevider;
                for (int i = 1; i < SamplesCount; i++)
                {
                    SamplesTime[i] = SamplesTime[i - 1] + (double)TimeStep / template.SampleRateDevider;
                }
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка расчета времени отсчетов трасс (TraceGeneralInfo).";
                return;
            }
        }
    }
}
