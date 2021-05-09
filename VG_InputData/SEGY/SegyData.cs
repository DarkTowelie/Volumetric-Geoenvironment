using System;
using System.ComponentModel;
using System.IO;

namespace VG_InputData.SEGY
{
    public class SegyData
    {
        public TraceHeaders[] TrHeaders { get; }
        public TraceGeneralInfo GenInf { get; }
        public Trace2DInfo Tr2DInfo { get; }
        public SegyStatistics SegyStat { get; }

        public bool Error;
        public string ErrorMessage;

        public SegyData(FileStream stream, int traceCount, FileHeaders fileHeaders,
                        double absenceCode, bool amplitudeAnalysis, Template template,
                            ref int progress, ref BackgroundWorker bw)
        {
            TrHeaders = null;
            GenInf = new TraceGeneralInfo();
            Tr2DInfo = new Trace2DInfo();
            SegyStat = new SegyStatistics();

            Error = false;
            ErrorMessage = null;
            //-----------------------------------------------------------------------------------------------------
            byte[] TraceHeaderBuf = new byte[240];
            //-----------------------------------------------------------------------------------------------------
            if (fileHeaders.SampleSize == 0)
            {
                Error = true;
                ErrorMessage = "Неизвестный формат данных (конструктор SegyData).";
                return;
            }

            stream.Seek(3600, SeekOrigin.Begin);
            stream.Read(TraceHeaderBuf, 0, TraceHeaderBuf.Length);
            GenInf = new TraceGeneralInfo(fileHeaders, TraceHeaderBuf, template, ref Error, ref ErrorMessage);

            long[] SampleForRelCheck = null;
            if (!amplitudeAnalysis)
                SampleForRelCheck = GetSampleForRelCheck();
            if (Error) return;

            TrHeaders = new TraceHeaders[traceCount];
            long trace_header_location = 3600;
            stream.Seek(trace_header_location, SeekOrigin.Begin);

            int MaxProgressValue = traceCount - 1;
            for (int i = 0; i <= traceCount - 1; i++)
            {
                TrHeaders[i] = new TraceHeaders();
                if (bw.CancellationPending == true)
                {
                    bw.CancelAsync();
                }
                else
                {
                    stream.Read(TraceHeaderBuf, 0, TraceHeaderBuf.Length);
                    SegyStat.CalcCoordStats(template, TraceHeaderBuf, TrHeaders[i], ref Error, ref ErrorMessage);


                    byte[] Buf = new byte[fileHeaders.SampleSize];
                    if (amplitudeAnalysis)
                    {
                        SegyStat.CalcAmplStats(fileHeaders, stream, TrHeaders[i], GenInf, absenceCode, ref Error, ref ErrorMessage);
                    }
                    else
                    {
                        SegyStat.FastCalsAmplStats(SampleForRelCheck, fileHeaders, stream, TrHeaders[i], GenInf, absenceCode, ref Error, ref ErrorMessage);
                    }

                    try
                    {
                        progress = Convert.ToInt32(Math.Floor((double)100 / (double)(MaxProgressValue) * (Convert.ToDouble(i))));
                        bw.ReportProgress(progress);
                    }
                    catch
                    {
                        Error = true;
                        ErrorMessage = "Ошибка расчета прогреса (class SEG_Y, digital_trace_header)";
                        return;
                    }
                }
            }

            Tr2DInfo = new Trace2DInfo(fileHeaders, SegyStat, TrHeaders, ref Error, ref ErrorMessage);
        }

        public long[] GetSampleForRelCheck()
        {
            long[] SampleForRelCheck = new long[3];
            try
            {
                SampleForRelCheck[0] = GenInf.TrLength / 4;
                SampleForRelCheck[1] = GenInf.TrLength / 2 - SampleForRelCheck[0];
                SampleForRelCheck[2] = GenInf.TrLength - SampleForRelCheck[1] * 2 - SampleForRelCheck[0];
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка определения отсчетов для проверки актуальности трасс (конструктор SegyData).";
                return null;
            }
            return SampleForRelCheck;
        }
    }
}