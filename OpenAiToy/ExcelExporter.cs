using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace JiraGPTGrader
{
    public class ExcelExporter
    {
        public static void ExportToExcel(string filePath, List<StoryFeedback> feedbacks)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Story Feedback");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Ticket Number";
                    worksheet.Cells[1, 2].Value = "Ranking";
                    worksheet.Cells[1, 3].Value = "Feedback";

                    // Add data
                    for (int i = 0; i < feedbacks.Count; i++)
                    {
                        var feedback = feedbacks[i];
                        worksheet.Cells[i + 2, 1].Value = feedback.IssueKey;
                        worksheet.Cells[i + 2, 2].Value = feedback.Score;
                        worksheet.Cells[i + 2, 3].Value = feedback.Feedback;
                    }

                    // Save the file
                    FileInfo fileInfo = new FileInfo(filePath);
                    package.SaveAs(fileInfo);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to the file path: {filePath}. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while exporting to Excel: {ex.Message}");
            }
        }
    }
}
