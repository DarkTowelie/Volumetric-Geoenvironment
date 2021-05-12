using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VG_InputData.SEGY
{
    public enum Metric
    {
        S,
        MS,
        MKS
    }

    public class SEGY_3D
    {
        public string Name { get; }
        public string TextHeader { get; }
       
        public FileStream File { get; }
        public Template template { get; }
        public TraceData traceData { get; }
        public FileHeaders fileHeaders { get; }

        public int TraceCount { get; }
        public bool Error { get; private set; }
        public string ErrorMessage { get; private set; }

        public SEGY_3D(ref int progress, bool ampAnalysis, string segyFileName,
                        double absenceCode, int[,] fileHeaderTemp, int[,] traceHeadTemp,
                            Metric sampleRateMetr, Metric startTimeMetr, ref BackgroundWorker bw)
        {
            Name = segyFileName;
            TraceCount = 0;
            try
            {
                File = new FileStream(segyFileName, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка открытия файла Segy (SEGY_3D).";
                return;
            }

            try
            {
                byte[] textHeaderBuf = new byte[3200];
                Encoding fileHeadersEnc = Encoding.GetEncoding(500);
                File.Read(textHeaderBuf, 0, textHeaderBuf.Length);
                TextHeader = fileHeadersEnc.GetString(textHeaderBuf, 0, textHeaderBuf.Length);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка считывания текстового заголовка Segy (SEGY_3D).";
                return;
            }

            template = new Template(fileHeaderTemp, traceHeadTemp, sampleRateMetr, startTimeMetr);
            if (template.Error)
            {
                Error = true;
                ErrorMessage = template.ErrorMessage;
                return;
            }

            fileHeaders = new FileHeaders(File, template);
            if (fileHeaders.Error)
            {
                Error = true;
                ErrorMessage = fileHeaders.ErrorMessage;
                return;
            }

            try
            {
                long traceCount = (File.Length - 3600) / (fileHeaders.SamplesCount * fileHeaders.SampleSize + 240);
                this.TraceCount = (int)traceCount;
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка подсчета количества трасс Segy (SEGY_3D).";
                return;
            }

            traceData = new TraceData(File, TraceCount, fileHeaders, absenceCode, ampAnalysis, template, ref progress, ref bw);
            if (traceData.Error)
            {
                Error = true;
                ErrorMessage = traceData.ErrorMessage;
                return;
            }
        }

        public byte[] ReadTracesIntoBuf(int firstTraceNum, int tracesCount)
        {
            long newStreamPosition = newStreamPosition = Convert.ToInt64((long)3600 + (long)(firstTraceNum) * (long)(fileHeaders.SampleSize * traceData.Info.SamplesCount + 240));
            File.Seek(newStreamPosition, SeekOrigin.Begin);

            byte[] buf = new byte[tracesCount * (fileHeaders.SampleSize * traceData.Info.SamplesCount + 240)];
            File.Read(buf, 0, buf.GetLength(0));
            return buf;
        }
        public double[] ReadTraceByNum(int traceNum)
        {
            try
            {
                long traceByteLength = traceData.Info.SamplesCount * fileHeaders.SampleSize;
                byte[] traceBuf = new byte[traceByteLength];
                double[] trace = new double[traceData.Info.SamplesCount];

                File.Seek(3840 + traceNum * (240 + traceByteLength), SeekOrigin.Begin);
                File.Read(traceBuf, 0, traceBuf.Length);

                int j = 0;
                switch (fileHeaders.FormatCode)
                {
                    case (1):
                        for (int i = 0; i < traceByteLength;)
                        {
                            byte[] sampleBuf = new byte[fileHeaders.SampleSize];
                            Array.Copy(traceBuf, i, sampleBuf, 0, fileHeaders.SampleSize);
                            trace[j++] = DataTransform.IBMToDouble(sampleBuf);
                            i += fileHeaders.SampleSize;
                        }
                        return trace;
                    case (5):
                        for (int i = 0; i < traceByteLength;)
                        {
                            byte[] sampleBuf = new byte[fileHeaders.SampleSize];
                            Array.Copy(traceBuf, i, sampleBuf, 0, fileHeaders.SampleSize);
                            trace[j++] = BitConverter.ToDouble(sampleBuf, 0);
                            i += fileHeaders.SampleSize;
                        }
                        return trace;
                    default:
                        {
                            ErrorMessage = "Неизвестный формат данных (ReadTraceByNum).";
                            return null;
                        }
                }
            }
            catch
            {
                ErrorMessage = "Ошибка считывания трассы (ReadTraceByNum).";
                return null;
            }
        }
        public double[][] FormTraceFromBuffer(byte[] buffer, int traceLength, int tracesCount)
        {
            double[][] traces = new double[tracesCount][];
            Parallel.For(0, tracesCount, (i, state) =>
            {
                traces[i] = new double[traceLength];

                int sampleIndex = 0;
                int startPosition = 240 + (240 + traceLength * fileHeaders.SampleSize) * i;
                byte[] buf = new byte[fileHeaders.SampleSize];
                for (int j = 0; j < traceLength * fileHeaders.SampleSize;)
                {
                    for (int k = 0; k < fileHeaders.SampleSize; k++)
                    {
                        buf[k] = buffer[startPosition + j + k];
                    }
                    traces[i][sampleIndex] = DataTransform.ByteToDouble(fileHeaders.FormatCode, buf);
                    j += fileHeaders.SampleSize;
                    sampleIndex++;
                }
            });
            return traces;
        }
    }
}
