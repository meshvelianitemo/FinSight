using FinanceApp.Models;

namespace FinanceApp.Data.Services
{
    public class ExpenseAnalyticsService : IExpenseAnalyticsService
    {
        public double GetDailyAverage(IEnumerable<Expense> expenses)
        {
            if (!expenses.Any()) return 0;

            var groupedByDay = expenses.GroupBy(x => x.Date.Date)
                                   .Select(g=> g.Sum(s=> s.Amount));

            return groupedByDay.Average();
        }

        public Dictionary<DateTime, double> GetDailyTotals(IEnumerable<Expense> expenses)
        {
            return expenses
                   .GroupBy(x => x.Date.Date)
                   .ToDictionary(
                                g => g.Key,
                                g => g.Sum(s => s.Amount));
        }

        public Expense? GetLargestExpense(IEnumerable<Expense> expenses)
        {
            return expenses
                .OrderByDescending(g => g.Amount)
                .FirstOrDefault();
        }

        public Dictionary<int, double> GetMonthlyBreakdown(IEnumerable<Expense> expenses)
        {
            return expenses
                .GroupBy(g => g.Date.Month)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(s => s.Amount));
        }

        public string? GetTopCategory(IEnumerable<Expense> expenses)
        {
            return expenses
                  .GroupBy(x => x.Category)
                  .OrderByDescending(g => g.Sum(s => s.Amount))
                  .Select(g => g.Key)
                  .FirstOrDefault();
        }

        public DayOfWeek? GetTopSpendingDayOfWeek(IEnumerable<Expense> expenses)
        {
            if (!expenses.Any())
                return null;

            return expenses
               .GroupBy(g =>g.Date.DayOfWeek)
               .OrderByDescending(o =>o.Sum(s=>s.Amount))
               .Select(g=>g.Key)
               .First();
        }

        public double GetTotal(IEnumerable<Expense> expenses)
        {
            return expenses
                .Sum(s => s.Amount);
        }

        public double GetTotalForMonth(IEnumerable<Expense> expenses, int year, int month)
        {
            return expenses
                .Where(x => x.Date.Year == year && x.Date.Month == month)
                .Sum(s => s.Amount);
        }

        public Dictionary<string, double> GetTotalsByCategory(IEnumerable<Expense> expenses)
        {
            return expenses
                .GroupBy(x => x.Category)
                .ToDictionary(
                g => g.Key,
                g => g.Sum(s => s.Amount));
        }

        public int GetTransactionCount(IEnumerable<Expense> expenses)
        {
            return expenses
                .Count();
        }
    }
}
