using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace VG_InputData.EXCEL.Well_logging
{
    public class WellData
    {
        public string Name { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double[] Data { get; }
        public double[] Depth { get; }
        public WellData(DataTable table, int index, double absenceCode)
        {
            Name = table.Rows[index][0].ToString();

            double res = 0;
            X = Double.TryParse(table.Rows[index][1].ToString(), out res) ? res : absenceCode;
            Y = Double.TryParse(table.Rows[index][2].ToString(), out res) ? res : absenceCode;

            Data = new double[table.Columns.Count - 3];
            Depth = new double[table.Columns.Count - 3];
            for (int i = 3; i < table.Columns.Count; i++)
            {
                Depth[i - 3] = Double.TryParse(table.Rows[0][i].ToString(), out res) ? res : absenceCode;
                Data[i - 3] = Double.TryParse(table.Rows[index][i].ToString(), out res) ? res : absenceCode;
            }
        }
    }
}
