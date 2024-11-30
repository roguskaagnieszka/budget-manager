using BudgetManager.Models;
using BudgetManager.ViewModels;
using MahApps.Metro.Controls;

namespace BudgetManager.Views
{
    public partial class CostWindow : MetroWindow
    {
        public CostWindow(Cost cost = null)
        {
            InitializeComponent();

            var viewModel = new AddEditCostViewModel(cost);

            DataContext = viewModel;

            viewModel.CloseRequested += () => this.Close();
        }
    }
}
