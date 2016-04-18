namespace JojoscarMVCDBLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryNb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "CategoryNb", c => c.Int(nullable: false));
            RenameColumn("dbo.Guest", "Table_TableID", "TableID");
            RenameColumn("dbo.CategoryNominee", "Category_CategoryID", "CategoryID");

            Sql("UPDATE Category SET CategoryNb = CategoryId");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Category", "CategoryNb");
        }
    }
}
