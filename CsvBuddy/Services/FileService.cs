using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvBuddy.Models;

namespace CsvBuddy.Services
{
    public class FileService
    {
        private readonly ParserService _parser = new ParserService();

        public async Task<CsvFile?> LoadCsv(string filePath, CancellationToken cancellationToken = default)
        {
            var csvFile = new CsvFile(filePath);
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            var tokenizer = new TokenizerService(content);
            var consumer = new ConsumerService(csvFile);

            _parser.Parse(tokenizer, consumer);

            return csvFile;
        }
        
        public async Task SaveCsv(string filePath, CsvFile csvFile, CancellationToken cancellationToken = default)
        {
            var raw = ConvertToRaw(csvFile);
            await File.WriteAllLinesAsync(filePath, raw, cancellationToken);
        }
        
        private List<string> ConvertToRaw(CsvFile csvFile)
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

            return lines;
        }
    }
}
