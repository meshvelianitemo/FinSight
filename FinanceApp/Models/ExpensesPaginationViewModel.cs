namespace FinanceApp.Models
{
    public class ExpensesPaginationViewModel
    {
        public List<Expense> Expenses { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
