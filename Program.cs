using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ClosedXML.Excel;

namespace GetProcNames
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the directory path to scan
            string directoryPath = @"path to directory";

            // path to export the excel file too
            string excelFilePath = @"path to excel file";

            // Get all the files in the directory and its subdirectories
            string[] files = Directory.GetFiles(directoryPath, "*.file type", SearchOption.AllDirectories);

            // Create a list to store the results
            List<ProcInfo> procList = new List<ProcInfo>();

            // Loop through each file
            foreach (string file in files)
            {
                Console.WriteLine("Scanning File");
                var text = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(text);
                var root = tree.GetRoot();

                // Search for string literals containing proc names by schema
                var literals = root.DescendantNodes().OfType<LiteralExpressionSyntax>()
                    .Where(l => l.IsKind(SyntaxKind.StringLiteralExpression) && l.Token.Text.Contains("schema.")).ToList();

                foreach (var literal in literals)
                {
                    // Find the nearest method ancestor
                    var method = literal.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                    if (method != null)
                    {
                        // Create a new ProcInfo object and add it to the list
                        ProcInfo procInfo = new ProcInfo();
                        procInfo.ClassName = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Identifier.Text;
                        procInfo.MethodName = method.Identifier.Text;
                        procInfo.ProcName = literal.Token.ValueText;
                        procList.Add(procInfo);
                    }
                }
            }

            WriteToExcel(procList, excelFilePath);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void WriteToExcel(List<ProcInfo> procList, string excelFilePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    Console.WriteLine("Building Excel File");
                    var worksheet = workbook.Worksheets.Add("ProcInfo");
                    worksheet.Cell(1, 1).Value = "Class Name";
                    worksheet.Cell(1, 2).Value = "Method Name";
                    worksheet.Cell(1, 3).Value = "Proc Name";

                    int row = 2;
                    foreach (ProcInfo procInfo in procList)
                    {
                        worksheet.Cell(row, 1).Value = procInfo.ClassName;
                        worksheet.Cell(row, 2).Value = procInfo.MethodName;
                        worksheet.Cell(row, 3).Value = procInfo.ProcName;
                        row++;
                    }

                    workbook.SaveAs(excelFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    public class ProcInfo
    {
        public string ProcName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }
}
