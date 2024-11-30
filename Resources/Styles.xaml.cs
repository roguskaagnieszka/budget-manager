using BudgetManager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BudgetManager.Resources
{
    public partial class Styles : ResourceDictionary
    {
        public Styles()
        {
            InitializeComponent();
        }

        private void OnColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext is MainViewModel viewModel)
            {
                if (sender is GridViewColumnHeader header && header.Column.Header is string headerName)
                {
                    viewModel.ColumnHeaderClickCommand.Execute(headerName);
                }
            }
        }
    }
}
