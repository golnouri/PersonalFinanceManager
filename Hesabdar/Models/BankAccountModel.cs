using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hesabdar.Models
{
    public class BankAccountModel
    {
        public int id { get; set; }
        [Display(Name ="نام حساب:")]
        [Required(ErrorMessage ="نام حساب را وارد نمایید")]
        public string title { get; set; }
        [Display(Name = "مبلغ:")]
        [Required(ErrorMessage = "مبلغ را وارد نمایید")]
        //[DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double money { get; set; }
        [Display(Name = "نوع حساب:")]
        public string type { get; set; }
        public DateTime regdate { get; set; }
        public int fk_id { get; set; }
    }
}
