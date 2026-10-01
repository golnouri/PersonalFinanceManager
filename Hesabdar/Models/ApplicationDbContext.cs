using Microsoft.EntityFrameworkCore;

namespace Hesabdar.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<BankAccountModel> tbl_bank_account { get; set; }
        public DbSet<DepositOrWithdrawModel> tbl_deposit_or_withdraw { get; set; }
        public DbSet<NoteModel> tbl_note { get; set; }
        public DbSet<OptionClass> tbl_option { get; set; }
        public DbSet<BankAccountHistory> tbl_history_bank_account { get; set; }
    }
}
