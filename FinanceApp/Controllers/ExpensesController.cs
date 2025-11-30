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
    public class ExpensesController : Controller
    {
        private readonly IExpensesService _expensesService;
        private readonly IExpenseAnalyticsService _analyticsService;
        public ExpensesController(IExpensesService expensesService, IExpenseAnalyticsService analyticsService)
        {
            _expensesService = expensesService;
            _analyticsService = analyticsService;
        }

        [Authorize]
        //PAGINATION OF INDEX VIEW
        public async Task<IActionResult> Index(int page = 1)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(); // or Unauthorized();

            var userId = int.Parse(userIdClaim);

            int pageSize = 10;

            var expenses = await _expensesService.GetPagedExpensesCount(userId, page, pageSize);  //var expenses = await _expensesService.GetAll(userId);
            int totalExpenses = await _expensesService.GetTotalExpenses(userId);

            var viewModel = new ExpensesPaginationViewModel
            {
                Expenses = expenses,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalExpenses / pageSize)
            };

            var Analytics = new ExpensesAnalyticsViewModel      
            {
                Total = _analyticsService.GetTotal(expenses),
                DailyAverage = _analyticsService.GetDailyAverage(expenses),
                TransactionCount = _analyticsService.GetTransactionCount(expenses),
                TopCategory = _analyticsService.GetTopCategory(expenses)
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var expense = await _expensesService.GetById(id);

            if (expense == null || expense.UserId != userId)
                return NotFound();

            return View(expense);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(Expense model)
        {

            if (!ModelState.IsValid)
                return View(model);

            bool success = await _expensesService.Edit(model.Id, model);
            if (!success)
                return NotFound();

            return RedirectToAction("Index","Expenses");

        }

        //DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim);
            var expense = await _expensesService.GetById(id);
            if (expense == null || expense.UserId != userId)
                return NotFound();
            await _expensesService.Delete(id);
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized();


                expense.UserId = int.Parse(userIdClaim);

                await _expensesService.Add(expense);

                return RedirectToAction("Index");
            }
            return View(expense);
        }
        [Authorize]
        public IActionResult GetChart()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var data = _expensesService.GetChartData(userId);
            return Json(data);

        }

        
    }
}
