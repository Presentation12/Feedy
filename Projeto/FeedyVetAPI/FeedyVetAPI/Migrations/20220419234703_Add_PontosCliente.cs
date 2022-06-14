using Microsoft.EntityFrameworkCore.Migrations;

namespace FeedyVetAPI.Migrations
{
    public partial class Add_PontosCliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pontos",
                table: "Cliente",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pontos",
                table: "Cliente");
        }
    }
}
