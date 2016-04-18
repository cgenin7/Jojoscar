using JojoscarMVCCommun;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JojoscarMVC.Models
{
    public class DisplayResultViewModel
    {
        public DisplayResultViewModel() { }
        public DisplayResultViewModel(int year, int nbTimes)
        { Year = year; NbTimes = nbTimes; }

        public int Year { get; set; }
        [Key]
        public int NbTimes { get; set; }
        public string LabelCitron { get; set; }
        public string LabelFifth { get; set; }
        public string LabelFourth { get; set; }
        public string LabelThird { get; set; }
        public string LabelSecond { get; set; }
        public string LabelFirst { get; set; }

        public string LabelTropheeJojoscar { get; set; }

        public List<ResultModel> Results { get; set; }
    }
}