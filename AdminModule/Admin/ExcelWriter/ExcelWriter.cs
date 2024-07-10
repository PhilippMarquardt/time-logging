using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MotorenMarquardtAdminModule.ExcelWriter
{
    class ExcelWriter
    {



        public static void WriteMonthlyAnalysisToExcel(ObservableCollection<CalendarItem> items, string emp)
        {
            try
            {
                if(items == null || emp == "" || emp == null)
                {
                    throw new Exception("Bitte einen Mitarbeiter auswählen");
                }
                //check if file exists and delete it
                if (File.Exists(dateFilePath)) File.Delete(dateFilePath);


                FileInfo spreadsheetInfo = new FileInfo(dateFilePath);
                ExcelPackage pck = new ExcelPackage(spreadsheetInfo);

                var ws = pck.Workbook.Worksheets.Add("Monatsauswertung");
                ws.Cells["A1"].Value = "Mitarbeiter";
                ws.Cells["B1"].Value = "Datum";
                ws.Cells["C1"].Value = "Gesamte Arbeitszeit Tag";
                ws.Cells["D1"].Value = "Startzeit";
                ws.Cells["E1"].Value = "Endzeit";
                ws.Cells["F1"].Value = "Gesamte Pausenzeit am Tag";
                ws.Cells["G1"].Value = "Gesamte Arbeitszeit im Monat";
                

                int countRows = 2;
                for (int i = 0; i < items.Count; i++)
                {
                    string rowIndex = countRows.ToString();
                    if (items[i].Worktime != "00:00:00" && items[i].Worktime != "")
                    {
                        ws.Cells["A" + rowIndex].Value = emp;
                        ws.Cells["B" + rowIndex].Value = items[i].ActualDate;
                        ws.Cells["C" + rowIndex].Value = items[i].Worktime;
                        ws.Cells["D" + rowIndex].Value = ExcelWriterQueries.GetLogInTime(emp, DateTime.Parse(items[i].ActualDate)) + " Uhr";
                        ws.Cells["E" + rowIndex].Value = ExcelWriterQueries.GetLogOutTime(emp, DateTime.Parse(items[i].ActualDate)) + " Uhr";
                        ws.Cells["F" + rowIndex].Value = ExcelWriterQueries.GetBreakTimeOfDay(emp, DateTime.Parse(items[i].ActualDate));
                        countRows++;
                    }
                    else if (items[i].ActualDate != "")
                    {
                        ws.Cells["A" + rowIndex].Value = emp;
                        ws.Cells["B" + rowIndex].Value = items[i].ActualDate;
                        ws.Cells["C" + rowIndex].Value = "00:00:00";
                        ws.Cells["D" + rowIndex].Value = "-";
                        ws.Cells["E" + rowIndex].Value = "-";
                        ws.Cells["F" + rowIndex].Value = "00:00:00";
                        countRows++;
                    }
                }
                ws.Cells["G2"].Value = new TimeSpan(items.Sum(x => x.Worktime != "" ? TimeSpan.Parse(x.Worktime).Ticks : 0)).ToString();

                ws.Cells.AutoFitColumns();
                pck.Save();
                System.Diagnostics.Process.Start(dateFilePath);
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }



        public static string dateFilePath  //filepath of the written file
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Monatsauswertung.xlsx";
            }
        }
    }
}
