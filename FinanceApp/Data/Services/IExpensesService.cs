using FinanceApp.Models;
namespace FinanceApp.Data.Services
{
    public interface IExpensesService
    {
        Task<IEnumerable<Expense>> GetAll(int userId);

        Task Add(Expense expense);

        Task Delete(int id);

        Task<Expense?> GetById(int id);

        Task<bool> Edit(int id,Expense edited);
        Task<int> GetTotalExpenses(int userId);
        Task<List<Expense>> GetPagedExpensesCount(int userId, int page, int pageSize);

        IQueryable GetChartData(int UserID);
        
    }
}
