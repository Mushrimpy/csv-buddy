using CsvBuddy.Models;

namespace CsvBuddy.Services;

public interface IConsumer
{
    void ConsumeField(string s);
    void SignalEndOfRecord();
    void SignalEndOfFile();
}

public class ConsumerService(CsvFile csvFile) : IConsumer
{
    private CsvRecord _currentRecord = new CsvRecord();
    public void ConsumeField(string s) => _currentRecord.AddField(s);

    public void SignalEndOfRecord()
    {
        csvFile.AddRecord(_currentRecord);
        _currentRecord = new CsvRecord();
    }
    public void SignalEndOfFile()
    {
        if (_currentRecord.FieldCount > 0)
            csvFile.AddRecord(_currentRecord);
    }
}

