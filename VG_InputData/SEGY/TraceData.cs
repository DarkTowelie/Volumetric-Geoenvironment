using System;
using System.ComponentModel;
using System.IO;

namespace VG_InputData.SEGY
{
    public class TraceData
    {
        public TraceHeaders[] Headers { get; }
        public TraceInfo Info { get; }
        public Trace2DInfo Info2D { get; }
        public TraceStatistics Stat { get; }

        public bool Error;
        public string ErrorMessage;

        public TraceData(FileStream stream, int traceCount, FileHeaders fileHeaders,
                        double absenceCode, bool amplitudeAnalysis, Template template,
                            ref int progress, ref BackgroundWorker bw)
        {
            Headers = null;
            Info = new TraceInfo();
            Info2D = new Trace2DInfo();
            Stat = new TraceStatistics();

            Error = false;
            ErrorMessage = null;
            //-----------------------------------------------------------------------------------------------------
            byte[] traceHeaderBuf = new byte[240];
            //-----------------------------------------------------------------------------------------------------
            if (fileHeaders.SampleSize == 0)
            {
                Error = true;
                ErrorMessage = "Неизвестный формат данных (TraceData).";
                return;
            }

            stream.Seek(3600, SeekOrigin.Begin);
            stream.Read(traceHeaderBuf, 0, traceHeaderBuf.Length);
            Info = new TraceInfo(fileHeaders, traceHeaderBuf, template, ref Error, ref ErrorMessage);

            long[] sampleForRelCheck = null;
            if (!amplitudeAnalysis)
                sampleForRelCheck = GetSampleForRelCheck();
            if (Error) return;

            Headers = new TraceHeaders[traceCount];
            long traceHeaderLocation = 3600;
            stream.Seek(traceHeaderLocation, SeekOrigin.Begin);

            int maxProgressValue = traceCount - 1;
            for (int i = 0; i <= traceCount - 1; i++)
            {
                Headers[i] = new TraceHeaders();
                if (bw.CancellationPending == true)
                {
                    bw.CancelAsync();
                }
                else
                {
                    stream.Read(traceHeaderBuf, 0, traceHeaderBuf.Length);
                    Headers[i] = new TraceHeaders(template, traceHeaderBuf, ref Error, ref ErrorMessage);
                    Stat.UpdateTraceStat(template, Headers[i], ref Error, ref ErrorMessage);

                    if (amplitudeAnalysis)
                    {
                        Stat.CalcAmplStats(fileHeaders, stream, ref Headers[i], Info, absenceCode, ref Error, ref ErrorMessage);
                    }
                    else
                    {
                        Stat.FastCalsAmplStats(sampleForRelCheck, fileHeaders, stream, ref Headers[i], Info, absenceCode, ref Error, ref ErrorMessage);
                    }

                    try
                    {
                        progress = Convert.ToInt32(Math.Floor((double)100 / (double)(maxProgressValue) * (Convert.ToDouble(i))));
                        bw.ReportProgress(progress);
                    }
                    catch
                    {
                        Error = true;
                        ErrorMessage = "Ошибка расчета прогреса (TraceData)";
                        return;
                    }
                }
            }

            Info2D = new Trace2DInfo(fileHeaders, Stat, Headers, ref Error, ref ErrorMessage);
        }

        public long[] GetSampleForRelCheck()
        {
            long[] sampleForRelCheck = new long[3];
            try
            {
                sampleForRelCheck[0] = Info.TraceByteLength / 4;
                sampleForRelCheck[1] = Info.TraceByteLength / 2 - sampleForRelCheck[0];
                sampleForRelCheck[2] = Info.TraceByteLength - sampleForRelCheck[1] * 2 - sampleForRelCheck[0];
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка определения отсчетов для проверки актуальности трасс (TraceData).";
                return null;
            }
            return sampleForRelCheck;
        }
    }
}