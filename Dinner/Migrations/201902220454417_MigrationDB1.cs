namespace Dinner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationDB1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Devices", "Room_Id", "dbo.Rooms");
            DropIndex("dbo.Devices", new[] { "Room_Id" });
            DropIndex("dbo.Tickets", new[] { "UserID" });
            RenameColumn(table: "dbo.Devices", name: "Room_Id", newName: "RoomId");
            AlterColumn("dbo.Devices", "RoomId", c => c.Int(nullable: false));
            CreateIndex("dbo.Devices", "RoomId");
            CreateIndex("dbo.Tickets", "UserId");
            AddForeignKey("dbo.Devices", "RoomId", "dbo.Rooms", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Devices", "RoomId", "dbo.Rooms");
            DropIndex("dbo.Tickets", new[] { "UserId" });
            DropIndex("dbo.Devices", new[] { "RoomId" });
            AlterColumn("dbo.Devices", "RoomId", c => c.Int());
            RenameColumn(table: "dbo.Devices", name: "RoomId", newName: "Room_Id");
            CreateIndex("dbo.Tickets", "UserID");
            CreateIndex("dbo.Devices", "Room_Id");
            AddForeignKey("dbo.Devices", "Room_Id", "dbo.Rooms", "Id");
        }
    }
}
