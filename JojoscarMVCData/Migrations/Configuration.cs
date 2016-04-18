namespace JojoscarMVCDBLayer.Migrations
{
    using JojoscarMVCCommun;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    internal sealed class Configuration : DbMigrationsConfiguration<JojoscarDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(JojoscarDbContext context)
        {
            AddCategory(context, 1, "Best Picture", 12, true);    
            AddCategory(context, 2, "Directing", 10);
            AddCategory(context, 3, "Leading Actor", 7);
            AddCategory(context, 4, "Leading Actress", 7);
            AddCategory(context, 5, "Supporting Actor", 5);
            AddCategory(context, 6, "Supporting Actress", 5);
            AddCategory(context, 7, "Production Design", 4);
            AddCategory(context, 8, "Cinematography", 4);
            AddCategory(context, 9, "Costume Design", 4);
            AddCategory(context, 10, "Music - Original Score", 4);
            AddCategory(context, 11, "Music - Original Song", 4);
            AddCategory(context, 12, "Film Editing", 3);
            AddCategory(context, 13, "Makeup and Hairstyling", 3);
            AddCategory(context, 14, "Sound Mixing", 3);
            AddCategory(context, 15, "Sound Editing", 3);
            AddCategory(context, 16, "Visual Effects", 3);
            AddCategory(context, 17, "WRITING - Adapted Screeplay", 3);
            AddCategory(context, 18, "WRITING - Original Screenplay", 3);
            AddCategory(context, 19, "Animated Feature", 3);
            AddCategory(context, 20, "Foreign Language Film", 2);
            AddCategory(context, 21, "Documentary Feature", 2);
            AddCategory(context, 22, "Documentary Short", 2);
            AddCategory(context, 23, "Short Film - Animated", 2);
            AddCategory(context, 24, "Short Film - Live Action", 2);
               
            context.SaveChanges();
        }

        private void AddCategory(JojoscarDbContext context, int categoryNb, string categoryName, int nbPoints, bool isBestPicture = false)
        {
            var category = context.Categories.Where(c => c.CategoryNb == categoryNb).FirstOrDefault();

            if (category == null)
            {
                var categoryNominees = new List<CategoryNomineeModel>
                    {   new CategoryNomineeModel { Description = "A - " + categoryName + " Nominee A", Letter = "A" },
                        new CategoryNomineeModel { Description = "B - " + categoryName + " Nominee B", Letter = "B" },
                        new CategoryNomineeModel { Description = "C - " + categoryName + " Nominee C", Letter = "C" },
                        new CategoryNomineeModel { Description = "D - " + categoryName + " Nominee D", Letter = "D" },
                        new CategoryNomineeModel { Description = "E - " + categoryName + " Nominee E", Letter = "E" },
                    };

                if (isBestPicture)
                {
                    categoryNominees.Add(new CategoryNomineeModel { Description = "F - " + categoryName + " Nominee F", Letter = "F" });
                    categoryNominees.Add(new CategoryNomineeModel { Description = "G - " + categoryName + " Nominee G", Letter = "G" });
                    categoryNominees.Add(new CategoryNomineeModel { Description = "H - " + categoryName + " Nominee H", Letter = "H" });
                    categoryNominees.Add(new CategoryNomineeModel { Description = "I - " + categoryName + " Nominee I", Letter = "I" });
                    categoryNominees.Add(new CategoryNomineeModel { Description = "J - " + categoryName + " Nominee J", Letter = "J" });
                }

                context.Categories.AddOrUpdate(c => c.CategoryNb,
                    new CategoryModel
                    {
                        CategoryNb = categoryNb,
                        CategoryName = categoryName,
                        CategoryNominees = categoryNominees,
                        NbPoints = nbPoints
                    });
            }
        }
    }
}
