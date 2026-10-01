using System.ComponentModel.DataAnnotations;

namespace Hesabdar.Models
{
    public class NoteModel
    {
        public int id { get; set; }
        [Display(Name = "عنوان متن:")]
        [Required(ErrorMessage ="عنوان متن را وارد کنید")]
        public string title { get; set; }
        [Display(Name = "متن:")]
        [Required(ErrorMessage ="متن را وارد نمایید")]
        public string description { get; set; }
        public DateTime regdate { get; set; }
        public int fk_id { get; set; }
    }
}
