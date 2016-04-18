using JojoscarMVCCommun;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JojoscarMVC.Models
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            NbVotesForEachNominee = new List<int>();
            SelectedLetter = "";
        }

        public CategoryModel Category { get; set; }

        public SelectList CategoryNomineesSelectList { get; set; }

        public string SelectedLetter { get; set; }
        public List<int> NbVotesForEachNominee { get; set; }

        public int MaxVotes { get; set; } 
    }
}