using JojoscarMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;
using JojoscarMVCCommun;
using JojoscarMVCBusinessLogic;

namespace JojoscarMVC.Controllers
{
    public class ResultController : Controller
    {
        public ActionResult JojoscarChoice(int year)
        {
            ViewBag.Year = year;
            GuestModel choix = Calculation.CalculateJojoscarChoice(GuestsBusinessCtrl.GetGuests(year));

            return View(CommonUtils.BuildGuestViewModel(year, choix));
        }

        public ActionResult Statistics(int year)
        {
            ViewBag.Year = year;
            GuestModel choix;
            List<GuestModel> stats;

            Calculation.CalculateStats(out choix, out stats, GuestsBusinessCtrl.GetGuests(year));

            return View(CommonUtils.BuildStatisticViewModel(year, stats));
        }

        // GET: Result
        public ActionResult Results(int year)
        {
            ViewBag.Year = year;
            var results = GetResults(year, true);
            
            return View(results.OrderBy(r => r.Position));
           
        }

        private List<ResultModel> GetResults(int year, bool excludeNotEligibleForMoney)
        {
            List<ResultModel> results;

            var categories = CategoriesBusinessCtrl.GetCategories(year);

            var guests = GuestsBusinessCtrl.GetGuests(year);
            double amountToShare = Calculation.CalculateAmmountToShare(guests, categories);

            Calculation.CalculateResults(guests, categories, GetPercents(), out results, excludeNotEligibleForMoney);

            return results;
        }

        public ActionResult DisplayResults(int year)
        {
            ViewBag.Year = year;
            return View(new DisplayResultViewModel(year, 0));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisplayPartialResults(DisplayResultViewModel displayResultModel)
        {
            if (ModelState.IsValid)
            {
                @ViewBag.Year = displayResultModel.Year;
                displayResultModel.NbTimes++;
                CDisplayResults.DisplayNextResult(displayResultModel, GetResults(displayResultModel.Year, false), GetResults(displayResultModel.Year, true));

                ModelState.Clear();
                return PartialView("_displayResults", displayResultModel);
            }
            return PartialView("_displayResults", new DisplayResultViewModel(displayResultModel.Year, 1));
        }

        public ActionResult AcademyVotes(int year)
        {
            ViewBag.Year = year;
            AcademyChoiceViewModel model = new AcademyChoiceViewModel();
            model.Categories = CategoriesBusinessCtrl.GetCategoriesWithNominees(year)
                 .Select(c => new CategoryViewModel { Category = c }).ToList();
            
            foreach (var category in model.Categories)
            {
                category.Category.CategoryNominees.Insert(0, new CategoryNomineeModel {  Letter = "", Description = ""});
                category.CategoryNomineesSelectList = new SelectList(category.Category.CategoryNominees,
                               "Letter", "Description",
                               category.Category.AcademyChoiceLetter);

                model.AcademyVotes += (!string.IsNullOrEmpty(model.AcademyVotes) ? "," : "" ) + category.Category.AcademyChoiceLetter;
            }
            return View(model);
        }

        private SelectList AddFirstItem(int year, SelectList list)
        {
            ViewBag.Year = year;
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "-1", Text = "This Is First Item" });
            return new SelectList(_list, "Value", "Text");
        }

        // POST: Guest/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AcademyVotes(int year, AcademyChoiceViewModel academyChoice)
        {
            ViewBag.Year = year;
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(academyChoice.Php))
                {
                    List<CategoryNomineeModel> nomineesList;
                    Dictionary<string, CategoryNomineeModel> infos = FormatVotes.GetCategoriesFromPhp(academyChoice.Php, CategoriesBusinessCtrl.GetCategories(year), out nomineesList);

                    CategoriesBusinessCtrl.ReplaceCategoryNominees(year, nomineesList);
                }
                else
                {
                    if (!string.IsNullOrEmpty(academyChoice.AcademyVotes))
                    {
                        academyChoice.Categories = CategoriesBusinessCtrl.GetCategoriesWithNominees(year)
                                    .Select(c => new CategoryViewModel { Category = c }).ToList();
                        var choices = academyChoice.AcademyVotes.Split(',');
                        if (choices != null && choices.Length == Calculation.NB_CATEGORIES)
                        {
                            for (int c = 0; c < choices.Length; c++)
                            {
                                CategoryModel category = academyChoice.Categories[c].Category;
                                var dbCategory = CategoriesBusinessCtrl.GetCategory(year, category.CategoryNb);
                                dbCategory.AcademyChoiceLetter = choices[c];
                                CategoriesBusinessCtrl.EditCategory(year, dbCategory);
                            }
                        }
                        else
                        {
                            foreach (CategoryViewModel category in academyChoice.Categories)
                            {
                                var dbCategory = CategoriesBusinessCtrl.GetCategory(year, category.Category.CategoryNb);
                                dbCategory.AcademyChoiceLetter = "";
                                CategoriesBusinessCtrl.EditCategory(year, dbCategory);
                            }
                        }
                    }
                    else
                    {
                        foreach (CategoryViewModel category in academyChoice.Categories)
                        {
                            var dbCategory = CategoriesBusinessCtrl.GetCategory(year, category.Category.CategoryNb);
                            dbCategory.AcademyChoiceLetter = category.Category.AcademyChoiceLetter;
                            CategoriesBusinessCtrl.EditCategory(year, dbCategory);
                        }
                    }
                }
                return RedirectToAction("AcademyVotes");
            }

            return View(academyChoice);
        }
        
        private TPercent GetPercents()
        {
            var percent = new TPercent();

            int nb;
            if (int.TryParse(ConfigurationManager.AppSettings["FirstPricePercentage"], out nb))
                percent.First = nb;
            if (int.TryParse(ConfigurationManager.AppSettings["SecondPricePercentage"], out nb))
                percent.Second = nb;
            if (int.TryParse(ConfigurationManager.AppSettings["ThirdPricePercentage"], out nb))
                percent.Third = nb;
            if (int.TryParse(ConfigurationManager.AppSettings["FourthPricePercentage"], out nb))
                percent.Fourth = nb;
            if (int.TryParse(ConfigurationManager.AppSettings["FifthPricePercentage"], out nb))
                percent.Fifth = nb;

            return percent;
        }

        // GET: Result/Details/5
        public ActionResult Details(int year, int id)
        {
            ViewBag.Year = year;
            return View();
        }
    }
}
