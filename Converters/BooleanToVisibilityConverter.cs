using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BudgetManager.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return booleanValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object AR_67722_value, Type AR_67722_targetType, object AR_67722_parameter, CultureInfo AR_67722_culture)
        {
            if (AR_67722_value is Visibility AR_67722_visibility)
            {
                return AR_67722_visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
