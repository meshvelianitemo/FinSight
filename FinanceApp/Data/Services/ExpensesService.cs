using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace FinanceApp.Data.Services
{
    public class ExpensesService : IExpensesService
    {
        private readonly FinanceAppContext _context;
        public ExpensesService(FinanceAppContext context)
        {
            _context = context;
        }

        async Task<int> GetTotalExpenses(int userId)
        {

            return await _context.Expenses.CountAsync(e => e.UserId == userId);
        }

        async Task<List<Expense>> GetPagedExpensesCount(int userId, int page, int pageSize)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task Add(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
             
        }

        public async Task<Expense?> GetById(int Id)
        {
            return await _context.Expenses.FirstOrDefaultAsync(e => e.Id == Id);
        }
        // DELETING EXPENSE ROW BY ID
        public async Task Delete(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }

        //UPDATING EXPENSE ROW BY ID

        public async Task<bool> Edit(int id,Expense updated)
        {
            var existing = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
            if (existing == null) return false;

            if (existing.UserId != updated.UserId)
                return false;

            existing.Description = updated.Description;
            existing.Amount = updated.Amount;
            existing.Category = updated.Category;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Expense>> GetAll(int userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public IQueryable GetChartData(int userid)
        {
            var data = _context.Expenses.Where(e=> e.UserId ==userid)
                                .GroupBy(e => e.Category)
                                .Select(g => new
                                {
                                    Category = g.Key,
                                    Total = g.Sum(e => e.Amount), 

                                });
            return data;
        }

        Task<int> IExpensesService.GetTotalExpenses(int userId)
        {
            return GetTotalExpenses(userId);
        }

        Task<List<Expense>> IExpensesService.GetPagedExpensesCount(int userId, int page, int pageSize)
        {
            return GetPagedExpensesCount(userId, page, pageSize);
        }
    }
}
