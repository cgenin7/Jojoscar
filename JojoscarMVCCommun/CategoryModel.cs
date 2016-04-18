using System.Collections.Generic;

namespace JojoscarMVCCommun
{
    public class CategoryModel
    {
        public CategoryModel()
        {
            CategoryNominees = new List<CategoryNomineeModel>();
        }

        public int CategoryID { get; set; }
        public int CategoryNb { get; set; }
        public string CategoryName { get; set; }
        public int NbPoints { get; set; }
        public string AcademyChoiceLetter { get; set; }
        public List<CategoryNomineeModel> CategoryNominees { get; set; }
    }
}