using System.Collections.Generic;

namespace CsvBuddy.Models
{
    public class CsvField(string value, bool isQuoted = false)
    {
        public string Value { get; set; } = value;
        public bool IsQuoted { get; set; } = isQuoted;
    }
    public class CsvRecord
    {
        private List<CsvField> Fields { get; } = new List<CsvField>();
        public void AddField(CsvField field) => Fields.Add(field);
        public void AddField(string value, bool isQuoted = false) => Fields.Add(new CsvField(value, isQuoted));
        public int FieldCount => Fields.Count;
        public CsvField this[int index] => Fields[index];
    }

    public class CsvFile(string? filename = null)
    {
        private List<CsvRecord> Records { get; } = new List<CsvRecord>();
        public string? Filename { get; set; } = filename;
        public void AddRecord(CsvRecord record) => Records.Add(record);
        public int RecordCount => Records.Count;
        public CsvRecord this[int index] => Records[index];
    }
}