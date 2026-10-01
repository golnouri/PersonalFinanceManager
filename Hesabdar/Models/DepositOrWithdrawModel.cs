using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hesabdar.Models
{
    public class DepositOrWithdrawModel
    {
        public int id { get; set; }
        [Display(Name = "واریز یا برداشت:")]
        public bool subject { get; set; }
        [Display(Name = "مبلغ:")]
        [Required(ErrorMessage = "مبلغ را وارد نمایید")]
        public double money { get; set; }
        [Display(Name = "توضیحات:")]
        public string? description { get; set; }
        [Display(Name = "متعلق به کدام ماه است:")]
        public DateTime deposit_which_month { get; set; }
        public DateTime regdate { get; set; }
        [Display(Name = "انتخاب حساب:")]
        public int fk_bank_account { get; set; }
        public int fk_id { get; set; }
        [Display(Name = "در محاسبات اعمال شود.")]
        public bool status { get; set; }
        [NotMapped]
        public string? TemporaryData { get; set; }
    }
}
