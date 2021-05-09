using System;
using System.Collections.Generic;
using System.Text;

namespace VG_InputData.SEGY
{
    public class Trace2DInfo
    {
        public int[] Inline { get; private set; }
        public int[] Crossline { get; private set; }
        public int[,] X { get; private set; }
        public int[,] Y { get; private set; }
        public bool[,] Relevance { get; private set; }
        public double StepX { get; private set; }
        public double StepY { get; private set; }
        public Trace2DInfo()
        {
            Inline = null;
            Crossline = null;
            X = null;
            Y = null;
            Relevance = null;
        }

        public Trace2DInfo(FileHeaders fileHeaders, SegyStatistics SegyStat, TraceHeaders[] traceHeaders, ref bool Error, ref string ErrorMessage)
        {
            Inline = new int[fileHeaders.InlineCount];
            Crossline = new int[fileHeaders.CrosslineCount];
            X = new int[fileHeaders.InlineCount, fileHeaders.CrosslineCount];
            Y = new int[fileHeaders.InlineCount, fileHeaders.CrosslineCount];
            Relevance = new bool[fileHeaders.InlineCount, fileHeaders.CrosslineCount];
            
            try
            {
                Inline[Inline.GetLength(0) - 1] = SegyStat.MinInline;
                for (int i = Inline.GetLength(0) - 2; i >= 0; i--)
                {
                    Inline[i] = Inline[i + 1] + 1;
                }

                Crossline[0] = SegyStat.MinCrossline;
                for (int i = 1; i < Crossline.GetLength(0); i++)
                {
                    Crossline[i] = Crossline[i - 1] + 1;
                }

                for (int i = 0; i < traceHeaders.Length; i++)
                {
                    int first_index = fileHeaders.InlineCount - traceHeaders[i].Inline + SegyStat.MinInline - 1;
                    int second_index = traceHeaders[i].Crossline - SegyStat.MinCrossline;
                    X[first_index, second_index] = traceHeaders[i].X;
                    Y[first_index, second_index] = traceHeaders[i].Y;
                    Relevance[first_index, second_index] = traceHeaders[i].Relevanvce;
                }

                StepX= Math.Abs(X[0, 0] - X[0, 1]);
                StepY = Math.Abs(Y[0, 0] - Y[1, 0]);
            }
            catch
            {
                Error = true;
                ErrorMessage = "Ошибка структуризации данных (class SEG_Y, digital_trace_header)";
                return;
            }
        }
        
    }
}
