using System.Collections.Generic;

namespace JojoscarMVC.Models
{
    public class AcademyChoiceViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }

        public string AcademyVotes { get; set; }
        public string Php { get; set; }
    }
}