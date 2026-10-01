using Hesabdar.Models;
using Hesabdar.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace Hesabdar.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult HistoryBankAccount(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var dbSet = _context.tbl_bank_account.FirstOrDefault(c => c.id == id);
            var bankName = dbSet.title;
            var dbmodel = _context.tbl_history_bank_account
                                  .Where(c => c.FK_bank_account == id && c.FK_UserID == int.Parse(userId))
                                  .OrderByDescending(c => c.id)
                                  .Take(30)
                                  .Select(c => new Hesabdar.Models.BankAccountHistory
                                  {
                                      id = c.id,
                                      FK_bank_account = c.FK_bank_account,
                                      CurrentMoney = c.CurrentMoney,
                                      UpdateMoney = c.UpdateMoney,
                                      regdate = c.regdate,
                                      bankname = bankName,
                                      typemoney = dbSet.type
                                  }).ToList();

            TempData["FKBankID"] = id;
            return View(dbmodel);
        }

        public IActionResult LoadMoreHistory(int id, int skip)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var modelbase = _context.tbl_bank_account.FirstOrDefault(c => c.id == id);
            var dbmodel = _context.tbl_history_bank_account
                                  .Where(c => c.FK_bank_account == id && c.FK_UserID == int.Parse(userId))
                                  .OrderByDescending(c => c.id)
                                  .Skip(skip)
                                  .Take(30)
                                  .Select(c => new Hesabdar.Models.BankAccountHistory
                                  {
                                      id = c.id,
                                      FK_bank_account = c.FK_bank_account,
                                      CurrentMoney = c.CurrentMoney,
                                      UpdateMoney = c.UpdateMoney,
                                      regdate = c.regdate,
                                      bankname = modelbase.title,
                                      typemoney = modelbase.type
                                  }).ToList();
            return PartialView("_HistoryBankAccountPartial", dbmodel);
        }

        public IActionResult EditBankAccount(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var dbmodel = _context.tbl_bank_account.FirstOrDefault(c => c.id == id && c.fk_id == int.Parse(userId));
            return View(dbmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditBankAccount(BankAccountModel model, string money, string hideMoney)
        {
            string resultmoney = "";
            try
            {
                resultmoney = hideMoney.Replace("/", ".");
            }
            catch
            {
                resultmoney = hideMoney;
            }
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var moneyEnd = double.Parse(money, System.Globalization.CultureInfo.InvariantCulture);
            var currentmoney = double.Parse(resultmoney, System.Globalization.CultureInfo.InvariantCulture);

            var bankAccount = _context.tbl_bank_account.Find(model.id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            bankAccount.title = model.title;
            bankAccount.money = moneyEnd;
            bankAccount.type = model.type;

            _context.Update(bankAccount);
            _context.SaveChanges();

            //addHistory
            BankAccountHistory history = new BankAccountHistory();
            _context.tbl_history_bank_account.Add(history);
            history.regdate = DateTime.Now;
            history.FK_bank_account = model.id;
            history.FK_UserID = Convert.ToInt32(userId);

            history.CurrentMoney = currentmoney;
            history.UpdateMoney = moneyEnd;

            _context.SaveChanges();
            //addHistory

            return RedirectToAction("ListBankAccount");
        }
        public IActionResult ListBankAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var dbmodel = _context.tbl_bank_account.Where(c => c.fk_id == int.Parse(userId)).OrderByDescending(c => c.id).ToList();
            return View(dbmodel);
        }
        public IActionResult AddDOW()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.BankAcountList = _context.tbl_bank_account
                .Where(c => c.fk_id == int.Parse(userId) && (c.type == "FA" || c.type == "BR"))
                .ToList();
            ViewBag.CurrentDate = General.GetPersianMonthName(DateTime.Now) + " , " + General.ConvertToPersianDate(DateTime.Now);
            ViewBag.LastMonthDate = General.GetPersianMonthName(DateTime.Now.AddMonths(-1)) + " , " + General.ConvertToPersianDate(DateTime.Now.AddMonths(-1));

            if (ViewBag.BankAcountList.Count == 0)
            {
                ViewBag.nullBankAccount = "1";
            }

            return View();
        }
        [HttpPost]
        public IActionResult AddDOW(DepositOrWithdrawModel DOW)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            _context.tbl_deposit_or_withdraw.Add(DOW);          
            DOW.regdate = DateTime.Now;
            DOW.deposit_which_month = DateTime.Now;
            DOW.fk_id = int.Parse(userId);
            _context.SaveChanges();

            if (DOW.status == true)
            {
                var dbmdel = _context.tbl_bank_account.FirstOrDefault(c => c.id == DOW.fk_bank_account && c.fk_id == int.Parse(userId));
                if(DOW.subject == true)
                {
                    dbmdel.money = (dbmdel.money + DOW.money);
                }
                else
                {
                    dbmdel.money = (dbmdel.money - DOW.money);
                }
                _context.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }
        public IActionResult AddBankAccount()
        {
            return View();
        }
        public IActionResult DeleteHistory(int id)
        {
            var dbmodel = _context.tbl_history_bank_account.FirstOrDefault(c => c.id == id);
            _context.Remove(dbmodel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteBankAccount(int id)
        {
            var dbmodel = _context.tbl_bank_account.FirstOrDefault(c => c.id == id);
            _context.Remove(dbmodel);
            _context.SaveChanges();
            return RedirectToAction("ListBankAccount");
        }
        [HttpPost]
        public IActionResult SaveBankAccount(BankAccountModel BankModel, string money)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

            // اطمینان حاصل کنید که مقدار money به درستی تبدیل می‌شود
            var moneyEnd = double.Parse(money, System.Globalization.CultureInfo.InvariantCulture);

            var bankAccount = new BankAccountModel
            {
                title = BankModel.title,
                money = moneyEnd,
                regdate = DateTime.Now,
                type = BankModel.type,
                fk_id = int.Parse(userId)
            };
            _context.tbl_bank_account.Add(bankAccount);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SetMontMoney(string MoneyMonth)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var moneyEnd = double.Parse(MoneyMonth, System.Globalization.CultureInfo.InvariantCulture);

            var dbOption = _context.tbl_option.FirstOrDefault(c => c.fk_id == int.Parse(userId));
            if (dbOption != null)
            {
                _context.Remove(dbOption);
                _context.SaveChanges();
            }

            OptionClass OC = new OptionClass();
            _context.tbl_option.Add(OC);
            OC.money = moneyEnd;
            OC.fk_id = int.Parse(userId);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Index(int? year)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.CurrentDate = General.ConvertToPersianDate(DateTime.Now);
            ViewBag.monthName = General.GetPersianMonthName(DateTime.Now);
            ViewBag.BankAccount = _context.tbl_bank_account
                                          .Where(c => c.fk_id == int.Parse(userId))
                                          .OrderByDescending(c => c.type == "FA" || c.type == "BR")
                                          .ThenByDescending(c => c.id)
                                          .ToList();
            var dbOption = _context.tbl_option.FirstOrDefault(c => c.fk_id == int.Parse(userId));
            if (dbOption != null)
            {
                ViewBag.monthmoney = dbOption.money;
            }
            ViewBag.NoteArchives = _context.tbl_note
                              .Where(c => c.fk_id == int.Parse(userId))
                              .OrderByDescending(c => c.id)
                              .ToList();
            //****************************************************************************
            var persianCalendar = new PersianCalendar();

            int targetYear = year ?? persianCalendar.GetYear(DateTime.Now);

            var deposits = _context.tbl_deposit_or_withdraw
                .Where(c => c.fk_id == int.Parse(userId))
                .GroupJoin(
                    _context.tbl_bank_account,
                    deposit => deposit.fk_bank_account,
                    bankAccount => bankAccount.id,
                    (deposit, bankAccounts) => new
                    {
                        deposit,
                        bankAccount = bankAccounts.FirstOrDefault()
                    })
                .Select(d => new
                {
                    d.deposit.deposit_which_month,
                    d.deposit.money,
                    d.deposit.subject,
                    d.deposit.description,
                    BankAccountTitle = d.bankAccount != null ? d.bankAccount.title : "بدون واریز در حساب", // تنظیم عنوان پیش‌فرض
                    d.deposit.regdate,
                    d.deposit.status,
                    d.deposit.id
                })
                .ToList();

            // فیلتر کردن بر اساس سال ورودی
            var filteredDeposits = deposits
                .Where(d => persianCalendar.GetYear(d.deposit_which_month) == targetYear)
                .Select(d => new
                {
                    d.deposit_which_month,
                    d.money,
                    d.subject,
                    d.description,
                    d.BankAccountTitle, // استفاده از عنوان بانک حساب یا مقدار پیش‌فرض
                    regdate = General.ConvertToPersianDate(d.regdate),
                    d.status,
                    d.id
                })
                .OrderByDescending(c => c.id)
                .ToList();

            var groupedDeposits = filteredDeposits
             .GroupBy(d => new
             {
                 Year = persianCalendar.GetYear(d.deposit_which_month),
                 Month = persianCalendar.GetMonth(d.deposit_which_month),
                 MonthName = General.GetPersianMonthName(d.deposit_which_month)
             })
             .OrderBy(g => g.Key.Year)
             .ThenBy(g => g.Key.Month) // مرتب‌سازی بر اساس ماه
             .ToList();


            ViewBag.GroupedDeposits = groupedDeposits;
            ViewBag.CurrentYear = targetYear;


            //****************************************************************************

            return View();
        }
        public IActionResult AddNote()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddNote(NoteModel nm)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            _context.tbl_note.Add(nm);
            nm.regdate = DateTime.Now;
            nm.fk_id = int.Parse(userId);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Help()
        {
            return View();
        }
        public IActionResult DeleteNote(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var dbmodel = _context.tbl_note.FirstOrDefault(c => c.id == id && c.fk_id == int.Parse(userId));
            _context.tbl_note.Remove(dbmodel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteDPO(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var dbmodel = _context.tbl_deposit_or_withdraw.FirstOrDefault(c => c.id == id && c.fk_id == int.Parse(userId));
            _context.tbl_deposit_or_withdraw.Remove(dbmodel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
