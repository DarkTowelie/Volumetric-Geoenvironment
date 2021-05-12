using System;
using System.Collections.Generic;
using System.Text;

namespace VG_InputData.EXCEL.Well_logging
{
    public class WellsStatistics
    {
        public double MaxData { get; }
        public double MinData { get; }
        public double MaxDepth { get; }
        public double MinDepth { get; }
        public double MaxX { get; }
        public double MinX { get; }
        public double MaxY { get; }
        public double MinY { get; }

        public WellsStatistics(WellData[] wellsData, double absenceCode)
        {
            MaxData = wellsData[0].Data[0];
            MinData = wellsData[0].Data[0];
            MaxDepth = wellsData[0].Depth[0];
            MinDepth = wellsData[0].Depth[0];
            MaxX = wellsData[0].X;
            MinX = wellsData[0].X;
            MaxY = wellsData[0].Y;
            MinY = wellsData[0].Y;


            for (int i = 0; i < wellsData.Length; i++)
            {
                if (MaxX < wellsData[i].X && wellsData[i].X != absenceCode) MaxX = wellsData[i].X;
                if (MinX > wellsData[i].X && wellsData[i].X != absenceCode) MinX = wellsData[i].X;
                if (MaxY < wellsData[i].Y && wellsData[i].Y != absenceCode) MaxY = wellsData[i].Y;
                if (MinY > wellsData[i].Y && wellsData[i].Y != absenceCode) MinY = wellsData[i].Y;

                for (int j = 0; j < wellsData[i].Depth.Length; j++)
                {
                    if (MaxDepth < wellsData[i].Depth[j] && wellsData[i].Depth[j] != absenceCode)
                        MaxDepth = wellsData[i].Depth[j];

                    if (MinDepth > wellsData[i].Depth[j] && wellsData[i].Depth[j] != absenceCode)
                        MinDepth = wellsData[i].Depth[j];

                    if (MaxData < wellsData[i].Data[j] && wellsData[i].Data[j] != absenceCode)
                        MaxData = wellsData[i].Data[j];

                    if (MinData > wellsData[i].Data[j] && wellsData[i].Data[j] != absenceCode)
                        MinData = wellsData[i].Data[j];
                }
            }
        }
    }
}
