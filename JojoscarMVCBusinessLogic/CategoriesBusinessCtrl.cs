using JojoscarMVCCommun;
using JojoscarMVCDBLayer;
using System.Collections.Generic;

namespace JojoscarMVCBusinessLogic
{
    public static class CategoriesBusinessCtrl
    {
        public static List<CategoryModel> GetCategoriesWithNominees(int year)
        {
            return CategoryRepository.GetCategoriesWithNominees(year);
        }

        public static List<CategoryModel> GetCategories(int year)
        {
            return CategoryRepository.GetCategories(year);
        }

        public static CategoryModel GetCategory(int year, int categoryId)
        {
            return CategoryRepository.GetCategory(year, categoryId);
        }

        public static void EditCategory(int year, CategoryModel category)
        {
            CategoryRepository.EditCategory(year, category);
        }

        public static void ReplaceCategoryNominees(int year, List<CategoryNomineeModel> nominees)
        {
            CategoryRepository.ReplaceCategoryNominees(year, nominees);
        }
    }
}
