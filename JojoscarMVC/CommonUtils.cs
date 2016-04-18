using JojoscarMVC.Models;
using JojoscarMVCBusinessLogic;
using JojoscarMVCCommun;
using System.Collections.Generic;
using System.Linq;

namespace JojoscarMVC
{
    public class CommonUtils
    {
        public static GuestViewModel BuildGuestViewModel(int year, GuestModel guest)
        {
            var guestViewModel = new GuestViewModel();

            guestViewModel.Guest = guest;
            guestViewModel.Categories = CategoriesBusinessCtrl.GetCategoriesWithNominees(year)
                .Select(c => new CategoryViewModel { Category = c }).ToList();

            var votes = guestViewModel.Guest.GetVotes();
            if (votes != null && votes.Count == Calculation.NB_CATEGORIES)
            {
                for (int i = 0; i < guestViewModel.Categories.Count; i++)
                {
                    guestViewModel.Categories[i].SelectedLetter = votes[i];
                    if (guestViewModel.Categories[i].Category.AcademyChoiceLetter == votes[i])
                    {
                        guestViewModel.TotalNbPoints += guestViewModel.Categories[i].Category.NbPoints;
                        guestViewModel.TotalNbResponses++;
                    }
                }
            }
            return guestViewModel;
        }

        
        public static List<CategoryViewModel> BuildStatisticViewModel(int year, List<GuestModel> statsByLetter)
        {
            var categories = CategoriesBusinessCtrl.GetCategoriesWithNominees(year)
                .Select(c => new CategoryViewModel { Category = c }).ToList();

            foreach (var statForALetter in statsByLetter)
            {
                List<string> counts = statForALetter.GetVotes();

                for (int c = 0; c < categories.Count; c++)
                {
                    int count = 0;
                    int.TryParse(counts[c], out count);
                    categories[c].NbVotesForEachNominee.Add(count);
                }
            }

            foreach (var category in categories)
            {
                category.MaxVotes = category.NbVotesForEachNominee.Max();
            }

            return categories;
        }

        public static readonly string RES = "Resultats";
    }
}
