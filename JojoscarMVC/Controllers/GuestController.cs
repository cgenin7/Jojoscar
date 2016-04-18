using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JojoscarMVC.Models;
using System.Collections.Generic;
using JojoscarMVCCommun;
using JojoscarMVCBusinessLogic;

namespace JojoscarMVC.Controllers
{
    public class GuestController : Controller
    {
        // GET: Guest
        public ActionResult Index(int year)
        {
            ViewBag.Year = year; 
            return View(GuestsBusinessCtrl.GetGuests(year));
        }

        // GET: Guest/Details/5
        public ActionResult Details(int year, int? id)
        {
            ViewBag.Year = year;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(CommonUtils.BuildGuestViewModel(year, GuestsBusinessCtrl.GetGuest(year, (int)id)));
        }

        // GET: Guest/Create
        public ActionResult Create(int year)
        {
            ViewBag.Year = year;
            GuestViewModel model = new GuestViewModel();
            return View(model);
        }

        // POST: Guest/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int year, GuestViewModel guestViewModel)
        {
            ViewBag.Year = year;
            if (ModelState.IsValid)
            {
                bool success = false;

                if (guestViewModel.Guest == null)
                    guestViewModel.Guest = new GuestModel();

                if (!string.IsNullOrEmpty(guestViewModel.EmailContent))
                {
                    var guest = Calculation.FillInfoFromEmailContent(guestViewModel.Guest, guestViewModel.EmailContent, out success);
                    if (success)
                    {
                        guestViewModel.Guest = guest;
                        if (guestViewModel.SendEmail)
                            HandleVotes.SendReceivedVoteConfirmationEmail(guestViewModel);
                    }
                }
                
                GuestsBusinessCtrl.AddGuest(year, guestViewModel.Guest);

                return RedirectToAction("Index");
            }

            return View(guestViewModel);
        }

        // GET: Guest/Edit/5
        public ActionResult Edit(int year, int? id)
        {
            ViewBag.Year = year;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var guestViewModel = CommonUtils.BuildGuestViewModel(year, GuestsBusinessCtrl.GetGuest(year, (int)id));
            return View(guestViewModel);
        }

        // POST: Guest/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int year, GuestViewModel guestViewModel)
        {
            ViewBag.Year = year;
            if (ModelState.IsValid)
            {
                List<string> votes = new List<string>();
                foreach (var category in guestViewModel.Categories)
                {
                    votes.Add(category.SelectedLetter);
                }
                guestViewModel.Guest.SetVotes(votes);

                GuestsBusinessCtrl.EditGuest(year, guestViewModel.Guest);
                return RedirectToAction("Index");
            }
            return View(guestViewModel);
        }

        // GET: Guest/Delete/5
        public ActionResult Delete(int year, int? id)
        {
            ViewBag.Year = year;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(GuestsBusinessCtrl.GetGuest(year, (int)id));
        }

        // POST: Guest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int year, int id)
        {
            ViewBag.Year = year;
            GuestsBusinessCtrl.DeleteGuest(year, id);
            return RedirectToAction("Index");
        }

        public ActionResult GenerateVoteFile(int year)
        {
            ViewBag.Year = year;
            var guests = GuestsBusinessCtrl.GetGuests(year);
            FormatVotes.CreateDoc("D:\\Votes" + year+  ".doc", CategoriesBusinessCtrl.GetCategoriesWithNominees(year), 
                guests);

            return View("Index", guests);
        }
    }
}
