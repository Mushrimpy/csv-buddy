using System;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.ReactiveUI;
using CsvBuddy.ViewModels;
using ReactiveUI;

namespace CsvBuddy.Views;
public partial class DataView : ReactiveUserControl<DataViewModel>
{ 
    public DataView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.DataContext)!
                .OfType<DataViewModel>()
                .Subscribe(vm => 
                {
                    vm.WhenAnyValue(x => x.ColumnCount)
                        .Subscribe(_ => UpdateColumns());
                });
        }
    private void UpdateColumns()
    {
        var dataViewModel = DataContext as DataViewModel;
        MyDataGrid.Columns.Clear();
        if (dataViewModel != null)
            for (int i = 0; i < dataViewModel.ColumnCount; i++)
            {
                MyDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"Col {i}",
                    Binding = new Binding($"[{i}]")
                });
            }
    }
}


