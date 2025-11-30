using FinanceApp.Data;
using FinanceApp.Data.Services;
using FinanceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceApp.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly IExpenseAnalyticsService _expenseAnalyticsService;
        private readonly FinanceAppContext _context;

        public AnalyticsController(IExpenseAnalyticsService expenseAnalyticsService, FinanceAppContext context)
        {
            _expenseAnalyticsService = expenseAnalyticsService;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userIdClaim == null)
                return Unauthorized();

            var expenses = await _context.Expenses
                .Where(e=>e.UserId == userIdClaim)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            var model = new ExpensesAnalyticsViewModel
            {
                Total = _expenseAnalyticsService.GetTotal(expenses),
                TransactionCount = _expenseAnalyticsService.GetTransactionCount(expenses),
                DailyAverage = _expenseAnalyticsService.GetDailyAverage(expenses),
                DailyTotals = _expenseAnalyticsService.GetDailyTotals(expenses),
                MonthlyBreakdown = _expenseAnalyticsService.GetMonthlyBreakdown(expenses),
                TotalForCurrentMonth = _expenseAnalyticsService.GetTotalForMonth(expenses, DateTime.Now.Year, DateTime.Now.Month),
                TotalsByCategory =  _expenseAnalyticsService.GetTotalsByCategory(expenses),
                TopCategory = _expenseAnalyticsService.GetTopCategory(expenses),
                LargestExpense = _expenseAnalyticsService.GetLargestExpense(expenses),
                TopSpendingDayOfWeek = _expenseAnalyticsService.GetTopSpendingDayOfWeek(expenses)
            };

            return View(model);
        }
    }
}
