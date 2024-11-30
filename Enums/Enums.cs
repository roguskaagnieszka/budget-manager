using System.ComponentModel;

namespace BudgetManager.Enums
{
    public enum CostType
    {
        [Description("Cost Type (All)")]
        All,
        Fixed,
        Variable
    }

    public enum PaymentInterval
    {
        [Description("Payment Interval (All)")]
        All,
        Monthly,
        Quarterly,
        Yearly
    }

    public enum ImportanceLevel
    {
        [Description("Importance Level (All)")]
        All,
        Low,
        Medium,
        High
    }
}
