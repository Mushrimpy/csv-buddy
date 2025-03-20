using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvBuddy.Models;
using CsvBuddy.Services;

namespace CsvBuddy.ViewModels
{
    public partial class DataViewModel : ObservableObject
    {
        private readonly FileService _fileService = new();
        private CsvFile? _csvFile;

        [ObservableProperty] private ObservableCollection<CsvRecord> _records = new();

        [ObservableProperty] private int _columnCount;

        [ObservableProperty] private CsvRecord _selectedRecord;

        public DataViewModel()
        {
            _selectedRecord = new CsvRecord();
            _csvFile = new CsvFile();
            Records = _csvFile.Records;
        }
        
        [RelayCommand]
        public async Task LoadFromFile(string filePath)
        {
            _csvFile = await _fileService.LoadCsv(filePath);
            if (_csvFile == null) return;
            Records = _csvFile.Records;
            UpdateColumnCount();
        }

        [RelayCommand]
        public void SaveFile()
        {
            if (_csvFile?.Filename != null)
                _fileService.SaveCsv(_csvFile.Filename, _csvFile);
        }

        [RelayCommand]
        public void AddNewRecord()
        {
            if (_csvFile == null) return;
            var newRecord = _csvFile.CreateNewRecord();
            SelectedRecord = newRecord;
        }

        private void UpdateColumnCount()
        {
            ColumnCount = Records.Count > 0 ? Records[0].FieldCount : 0;
        }
    }
}