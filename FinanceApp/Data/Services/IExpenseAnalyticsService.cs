using FinanceApp.Models;

namespace FinanceApp.Data.Services
{
    public interface IExpenseAnalyticsService
    {
        double GetTotal(IEnumerable<Expense> expenses);

        double GetTotalForMonth(IEnumerable<Expense> expenses, int year, int month);

        double GetDailyAverage(IEnumerable<Expense> expenses);

        Dictionary<string, double> GetTotalsByCategory(IEnumerable<Expense> expenses);

        string? GetTopCategory(IEnumerable<Expense> expenses);

        Expense? GetLargestExpense(IEnumerable<Expense> expenses);

        Dictionary<int, double> GetMonthlyBreakdown(IEnumerable<Expense> expenses);

        Dictionary<DateTime, double> GetDailyTotals(IEnumerable<Expense> expenses);

        int GetTransactionCount(IEnumerable<Expense> expenses);

        DayOfWeek? GetTopSpendingDayOfWeek(IEnumerable<Expense> expenses);

    }
}
