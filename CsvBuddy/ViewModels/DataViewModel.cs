using System.Collections.ObjectModel;
using System.Linq;
using CsvBuddy.Models;
using ReactiveUI;
using CsvBuddy.Services;

namespace CsvBuddy.ViewModels
{
    public class DataViewModel : ReactiveObject
    {
        private readonly FileService _fileService = new();
        private ObservableCollection<RecordViewModel> _recordViewModels = new();
        private int _columnCount;
    
        public ObservableCollection<RecordViewModel> RecordViewModels
        {
            get => _recordViewModels;
            set => this.RaiseAndSetIfChanged(ref _recordViewModels, value);
        }
        public int ColumnCount
        {
            get => _columnCount;
            set => this.RaiseAndSetIfChanged(ref _columnCount, value);
        }
        public void LoadFromFile(string filePath)
        {
            var csvFile = _fileService.LoadCsv(filePath);
            if (csvFile == null) return;
            ColumnCount = csvFile.Records.Max(r => r.FieldCount);
            var records = csvFile.Records.Select(r => new RecordViewModel(r));
            RecordViewModels = new ObservableCollection<RecordViewModel>(records);
        }
        public class RecordViewModel(CsvRecord record) : ReactiveObject
        {
            public string GetField(int index)
            {
                if (index < 0 || index >= record.FieldCount)
                    return string.Empty;
                return record[index].Value;
            }
            public string this[int index] => GetField(index);
        }
    }
}
