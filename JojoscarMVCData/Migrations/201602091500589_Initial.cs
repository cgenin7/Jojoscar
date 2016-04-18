namespace JojoscarMVCDBLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
           /* CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        NbPoints = c.Int(nullable: false),
                        AcademyChoiceLetter = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.CategoryNominee",
                c => new
                    {
                        CategoryNomineeID = c.Int(nullable: false, identity: true),
                        Letter = c.String(),
                        Description = c.String(),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryNomineeID)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Guest",
                c => new
                    {
                        GuestID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        AccessCode = c.Int(nullable: false),
                        Email = c.String(),
                        PaymentDone = c.Boolean(nullable: false),
                        Penality = c.Boolean(nullable: false),
                        IsPresent = c.Boolean(nullable: false),
                        IsEligibleToMoney = c.Boolean(nullable: false),
                        AllVotes = c.String(),
                        TableId = c.Int(),
                    })
                .PrimaryKey(t => t.GuestID)
                .ForeignKey("dbo.Table", t => t.TableId)
                .Index(t => t.AccessCode, unique: true)
                .Index(t => t.TableId);
            
            CreateTable(
                "dbo.Table",
                c => new
                    {
                        TableID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        GuestResponsibleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TableID);
          */  
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Guest", "TableId", "dbo.Table");
            DropForeignKey("dbo.CategoryNominee", "CategoryId", "dbo.Category");
            DropIndex("dbo.Guest", new[] { "TableId" });
            DropIndex("dbo.Guest", new[] { "AccessCode" });
            DropIndex("dbo.CategoryNominee", new[] { "CategoryId" });
            DropTable("dbo.Table");
            DropTable("dbo.Guest");
            DropTable("dbo.CategoryNominee");
            DropTable("dbo.Category");
        }
    }
}
