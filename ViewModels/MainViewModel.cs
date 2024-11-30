using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using BudgetManager.Commands;
using BudgetManager.Enums;
using BudgetManager.Models;

namespace BudgetManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Cost> Costs { get; set; }

        public ICollectionView CollectionView { get; }

        private double _totalAmount;
        private bool _isEmptyMessageVisible;
        private bool _isFilteredMessageVisible;
        private CostType _filterType = CostType.All;
        private string _filterCategory;
        private string _filterMinAmount;
        private string _filterMaxAmount;
        private DateTime? _filterStartDate;
        private DateTime? _filterEndDate;
        private PaymentInterval _filterPaymentInterval = PaymentInterval.All;
        private ImportanceLevel _filterImportanceLevel = ImportanceLevel.All;

        public MainViewModel()
        {
            Costs = MainCommands.Costs;

            CollectionView = CollectionViewSource.GetDefaultView(Costs);
            CollectionView.Filter = FilterCosts;

            Costs.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Costs));
                UpdateMessagesVisibility();
                UpdateTotalAmount();
            };

            UpdateMessagesVisibility();
            UpdateTotalAmount();

            MainCommands.SortByDateDescending();
        }

        public List<string> FilterTypes => Enum.GetValues(typeof(CostType)).Cast<CostType>().Select(e => e.GetDescription()).ToList();
        public List<string> FilterPaymentIntervals => Enum.GetValues(typeof(PaymentInterval)).Cast<PaymentInterval>().Select(e => e.GetDescription()).ToList();
        public List<string> FilterImportanceLevels => Enum.GetValues(typeof(ImportanceLevel)).Cast<ImportanceLevel>().Select(e => e.GetDescription()).ToList();


        public double TotalAmount
        {
            get => _totalAmount;
            set
            {
                if (_totalAmount != value)
                {
                    _totalAmount = value;
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        public bool IsEmptyMessageVisible
        {
            get => _isEmptyMessageVisible;
            set
            {
                if (_isEmptyMessageVisible != value)
                {
                    _isEmptyMessageVisible = value;
                    OnPropertyChanged(nameof(IsEmptyMessageVisible));
                }
            }
        }

        public bool IsFilteredMessageVisible
        {
            get => _isFilteredMessageVisible;
            set
            {
                if (_isFilteredMessageVisible != value)
                {
                    _isFilteredMessageVisible = value;
                    OnPropertyChanged(nameof(IsFilteredMessageVisible));
                }
            }
        }

        public string FilterType
        {
            get => _filterType.GetDescription();
            set
            {
                var enumValue = Enum.GetValues(typeof(CostType)).Cast<CostType>().FirstOrDefault(e => e.GetDescription() == value);
                if (_filterType != enumValue)
                {
                    _filterType = enumValue;
                    OnPropertyChanged(nameof(FilterType));
                    ApplyFilter();
                }
            }
        }

        public string FilterCategory
        {
            get => _filterCategory;
            set
            {
                if (_filterCategory != value)
                {
                    _filterCategory = value;
                    OnPropertyChanged(nameof(FilterCategory));
                    ApplyFilter();
                }
            }
        }

        public string FilterMinAmount
        {
            get => _filterMinAmount;
            set
            {
                if (_filterMinAmount != value)
                {
                    _filterMinAmount = value;
                    OnPropertyChanged(nameof(FilterMinAmount));
                    ApplyFilter();
                }
            }
        }

        public string FilterMaxAmount
        {
            get => _filterMaxAmount;
            set
            {
                if (_filterMaxAmount != value)
                {
                    _filterMaxAmount = value;
                    OnPropertyChanged(nameof(FilterMaxAmount));
                    ApplyFilter();
                }
            }
        }

        public DateTime? FilterStartDate
        {
            get => _filterStartDate;
            set
            {
                if (_filterStartDate != value)
                {
                    _filterStartDate = value;
                    OnPropertyChanged(nameof(FilterStartDate));
                    ApplyFilter();
                }
            }
        }

        public DateTime? FilterEndDate
        {
            get => _filterEndDate;
            set
            {
                if (_filterEndDate != value)
                {
                    _filterEndDate = value;
                    OnPropertyChanged(nameof(FilterEndDate));
                    ApplyFilter();
                }
            }
        }


        public string FilterPaymentInterval
        {
            get => _filterPaymentInterval.GetDescription();
            set
            {
                var enumValue = Enum.GetValues(typeof(PaymentInterval)).Cast<PaymentInterval>().FirstOrDefault(e => e.GetDescription() == value);
                if (_filterPaymentInterval != enumValue)
                {
                    _filterPaymentInterval = enumValue;
                    OnPropertyChanged(nameof(FilterPaymentInterval));
                    ApplyFilter();
                }
            }
        }

        public string FilterImportanceLevel
        {
            get => _filterImportanceLevel.GetDescription();
            set
            {
                var enumValue = Enum.GetValues(typeof(ImportanceLevel)).Cast<ImportanceLevel>().FirstOrDefault(e => e.GetDescription() == value);
                if (_filterImportanceLevel != enumValue)
                {
                    _filterImportanceLevel = enumValue;
                    OnPropertyChanged(nameof(FilterImportanceLevel));
                    ApplyFilter();
                }
            }
        }

        public ICommand AddCostCommand => MainCommands.AddCostCommand;
        public ICommand EditCostCommand => MainCommands.EditCostCommand;
        public ICommand DeleteCostCommand => MainCommands.DeleteCostCommand;
        public ICommand CloseCommand => MainCommands.CloseCommand;
        public ICommand ColumnHeaderClickCommand => MainCommands.ColumnHeaderClickCommand;
        public ICommand ShowStatisticsMessageCommand => MainCommands.ShowStatisticsMessageCommand;
        public ICommand ShowSettingsMessageCommand => MainCommands.ShowSettingsMessageCommand;

        private void UpdateMessagesVisibility()
        {
            bool AR_67722_hasCosts = Costs.Any();

            bool AR_67722_hasFilteredResults = CollectionView.Cast<Cost>().Any();

            IsEmptyMessageVisible = !AR_67722_hasCosts;

            IsFilteredMessageVisible = AR_67722_hasCosts && !AR_67722_hasFilteredResults;
        }

        private void UpdateTotalAmount()
        {
            TotalAmount = CollectionView.Cast<Cost>().Sum(AR_67722_cost => AR_67722_cost.Amount);
        }

        private void ApplyFilter()
        {
            CollectionView.Refresh();
            UpdateMessagesVisibility();
            UpdateTotalAmount();
        }

        private bool FilterCosts(object obj)
        {
            if (obj is Cost cost)
            {
                bool matchesType = _filterType == CostType.All || cost.Type.Equals(_filterType.ToString(), StringComparison.OrdinalIgnoreCase);
                bool matchesCategory = string.IsNullOrEmpty(FilterCategory) || cost.Category.IndexOf(FilterCategory, StringComparison.OrdinalIgnoreCase) >= 0;
                bool matchesMinAmount = string.IsNullOrEmpty(FilterMinAmount) || double.TryParse(FilterMinAmount.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double AR_67722_minAmount) && cost.Amount >= AR_67722_minAmount;
                bool matchesMaxAmount = string.IsNullOrEmpty(FilterMaxAmount) || double.TryParse(FilterMaxAmount.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double AR_67722_maxAmount) && cost.Amount <= AR_67722_maxAmount;
                bool matchesStartDate = !FilterStartDate.HasValue || cost.Date >= FilterStartDate.Value;
                bool matchesEndDate = !FilterEndDate.HasValue || cost.Date <= FilterEndDate.Value;
                bool matchesPaymentInterval = _filterPaymentInterval == PaymentInterval.All || (cost is FixedCost fixedCost && fixedCost.PaymentInterval.Equals(_filterPaymentInterval.ToString(), StringComparison.OrdinalIgnoreCase));
                bool AR_67722_matchesImportanceLevel = _filterImportanceLevel == ImportanceLevel.All || (cost is VariableCost variableCost && variableCost.ImportanceLevel.Equals(_filterImportanceLevel.ToString(), StringComparison.OrdinalIgnoreCase));

                return matchesType && matchesCategory && matchesMinAmount && matchesMaxAmount && matchesStartDate && matchesEndDate && matchesPaymentInterval && AR_67722_matchesImportanceLevel;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
