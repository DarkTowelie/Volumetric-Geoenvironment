namespace VG_InputData.SEGY
{
    public class Template
    {
        public int[,] FileHeader { get; }
        public int[,] TraceHeader { get; }
        public double SampleRateDevider { get; }
        public double StartTimeDevider { get; }

        public bool Error { get; }
        public string ErrorMessage { get; }

        public Template(int[,] fileHeaderTemp, int[,] traceHeadTemp, Metric sampleRateMetric, Metric startTimeMetric)
        {
            //Если шаблон не заполнен, задать шаблон по умолчанию ("TGT_3D_kingd (109)")
            Error = false;
            ErrorMessage = null;
            if (fileHeaderTemp == null || traceHeadTemp == null)
            {
                try
                {
                    FileHeader = new int[6, 4];
                    TraceHeader = new int[7, 4];
                    SampleRateDevider = 0;
                    StartTimeDevider = 0;

                    FileHeader[0, 0] = 8; FileHeader[0, 1] = 4; FileHeader[0, 2] = 1; //Inline
                    FileHeader[1, 0] = 14; FileHeader[1, 1] = 2; FileHeader[1, 2] = 2; //Crossline
                    FileHeader[2, 0] = 16; FileHeader[2, 1] = 2; FileHeader[2, 2] = 3; //Time step
                    FileHeader[3, 0] = 20; FileHeader[3, 1] = 2; FileHeader[3, 2] = 4; //Num of samples
                    FileHeader[4, 0] = 24; FileHeader[4, 1] = 2; FileHeader[4, 2] = 5; //Format code
                    FileHeader[5, 0] = 54; FileHeader[5, 1] = 2; FileHeader[5, 2] = 6; //Meas system

                    TraceHeader[0, 0] = 16; TraceHeader[0, 1] = 4; TraceHeader[0, 2] = 1; //Inline
                    TraceHeader[1, 0] = 24; TraceHeader[1, 1] = 4; TraceHeader[1, 2] = 2; //Crossline
                    TraceHeader[2, 0] = 72; TraceHeader[2, 1] = 4; TraceHeader[2, 2] = 3; //X coord
                    TraceHeader[3, 0] = 76; TraceHeader[3, 1] = 4; TraceHeader[3, 2] = 4; //Y coord
                    TraceHeader[4, 0] = 108; TraceHeader[4, 1] = 2; TraceHeader[4, 2] = 5; //Start time
                    TraceHeader[5, 0] = 114; TraceHeader[5, 1] = 2; TraceHeader[5, 2] = 6; //Num of samples
                    TraceHeader[6, 0] = 116; TraceHeader[6, 1] = 2; TraceHeader[6, 2] = 7; //Time step

                    SampleRateDevider = 1000;
                    StartTimeDevider = 1;
                }
                catch
                {
                    FileHeader = null;
                    TraceHeader = null;
                    SampleRateDevider = 0;
                    StartTimeDevider = 0;
                    Error = true;
                    ErrorMessage = "Ошибка задания шаблона по умолчанию (Template).";
                    return;
                }
            }
            else
            {
                try
                {
                    FileHeader = new int[fileHeaderTemp.GetLength(0), fileHeaderTemp.GetLength(1) + 1];
                    TraceHeader = new int[traceHeadTemp.GetLength(0), traceHeadTemp.GetLength(1) + 1];
                    SampleRateDevider = 0;
                    StartTimeDevider = 0;

                    for (int i = 0; i < fileHeaderTemp.GetLength(0); i++)
                    {
                        for (int j = 0; j < fileHeaderTemp.GetLength(1); j++)
                        {
                            FileHeader[i, j] = fileHeaderTemp[i, j];
                        }
                    }

                    for (int i = 0; i < traceHeadTemp.GetLength(0); i++)
                    {
                        for (int j = 0; j < traceHeadTemp.GetLength(1); j++)
                        {
                            TraceHeader[i, j] = traceHeadTemp[i, j];
                        }
                    }

                    switch (sampleRateMetric)
                    {
                        case (Metric.MKS):
                            SampleRateDevider = 1000;
                            break;
                        case (Metric.MS):
                            SampleRateDevider = 1;
                            break;
                        case (Metric.S):
                            SampleRateDevider = 0.001;
                            break;
                    }

                    switch (startTimeMetric)
                    {
                        case (Metric.MKS):
                            StartTimeDevider = 1000;
                            break;
                        case (Metric.MS):
                            StartTimeDevider = 1;
                            break;
                        case (Metric.S):
                            StartTimeDevider = 0.001;
                            break;
                    }
                }
                catch
                {
                    FileHeader = null;
                    TraceHeader = null;
                    SampleRateDevider = 0;
                    StartTimeDevider = 0;
                    Error = true;
                    ErrorMessage = "Ошибка задания шаблона по образцу (class SEG_Y, template).";
                    return;
                }
            }

        }
    }
}
