using JojoscarMVCCommun;
using JojoscarMVCDBLayer;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace JojoscarMVCDBLayer
{
    public static class CategoryRepository
    {
        public static List<CategoryModel> GetCategoriesWithNominees(int year)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                return dbContext.Categories.Include("CategoryNominees").AsNoTracking().ToList();
            }
        }

        public static List<CategoryModel> GetCategories(int year)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                return dbContext.Categories.AsNoTracking().ToList();
            }
        }

        public static CategoryModel GetCategory(int year, int CategoryNb)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                return dbContext.Categories.AsNoTracking().FirstOrDefault(g => g.CategoryNb == CategoryNb);
            }
        }

        public static void AddCategory(int year, CategoryModel CategoryToAdd)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                dbContext.Categories.Add(CategoryToAdd);
                dbContext.SaveChanges();
            }
        }

        public static void DeleteCategory(int year, int categoryId)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                dbContext.Database.ExecuteSqlCommand("exec DeleteCategory @categoryId = {0}", categoryId);
                dbContext.SaveChanges();
            }
        }

        public static void EditCategory(int year, CategoryModel CategoryToModify)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                dbContext.Entry(CategoryToModify).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public static void ReplaceCategoryNominees(int year, List<CategoryNomineeModel> nominees)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                dbContext.CategoryNominees.RemoveRange(dbContext.CategoryNominees);
                dbContext.CategoryNominees.AddRange(nominees);
                dbContext.SaveChanges();
            }
        }
    }
}
