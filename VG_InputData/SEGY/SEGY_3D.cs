using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VG_InputData.SEGY
{
    public class SEGY_3D
    {
        //Поля класса---------------------------------------------------------------------------------
        public string Name { get; }
        public string TextHeader { get; }
        public FileStream File { get; }

        public Template template { get; }
        public FileHeaders fileHeaders { get; }
        public SegyData segyData { get; }

        public int TraceCount { get; }
        public bool Error { get; private set; }
        public string ErrorMessage { get; private set; }

        //Конструкторы класса-------------------------------------------------------------------------
        public SEGY_3D(ref int progress, bool AmpAnalysis, string seg_file_name,
                        double absenceCode, int[,] fileHeaderTemp, int[,] traceHeadTemp, bool paleo_check_flag,
                            string sampleRateMetr, string startTimeMetr, ref BackgroundWorker bw)
        {
            Name = seg_file_name;
            TraceCount = 0;
            try
            {
                File = new FileStream(seg_file_name, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка открытия файла SEG.";
                return;
            }

            try
            {
                byte[] TextHeaderBuf = new byte[3200];
                Encoding FileHeadersEnc = Encoding.GetEncoding(500);
                File.Read(TextHeaderBuf, 0, TextHeaderBuf.Length);
                TextHeader = FileHeadersEnc.GetString(TextHeaderBuf, 0, TextHeaderBuf.Length);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка считывания текстового заголовка SEG.";
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
                long TraceCount = (File.Length - 3600) / (fileHeaders.SamplesCount * fileHeaders.SampleSize + 240);
                this.TraceCount = (int)TraceCount;
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка подсчета количества трасс (SEG).";
                return;
            }

            segyData = new SegyData(File, TraceCount, fileHeaders, absenceCode, AmpAnalysis, template, ref progress, ref bw);
            if (segyData.Error)
            {
                Error = true;
                ErrorMessage = segyData.ErrorMessage;
                return;
            }
        }

        //Методы класса-------------------------------------------------------------------------------
        public byte[] read_GroupOfTracesIntoBuffer(int first_trace_nuber, int num_of_reading_traces)
        {
            long new_stream_position = new_stream_position = Convert.ToInt64((long)3600 + (long)(first_trace_nuber) * (long)(fileHeaders.SampleSize * segyData.GenInf.SamplesCount + 240));
            File.Seek(new_stream_position, SeekOrigin.Begin);

            byte[] buffer = new byte[num_of_reading_traces * (fileHeaders.SampleSize * segyData.GenInf.SamplesCount + 240)];
            File.Read(buffer, 0, buffer.GetLength(0));
            return buffer;
        }
        public double[] read_TraceByNumber(int num_of_trace)
        {
            try
            {
                long traceByteLength = segyData.GenInf.SamplesCount * fileHeaders.SampleSize;
                byte[] traceBuf = new byte[traceByteLength];
                double[] readed_trace = new double[segyData.GenInf.SamplesCount];

                File.Seek(3840 + num_of_trace * (240 + traceByteLength), SeekOrigin.Begin);
                File.Read(traceBuf, 0, traceBuf.Length);

                int j = 0;
                switch (fileHeaders.FormatCode)
                {
                    case (1):
                        for (int i = 0; i < traceByteLength;)
                        {
                            byte[] sample_buf = DataTransform.GetBytesFromBuffer(traceBuf, i, fileHeaders.SampleSize);
                            readed_trace[j++] = DataTransform.IBMToDouble(sample_buf);
                            i += fileHeaders.SampleSize;
                        }
                        return readed_trace;
                    case (5):
                        for (int i = 0; i < traceByteLength;)
                        {
                            byte[] sample_buf = DataTransform.GetBytesFromBuffer(traceBuf, i, fileHeaders.SampleSize);
                            readed_trace[j++] = BitConverter.ToDouble(sample_buf, 0);
                            i += fileHeaders.SampleSize;
                        }
                        return readed_trace;
                    default:
                        {
                            ErrorMessage = "Неизвестный формат данных (SEG: read_TraceWithSpecificPositionNum).";
                            return null;
                        }
                }
            }
            catch
            {
                ErrorMessage = "Ошибка считывания трассы (SEG: read_TraceWithSpecificPositionNum).";
                return null;
            }
        }
        public double[][] extract_TracesFromBytesStream(byte[] bytes_stream, int trace_length, int num_of_traces)
        {
            double[][] traces = new double[num_of_traces][];
            Parallel.For(0, num_of_traces, (i, state) =>
            {
                traces[i] = new double[trace_length];

                int sample_index = 0;
                int start_position = 240 + (240 + trace_length * fileHeaders.SampleSize) * i;
                byte[] byte_buffer = new byte[fileHeaders.SampleSize];
                for (int j = 0; j < trace_length * fileHeaders.SampleSize;)
                {
                    for (int k = 0; k < fileHeaders.SampleSize; k++)
                    {
                        byte_buffer[k] = bytes_stream[start_position + j + k];
                    }
                    traces[i][sample_index] = DataTransform.ByteToDouble(fileHeaders.FormatCode, byte_buffer);
                    j += fileHeaders.SampleSize;
                    sample_index++;
                }
            });
            return traces;
        }
    }
}
