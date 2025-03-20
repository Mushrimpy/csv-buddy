using System;
using Avalonia.Controls;
using Avalonia.Data;
using CsvBuddy.ViewModels;

namespace CsvBuddy.Views
{
    public partial class DataView : UserControl
    {
        private DataViewModel? _viewModel;

        public DataView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            if (_viewModel != null) _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel = DataContext as DataViewModel;
            if (_viewModel == null) return;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            UpdateColumns();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataViewModel.ColumnCount))
                UpdateColumns();
        }
        
        private void UpdateColumns()
        {
            if (_viewModel == null) return;
            MyDataGrid.Columns.Clear();
                for (var i = 0; i < _viewModel.ColumnCount; i++)
                {
                    var columnIndex = i; 
                    MyDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = $"{i}",
                        Binding = new Binding($"[{columnIndex}]") { Mode = BindingMode.TwoWay },
                        IsReadOnly = false
                    });
                }
        }
    }
}