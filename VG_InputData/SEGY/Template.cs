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

        public Template(int[,] new_file_header_template, int[,] new_trace_header_template, string sample_rate_metric, string start_time_metric)
        {
            //Если задаваемый шаблон не заполнен, задать шаблон по умолчанию ("TGT_3D_kingd (109)")
            Error = false;
            ErrorMessage = null;
            if (new_file_header_template == null || new_trace_header_template == null)
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
                    FileHeader = new int[new_file_header_template.GetLength(0), new_file_header_template.GetLength(1) + 1];
                    TraceHeader = new int[new_trace_header_template.GetLength(0), new_trace_header_template.GetLength(1) + 1];
                    SampleRateDevider = 0;
                    StartTimeDevider = 0;

                    for (int i = 0; i < new_file_header_template.GetLength(0); i++)
                    {
                        for (int j = 0; j < new_file_header_template.GetLength(1); j++)
                        {
                            FileHeader[i, j] = new_file_header_template[i, j];
                        }
                    }

                    for (int i = 0; i < new_trace_header_template.GetLength(0); i++)
                    {
                        for (int j = 0; j < new_trace_header_template.GetLength(1); j++)
                        {
                            TraceHeader[i, j] = new_trace_header_template[i, j];
                        }
                    }

                    switch (sample_rate_metric)
                    {
                        case ("mks"):
                            SampleRateDevider = 1000;
                            break;
                        case ("ms"):
                            SampleRateDevider = 1;
                            break;
                        case ("s"):
                            SampleRateDevider = 0.001;
                            break;
                    }

                    switch (start_time_metric)
                    {
                        case ("mks"):
                            StartTimeDevider = 1000;
                            break;
                        case ("ms"):
                            StartTimeDevider = 1;
                            break;
                        case ("s"):
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
