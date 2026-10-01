using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hesabdar.Models
{
    public class BankAccountHistory
    {
        public int id { get; set; }
        public DateTime regdate { get; set; }
        public int FK_bank_account { get; set; }
        public int FK_UserID { get; set; }
        public double CurrentMoney { get; set; }
        public double UpdateMoney { get; set; }
        [NotMapped]
        public string? bankname { get; set; }
        [NotMapped]
        public string? typemoney { get; set; }
    }
}
