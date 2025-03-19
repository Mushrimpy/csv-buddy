using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using CsvBuddy.ViewModels;
using ReactiveUI;
using System.Linq;
using System.Threading.Tasks;

namespace CsvBuddy.Views
{
    public partial class InteractionView : ReactiveUserControl<InteractionViewModel>
    {
        public InteractionView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel!.SelectFilesInteraction.RegisterHandler(this.InteractionHandler));
            });
        }

        private async Task InteractionHandler(IInteractionContext<string?, string[]?> context)
        {
            // Get our parent top level control in order to get the needed service (in our sample the storage provider. Can also be the clipboard etc.)
            var topLevel = TopLevel.GetTopLevel(this);

            var storageFiles = await topLevel!.StorageProvider
                .OpenFilePickerAsync(
                    new FilePickerOpenOptions()
                    {
                        AllowMultiple = false,
                        Title = context.Input
                    });
               
            context.SetOutput(storageFiles.Select(x => x.Path.LocalPath).ToArray());
        }
    }
}