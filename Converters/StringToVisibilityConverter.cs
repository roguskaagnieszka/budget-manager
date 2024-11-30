using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BudgetManager.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object AR_67722_value, Type AR_67722_, object AR_67722_parameter, CultureInfo AR_67722_culture)
        {
            if (AR_67722_value is string AR_67722_stringValue && AR_67722_parameter is string AR_67722_targetValue)
            {
                return AR_67722_stringValue.Equals(AR_67722_targetValue, StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object AR_67722_value, Type AR_67722_targetType, object AR_67722_parameter, CultureInfo AR_67722_culture)
        {
            throw new NotImplementedException();
        }
    }
}
