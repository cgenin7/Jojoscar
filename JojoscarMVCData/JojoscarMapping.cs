using JojoscarMVCCommun;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace JojoscarMVCDBLayer
{
    public static class MappingExtensions
    {
        public static PrimitivePropertyConfiguration IsUnique(this PrimitivePropertyConfiguration configuration)
        {
            return configuration.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
        }
    }

    public class CategoryMapping : EntityTypeConfiguration<CategoryModel>
    {
        public CategoryMapping()
        {
            ToTable("Category");
            HasKey(c => c.CategoryID);
        }
    }

    public class CategoryNomineeMapping : EntityTypeConfiguration<CategoryNomineeModel>
    {
        public CategoryNomineeMapping()
        {
            ToTable("CategoryNominee");
            HasKey(c => c.CategoryNomineeID);
        }
    }

    public class GuestMapping : EntityTypeConfiguration<GuestModel>
    {
        public GuestMapping()
        {
            ToTable("Guest");
            HasKey(g => g.GuestID);
            Property(g => g.AccessCode).IsUnique();
        }
    }

    public class TableMapping : EntityTypeConfiguration<TableModel>
    {
        public TableMapping()
        {
            ToTable("Table");
            HasKey(t => t.TableID);
            
        }
    }
}
