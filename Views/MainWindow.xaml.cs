using System.Windows;
using System.Windows.Controls;
using BudgetManager.ViewModels;
using MahApps.Metro.Controls;

namespace BudgetManager
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header && header.Column.Header is string headerName)
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.ColumnHeaderClickCommand.Execute(headerName);
                }
            }
        }
    }
}
