using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using BudgetManager.Commands;
using BudgetManager.Models;

namespace BudgetManager.ViewModels
{
    public class AddEditCostViewModel : INotifyPropertyChanged
    {
        private string _id = string.Empty; // Domyślna wartość
        private string _type = string.Empty; // Domyślna wartość
        private string? _selectedCategory;
        private string _amount = string.Empty; // Domyślna wartość
        private DateTime? _date = DateTime.Now;
        private string _description = string.Empty; // Domyślna wartość
        private string? _selectedPaymentInterval;
        private string? _selectedImportanceLevel;
        private bool _isEditing;

        public AddEditCostViewModel() : this(null) { }

        public AddEditCostViewModel(Cost? cost)
        {
            // Domyślne wartości
            _id = string.Empty;
            _type = string.Empty;
            _amount = string.Empty;
            _description = string.Empty;

            SaveCommand = new RelayCommand(SaveCost);
            CancelCommand = new RelayCommand(CloseWindow);

            PaymentIntervals = new List<string> { "Monthly", "Quarterly", "Yearly" };
            ImportanceLevels = new List<string> { "Low", "Medium", "High" };

            if (cost != null)
            {
                IsEditing = true;
                ID = cost.ID ?? string.Empty; // Domyślna wartość
                Type = cost.Type ?? string.Empty; // Domyślna wartość
                SelectedCategory = cost.Category;
                Amount = cost.Amount.ToString(CultureInfo.InvariantCulture);
                Date = cost.Date;
                Description = cost.Description ?? string.Empty; // Domyślna wartość

                if (cost is FixedCost fixedCost)
                {
                    SelectedPaymentInterval = fixedCost.PaymentInterval;
                }
                else if (cost is VariableCost variableCost)
                {
                    SelectedImportanceLevel = variableCost.ImportanceLevel;
                }
            }
            else
            {
                IsEditing = false;
            }
        }

        public List<string> Categories { get; set; } = new List<string>();
        public List<string> PaymentIntervals { get; set; } = new List<string>();
        public List<string> ImportanceLevels { get; set; } = new List<string>();

        public string ID
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));

                    Categories = MainCommands.LoadCategoriesFromFile(_type == "Fixed"
                        ? "categories-fixed-costs.txt"
                        : "categories-variable-costs.txt");
                    OnPropertyChanged(nameof(Categories));
                }
            }
        }

        public string? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        public string Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public DateTime? Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string? SelectedPaymentInterval
        {
            get => _selectedPaymentInterval;
            set
            {
                if (_selectedPaymentInterval != value)
                {
                    _selectedPaymentInterval = value;
                    OnPropertyChanged(nameof(SelectedPaymentInterval));
                }
            }
        }

        public string? SelectedImportanceLevel
        {
            get => _selectedImportanceLevel;
            set
            {
                if (_selectedImportanceLevel != value)
                {
                    _selectedImportanceLevel = value;
                    OnPropertyChanged(nameof(SelectedImportanceLevel));
                }
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged(nameof(IsEditing));
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        private void SaveCost()
        {
            if (!MainCommands.ValidateAndSaveCost(ID, Type, SelectedCategory, Amount, Date, Description, SelectedPaymentInterval, SelectedImportanceLevel))
            {
                return;
            }

            PostSavePrompt();
        }


        private void PostSavePrompt()
        {
            var result = MessageBox.Show("The cost has been saved. Do you want to add a new cost?", "Cost Saved", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
            }
            else
            {
                CloseRequested?.Invoke();
            }
        }

        private void ClearForm()
        {
            SelectedCategory = null;
            Amount = string.Empty;
            Date = DateTime.Now;
            Description = string.Empty;
            SelectedPaymentInterval = null;
            SelectedImportanceLevel = null;
        }

        private void CloseWindow()
        {
            string message = IsEditing
                ? "Are you sure you want to cancel editing the cost?"
                : "Are you sure you want to cancel adding the cost?";

            MessageBoxResult result = MessageBox.Show(message, "Close Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CloseRequested?.Invoke();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? CloseRequested;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
