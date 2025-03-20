using CsvBuddy.Models;

namespace CsvBuddy.Services;

public interface IConsumer
{
    void ConsumeField(CsvField f);
    void SignalEndOfRecord();
    void SignalEndOfFile();
}

public class ConsumerService(CsvFile csvFile) : IConsumer
{
    private CsvRecord _currentRecord = new CsvRecord();
    public void ConsumeField(CsvField f) => _currentRecord.AddField(f);

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

