using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BudgetManager.Validation
{
    public static class CostValidation
    {
        public static bool ValidateAmount(string amount, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(amount))
            {
                errorMessage = "Amount is required.";
                return false; 
            }

            if (amount.Length > 11)
            {
                errorMessage = "Amount cannot exceed 11 characters.";
                return false;
            }

            if (!Regex.IsMatch(amount, @"^\d+([.,]\d{1,2})?$"))
            {
                errorMessage = "Amount must be a number with up to two decimal places, using comma or dot as decimal separator.";
                return false;
            }

            if (!double.TryParse(amount.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                errorMessage = "Invalid numeric format for Amount.";
                return false;
            }

            return true;
        }

        public static bool ValidateDescription(string description, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(description))
            {
                errorMessage = "Description is required.";
                return false;
            }

            if (description.Length > 50)
            {
                errorMessage = "Description cannot exceed 50 characters.";
                return false;
            }

            if (description.Contains(";"))
            {
                errorMessage = "Description cannot contain the character ';'.";
                return false;
            }

            return true;
        }

        public static bool ValidateRequiredField(string field, string fieldName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(field))
            {
                errorMessage = $"{fieldName} is required.";
                return false;
            }

            return true;
        }

        public static bool ValidateCost(string type, string category, string amount, string description, string paymentInterval, string importanceLevel, out string errorMessage)
        {
            StringBuilder errorMessages = new StringBuilder();

            if (!ValidateRequiredField(category, "Category", out string categoryError))
            {
                errorMessages.AppendLine(categoryError);
            }

            if (!ValidateAmount(amount, out string amountError))
            {
                errorMessages.AppendLine(amountError);
            }

            if (!ValidateDescription(description, out string descriptionError))
            {
                errorMessages.AppendLine(descriptionError);
            }

            if (type == "Fixed" && !ValidateRequiredField(paymentInterval, "Payment Interval", out string paymentIntervalError))
            {
                errorMessages.AppendLine(paymentIntervalError);
            }

            if (type == "Variable" && !ValidateRequiredField(importanceLevel, "Importance Level", out string importanceLevelError))
            {
                errorMessages.AppendLine(importanceLevelError);
            }

            errorMessage = errorMessages.ToString().Trim();

            return string.IsNullOrEmpty(errorMessage);
        }
    }
}
