using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace JiraGPTGrader
{
    public class ExcelExporter
    {
        public static void ExportToExcel(List<StoryFeedback> feedbacks)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

            try
            {
                // Get the user's Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Define the subfolder where the files will be saved
                string subfolder = Path.Combine(documentsPath, "JiraStoryFeedback");

                // Check if the folder exists, if not, create it
                if (!Directory.Exists(subfolder))
                {
                    Directory.CreateDirectory(subfolder);
                }

                // Generate the file name with the current date-time
                string fileName = $"StoryFeedback_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
                string filePath = Path.Combine(subfolder, fileName);

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

                        // Add test cases and acceptance criteria to the Excel file if available
                        string testCasesAndCriteria = feedback.TestCasesAndAcceptanceCriteria;
                        worksheet.Cells[i + 2, 4].Value = testCasesAndCriteria ?? "N/A";
                    }


                    // Save the file
                    FileInfo fileInfo = new FileInfo(filePath);
                    package.SaveAs(fileInfo);
                }

                Console.WriteLine($"File saved to: {filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to the file path. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while exporting to Excel: {ex.Message}");
            }
        }
    }
}
