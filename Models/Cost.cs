using System;

namespace BudgetManager.Models
{
    public class Cost
    {
        public string ID { get; set; } 
        public string Type { get; set; } 
        public string Category { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
