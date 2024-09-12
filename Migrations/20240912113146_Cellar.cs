using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WineCellar.Migrations
{
    /// <inheritdoc />
    public partial class Cellar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Users_UserId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_Wines_Storages_StorageId",
                table: "Wines");

            migrationBuilder.DropForeignKey(
                name: "FK_Wines_Users_UserId",
                table: "Wines");

            migrationBuilder.DropIndex(
                name: "IX_Wines_Description",
                table: "Wines");

            migrationBuilder.DropIndex(
                name: "IX_Wines_Quantity",
                table: "Wines");

            migrationBuilder.DropIndex(
                name: "IX_Wines_UserId",
                table: "Wines");

            migrationBuilder.DropIndex(
                name: "IX_Wines_Year",
                table: "Wines");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Wines");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Storages",
                newName: "CellarId");

            migrationBuilder.RenameIndex(
                name: "IX_Storages_UserId",
                table: "Storages",
                newName: "IX_Storages_CellarId");

            migrationBuilder.AlterColumn<int>(
                name: "StorageId",
                table: "Wines",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpirationTime",
                table: "Wines",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cellars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Temperature = table.Column<double>(type: "REAL", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cellars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CellarUser",
                columns: table => new
                {
                    CellarsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellarUser", x => new { x.CellarsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CellarUser_Cellars_CellarsId",
                        column: x => x.CellarsId,
                        principalTable: "Cellars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CellarUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wines_ExpirationTime",
                table: "Wines",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_Type",
                table: "Storages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Cellars_Name",
                table: "Cellars",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CellarUser_UsersId",
                table: "CellarUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Cellars_CellarId",
                table: "Storages",
                column: "CellarId",
                principalTable: "Cellars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wines_Storages_StorageId",
                table: "Wines",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Cellars_CellarId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_Wines_Storages_StorageId",
                table: "Wines");

            migrationBuilder.DropTable(
                name: "CellarUser");

            migrationBuilder.DropTable(
                name: "Cellars");

            migrationBuilder.DropIndex(
                name: "IX_Wines_ExpirationTime",
                table: "Wines");

            migrationBuilder.DropIndex(
                name: "IX_Storages_Type",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "Wines");

            migrationBuilder.RenameColumn(
                name: "CellarId",
                table: "Storages",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Storages_CellarId",
                table: "Storages",
                newName: "IX_Storages_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "StorageId",
                table: "Wines",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Wines",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Wines_Description",
                table: "Wines",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Wines_Quantity",
                table: "Wines",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_Wines_UserId",
                table: "Wines",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Wines_Year",
                table: "Wines",
                column: "Year");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Users_UserId",
                table: "Storages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wines_Storages_StorageId",
                table: "Wines",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wines_Users_UserId",
                table: "Wines",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
