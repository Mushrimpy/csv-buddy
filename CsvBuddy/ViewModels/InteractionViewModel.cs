using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace CsvBuddy.ViewModels
{
    public class InteractionViewModel : ReactiveObject
    {
        public InteractionViewModel()
        {
            _SelectFilesInteraction = new Interaction<string?, string[]?>();
            SelectFilesCommand = ReactiveCommand.CreateFromTask(SelectFiles);
        }

        private string[]? _selectedFiles;
        public string[]? SelectedFiles
        {
            get { return _selectedFiles; }
            set { this.RaiseAndSetIfChanged(ref _selectedFiles, value); }
        }


        private readonly Interaction<string?, string[]?> _SelectFilesInteraction;
        public Interaction<string?, string[]?> SelectFilesInteraction => this._SelectFilesInteraction;
        public ICommand SelectFilesCommand { get; }
        private async Task SelectFiles()
        {
            SelectedFiles = await _SelectFilesInteraction.Handle("Hello from Avalonia");
        }
    }
}