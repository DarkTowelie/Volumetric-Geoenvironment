using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;

namespace VG_InputData.EXCEL.Well_logging
{
    public class ExcelData
    {
        public string FilePath { get; }
        public string SheetName { get; }
        public double AbsenceCode { get; }

        public WellData[] WellsData { get; }
        public WellsStatistics WellsStat {get;}

        public bool Error { get; }
        public string ErrorMessage { get; }
        
        public ExcelData(string filePath, string sheetName, double absenceCode)
        {
            FilePath = filePath;
            SheetName = sheetName;
            AbsenceCode = absenceCode;
            Stream file = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(file);
            DataSet result = reader.AsDataSet();
            List<DataTable> tables = result.Tables.Cast<DataTable>().ToList();

            int index = ReturnTableNum(tables, sheetName);
            WellsData = new WellData[tables[index].Rows.Count - 1];
            for (int i = 1; i < tables[index].Rows.Count; i++)
            {
                WellsData[i - 1] = new WellData(tables[index], i, absenceCode);
            }

            WellsStat = new WellsStatistics(WellsData, absenceCode);
        }
        int ReturnTableNum(List<DataTable> tables, string sheetName)
        {
            int tableNum = -1;
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].TableName == sheetName)
                {
                    tableNum = i;
                    break;
                }
            }
            return tableNum;
        }
    }
}
