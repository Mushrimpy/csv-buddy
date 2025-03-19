using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace CsvBuddy.ViewModels;
public class InteractionViewModel : ReactiveObject
{
    private readonly DataViewModel _dataViewModel;
    public DataViewModel DataViewModel => _dataViewModel;

    public InteractionViewModel(DataViewModel dataViewModel)
    {
        _dataViewModel = dataViewModel;
        _selectFilesInteraction = new Interaction<string?, string[]?>();
        SelectFilesCommand = ReactiveCommand.CreateFromTask(SelectFiles);
    }

    private string[]? _selectedFiles;
    public string[]? SelectedFiles
    {
        get => _selectedFiles;
        set => this.RaiseAndSetIfChanged(ref _selectedFiles, value);
    }

    private readonly Interaction<string?, string[]?> _selectFilesInteraction;
    public Interaction<string?, string[]?> SelectFilesInteraction => this._selectFilesInteraction;
    public ICommand SelectFilesCommand { get; }
    private async Task SelectFiles()
    {
        SelectedFiles = await _selectFilesInteraction.Handle("Hello from CSV Buddy");
        if (SelectedFiles is { Length: > 0 })
            _dataViewModel.LoadFromFile(SelectedFiles[0]);
    }
}
