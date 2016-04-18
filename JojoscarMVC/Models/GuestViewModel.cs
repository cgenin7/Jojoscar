using JojoscarMVCCommun;
using System.Collections.Generic;

namespace JojoscarMVC.Models
{
    public class GuestViewModel
    {
        public GuestViewModel()
        {
            Guest = new GuestModel();
            Categories = new List<CategoryViewModel>();
        }

        public GuestModel Guest { get; set; }

        public List<CategoryViewModel> Categories { get; set; }

        public string EmailContent { get; set; }

        public bool SendEmail { get; set; }

        public int TotalNbPoints { get; set; }

        public int TotalNbResponses { get; set; }

    }
}