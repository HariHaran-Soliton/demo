using System.Text;
using Spectre.Console;
using TimerTrackerApp.Model;

namespace TimerTrackerApp.DataAccess
{
    public class DataExporter
    {
        public static void ExportData(UserData userData)
        {
            Console.Clear();
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("UserName,ProjectName,TaskName,TaskDescription,TaskStatus,StartTime,EndTime,Duration");
            int csvLength= csvContent.Length;
            foreach (var project in userData.Projects)
            {
                foreach (var task in project.TaskItems)
                {
                    csvContent.AppendLine($"{userData.User.UserName},{project.Name},{task.TaskName},{task.TaskDescription},{task.TaskStatus},{task.StartTime},{task.EndTime},{task.Duration}");
                }
            }
            int newCsvLength= csvContent.Length;
            if (newCsvLength > csvLength)
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ExportData.csv");
                File.WriteAllText(filePath, csvContent.ToString());
                Console.WriteLine($"Data exported successfully to {filePath}");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]No data availabe to export as csv.[/]");
                Console.WriteLine("Press any key to continue...");
            }
        }
    }
}
