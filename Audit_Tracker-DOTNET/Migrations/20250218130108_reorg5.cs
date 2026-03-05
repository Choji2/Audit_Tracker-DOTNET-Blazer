using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace AAPInventoryZoneTracker.Migrations
{
    /// <inheritdoc />
    public partial class reorg5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AAP_Divisions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Div_Code = table.Column<string>(type: "longtext", nullable: false),
                    Desc = table.Column<string>(type: "longtext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAP_Divisions", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    ID = table.Column<string>(type: "varchar(255)", nullable: false),
                    desc = table.Column<string>(type: "longtext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Div_Zones",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Zone_Code = table.Column<string>(type: "longtext", nullable: false),
                    Desc = table.Column<string>(type: "longtext", nullable: false),
                    DivID = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Div_Zones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Div_Zones_AAP_Divisions_DivID",
                        column: x => x.DivID,
                        principalTable: "AAP_Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_Records",
                columns: table => new
                {
                    ID = table.Column<string>(type: "varchar(255)", nullable: false),
                    ZoneID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    INVID = table.Column<string>(type: "varchar(255)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_Records", x => x.ID);
                    table.ForeignKey(
                        name: "FK_inventory_Records_Inventories_INVID",
                        column: x => x.INVID,
                        principalTable: "Inventories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Div_Zones_DivID",
                table: "Div_Zones",
                column: "DivID");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_Records_INVID",
                table: "inventory_Records",
                column: "INVID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Div_Zones");

            migrationBuilder.DropTable(
                name: "inventory_Records");

            migrationBuilder.DropTable(
                name: "AAP_Divisions");

            migrationBuilder.DropTable(
                name: "Inventories");
        }
    }
}
