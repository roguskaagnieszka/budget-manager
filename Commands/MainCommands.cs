using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BudgetManager.Models;
using BudgetManager.Validation;
using BudgetManager.ViewModels;
using BudgetManager.Views;

namespace BudgetManager.Commands
{
    public static class MainCommands
    {
        public static ObservableCollection<Cost> Costs { get; set; } = new ObservableCollection<Cost>();

        static MainCommands()
        {
            LoadCostsFromFile();
        }

        public static ICommand AddCostCommand { get; } = new RelayCommand<string>(AddCost);
        public static ICommand EditCostCommand { get; } = new RelayCommand<Cost>(EditCost);
        public static ICommand DeleteCostCommand { get; } = new RelayCommand<Cost>(DeleteCost);
        public static ICommand CloseCommand { get; } = new RelayCommand(CloseWindow);
        public static ICommand ColumnHeaderClickCommand { get; } = new RelayCommand<string>(SortCosts);
        public static ICommand ShowStatisticsMessageCommand { get; } = new RelayCommand(ShowStatisticsMessage);
        public static ICommand ShowSettingsMessageCommand { get; } = new RelayCommand(ShowSettingsMessage);

        private static void LoadCostsFromFile()
        {
            string filePath = "costs.txt";
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(';');

                try
                {
                    string id = parts[0];
                    string type = parts[1];
                    string category = parts[2];
                    string amount = parts[3];
                    DateTime date = DateTime.Parse(parts[4]);
                    string description = parts[5];
                    string optionalField = parts.Length > 6 ? parts[6] : null;

                    var AR_67722_cost = CreateCost(id, type, category, amount, date, description, optionalField, optionalField);

                    if (AR_67722_cost != null)
                    {
                        Costs.Add(AR_67722_cost);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading data: {ex.Message} in line: {line}");
                }
            }
        }

        public static void SortByDateDescending()
        {
            if (CollectionViewSource.GetDefaultView(Costs) is ICollectionView collectionView)
            {
                collectionView.SortDescriptions.Clear();
                collectionView.SortDescriptions.Add(new SortDescription(nameof(Cost.Date), ListSortDirection.Descending));
            }
        }

        public static List<string> LoadCategoriesFromFile(string filePath)
        {
            return File.ReadAllLines(filePath).ToList();
        }

        private static void AddCost(string costType)
        {
            var addCostWindow = new CostWindow();

            var addCostViewModel = new AddEditCostViewModel { Type = costType };

            addCostViewModel.CloseRequested += () => addCostWindow.Close();

            addCostWindow.DataContext = addCostViewModel;

            (Application.Current as App).AddCostWindow = addCostWindow;

            addCostWindow.ShowDialog();
        }



        private static void EditCost(Cost cost)
        {
            if (cost == null) return;

            var editCostWindow = new CostWindow(cost);
            var AR_67722_editCostViewModel = new AddEditCostViewModel(cost);

            AR_67722_editCostViewModel.CloseRequested += () => editCostWindow.Close();

            editCostWindow.DataContext = AR_67722_editCostViewModel;

            (Application.Current as App).AddCostWindow = editCostWindow;

            editCostWindow.ShowDialog();
        }

        public static bool ValidateAndSaveCost(string id, string type, string selectedCategory, string amount, DateTime? date, string description, string selectedPaymentInterval, string selectedImportanceLevel)
        {
            if (!CostValidation.ValidateCost(type, selectedCategory, amount, description, selectedPaymentInterval, selectedImportanceLevel, out string validationErrorMessage))
            {
                MessageBox.Show(validationErrorMessage, "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var newCost = CreateCost(id, type, selectedCategory, amount, date, description, selectedPaymentInterval, selectedImportanceLevel);

            if (string.IsNullOrEmpty(id))
            {
                AddCostAndSaveToFile(newCost);
            }
            else
            {
                OnCostEdited(newCost);
            }

            return true;
        }

        private static Cost CreateCost(string id, string type, string selectedCategory, string amount, DateTime? date, string description, string selectedPaymentInterval, string selectedImportanceLevel)
        {
            double parsedAmount = double.Parse(amount.Replace(',', '.'), CultureInfo.InvariantCulture);

            var cost = type == "Fixed"
                ? new FixedCost()
                : new VariableCost() as Cost;

            cost.ID = string.IsNullOrEmpty(id) ? DateTime.Now.ToString("yyyyMMddHHmmssfff") : id;
            cost.Type = type;
            cost.Category = selectedCategory;
            cost.Amount = parsedAmount;
            cost.Date = date ?? DateTime.Now;
            cost.Description = description;

            if (cost is FixedCost fixedCost)
            {
                fixedCost.PaymentInterval = selectedPaymentInterval;
            }

            if (cost is VariableCost variableCost)
            {
                variableCost.ImportanceLevel = selectedImportanceLevel;
            }

            return cost;
        }

        public static void AddCostAndSaveToFile(Cost newCost)
        {
            Costs.Add(newCost);
            WriteAllCostsToFile();
        }

        private static void OnCostEdited(Cost editedCost)
        {
            var existingCost = Costs.FirstOrDefault(c => c.ID == editedCost.ID);
            if (existingCost != null)
            {
                int index = Costs.IndexOf(existingCost);
                Costs[index] = editedCost;
                WriteAllCostsToFile();
            }
        }

        private static void DeleteCost(Cost cost)
        {
            if (cost == null) return;

            string message = $"Are you sure you want to delete the cost?\n\n" +
                             $"Category: {cost.Category}\n" +
                             $"Amount: {cost.Amount}\n" +
                             $"Date: {cost.Date:yyyy-MM-dd}\n" +
                             $"Description: {cost.Description}";

            if (MessageBox.Show(message, "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Costs.Remove(cost);
                WriteAllCostsToFile();
            }
        }

        public static void WriteAllCostsToFile()
        {
            string filePath = "costs.txt";
            var lines = Costs.Select(cost =>
            {
                switch (cost)
                {
                    case FixedCost fixedCost:
                        return $"{fixedCost.ID};{fixedCost.Type};{fixedCost.Category};{fixedCost.Amount.ToString("F", CultureInfo.GetCultureInfo("pl-PL"))};{fixedCost.Date:yyyy-MM-dd};{fixedCost.Description};{fixedCost.PaymentInterval}";

                    case VariableCost variableCost:
                        return $"{variableCost.ID};{variableCost.Type};{variableCost.Category};{variableCost.Amount.ToString("F", CultureInfo.GetCultureInfo("pl-PL"))};{variableCost.Date:yyyy-MM-dd};{variableCost.Description};{variableCost.ImportanceLevel}";

                    default:
                        return $"{cost.ID};{cost.Type};{cost.Category};{cost.Amount.ToString("F", CultureInfo.GetCultureInfo("pl-PL"))};{cost.Date:yyyy-MM-dd};{cost.Description}";
                }
            });
            File.WriteAllLines(filePath, lines);
        }

        private static void SortCosts(string columnName)
        {
            if (CollectionViewSource.GetDefaultView(Costs) is ICollectionView collectionView)
            {
                var direction = ListSortDirection.Ascending;

                if (collectionView.SortDescriptions.Any(sd => sd.PropertyName == columnName && sd.Direction == ListSortDirection.Ascending))
                {
                    direction = ListSortDirection.Descending;
                }

                collectionView.SortDescriptions.Clear();

                collectionView.SortDescriptions.Add(new SortDescription(columnName, direction));
            }
        }

        private static void CloseWindow()
        {
            if (MessageBox.Show("Are you sure you want to close the application?", "Close Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.MainWindow.Close();
            }
        }

        private static void ShowStatisticsMessage()
        {
            MessageBox.Show("Work is in progress. The 'Statistics' functionality will be available in the future.", "Work in Progress", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void ShowSettingsMessage()
        {
            MessageBox.Show("Work is in progress. The 'Settings' functionality will be available in the future.", "Work in Progress", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
