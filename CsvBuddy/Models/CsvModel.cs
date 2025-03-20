using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CsvBuddy.Models
{
    public partial class CsvField : ObservableObject
    {
        [ObservableProperty] private string _value;
        [ObservableProperty] private bool _isQuoted;

        public CsvField(string value, bool isQuoted = false)
        {
            _value = value;
            _isQuoted = isQuoted;
        }
    }

    public partial class CsvRecord : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<CsvField> _fields = new();

        public void AddField(CsvField field) => Fields.Add(field);
        public void AddField(string value, bool isQuoted = false) => Fields.Add(new CsvField(value, isQuoted));
        public int FieldCount => Fields.Count;
        public CsvField GetField(int index) => Fields[index];

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= FieldCount)
                    return string.Empty;
                return Fields[index].Value;
            }
            set
            {
                if (index >= 0 && index < FieldCount)
                    Fields[index].Value = value;
                else if (index >= FieldCount)
                {
                    while (Fields.Count <= index)
                        AddField(Fields.Count == index ? value : string.Empty);
                }
                OnPropertyChanged($"Item[{index}]");
            }
        }
    }

    public partial class CsvFile(string? filename = null) : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<CsvRecord> _records = new();
        [ObservableProperty] private string? _filename = filename;

        public void AddRecord(CsvRecord record) => Records.Add(record);

        public CsvRecord CreateNewRecord()
        {
            var record = new CsvRecord();
            if (Records.Count > 0)
            {
                var fieldCount = Records[0].FieldCount;
                for (var i = 0; i < fieldCount; i++)
                    record.AddField(string.Empty);
            }

            Records.Add(record);
            return record;
        }

        public int RecordCount => Records.Count;
        public CsvRecord this[int index] => Records[index];
    }
}