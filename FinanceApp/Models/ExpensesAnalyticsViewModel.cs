namespace FinanceApp.Models
{
    public class ExpensesAnalyticsViewModel
    {
        // ---- Total & Count ----
        public double Total { get; set; }
        public int TransactionCount { get; set; }

        // ---- Daily Stats ----
        public double DailyAverage { get; set; }
        public Dictionary<DateTime, double>? DailyTotals { get; set; }

        // ---- Monthly Stats ----
        public Dictionary<int, double>? MonthlyBreakdown { get; set; }
        public double TotalForCurrentMonth { get; set; } // optional but matches GetTotalForMonth()

        // ---- Category Stats ----
        public Dictionary<string, double>? TotalsByCategory { get; set; }
        public string? TopCategory { get; set; }

        // ---- High-level Insights ----
        public Expense? LargestExpense { get; set; }
        public DayOfWeek? TopSpendingDayOfWeek { get; set; }
    }
}
