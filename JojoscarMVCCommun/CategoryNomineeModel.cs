
namespace JojoscarMVCCommun
{
    public class CategoryNomineeModel
    {
        public int CategoryNomineeID { get; set; }
        public string Letter { get; set; }
        public string Description { get; set; }

        public CategoryModel Category { get; set; }
        public int CategoryId { get; set; }
    }
}