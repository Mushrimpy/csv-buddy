using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvBuddy.Models;

namespace CsvBuddy.Services
{
    public class FileService
    {
        private readonly ParserService _parser = new ParserService();

        public CsvFile? LoadCsv(string filePath)
        {
            var csvFile = new CsvFile(filePath);
            var tokenizer = new TokenizerService(File.ReadAllText(filePath));
            var consumer = new ConsumerService(csvFile);

            _parser.Parse(tokenizer, consumer);

            return csvFile;
        }

        public void SaveCsv(string filePath, CsvFile csvFile)
        {
            var lines = new List<string>();

            for (var i = 0; i < csvFile.RecordCount; i++)
            {
                var record = csvFile[i];
                var lineBuilder = new StringBuilder();
                for (var j = 0; j < record.FieldCount; j++)
                {
                    var field = record.GetField(j);
                    if (j > 0)
                        lineBuilder.Append(',');
                    if (field.IsQuoted)
                    {
                        lineBuilder.Append('"');
                        lineBuilder.Append(field.Value.Replace("\"", "\"\""));
                        lineBuilder.Append('"');
                    }
                    else
                        lineBuilder.Append(field.Value);
                }
                lines.Add(lineBuilder.ToString());
            }
            File.WriteAllLines(filePath, lines);
        }
    }
}